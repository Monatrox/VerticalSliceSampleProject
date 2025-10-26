using FastEndpoints;
using Todo.Application.Extensions;
using Todo.Infrastructure.Extensions;
using Weather.Infrastructure.Extensions;
using Weather.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisDistributedCache("cache");
builder.AddRedisOutputCache("cache");

// builder.Services.AddLogging(logging =>
// {
//     logging.AddFilter("Microsoft.AspNetCore.OutputCaching", LogLevel.Debug);
// });
builder.Services.AddWeatherEndpoints();
builder.AddWeatherServices();

builder.Services.AddTodoEndpoints();
builder.AddTodoServices();

builder.Services.AddOpenApi();

var app = builder.Build();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseFastEndpoints();

app.Run();
