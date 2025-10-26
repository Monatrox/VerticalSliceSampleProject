using VerticalSlice.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.WithDistributedServices();

var app = builder.Build();
await app.RunAsync();
