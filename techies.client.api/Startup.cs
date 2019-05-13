using FluentValidation;
using HealthChecks.UI.Client;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using System.Reflection;
using Techies.Clients.ApplicationServices;
using Techies.Clients.ApplicationServices.Abstract;
using Techies.Clients.ApplicationServices.Validators;
using Techies.Clients.DTOs.Request;
using Techies.Clients.Infrastructure;
using Techies.Clients.Infrastructure.EFCore;
using Techies.Clients.Infrastructure.Persistence;

namespace techies.client.api
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
            var defaultConnection = Configuration.GetConnectionString("Default");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ClientsDbContext>(opt=> opt.UseNpgsql(defaultConnection))
                .BuildServiceProvider();

            var rabbitHostName = Configuration.GetValue<string>("RabbitMq:HostName");
            var rabbitUser = Configuration.GetValue<string>("RabbitMq:User");
            var rabbitPassword = Configuration.GetValue<string>("RabbitMq:Password");

            var rabbitConn = $"amqp://{rabbitUser}:{rabbitPassword}@{rabbitHostName}:5672/";

            services.AddCap(opts =>
            {                
                opts.UseDashboard();
                opts.UseRabbitMQ(o =>
                {
                    o.HostName = rabbitHostName;   
                    o.UserName = rabbitUser;
                    o.Password = rabbitPassword;
                });
                opts.UseEntityFramework<ClientsDbContext>();
            });

            services.AddHealthChecks()
                .AddRabbitMQ(rabbitConn)
                .AddNpgSql(defaultConnection);

            services.AddSwaggerDocument();

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

            

            services.AddScoped<IClientApplicationService,ClientsApplicationService>();
            services.AddScoped<IClientsRepository,EFClientsRepository>();
            services.AddScoped<IUnitOfWOrk,EFUnitOfWork>();

            services.AddSingleton<IValidator<RegisterClient>,RegisterClientValidator>();
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
           
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi3();
        }
    }
}
