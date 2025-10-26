using System.Text.Json.Serialization;
using FastEndpoints;
using Todo.Application.Extensions;
using Weather.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.AddRedisDistributedCache("cache");
builder.AddRedisOutputCache("cache");

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
