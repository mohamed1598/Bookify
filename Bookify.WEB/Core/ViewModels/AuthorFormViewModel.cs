using Bookify.WEB.Core.consts;

namespace Bookify.WEB.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Author")]
        [Remote("AllowItem", "Authors", AdditionalFields = "Id", ErrorMessage = Errors.Duplicated),
            RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string Name { get; set; } = null!;
    }
}
