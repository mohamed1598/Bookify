namespace Bookify.WEB.Core.Models
{
    public class Governerate:BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}
