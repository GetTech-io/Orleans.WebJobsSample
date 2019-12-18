using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Statistics;
using Orleans.WebJobsSample.Abstractions.Constants;
using Orleans.WebJobsSample.Grains;
using Orleans.WebJobsSample.Server.Functions;
using Orleans.WebJobsSample.Server.Options;
using Serilog;
using Serilog.Core;

namespace Orleans.WebJobsSample.Server
{


    public class Program
    {
        public static Task<int> Main(string[] args) => LogAndRunAsync(CreateHostBuilder(args).Build());

        public static async Task<int> LogAndRunAsync(IHost host)
        {
            Log.Logger = CreateLogger(host);

            try
            {
                Log.Information("Started application");
                await host.RunAsync().ConfigureAwait(false);
                Log.Information("Stopped application");
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    var b = services.AddWebJobs(opt => { })
                    .AddAzureStorageCoreServices()
                    .AddAzureStorage()
                    .AddTimers()
                    .AddFiles();

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, JobHostService>());
                    services.AddTransient<SampleFunctions, SampleFunctions>();

                }).ConfigureWebJobs()
                .ConfigureLogging((context, b) =>
                {
                    // If the key exists in settings, use it to enable Application Insights.
                    var instrumentationKey = context.Configuration.GetSection("ApplicationInsights")["InstrumentationKey"];
                    if (!string.IsNullOrEmpty(instrumentationKey))
                    {
                        _ = b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = instrumentationKey);
                    }
                })
                .UseOrleans(ConfigureSiloBuilder)
                .UseConsoleLifetime();



        private static void ConfigureSiloBuilder(
            Microsoft.Extensions.Hosting.HostBuilderContext context,
            ISiloBuilder siloBuilder) =>
            siloBuilder
                // Prevent the silo from automatically stopping itself when the cancel key is pressed.
                .Configure<ProcessExitHandlingOptions>(options => options.FastKillOnProcessExit = false)
                .ConfigureServices(
                    (context, services) =>
                    {
                        services.Configure<ApplicationOptions>(context.Configuration);
                        services.Configure<ClusterOptions>(context.Configuration.GetSection(nameof(ApplicationOptions.Cluster)));
                        services.Configure<StorageOptions>(context.Configuration.GetSection(nameof(ApplicationOptions.Storage)));
                    })
                .UseSiloUnobservedExceptionsHandler()
                .UseLocalhostClustering()
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .AddAzureTableGrainStorageAsDefault(
                    options =>
                    {
                        options.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString;
                        options.ConfigureJsonSerializerSettings = ConfigureJsonSerializerSettings;
                        options.UseJson = true;
                    })
                .UseAzureTableReminderService(options => options.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString)
                .AddSimpleMessageStreamProvider(StreamProviderName.Default)
                .AddAzureTableGrainStorage(
                    "PubSubStore",
                    options =>
                    {
                        options.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString;
                        options.ConfigureJsonSerializerSettings = ConfigureJsonSerializerSettings;
                        options.UseJson = true;
                    })
                .UseIf(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                    x => x.UseLinuxEnvironmentStatistics())
                .UseIf(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                    x => x.UsePerfCounterEnvironmentStatistics())
                .UseDashboard();

        private static Logger CreateLogger(IHost host) =>
            new LoggerConfiguration()
                .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
                .Enrich.WithProperty("Application", GetAssemblyProductName())
                .Enrich.With(new TraceIdEnricher())
                .CreateLogger();

        private static void ConfigureJsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
        }

        private static StorageOptions GetStorageOptions(IConfiguration configuration) =>
            configuration.GetSection(nameof(ApplicationOptions.Storage)).Get<StorageOptions>();

        private static string GetAssemblyProductName() =>
            Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
    }
}
