namespace Bookify.WEB.Core.Models
{
    public class Rental:BaseModel
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }
        public DateTime StartDate  { get; set; } = DateTime.Now;
        public bool PenaltyPaid { get; set; }
        public ICollection<RentalCopy> RentalCopies { get; set; } = new List<RentalCopy>();
    }
}
