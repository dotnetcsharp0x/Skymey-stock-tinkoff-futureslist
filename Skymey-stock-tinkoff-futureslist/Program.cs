﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skymey_stock_tinkoff_futureslist.Actions.GetFutures;

namespace Skymey_stock_tinkoff_futureslist
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IHostedService, MySpecialService>();
                });
            await builder.RunConsoleAsync();
        }
    }
    public class MySpecialService : BackgroundService
    {
        GetFutures gb = new GetFutures();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    gb.GetFuturesFromTinkoff();
                    await Task.Delay(TimeSpan.FromHours(24));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
