using HealthChecks.UI.Client;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using OpenTracing;
using OpenTracing.Util;
using System;
using System.Reflection;

namespace techies.client.stats.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerDocument();

            var rabbitHostName = Configuration.GetValue<string>("RabbitMq:HostName");
            var rabbitUser = Configuration.GetValue<string>("RabbitMq:User");
            var rabbitPassword = Configuration.GetValue<string>("RabbitMq:Password");            

            var rabbitConn = $"amqp://{rabbitUser}:{rabbitPassword}@{rabbitHostName}:5672/";

            services.AddCap(opt =>
            {
                opt.UseDashboard();
                opt.UseRabbitMQ(o =>
                {
                    o.HostName = rabbitHostName;                                        
                });
                opt.UseInMemoryStorage();
            });
            var elastic = Configuration.GetValue<string>("ElasticSearch:Host");

            var settings = new ConnectionSettings(new Uri(elastic))
                .DefaultIndex("clients");

            services.AddScoped(sp => new ElasticClient(settings));

            services.AddHealthChecks()
                .AddRabbitMQ(rabbitConn)
                .AddElasticsearch(elastic);

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                var sampler = new ConstSampler(sample: true);

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            services.AddOpenTracing();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi3();
        }
    }
}
