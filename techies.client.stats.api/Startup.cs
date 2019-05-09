using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi3();
        }
    }
}
