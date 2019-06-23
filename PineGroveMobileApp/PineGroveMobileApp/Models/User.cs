using Newtonsoft.Json;
using System.Collections.Generic;

namespace PineGroveMobileApp.Models
{
    public partial class User
    {
        public User()
        {
            AnnouncementRequest = new HashSet<AnnouncementRequest>();
            Attendance = new HashSet<Attendance>();
            EventRegistration = new HashSet<EventRegistration>();
            PrayerRequest = new HashSet<PrayerRequest>();
            VisitRequest = new HashSet<VisitRequest>();
        }

        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }
        [JsonProperty(PropertyName = "UserName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "EmailAddress")]
        public string EmailAddress { get; set; }
        [JsonProperty(PropertyName = "PhoneNumber")]
        public long? PhoneNumber { get; set; }

        public virtual ICollection<AnnouncementRequest> AnnouncementRequest { get; set; }
        public virtual ICollection<Attendance> Attendance { get; set; }
        public virtual ICollection<EventRegistration> EventRegistration { get; set; }
        public virtual ICollection<PrayerRequest> PrayerRequest { get; set; }
        public virtual ICollection<VisitRequest> VisitRequest { get; set; }
    }
}
