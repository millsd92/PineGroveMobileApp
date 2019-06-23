namespace PineGroveMobileApp.Models
{
    public partial class EventRegistration
    {
        public int EventRegistrationId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int Guests { get; set; }

        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
    }
}
