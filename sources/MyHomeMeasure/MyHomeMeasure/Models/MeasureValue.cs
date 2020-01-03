using System;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace MyHomeMeasure.Models
{
    public class MeasureValue
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "temperature_offset")]
        public decimal TemperatureOffset { get; set; }

        [JsonProperty(PropertyName = "humidity_offset")]
        public decimal HumidityOffset { get; set; }

        [JsonProperty(PropertyName = "temperature")]
        public decimal Temperature { get; set; }

        [JsonProperty(PropertyName = "humidity")]
        public decimal Humidity { get; set; }

        [JsonProperty(PropertyName = "illumination")]
        public decimal Illumination { get; set; }

        [JsonProperty(PropertyName = "movement")]
        public decimal Movement { get; set; }

        [JsonIgnore]
        public PartitionKey PertitionKey => new PartitionKey(Id);

        public static MeasureValue Create(GetDeviceResponse response, DateTime now)
        {
            return new MeasureValue() {
                Id = CreateId(response.Id.ToString(), now),
                CreatedAt = now,
                TemperatureOffset = response.TemperatureOffset,
                HumidityOffset = response.HumidityOffset,
                Temperature = response.NewestEvents.Temperature.Value,
                Humidity = response.NewestEvents.Humidity.Value,
                Illumination = response.NewestEvents.Illumination.Value,
                Movement = response.NewestEvents.Movement.Value
            };
        }

        private static string CreateId(string device_id, DateTime now)
        {
            return $"{device_id}_{now :yyyyMMdd-HHmmss}";
        }
    }
}
