using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MyHomeMeasure.Models;
using MyHomeMeasure.Services;
using Newtonsoft.Json;

namespace MyHomeMeasure
{
    public class GetStatus
    {
        private readonly HttpClient _client;
        private readonly ICosmosDbService _cosmosDbService;

        public GetStatus(IHttpClientFactory httpClientFactory, ICosmosDbService cosmosDbService)
        {
            _client = httpClientFactory.CreateClient("NatureRemo");
            _cosmosDbService = cosmosDbService;
        }


        [FunctionName("GetStatus")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var result = await _client.GetAsync("/1/devices");

            var contentString = await result.Content.ReadAsStringAsync();
            log.LogInformation($"response:{contentString}");

            if (result.IsSuccessStatusCode)
            {
                var content = JsonConvert.DeserializeObject<List<GetDeviceResponse>>(contentString);

                var now = DateTime.UtcNow;
                var values = content.Select(e => MeasureValue.Create(e, now));

                foreach(var val in values)
                {
                    await _cosmosDbService.AddMeasureValue(val);
                }
                
            }
            

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
