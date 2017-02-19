using System;
using System.Threading.Tasks;
using Orleans;

namespace Grains
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
	public class WorkerGrain : Grain, IWorkerGrain
    {
        public async Task DoWork(int delay)
        {
            Console.WriteLine($"Id: {this.GetPrimaryKey()}, Duration: {delay}");
            await Task.Delay(delay);
        }
    }
}
