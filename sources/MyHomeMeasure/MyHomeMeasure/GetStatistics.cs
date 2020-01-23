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
using MyHomeMeasure.Utl;
using System.Linq;
using MyHomeMeasure.Models;
using System.Net;
using Newtonsoft.Json.Converters;

namespace MyHomeMeasure
{
    public class GetStatistics
    {
        private readonly ICosmosDbService _cosmosDbService;

        public GetStatistics(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public enum StatisticType
        {
            Today,
            Week,
            Month
        }

        [FunctionName("GetStatistics")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetStatistics/{type}")] HttpRequest req,
            string type,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!Enum.TryParse<StatisticType>(type, out var typeEnum))
            {
                throw new ArgumentException("文字列がおかしい");
            }

            var nowDate = DateUtil.JstNow.Date;

            var to = nowDate.AddDays(1).ConvertJstToUtc();

            var from = (typeEnum switch
            {
                StatisticType.Month => nowDate.GetFirstDayOfMonth(),
                StatisticType.Week => nowDate.GetFirstDayOfWeek(),
                StatisticType.Today => nowDate,
                _ => throw new ArgumentNullException("Nullの可能性あり")
            }).ConvertJstToUtc();

            var list = await _cosmosDbService.GetMeasureValuesAsync(from, to);

            // TODO: CosmosDBのLINQにGroupByが入ったらそっちを使いたい
            var result = list.GroupBy(e => e.CreatedAt.ConvertUtcToJst().Date)
                       .Select(e => new MeasureMinMaxValue
                       {
                           Date = e.Key,
                           TemperatureMax = e.Max(t => t.Temperature + t.TemperatureOffset),
                           TemperatureMin = e.Min(t => t.Temperature + t.TemperatureOffset),
                           HumidityMax = e.Max(h => h.Humidity + h.HumidityOffset),
                           HumidityMin = e.Min(h => h.Humidity + h.HumidityOffset),
                       });

            var json = JsonConvert.SerializeObject(result);

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
