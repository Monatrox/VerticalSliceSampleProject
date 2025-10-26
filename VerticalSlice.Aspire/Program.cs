using VerticalSlice.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

builder.WithDistributedServices();

builder.Build().Run();