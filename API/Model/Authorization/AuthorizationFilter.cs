using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Model.Authorization
{
    public class AuthorizationFilter : /*Attribute,*/ IAuthorizationFilter
    {
        private readonly IConfiguration _config;

        public AuthorizationFilter(IConfiguration config)
        {
            _config = config;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                context.Result = new UnauthorizedObjectResult("Authorization is not presented in request headers");
                return;
            }

            var tokena = GenerateToken("rez");

            AttachUserToContext(context, tokena);
        }

        private string GenerateToken(string login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _config["Jwt:Secret"];
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("login", login)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void AttachUserToContext(AuthorizationFilterContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecret = _config["Jwt:Secret"];
                var key = Encoding.ASCII.GetBytes(jwtSecret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var login = jwtToken.Claims.First(x => x.Type == "login").Value;

                context.HttpContext.Items["login"] = login;
            }
            catch (Exception ex)
            {
                context.Result = new UnauthorizedObjectResult("Invalid authorization header value");
            }
        }
    }
}
