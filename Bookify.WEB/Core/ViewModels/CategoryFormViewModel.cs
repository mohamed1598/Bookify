using Microsoft.AspNetCore.Mvc;

namespace Bookify.WEB.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage ="Category Name cann't be more than 100 chars")]
        [Remote("AllowItem","Categories",AdditionalFields ="Id",ErrorMessage ="Category With the same name is already exists")]
        public string Name { get; set; } = null!;
    }
}
