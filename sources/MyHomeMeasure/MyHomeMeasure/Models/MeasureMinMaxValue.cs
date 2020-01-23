using System;
using Newtonsoft.Json;

namespace MyHomeMeasure.Models
{
    public class MeasureMinMaxValue
    {
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "temperature_max")]
        public decimal TemperatureMax { get; set; }

        [JsonProperty(PropertyName = "temperature_min")]
        public decimal TemperatureMin { get; set; }

        [JsonProperty(PropertyName = "humidity_max")]
        public decimal HumidityMax { get; set; }

        [JsonProperty(PropertyName = "humidity_min")]
        public decimal HumidityMin { get; set; }
    }
}
