using Orleans.Configuration;

namespace Orleans.WebJobsSample.Server.Options
{


    public class ApplicationOptions
    {
        public ClusterOptions Cluster { get; set; }

        public StorageOptions Storage { get; set; }
    }
}
