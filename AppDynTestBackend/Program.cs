using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AppDynTestBackend
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var levelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel = LogEventLevel.Information
                };

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .CreateLogger();

                var host = CreateHostBuilder(args).Build();

                var scope = host.Services.CreateScope();
                var serviceProvider = scope.ServiceProvider;
                var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

                if (env.IsDevelopment())
                {
                    levelSwitch.MinimumLevel = LogEventLevel.Debug;
                }

                await host.RunAsync();
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Host terminated unexpectedly: {ex}");
                
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
}
