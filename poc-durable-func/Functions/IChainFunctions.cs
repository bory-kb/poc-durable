using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace poc_durable_func.Functions
{
    public interface IChainFunctions
    {
        Task<string> SayHello(string name);
    }
}