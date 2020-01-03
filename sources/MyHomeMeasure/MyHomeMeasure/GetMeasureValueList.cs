using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MyHomeMeasure.Services;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Net;

namespace MyHomeMeasure
{
    public class GetMeasureValueList
    {

        private readonly ICosmosDbService _cosmosDbService;

        public GetMeasureValueList(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [FunctionName("GetMeasureValueList")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            DateTime? from = null;
            DateTime? to = null;

            var bodyString = await req.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(bodyString))
            {

                log.LogInformation($"body: {bodyString}");
                var body = JsonConvert.DeserializeObject<GetMeasureValueListRequest>(bodyString);
                from = body.From;
                to = body.To;
            }

            var list = await _cosmosDbService.GetMeasureValuesAsync(from, to);

            if (list == null || !list.Any())
            {
                log.LogInformation("NotFound.");
                return new NotFoundResult();
            }

            var json = JsonConvert.SerializeObject(list);

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public class GetMeasureValueListRequest
        {
            [JsonProperty(PropertyName = "from")]
            public DateTime? From { get; set; }

            [JsonProperty(PropertyName = "to")]
            public DateTime? To { get; set; }
        }
    }
}
