using System.ComponentModel.DataAnnotations;

namespace backend.Core.Dtos.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required"), MinLength(6, ErrorMessage = "Please enter at least 6 characteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required"), Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
