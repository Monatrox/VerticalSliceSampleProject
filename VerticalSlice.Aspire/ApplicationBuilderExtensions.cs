using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace VerticalSlice.Aspire;

public static class ApplicationBuilderExtensions
{
    public static IDistributedApplicationBuilder WithDistributedServices(this IDistributedApplicationBuilder builder)
    {
        var cache = builder.AddValkey("cache");
        
        // var kafka = builder.AddKafka("kafka")
        //     .WithLifetime(ContainerLifetime.Persistent)
        //     .WithKafkaUI()
        //     .WithLifetime(ContainerLifetime.Persistent);
        //
        // builder.Eventing.Subscribe<ResourceReadyEvent>(kafka.Resource, async (@event, ct) =>
        // {
        //     var connString = await kafka.Resource.ConnectionStringExpression.GetValueAsync(ct);
        //
        //     var config = new AdminClientConfig {BootstrapServers = connString};
        //     
        //     using var adminClient = new AdminClientBuilder(config).Build();
        //     try
        //     {
        //         await adminClient.CreateTopicsAsync(new TopicSpecification[]
        //         {
        //             // new TopicSpecification {Name = "test", NumPartitions = 1, ReplicationFactor = 1}
        //         });
        //     }
        //     catch (CreateTopicsException ex)
        //     {
        //         Console.WriteLine($"Error creating topics: {ex.Message}");
        //         throw;
        //     }
        // });
        
        var postgres = builder
            .AddPostgres("postgres")
            .WithLifetime(ContainerLifetime.Persistent);
        
        var weatherDb = postgres.AddDatabase("weatherdb");
        var todoDb = postgres.AddDatabase("tododb");

        var migrations = builder.AddProject<Projects.VerticalSlice_MigrationService>("verticalslice-migrationservice")
            .WithReference(weatherDb)
            .WithReference(todoDb)
            .WaitFor(weatherDb)
            .WaitFor(todoDb);
        
        var verticalSliceApi = builder.AddProject<Projects.VerticalSlice_Api>("verticalslice-api")
            .WithReference(weatherDb)
            .WithReference(todoDb)
            .WithReference(cache)
            .WithEnvironment("Services__WeatherApi", "http://verticalslice-api")
            .WithEnvironment("Services__TodoApi", "http://verticalslice-api")
            .WithEnvironment("ConnectionStrings__Database", postgres)
            .WaitFor(weatherDb)
            .WaitFor(todoDb)
            .WaitFor(cache)
            .WaitFor(migrations);

        // var yarpProxy = builder
        //     .AddYarp("gateway")
        //     .WithConfigFile("yarp.json")
        //     .WithReference(verticalSliceApi);

        return builder;
    }
}