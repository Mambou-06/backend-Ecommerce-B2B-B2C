using backend.Core.Dtos.Auth;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponseDto> SeedRolesAsync();
        Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthServiceResponseDto> ExistEmailAsync(ExistDataDto data);
        Task<AuthServiceResponseDto> ExistUserNameAsync(ExistDataDto data);
    }
}
