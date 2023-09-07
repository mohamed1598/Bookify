namespace Bookify.WEB.Core.ViewModels
{
    public class SubscribtionViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        private string status =string.Empty;
        public String Status
        {
            get
            {
                return !string.IsNullOrEmpty(status)?status: DateTime.Today > EndDate ? "Expired" : DateTime.Today < StartDate ? string.Empty : "Active";
            }
            set{
                status = value;
            }
        }
    }
}
