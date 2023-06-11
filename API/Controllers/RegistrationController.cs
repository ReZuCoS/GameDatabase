using API.Model;
using API.Model.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shared.Model;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(DatabaseContext databaseContext, IMemoryCache cache, ILogger<RegistrationController> logger)
        {
            _databaseContext = databaseContext;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Returns salt to auth with lifetime 5 min
        /// </summary>
        /// <response code="200">Successfully generated salt</response>
        /// <response code="400">Validation error</response>
        /// <response code="409">User with this login alredy exists</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<ActionResult<string>> GetSalt(string login)
        {
            try
            {
                if (await _databaseContext.Users.AnyAsync(u => u.Login == login))
                {
                    return Conflict("Login exists");
                }

                var salt = HashGenerator.GenerateSalt(64);

                _cache.Set(login, salt, TimeSpan.FromSeconds(30));
                return Ok(salt);
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        /// <summary>
        /// Adds new user.
        /// Password field must match format: SHA256([password][salt])
        /// </summary>
        /// <response code="200">Successfully added new user</response>
        /// <response code="400">Validation error</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<ActionResult<UserDto>> AddUser(User user)
        {
            user.Salt = _cache.Get<string>(user.Login);

            if (user.Salt == null)
            {
                return NotFound("This login haven't generated salt or it's expired");
            }

            try
            {
                await _databaseContext.Users.AddAsync(user);
                await _databaseContext.SaveChangesAsync();

                _cache.Remove(user.Login);
                _logger.LogInformation("Added new user: {login}", user.Login);

                return Ok(user.Salt);
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }
    }
}
