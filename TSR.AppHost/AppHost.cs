var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TSR_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.TSR_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);


var workerService = builder.AddProject<Projects.TSR_Worker>("worker");
    //.WithHttpHealthCheck("/health");

builder.Build().Run();
