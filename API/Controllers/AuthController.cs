﻿using API.Model;
using API.Model.DataTransferObjects;
using API.Utils;
using API.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OtpNet;
using Shared.Model;

namespace API.Controllers
{
    //TODO: Bearer token refreshing
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IJwtHandler _jwtHandler;
        private readonly ILogger<AuthController> _logger;
        private readonly DatabaseContext _databaseContext;

        public AuthController(
            DatabaseContext databaseContext,
            IMemoryCache cache,
            IJwtHandler jwtHandler,
            ILogger<AuthController> logger)
        {
            _cache = cache;
            _logger = logger;
            _jwtHandler = jwtHandler;
            _databaseContext = databaseContext;
        }

        /// <summary>
        /// Returns server secret wich includes random hash challenge and client's salt
        /// </summary>
        /// <param name="login">User's login</param>
        /// <response code="200">Successfully returned server secret</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Unexpected server error</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ServerSecretDto>> GetServerSecret(string login)
        {
            var serverSecret = _cache.Get<ServerSecretDto>(login);

            if (serverSecret is not null)
            {
                return Ok(serverSecret);
            }

            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Login == login);

                if (user is null)
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

        /// <summary>
        /// If challenge was found in cache and calculations is right, returns user data
        /// </summary>
        /// <response code="200">Returned user data. If Authorization and RefreshToken is empty, continue with 2FA code</response>
        /// <response code="500">Unexpected server error</response>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserData(ClientSecretDto clientSecret)
        {
            try
            {
                var serverSecret = _cache.Get<ServerSecretDto>(clientSecret.Login);
                
                if (serverSecret is null)
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
                _logger.LogInformation("User authenticated with password: {login}, OTP required: {otp}", user.Login, user.TotpKey is not null);

                var authorization = _jwtHandler.GetUserToken(user);
                
                if (user.RefreshToken is null)
                {
                    user.RefreshToken = _jwtHandler.GetRefreshToken();

                    _databaseContext.Users.Update(user);
                    _databaseContext.SaveChanges();
                }

                if (user.TotpKey is not null)
                {
                    _cache.Set(
                        user.Login,
                        new UserTotpAccessDto(authorization, user.RefreshToken, user.TotpKey),
                        TimeSpan.FromMinutes(5));
                }

                return Ok(new UserDto()
                {
                    ProfileImage = user.ProfileImage,
                    RefreshToken = user.RefreshToken,
                    Authorization = user.TotpKey is null ? authorization : string.Empty
                });
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        /// <summary>
        /// Generates salt if login not exists
        /// </summary>
        /// <param name="login">User's login</param>
        /// <response code="200">Successfully returned salt</response>
        /// <response code="409">Login exists</response>
        /// <response code="500">Unexpected server error</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult<string>> GenerateSalt(string login)
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
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }

            salt = HashGenerator.GenerateSalt(64);

            _cache.Set(login, salt, TimeSpan.FromMinutes(5));
            return Ok(salt);
        }

        /// <summary>
        /// If salt was found in cache, adds new user to database and returns it's data
        /// </summary>
        /// <response code="200">Successfully added new user</response>
        /// <response code="404">Salt not found in cache</response>
        /// <response code="500">Unexpected server error</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult<UserDto>> AddUser(User user)
        {
            user.Salt = _cache.Get<string>(user.Login) ?? string.Empty;

            if (string.IsNullOrEmpty(user.Salt))
            {
                return NotFound("This login haven't generated salt or it's expired");
            }

            try
            {
                user.RefreshToken = _jwtHandler.GetRefreshToken();

                await _databaseContext.Users.AddAsync(user);
                await _databaseContext.SaveChangesAsync();

                _cache.Remove(user.Login);
                _logger.LogInformation("Added new user: {login}", user.Login);

                return Ok(new UserDto
                {
                    Authorization = _jwtHandler.GetUserToken(user),
                    RefreshToken = user.RefreshToken,
                    ProfileImage = user.ProfileImage
                });
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        /// <summary>
        /// Generates 2FA for user is he hasn't
        /// </summary>
        /// <response code="200">Successfully generated a TOTP code with a lifetime of 5 minutes. Requires POST confirmation</response>
        /// <response code="400">Invalid access token</response>
        /// <response code="404">User not found or TOTP alredy generated</response>
        /// <response code="500">Unexpected server error</response>
        [HttpGet]
        [Authorize]
        [Route("2fa")]
        public async Task<ActionResult<TwoFactorDto>> Get2FA()
        {
            var login = _jwtHandler.GetContextClaim("login");

            if (login is null)
            {
                return BadRequest("Access token is invalid");
            }

            var totpData = _cache.Get<TwoFactorDto>(login);

            if (totpData is not null)
            {
                return Ok(totpData);
            }

            try
            {
                if (!await _databaseContext.Users.AnyAsync(u => u.Login == login && u.TotpKey == null))
                {
                    return NotFound("User not found or alredy has generated totp key");
                }
            }
            catch(Exception)
            {
                return Problem("Internal server error");
            }

            totpData = new TwoFactorDto();

            _cache.Set(login, totpData, TimeSpan.FromMinutes(5));

            return Ok(totpData);
        }

        /// <summary>
        /// Saves generated user's TOTP key
        /// </summary>
        /// <param name="code">TOTP code</param>
        /// <response code="200">Successfully saved TOTP code</response>
        /// <response code="400">Invalid access token or TOTP code</response>
        /// <response code="404">User not found or TOTP alredy generated; TOTP data key not generated by GET</response>
        /// <response code="500">Unexpected server error</response>
        [HttpPost]
        [Authorize]
        [Route("2fa")]
        public async Task<ActionResult> Set2FA(string code)
        {
            var login = _jwtHandler.GetContextClaim("login");

            if (login is null)
            {
                return BadRequest("Access token is invalid");
            }

            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Login == login);

                if (user is null)
                {
                    return NotFound("User not found");
                }

                var totp = _cache.Get<TwoFactorDto>(login);

                if (totp is null)
                {
                    return NotFound("This login haven't generated key or it's expired");
                }

                var isValid = new Totp(Base32Encoding.ToBytes(totp.Key))
                    .VerifyTotp(code, out long _, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (!isValid)
                {
                    return BadRequest("Totp code is invalid");
                }

                _cache.Remove(login);
                user.TotpKey = totp.Key;
                user.TotpRecoveries = totp.Recoveries;
                _databaseContext.Users.Update(user);
                await _databaseContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        /// <summary>
        /// Verifies request 2FA totp code
        /// </summary>
        /// <param name="login">User's login</param>
        /// <param name="code">TOTP code</param>
        /// <response code="200">Successfully returned UserToken</response>
        /// <response code="404">Login haven't generated token or it's expired</response>
        /// <response code="500">Unexpected server error</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("2fa/verify")]
        public async Task<ActionResult<string>> Verify2Factor(string login, string code)
        {
            var accessData = _cache.Get<UserTotpAccessDto>(login);

            if (accessData is null)
            {
                return NotFound("This login haven't generated totp key or it's expired");
            }

            var isValid = new Totp(Base32Encoding.ToBytes(accessData.TotpKey))
                .VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (isValid)
            {
                _cache.Remove(login);
                return Ok(accessData.Authorization);
            }

            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Login == login);

                if (user?.TotpRecoveries is null || !user.TotpRecoveries.Contains(code))
                {
                    return BadRequest("Invalid TOTP code");
                }

                user.TotpRecoveries = user.TotpRecoveries.Where(r => r != code).ToArray();
                _databaseContext.Users.Update(user);
                await _databaseContext.SaveChangesAsync();
                _cache.Remove(login);
                return Ok(accessData.Authorization);
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }

        /// <summary>
        /// Updates user's bearer token
        /// </summary>
        /// <param name="refreshToken">User's refresh token</param>
        /// <response code="200"></response>
        [HttpPost]
        [Authorize]
        [Route("refresh_token")]
        public async Task<ActionResult<string>> RefreshToken([FromBody] string refreshToken)
        {
            var login = _jwtHandler.GetContextClaim("login");
            
            if (login is null)
            {
                return BadRequest("Access token is invalid");
            }

            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Login == login);
                
                if (user is null)
                {
                    return NotFound("User not found");
                }
                
                var totp = _cache.Get<TwoFactorDto>(login);
                
                if (totp is null)
                {
                    return NotFound("This login haven't generated key or it's expired");
                }
                //
                //var isValid = new Totp(Base32Encoding.ToBytes(totp.Key))
                //    .VerifyTotp(code, out long _, VerificationWindow.RfcSpecifiedNetworkDelay);
                //
                //if (!isValid)
                //{
                //    return BadRequest("Totp code is invalid");
                //}
                //
                //_cache.Remove(login);
                //user.TotpKey = totp.Key;
                //user.TotpRecoveries = totp.Recoveries;
                //_databaseContext.Users.Update(user);
                //await _databaseContext.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception)
            {
                return Problem("Internal server error");
            }
        }
    }
}
