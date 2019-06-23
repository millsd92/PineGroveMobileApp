using System;
using System.Collections.Generic;

namespace PineGroveMobileApp.Models
{
    public partial class Service
    {
        public Service()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int ServiceId { get; set; }
        public DateTime ServiceDate { get; set; }
        public string ServiceDescription { get; set; }

        public virtual ICollection<Attendance> Attendance { get; set; }
    }
}
