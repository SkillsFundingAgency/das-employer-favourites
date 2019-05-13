using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using SFA.DAS.Configuration.AzureTableStorage;

namespace DfE.EmployerFavourites.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting up host");
                var host = CreateWebHostBuilder(args).Build();

                host.Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Fatal(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile($"appSettings.{environmentName}.json", optional: true, reloadOnChange: false);
                    config.AddUserSecrets<Startup>();
                    config.AddAzureTableStorage(options => {
                        options.ConfigurationKeys = new [] { "SFA.DAS.Employer.Shared.UI" };
                        options.EnvironmentNameEnvironmentVariableName = "APPSETTING_EnvironmentName";
                        options.StorageConnectionStringEnvironmentVariableName = "APPSETTING_ConfigurationStorageConnectionString";
                    });
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .UseUrls("https://localhost:5040")
                .UseNLog();
    }
}
