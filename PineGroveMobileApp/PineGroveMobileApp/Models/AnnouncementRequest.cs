using System;
using System.Collections.Generic;

namespace PineGroveMobileApp.Models
{
    public partial class AnnouncementRequest
    {
        public int AnnouncementId { get; set; }
        public int UserId { get; set; }
        public string Announcement { get; set; }
        public DateTime AnnouncementDate { get; set; }

        public virtual User User { get; set; }
    }
}
