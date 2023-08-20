using System.ComponentModel.DataAnnotations;

namespace backend.Core.Dtos.Auth
{
    public class LoginDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
    }
}
