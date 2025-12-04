using System.Collections.Generic;
using System.Net;
using TSR.Worker.Models;
using TSR.Worker.Services;

namespace TSR.Worker
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {

        //private HttpClient client;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"From Worker: Starting");

            //client = new HttpClient();
                        
            base.StartAsync(cancellationToken);
            


            return Task.CompletedTask;
        }

        protected override async Task<AustralianPostcode[]> ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"From Worker: Executing");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                //var result = await client.GetAsync("https://stackoverflow.com/ristoikonen");
                HttpGenericClient<AustralianPostcode[]> httpGenericClient = new HttpGenericClient<AustralianPostcode[]>();
                var codes = await httpGenericClient.PostAsyncParams(@"https://www.matthewproctor.com/Content/postcodes/australian_postcodes.json", null);
                return codes ?? Array.Empty<AustralianPostcode>();
                //await Task.Delay(1000, stoppingToken);
            }
            return Array.Empty<AustralianPostcode>();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"From Worker: Stopping");
            
            //client.Dispose();

            base.StopAsync(cancellationToken);
            
            return Task.CompletedTask;
        }
    }
}
