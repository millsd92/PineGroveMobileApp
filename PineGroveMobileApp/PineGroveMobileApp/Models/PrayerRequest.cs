using Newtonsoft.Json;
using System;

namespace PineGroveMobileApp.Models
{
    public partial class PrayerRequest
    {
        [JsonProperty(PropertyName = "PrayerId")]
        public int PrayerId { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }
        [JsonProperty(PropertyName = "PrayerDescription")]
        public string PrayerDescription { get; set; }
        [JsonProperty(PropertyName = "PrayerDate")]
        public DateTime PrayerDate { get; set; }

        public virtual User User { get; set; }
    }
}
