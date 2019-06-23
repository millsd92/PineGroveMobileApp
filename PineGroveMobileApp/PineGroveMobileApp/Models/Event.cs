using System;
using System.Collections.Generic;

namespace PineGroveMobileApp.Models
{
    public partial class Event
    {
        public Event()
        {
            EventRegistration = new HashSet<EventRegistration>();
        }

        public int EventId { get; set; }
        public string EventDescription { get; set; }
        public byte[] Picture { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Address { get; set; }
        public int? MaxAttendees { get; set; }
        public int CurrentAttendees { get; set; }

        public virtual ICollection<EventRegistration> EventRegistration { get; set; }
    }
}
