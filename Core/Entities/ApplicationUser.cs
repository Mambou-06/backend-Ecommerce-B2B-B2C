using Microsoft.AspNetCore.Identity;

namespace backend.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string VerificationToken { get; set; } = string.Empty;
        public DateTime? VerifiedAt { get; set; }
        public string PasswordRestToken { get; set; } = string.Empty;
        public DateTime? ResetTokenExpires { get; set; }

        public static implicit operator ApplicationBuilder?(ApplicationUser? v)
        {
            throw new NotImplementedException();
        }
    }
}
