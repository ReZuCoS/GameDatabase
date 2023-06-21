using API.Utils.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Shared.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Utils
{
    public class JwtHandler : IJwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;

        public JwtHandler(IConfiguration configuration, IHttpContextAccessor context)
        {
            _context = context;
            _configuration = configuration;
        }

        public string? GetContextClaim(string key)
        {
            return _context.HttpContext?.User.FindFirstValue(key);
        }

        public string GetRefreshToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:Key")!);
            var tokenLifetime = TimeSpan.FromDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenLifetimeInMonth")!);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(tokenLifetime),
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GetUserToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:Key")!);
            var tokenLifetime = TimeSpan.FromSeconds(_configuration.GetValue<int>("JwtSettings:AuthTokenLifetimeInSeconds")!);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim("login", user.Login),
                new Claim("languageKey", user.LanguageKey),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(tokenLifetime),
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
