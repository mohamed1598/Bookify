using Bookify.WEB.Core.consts;

namespace Bookify.WEB.Core.ViewModels
{
    public class ResetPasswordFormViewModel
    {
        public string? Id { get; set; }
        [DataType(DataType.Password), StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 8), Display(Name = "Password")]
        public string Password { get; set; } = null!;


        [DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch),
            RegularExpression(RegexPatterns.Password, ErrorMessage = Errors.WeakPassword)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
