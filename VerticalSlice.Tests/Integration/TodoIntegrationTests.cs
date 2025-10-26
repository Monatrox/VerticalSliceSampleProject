using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Todo.Application.Endpoints.GetAllTodos;
using VerticalSlice.Tests.Fixtures;

namespace VerticalSlice.Tests.Integration;

[Collection("AppHost")]
public class TodoIntegrationTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetAllTodos_ShouldReturnAllItems()
    {
        // Act
        var response = await fixture.Client!.GetAsync("/todos");
        var result = await response.Content.ReadFromJsonAsync<GetAllTodosResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBeGreaterThanOrEqualTo(5);
    }
}