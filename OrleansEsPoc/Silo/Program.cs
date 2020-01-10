using Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using Serilog;

namespace Silo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ConfigureLogging();
            return RunMainAsync().Result;

            static void ConfigureLogging()
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var invariant = "System.Data.SqlClient"; // for Microsoft SQL Server
            var cs = @"Server=(localdb)\orlpoc;Integrated Security=True;Initial Catalog=orlpoc_Orleans;Application Name=SiloHost;Connect Timeout=30;Min Pool Size=1;Max Pool Size=100;MultipleActiveResultSets=False";

            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .AddAdoNetGrainStorageAsDefault(opts =>
                {
                    opts.ConnectionString = cs;
                    opts.Invariant = invariant;
                })
                .AddLogStorageBasedLogConsistencyProvider()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(TestGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
