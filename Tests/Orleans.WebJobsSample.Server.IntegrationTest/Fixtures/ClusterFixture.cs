namespace Orleans.WebJobsSample.Server.IntegrationTest.Fixtures
{
    using System;
    using Orleans.TestingHost;

    public class ClusterFixture : IDisposable
    {
        public ClusterFixture()
        {
            this.Cluster = this.CreateTestCluster();
            this.Cluster.Deploy();
        }

        public TestCluster Cluster { get; }

        public TestCluster CreateTestCluster() =>
            new TestClusterBuilder()
                .AddClientBuilderConfigurator<TestClientBuilderConfigurator>()
                .AddSiloBuilderConfigurator<TestSiloBuilderConfigurator>()
                .Build();

        // Switch to IAsyncDisposable.DisposeAsync and call Cluster.DisposeAsync in .NET Core 3.0.
        public void Dispose() => this.Cluster.Dispose();
    }
}
