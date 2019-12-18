namespace Orleans.WebJobsSample.Server.IntegrationTest.Fixtures
{
    using Orleans.Hosting;
    using Orleans.TestingHost;
    using Orleans.WebJobsSample.Abstractions.Constants;

    public class TestSiloBuilderConfigurator : ISiloBuilderConfigurator
    {
        public void Configure(ISiloHostBuilder siloHostBuilder) =>
            siloHostBuilder
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("PubSubStore")
                .AddSimpleMessageStreamProvider(StreamProviderName.Default);
    }
}
