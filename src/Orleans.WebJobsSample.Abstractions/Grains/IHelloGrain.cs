using System.Threading.Tasks;
using Orleans;

namespace Orleans.WebJobsSample.Abstractions.Grains
{

    public interface IHelloGrain : IGrainWithGuidKey
    {
        Task<string> SayHello(string name);
    }
}
