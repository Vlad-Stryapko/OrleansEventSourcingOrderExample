using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class TestGrain : JournaledGrain<TestGrainState>, ITestGrain
    {
        public override async Task OnActivateAsync()
        {
            //Uncomment to enforce correct order
            await base.OnActivateAsync();
        }

        public async Task Increment()
        {
            Log.Information("Current counter is {counter}", State.Counter);
            this.RaiseEvent(new TestGrainEvent());
            await this.ConfirmEvents();
            Log.Information("Confirmed events");
        }        
    }

    public interface ITestGrain : IGrainWithGuidKey
    {
        Task Increment();
    }

    public class TestGrainState
    {
        public int Counter { get; set; }

        public void Apply(TestGrainEvent _) => Counter += 1;
    }

    public class TestGrainEvent { }
}
