using System.Threading.Tasks;
using Orleans;

namespace Orleans.WebJobsSample.Abstractions.Grains
{
    public interface IReminderGrain : IGrainWithGuidKey
    {
        Task SetReminder(string reminder);
    }
}
