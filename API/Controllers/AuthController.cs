using API.Model;
using API.Model.DataTransferObjects;
using API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(
            DatabaseContext databaseContext,
            IMemoryCache cache,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("token")]
        public async Task<ActionResult<string>> GetToken(string login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("gLVJUYSzIugLVJUYSzIugLVJUYSzIugLVJUYSzIugLVJUYSzIugLVJUYSzIugLVJUYSzIu");

            var claims = new List<Claim>()
            {
                new Claim("login", login)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);

            return Ok(jwt);
        }

        [HttpGet]
        [Route("register")]
        public async Task<ActionResult<string>> GetSalt(string login)
        {
            var salt = _cache.Get<string>(login) ?? "";

            if (!string.IsNullOrEmpty(salt))
            {
                return Ok(salt);
            }

            try
            {
                if (await _databaseContext.Users.AnyAsync(u => u.Login == login))
                {
                    return Conflict("Login exists");
                }

                salt = HashGenerator.GenerateSalt(64);

                _cache.Set(login, salt, TimeSpan.FromMinutes(5));
                return Ok(salt);
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserDto>> AddUser(User user)
        {
            user.Salt = _cache.Get<string>(user.Login) ?? "";

            if (string.IsNullOrEmpty(user.Salt))
            {
                return NotFound("This login haven't generated salt or it's expired");
            }

            try
            {
                await _databaseContext.Users.AddAsync(user);
                await _databaseContext.SaveChangesAsync();

                _cache.Remove(user.Login);
                _logger.LogInformation("Added new user: {login}", user.Login);

                return Ok(new UserDto(user));
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        [HttpGet]
        [Route("auth")]
        public async Task<ActionResult<ServerSecretDto>> GetAuthData(string login)
        {
            var serverSecret = _cache.Get<ServerSecretDto>(login);

            if (serverSecret != null)
            {
                return Ok(serverSecret);
            }

            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Login == login);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var challenge = HashGenerator.GenerateSalt(64);

                serverSecret = new ServerSecretDto()
                {
                    Salt = user.Salt,
                    Challenge = challenge
                };

                _cache.Set(user.Login, serverSecret, TimeSpan.FromMinutes(5));

                return Ok(serverSecret);
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> VerifySecret(ClientSecretDto clientSecret)
        {
            try
            {
                var serverSecret = _cache.Get<ServerSecretDto>(clientSecret.Login);
                
                if (serverSecret == null)
                {
                    return Conflict("This login haven't generated challenge or it's expired");
                }
                
                var user = await _databaseContext.Users.FirstAsync(u => u.Login == clientSecret.Login);

                var calculationResult = HashGenerator.GetSHA256Hash(user.Password + serverSecret.Challenge + clientSecret.Challenge);

                if (clientSecret.HashSecret != calculationResult)
                {
                    return NotFound("Wrong username or password");
                }

                _cache.Remove(clientSecret.Login);
                _logger.LogInformation("User authenticated with password: {login}", user.Login);
                return Ok(new UserDto(user));
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        [HttpPost]
        [Route("/api/2fa")]
        public async Task<ActionResult<UserDto>> GetUserData(ClientSecretDto clientSecret)
        {
            try
            {
                return null;
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }


    }
}
