namespace Bookify.WEB.Core.ViewModels
{
    public class RentalFormViewModel
    {
        public string SubscriberKey { get; set; } = null!;
        public IList<int> SelectedCopies { get; set; } = new List<int>();
    }
}
