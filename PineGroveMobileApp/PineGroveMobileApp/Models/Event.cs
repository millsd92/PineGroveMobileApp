using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Drawing;
using Xamarin.Forms;

namespace PineGroveMobileApp.Models
{
    public partial class Event
    {
        public Event()
        {
            EventRegistration = new HashSet<EventRegistration>();
        }

        [JsonProperty(PropertyName = "EventId")]
        public int EventId { get; set; }
        [JsonProperty(PropertyName = "EventTitle")]
        public string EventTitle { get; set; }
        [JsonProperty(PropertyName = "EventDescription")]
        public string EventDescription { get; set; }
        [JsonProperty(PropertyName = "Picture")]
        public byte[] Picture { get; set; }
        [JsonProperty(PropertyName = "StartTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty(PropertyName = "EndTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "MaxAttendees")]
        public int? MaxAttendees { get; set; }
        [JsonProperty(PropertyName = "CurrentAttendees")]
        public int CurrentAttendees { get; set; }

        public virtual ICollection<EventRegistration> EventRegistration { get; set; }
    }
}
