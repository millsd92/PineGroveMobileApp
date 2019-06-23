using System;

namespace PineGroveMobileApp.Models
{
    public partial class VisitRequest
    {
        public int VisitId { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime VisitDate { get; set; }
        public bool Visited { get; set; }

        public virtual User User { get; set; }
    }
}
