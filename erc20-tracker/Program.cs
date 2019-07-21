using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace erc20_tracker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var envVariables = Environment.GetEnvironmentVariables();


            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) => {
                    var env = envVariables["ASPNETCORE_ENVIRONMENT"];

                    configApp
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env}.json", optional: true)
                        .AddEnvironmentVariables();
                    if (args != null) configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) => {
                    services.AddOptions();

                    services.Configure<Settings>(config: hostContext.Configuration);
                    services.AddSingleton<IHostedService, Erc20TrackerService>();
                });
            await builder.RunConsoleAsync();
        }
    }
}
