using Microsoft.AspNetCore.Identity;

namespace Bookify.WEB.Core.Models
{
    [Index(nameof(Email),IsUnique = true)]
    [Index(nameof(UserName),IsUnique = true)]
    public class ApplicationUser :IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; }= string.Empty;

        public bool IsDeleted { get; set; }
        public String? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }
        public String? LastUpdatedById { get; set; } 
    }
}
