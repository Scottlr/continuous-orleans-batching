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

        public async Task StartHeavyOperationBaseline()
        {
            List<int> grainIdentifiers = Enumerable.Range(0, 100).ToList();
            int totalBatchCount = grainIdentifiers.Count;
            int batchSize = Environment.ProcessorCount;
            int batchOffset = 0;

            var totalSw = Stopwatch.StartNew();
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
            totalSw.Stop();
            Console.WriteLine($"Completed heavy operation of {totalBatchCount} items. Took: {totalSw.ElapsedMilliseconds}");
        }


        public async Task StartHeavyOperationStreamed()
        {
            List<int> grainIdentifiers = Enumerable.Range(0, 100).ToList();
            int totalBatchCount = grainIdentifiers.Count;
            int batchSize = Environment.ProcessorCount;
            var totalSw = Stopwatch.StartNew();
            List<Task> grainInvocations = new List<Task>();

            for (int x = 0; x < batchSize; x++)
            {
                var grainId = grainIdentifiers[0];
                grainIdentifiers.RemoveRange(0, 1);
                grainInvocations.Add(ConstructGrainInvocation(grainId));
            }
            try
            {
                while (grainInvocations.Any())
                {
                    await Task.WhenAny(grainInvocations);
                    grainInvocations.RemoveAt(0);
                    if (grainIdentifiers.Any())
                    {
                        grainInvocations.Add(ConstructGrainInvocation(grainIdentifiers[0]));
                        grainIdentifiers.RemoveAt(0);
                    }
                }
            }
            catch(Exception e)
            {

            }
            totalSw.Stop();
            Console.WriteLine($"Completed heavy operation of {totalBatchCount} items. Took: {totalSw.ElapsedMilliseconds}");
        }

        public async Task StartHeavyOperationsAttempt()
        {
            var sw = Stopwatch.StartNew();
            List<Task> grainInvocations = Enumerable
                .Range(0, 10)
                .Select(
                    a => GrainFactory.GetGrain<IWorkerGrain>(a).DoWork(1000000))
                        .ToList();

            await Task.WhenAny(grainInvocations);
            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds} for one grain invocation to complete");
        }


    }
}
