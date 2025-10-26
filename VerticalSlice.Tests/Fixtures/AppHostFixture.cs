using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Configuration;

namespace VerticalSlice.Tests.Fixtures;

[CollectionDefinition("AppHost")]
public class AppHostCollection : ICollectionFixture<AppHostFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class AppHostFixture : IAsyncLifetime
{
    public HttpClient? Client { get; private set; } = default!;
    private DistributedApplication? _app;
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    
    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("INTEGRATION__TESTS", "true");
        using var cts = new CancellationTokenSource(DefaultTimeout);
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.VerticalSlice_AppHost>(cts.Token);
        
        _app = await appHost.BuildAsync(cts.Token);
        await _app.StartAsync(cts.Token);

        // Wait for API to be healthy
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("verticalslice-api", cts.Token);

        // Act
        Client = _app.CreateHttpClient("verticalslice-api");
    }
    
    public async Task DisposeAsync()
    {
        Client?.Dispose();
        
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
        
        Environment.SetEnvironmentVariable("INTEGRATION__TESTS", null);
    }
}