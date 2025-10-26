using Order.Messaging;
using Order.Application.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddOrderServices();

var host = builder.Build();
host.Run();