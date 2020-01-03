using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyHomeMeasure.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetDeviceResponse
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "temperature_offset")]
        public decimal TemperatureOffset { get; set; }

        [JsonProperty(PropertyName = "humidity_offset")]
        public decimal HumidityOffset { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonProperty(PropertyName = "mac_address")]
        public string MacAddress { get; set; }

        [JsonProperty(PropertyName = "serial_number")]
        public string SerialNumber { get; set; }

        [JsonProperty(PropertyName = "users")]
        public List<User> Users { get; set; }

        [JsonProperty(PropertyName = "newest_events")]
        public Event NewestEvents { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Event
    {
        /// <summary>
        /// 温度
        /// </summary>
        [JsonProperty(PropertyName = "te")]
        public ValueInfo Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        [JsonProperty(PropertyName = "hu")]
        public ValueInfo Humidity { get; set; }

        /// <summary>
        /// 照度
        /// </summary>
        [JsonProperty(PropertyName = "il")]
        public ValueInfo Illumination { get; set; }

        /// <summary>
        /// 動く熱源
        /// </summary>
        [JsonProperty(PropertyName = "mo")]
        public ValueInfo Movement { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "superuser")]
        public bool Superuser { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ValueInfo
    {
        /// <summary>
        /// 値
        /// </summary>
        [JsonProperty(PropertyName = "val")]
        public decimal Value { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }
    }
}