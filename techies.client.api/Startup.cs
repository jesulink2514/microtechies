using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddCap(opts =>
            {                
                opts.UseDashboard();
                opts.UseRabbitMQ(o =>
                {
                    o.HostName = rabbitHostName;                    
                });
                opts.UseEntityFramework<ClientsDbContext>();
            });

            services.AddSwaggerDocument();

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
