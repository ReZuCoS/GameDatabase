using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<AuthController> _logger;

        public AuthController(DatabaseContext databaseContext, IMemoryCache cache, ILogger<AuthController> logger)
        {
            _databaseContext = databaseContext;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Returns user's salt by login
        /// </summary>
        /// <response code="200">Successfully returned</response>
        /// <response code="400">Validation error</response>
        /// <response code="409">User with this login doesn't exists</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<ActionResult<string>> GetSalt(string login)
        {
            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Login == login);

                if (user == null)
                {
                    return Conflict("Login doesn't exists");
                }

                return Ok(user.Salt);
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }
    }
}
