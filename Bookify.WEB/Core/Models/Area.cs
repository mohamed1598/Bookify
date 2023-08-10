namespace Bookify.WEB.Core.Models
{
    [Index(nameof(Name),nameof(GovernerateId),IsUnique =true)]
    public class Area:BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public int GovernerateId { get; set; }
        public Governerate? Governerate { get; set; }
    }
}
 