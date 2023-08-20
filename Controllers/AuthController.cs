using backend.Core.Dtos.Auth;
using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Route For Seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> seedRoles()
        {
            var seedRoles = await _authService.SeedRolesAsync();
            return Ok(seedRoles);
        }

        // Route Register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            var registerResult = await _authService.RegisterAsync(registerDto);

            if (registerResult.IsSucceed)
                return Ok(registerResult);

            return BadRequest(registerResult);
        }

        // Route Login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult.IsSucceed)
                return Ok(loginResult);

            return Unauthorized(loginResult);
        }

        // Route Login
        [HttpPost]
        [Route("EmailExist")]
        public async Task<IActionResult> ExistEmail([FromBody] ExistDataDto data)
        {
            var Result = await _authService.ExistEmailAsync(data);

            if (!Result.IsSucceed)
                return Ok(Result);

            return Unauthorized(Result);
        }

        // Route Login
        [HttpPost]
        [Route("UserNameExist")]
        public async Task<IActionResult> ExistUserName([FromBody] ExistDataDto data)
        {
            var Result = await _authService.ExistUserNameAsync(data);

            if (!Result.IsSucceed)
                return Ok(Result);

            return Unauthorized(Result);
        }
    }
}
