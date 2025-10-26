using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using Shouldly;
using VerticalSlice.Tests.Fixtures;
using Weather.Application.Endpoints.CreateLocation;
using Weather.Application.Endpoints.CreateWeatherForecast;
using Weather.Application.Endpoints.GetWeatherForecast;

namespace VerticalSlice.Tests.Integration;

[Collection("AppHost")]
public class WeatherIntegrationTests(AppHostFixture fixture)
{
}