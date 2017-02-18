using System;
using System.Threading.Tasks;
using Orleans;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Grains
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
	public class OrchestratorGrain : Grain, IOrchestratorGrain
    {
        public async Task StartHeavyOperation()
        {
            Random r = new Random();
            List<int> grainIdentifiers = Enumerable.Range(0, 100).ToList();
            const int batchSize = 9;
            int batchOffset = 0;

            while(grainIdentifiers.Any())
            {
                var timer = Stopwatch.StartNew();
                List<int> grainIdBatch = grainIdentifiers.Take(batchSize).ToList();
                grainIdentifiers.RemoveRange(batchOffset, batchSize);
                List<Task> tasksInBatch = new List<Task>();
                foreach (int i in grainIdBatch)
                {
                    tasksInBatch.Add(GrainFactory.GetGrain<IWorkerGrain>(i).DoWork(r.Next(10, 100)));
                }
                tasksInBatch.Add(GrainFactory.GetGrain<IWorkerGrain>((batchOffset + batchSize) + 1).DoWork(r.Next(4000, 4500)));

                await Task.WhenAll(tasksInBatch);
                batchOffset += batchSize;
                timer.Stop();
                Console.WriteLine($"Batch completed, took: {timer.ElapsedMilliseconds}");
            }

        }
    }
}
