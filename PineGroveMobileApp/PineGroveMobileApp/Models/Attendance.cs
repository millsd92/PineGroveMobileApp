namespace PineGroveMobileApp.Models
{
    public partial class Attendance
    {
        public int AttendanceId { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int Guests { get; set; }

        public virtual Service Service { get; set; }
        public virtual User User { get; set; }
    }
}
