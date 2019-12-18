using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.WebJobsSample.Abstractions.Constants;
using Orleans.WebJobsSample.Abstractions.Grains;

namespace Orleans.WebJobsSample.Grains
{

    public class HelloGrain : Grain, IHelloGrain
    {
        public async Task<string> SayHello(string name)
        {
            await IncrementCounter();
            await PublishSaidHello(name);

            return $"Hello {name}!";
        }

        private Task IncrementCounter()
        {
            var counter = GrainFactory.GetGrain<ICounterStatelessGrain>(0L);
            return counter.Increment();
        }

        private Task PublishSaidHello(string name)
        {
            var streamProvider = GetStreamProvider(StreamProviderName.Default);
            var stream = streamProvider.GetStream<string>(Guid.Empty, StreamName.SaidHello);
            return stream.OnNextAsync(name);
        }
    }
}
