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
            
            HttpGenericClient<AustralianPostcode[]> httpGenericClient = new HttpGenericClient<AustralianPostcode[]>();
            var codes = httpGenericClient.GetAsync(@"https://www.matthewproctor.com/Content/postcodes/australian_postcodes.json", "www.matthewproctor.com");


            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"From Worker: Executing");

            while (!stoppingToken.IsCancellationRequested)
            {

                //var result = await client.GetAsync("https://stackoverflow.com/ristoikonen");


                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
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
