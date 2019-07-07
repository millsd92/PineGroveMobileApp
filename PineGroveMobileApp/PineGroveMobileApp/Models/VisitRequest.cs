using Newtonsoft.Json;
using System;

namespace PineGroveMobileApp.Models
{
    public partial class VisitRequest
    {
        [JsonProperty(PropertyName = "VisitId")]
        public int VisitId { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }
        [JsonProperty(PropertyName = "Reason")]
        public string Reason { get; set; }
        [JsonProperty(PropertyName = "RequestDate")]
        public DateTime RequestDate { get; set; }
        [JsonProperty(PropertyName = "VisitDate")]
        public DateTime? VisitDate { get; set; }
        [JsonProperty(PropertyName = "Visited")]
        public bool Visited { get; set; }
        [JsonProperty(PropertyName = "AddressLineOne")]
        public string AddressLineOne { get; set; }
        [JsonProperty(PropertyName = "AddressLineTwo")]
        public string AddressLineTwo { get; set; }
        [JsonProperty(PropertyName = "ZipCode")]
        public string ZipCode { get; set; }
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        public virtual User User { get; set; }
    }
}
