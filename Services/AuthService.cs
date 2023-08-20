using backend.Core.Dtos.Auth;
using backend.Core.Entities;
using backend.Interfaces;
using backend.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistUserName = await _userManager.FindByNameAsync(registerDto.UserName);

            if (isExistUserName != null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Email already Exists"
                };


            ApplicationUser newUser = new ApplicationUser()
            {
               
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                VerificationToken = CreateRandomToken()
            };

            var createUserRsult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createUserRsult.Succeeded)
            {
                var errorString = "User Creation Failed Beacause: ";

                foreach (var error in createUserRsult.Errors)
                {
                    errorString += " # " + error.Description;
                }

                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = errorString
                };

            }

            // Add a Default USER Role to all users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            // send an email with the account confirmation link


            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "User Created Successfully"
            };
        }

        public async Task<AuthServiceResponseDto> SeedRolesAsync()
        {
            bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isOwnerRoleExists && isAdminRoleExists && isUserRoleExists)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = true,
                    Message = "Roles Seeding is Already Done"
                };



            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));


            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Role seeding Done Successfully"
            };
        }

        public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            ApplicationUser user;

            if(loginDto.UserName != null)
            {
                user = await _userManager.FindByNameAsync(loginDto.UserName);
            }
            else 
            {
                user = await _userManager.FindByEmailAsync(loginDto.Email);
            }

             

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID",Guid.NewGuid().ToString()),
                new Claim("Email",user.Email),

            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJsonWebToken(authClaims);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = token
            };
        }



        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:secret"]));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["jwt:validissuer"],
                    audience: _configuration["jwt:validaudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<AuthServiceResponseDto> ExistEmailAsync(ExistDataDto data)
        {
            var isExistUserName = await _userManager.FindByEmailAsync(data.Email);

            if (isExistUserName != null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = true,
                    Message = "Email already exist"
                };
            }
            else
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Email not exist"
                };
            }
                
        }

        public async Task<AuthServiceResponseDto> ExistUserNameAsync(ExistDataDto data)
        {
            var isExistEmail = await _userManager.FindByNameAsync(data.UserName);

            if (isExistEmail != null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = true,
                    Message = "Email already exist"
                };
            }
            else
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "UserName not exist"
                };
            }
                
        }
    }
}
