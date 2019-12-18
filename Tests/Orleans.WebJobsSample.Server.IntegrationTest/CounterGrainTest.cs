namespace Orleans.WebJobsSample.Server.IntegrationTest
{
    using System;
    using System.Threading.Tasks;
    using Orleans.WebJobsSample.Abstractions.Grains;
    using Orleans.WebJobsSample.Server.IntegrationTest.Fixtures;
    using Xunit;

    public class CounterGrainTest : ClusterFixture
    {
        [Fact]
        public async Task AddCount_PassValue_ReturnsTotalCount()
        {
            var grain = this.Cluster.GrainFactory.GetGrain<ICounterGrain>(Guid.Empty);

            var count = await grain.AddCount(10L);

            Assert.Equal(10L, count);
        }

        [Fact]
        public async Task GetCount_Default_ReturnsTotalCount()
        {
            var grain = this.Cluster.GrainFactory.GetGrain<ICounterGrain>(Guid.Empty);

            var count = await grain.GetCount();

            Assert.Equal(0L, count);
        }
    }
}
