using CoremakerChallenge.Attributes;
using CoremakerChallenge.Models;
using CoremakerChallenge.Models.Api;
using CoremakerChallenge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoremakerChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IConfiguration _configuration;

        public UserController(IUserManager userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest register)
        {
            var existingUser = _userManager.GetUser(register.Email);
            if (existingUser is not null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "A user with that email already exists." });
            }

            var user = new AppUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = register.Password
            };

            _userManager.Create(user);

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var user = _userManager.GetUser(login.Email);
            if (user is null || !_userManager.IsValidPassword(user, login.Password))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "Invalid credentials" });
            }

            var token = GetToken(new List<Claim>
            {
                new Claim("Email", user.Email)
            });

            return Ok(new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserDetails()
        {
            var userEmail = HttpContext.User.Claims.First(x => x.Type == "Email").Value;

            var user = _userManager.GetUser(userEmail);
            return Ok(new UserDetailsResponse
            {
                Name = user.Name,
                Email = user.Email,
            });
        }

        [Authorization]
        [HttpGet("alternateUserDetails")]
        public IActionResult AlternateUserDetails()
        {
            var user = (AppUser)HttpContext.Items["User"];
            return Ok(new UserDetailsResponse
            {
                Name = user.Name,
                Email = user.Email,
            });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
