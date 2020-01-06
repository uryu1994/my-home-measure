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
using MyHomeMeasure.Models;
using MyHomeMeasure.Utl;

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
                from = body.From?.ConvertJstToUtc();
                to = body.To?.ConvertJstToUtc();
            }

            var list = await _cosmosDbService.GetMeasureValuesAsync(from, to);

            if (list == null || !list.Any())
            {
                log.LogInformation("NotFound.");
                return new NotFoundResult();
            }

            var result = list.Select(GetMeasureValueListResult.Create);

            var json = JsonConvert.SerializeObject(result);

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

        public class GetMeasureValueListResult
        {
            [JsonProperty(PropertyName = "date")]
            public DateTime Date { get; set; }

            [JsonProperty(PropertyName = "temperature")]
            public decimal Temperature { get; set; }

            [JsonProperty(PropertyName = "humidity")]
            public decimal Humidity { get; set; }

            [JsonProperty(PropertyName = "illumination")]
            public decimal Illumination { get; set; }

            [JsonProperty(PropertyName = "movement")]
            public decimal Movement { get; set; }

            public static GetMeasureValueListResult Create(MeasureValue value)
            {
                return new GetMeasureValueListResult()
                {
                    Date = value.CreatedAt.ConvertUtcToJst(),
                    Temperature = value.Temperature + value.TemperatureOffset,
                    Humidity = value.Humidity + value.HumidityOffset,
                    Illumination = value.Illumination,
                    Movement = value.Movement
                };
            }
        }


    }
}
