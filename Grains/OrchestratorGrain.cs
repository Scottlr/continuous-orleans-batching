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
        private Random rnd;

        public override Task OnActivateAsync()
        {
            rnd = new Random();
            return TaskDone.Done;
        }

        private Task ConstructGrainInvocation(int grainId)
        {
            var waitTime = rnd.Next(10, 250);
            if (grainId % 7 == 0)
            {
                waitTime = rnd.Next(3000, 5000);
            }
            return GrainFactory.GetGrain<IWorkerGrain>(grainId).DoWork(waitTime);
        }

        public async Task StartHeavyOperation()
        {
            Random r = new Random();
            List<int> grainIdentifiers = Enumerable.Range(0, 100).ToList();
            int totalBatchCount = grainIdentifiers.Count;
            int batchSize = Environment.ProcessorCount;
            int batchOffset = 0;
            try
            {
                while (grainIdentifiers.Any())
                {
                    var timer = Stopwatch.StartNew();
                    List<int> grainIdBatch = grainIdentifiers.Take(batchSize).ToList();
                    grainIdentifiers.RemoveRange(0, batchSize);

                    List<Task> tasksInBatch = grainIdBatch.Select(a => ConstructGrainInvocation(a)).ToList();
                    await Task.WhenAll(tasksInBatch);
                    timer.Stop();
                    batchOffset++;

                    Console.WriteLine($"Batch completed. Completed/Total: {batchOffset * batchSize}/{totalBatchCount}, took: {timer.ElapsedMilliseconds}");

                }
            }
            catch(Exception e)
            {
               
            }

        }
    }
}
