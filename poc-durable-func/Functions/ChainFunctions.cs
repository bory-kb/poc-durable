using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DurableTask.TypedProxy;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace poc_durable_func.Functions
{

    public class ChainFunctions : IChainFunctions
    {

        [FunctionName("ChainFunctions")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            var proxy = context.CreateActivityProxy<IChainFunctions>();

            // Replace "hello" with the name of your Durable Activity Function.
            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await proxy.SayHello("Tokyo"));
            outputs.Add(await proxy.SayHello("Seattle"));
            outputs.Add(await proxy.SayHello("London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(nameof(SayHello))]
        public Task<string> SayHello([ActivityTrigger] string name)
        {
            return Task.FromResult($"Hello {name}");
        }

        [FunctionName(nameof(HttpStart))]
        public  async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ChainFunctions", null);

            return await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}