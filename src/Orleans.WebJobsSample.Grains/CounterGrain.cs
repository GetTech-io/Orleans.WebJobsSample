using System.Threading.Tasks;
using Orleans;
using Orleans.WebJobsSample.Abstractions.Grains;

namespace Orleans.WebJobsSample.Grains
{
    public class CounterGrain : Grain<long>, ICounterGrain
    {
        public async Task<long> AddCount(long value)
        {
            State += value;
            await WriteStateAsync();
            return State;
        }

        public Task<long> GetCount() => Task.FromResult(State);
    }
}
