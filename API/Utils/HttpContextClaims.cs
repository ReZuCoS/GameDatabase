using System.Security.Claims;

namespace API.Utils
{
    public class HttpContextClaims
    {
        private readonly IHttpContextAccessor _context;

        public HttpContextClaims(IHttpContextAccessor httpContext)
        {
            _context = httpContext;
        }

        public string? Get(string id)
        {
            return _context.HttpContext?.User.FindFirstValue(id);
        }
    }
}
