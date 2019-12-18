using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.WebJobsSample.Abstractions.Constants;
using Orleans.WebJobsSample.Abstractions.Grains;

namespace Orleans.WebJobsSample.Grains
{

    public class ReminderGrain : Grain, IReminderGrain, IRemindable
    {
        private const string ReminderName = "SomeReminder";
        private string _reminder;

        public Task SetReminder(string reminder)
        {
            _reminder = reminder;
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            // Reminders are timers that are persisted to storage, so they are resilient if the node goes down. They
            // should not be used for high-frequency timers their period should be measured in minutes, hours or days.
            await RegisterOrUpdateReminder(
                ReminderName,
                TimeSpan.FromSeconds(150),
                TimeSpan.FromSeconds(60));
            await base.OnActivateAsync();
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (string.Equals(ReminderName, reminderName, StringComparison.Ordinal))
            {
                if (!string.IsNullOrEmpty(_reminder))
                {
                    await PublishReminder(_reminder);
                }
            }
        }

        private Task PublishReminder(string reminder)
        {
            var streamProvider = GetStreamProvider(StreamProviderName.Default);
            var stream = streamProvider.GetStream<string>(Guid.Empty, StreamName.Reminder);
            return stream.OnNextAsync(reminder);
        }
    }
}
