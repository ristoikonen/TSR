using Microsoft.Extensions.Hosting;
//using TSR.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Register your application services, e.g.,
                // services.AddHostedService<TSR.Worker>(); 
                // services.AddTransient<IMyService, MyService>();
            })


            //    {
            //        // Register your main console application logic
            //        services.AddTransient<MyConsoleAppLogic>();
            //        // Add typed client for external API calls
            //        services.AddHttpClient<IMyApiClient, MyApiClient>();
            //    })





            // You can add more configuration here, e.g., for logging or web host
            // .ConfigureLogging(...)
            // .ConfigureAppConfiguration(...)
            ;
}

//await Host.CreateDefaultBuilder(args)
//    .ConfigureServices((hostContext, services) =>
//    {
//        // Register your main console application logic
//        services.AddTransient<MyConsoleAppLogic>();
//        // Add typed client for external API calls
//        services.AddHttpClient<IMyApiClient, MyApiClient>();
//    })
//    .Build()
//    .RunAsync();
