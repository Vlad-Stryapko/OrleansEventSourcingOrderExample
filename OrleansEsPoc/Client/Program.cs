using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace OrleansBasics
{
    public class Program
    {
        static int Main(string[] args) => RunMainAsync().Result;        

        private static async Task<int> RunMainAsync()
        {
            ConfigureLogging();
            try
            {
                using var client = await ConnectClient();                
                Log.Information("Calling the client");
                await DoClientWork(client);
                Log.Information("Call successful");                
                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception while trying to run client");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }

            static void ConfigureLogging()
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }    

        private static async Task DoClientWork(IClusterClient client)
        {
            var friend = client.GetGrain<ITestGrain>(Guid.Parse("A7E9CD40-BD3B-42CE-AE81-0D6591ACA18B"));            
            await friend.Increment();            
        }


        private static async Task<IClusterClient> ConnectClient()
        {
            var invariant = "System.Data.SqlClient"; // for Microsoft SQL Server
            var cs = @"Server=(localdb)\orlpoc;Integrated Security=True;Initial Catalog=orlpoc_Orleans;Application Name=SiloHost;Connect Timeout=30;Min Pool Size=1;Max Pool Size=100;MultipleActiveResultSets=False";

            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Log.Information("Client successfully connected to silo host");
            return client;
        }
    }
}