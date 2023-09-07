namespace Bookify.WEB.Core.ViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public String BookTitle { get; set; } = string.Empty;
        public string? BookThumbnailUrl { get; set; }
        public bool IsAvailableForRental { get; set; }
        public int EditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
