using KubeClient;
using KubeClient.Extensions.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

//[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace techies.client.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {                    
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)                        
                        .AddEnvironmentVariables()
                        .AddUserSecrets("068a3a69-a8be-4cae-ae98-9c6c9d8b3f86")
                        .AddKubeConfigMap(KubeClientOptions.FromPodServiceAccount(),
                            configMapName:"techies-client-config",
                            kubeNamespace:"default");
                })
                .UseStartup<Startup>();
    }
}
