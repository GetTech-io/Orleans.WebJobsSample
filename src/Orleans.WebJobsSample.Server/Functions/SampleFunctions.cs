using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Orleans.WebJobsSample.Abstractions.Grains;

namespace Orleans.WebJobsSample.Server.Functions
{
    public class SampleFunctions
    {
        private readonly IClusterClient _clusterClient;
        public SampleFunctions(IClusterClient clusterClient) => _clusterClient = clusterClient;
        public async Task ProcessQueueMessage([QueueTrigger("queue")] string message, ILogger logger)
        {
            var helloGrain = _clusterClient.GetGrain<IHelloGrain>(Guid.NewGuid());
            var response = await helloGrain.SayHello(message);
            logger.LogInformation(response);
        }

        public async Task ProcessTimer([TimerTrigger("*/15 * * * * *")]TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation(myTimer.Schedule.ToString());
            var helloGrain = _clusterClient.GetGrain<IHelloGrain>(Guid.NewGuid());
            var response = await helloGrain.SayHello("TimerTrigger");
            logger.LogInformation(response);
        }
    }
}
