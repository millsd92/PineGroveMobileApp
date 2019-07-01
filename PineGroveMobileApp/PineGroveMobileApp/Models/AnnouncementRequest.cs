using System;
using Newtonsoft.Json;

namespace PineGroveMobileApp.Models
{
    public partial class AnnouncementRequest
    {
        [JsonProperty(PropertyName = "AnnouncementId")]
        public int AnnouncementId { get; set; }
        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }
        [JsonProperty(PropertyName = "Announcement")]
        public string Announcement { get; set; }
        [JsonProperty(PropertyName = "AnnouncementDate")]
        public DateTime AnnouncementDate { get; set; }

        [JsonProperty(PropertyName = "User")]
        public virtual User User { get; set; }
    }
}
