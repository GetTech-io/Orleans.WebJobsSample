using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.WebJobsSample.Abstractions.Grains;

namespace Orleans.WebJobsSample.Grains
{

    /// <summary>
    /// An implementation of the 'Reduce' pattern (See https://github.com/OrleansContrib/DesignPatterns/blob/master/Reduce.md).
    /// </summary>
    /// <seealso cref="Grain" />
    /// <seealso cref="ICounterStatelessGrain" />
    [StatelessWorker]
    public class CounterStatelessGrain : Grain, ICounterStatelessGrain
    {
        private long _count = 0;

        public Task Increment()
        {
            _count += 1;
            return Task.CompletedTask;
        }

        public override Task OnActivateAsync()
        {
            // Timers are stored in-memory so are not resilient to nodes going down. They should be used for short
            // high-frequency timers their period should be measured in seconds.
            RegisterTimer(OnTimerTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            return base.OnActivateAsync();
        }

        private Task OnTimerTick(object arg)
        {
            var count = _count;
            _count = 0;
            var counter = GrainFactory.GetGrain<ICounterGrain>(Guid.Empty);
            return counter.AddCount(count);
        }
    }
}
