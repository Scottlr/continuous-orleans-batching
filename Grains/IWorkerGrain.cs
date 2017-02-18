using System.Threading.Tasks;
using Orleans;

namespace Grains
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
	public interface IWorkerGrain : IGrainWithIntegerKey
    {
        Task DoWork(int delay);
    }
}
