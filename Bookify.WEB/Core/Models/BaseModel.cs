namespace Bookify.WEB.Core.Models
{
    public class BaseModel
    {

        public bool IsDeleted { get; set; }
        public String? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public String? LastUpdatedById { get; set; }
        public ApplicationUser? LastUpdatedBy { get; set; }
    }
}
