using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Polly;
using Ocelot.Logging;
using System.Reflection;
using Jaeger.Samplers;
using Jaeger;
using OpenTracing.Util;
using Microsoft.Extensions.Logging;

namespace techies.apigateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddOcelot()
                .AddCacheManager(x => x.WithDictionaryHandle())
                .AddPolly();

            services.AddHealthChecks();

            services.AddSingleton(serviceProvider =>
            {
                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                var sampler = new ConstSampler(sample: true);

                var tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer as OpenTracing.ITracer;
            });

            services.AddOpenTracing();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            var host = Configuration.GetValue<string>("Swagger:Host");
            var schema = Configuration.GetValue<string>("Swagger:Schema");

            app.UseSwaggerUi3(opt=> opt.DocumentPath = "/swagger/v1/swagger.json");
            app.Map("/swagger/v1/swagger.json", b =>
            {
                b.Run(async x => {
                    var json = File.ReadAllText("swagger.json");
                    json = json.Replace("#{host}", host);
                    json = json.Replace("#{schema}", schema);
                    await x.Response.WriteAsync(json);
                });
            });            
            app.UseHealthChecksUI(config => config.UIPath = "/hc-ui");
            app.UseOcelot().Wait();                                          
        }
    }
}
