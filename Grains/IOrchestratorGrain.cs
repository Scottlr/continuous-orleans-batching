using System.Threading.Tasks;
using Orleans;

namespace Grains
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
	public interface IOrchestratorGrain : IGrainWithGuidKey
    {
        Task StartHeavyOperationBaseline();
        Task StartHeavyOperationStreamed();
        Task StartHeavyOperationsAttempt();
    }
}
