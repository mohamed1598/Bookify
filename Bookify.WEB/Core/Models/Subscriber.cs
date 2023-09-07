namespace Bookify.WEB.Core.Models
{
    [Index(nameof(NationalId),IsUnique =true)]
    [Index(nameof(MobileNumber),IsUnique =true)]
    [Index(nameof(Email),IsUnique =true)]

    public class Subscriber :BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;
        [MaxLength(100)]
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        [MaxLength(20)]
        public string NationalId { get; set; } = null!;
        [MaxLength(15)]
        public string MobileNumber { get; set; } = null!; 
        public bool HasWhatsapp { get; set; }
        [MaxLength(150)]
        public string Email { get; set; } = null!;
        [MaxLength(500)]
        public string ImageUrl { get; set; } = null!;
        [MaxLength(500)]
        public string ImageThumbnailUrl { get; set; } = null!;
        public int AreaId { get; set; }
        public Area? Area { get; set; }
        public int GovernerateId { get; set; }
        public Governerate? Governerate { get; set; }
        [MaxLength(500)]
        public string Address { get; set; } = null!;
        public bool IsBlackListed { get; set; }
        public ICollection<Subscribtion> Subscribtions { get; set; } = new List<Subscribtion>();
    }
}
