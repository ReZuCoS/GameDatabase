using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    /// <summary>
    /// User totp access storage
    /// </summary>
    public class UserTotpAccessDto
    {
        /// <summary>
        /// User totp access storage constructor
        /// </summary>
        public UserTotpAccessDto(string authorization, string refreshToken, string totpKey)
        {
            TotpKey = totpKey;
            RefreshToken = refreshToken;
            Authorization = authorization;
        }

        /// <summary>
        /// User's totp key
        /// </summary>
        [Required]
        public string TotpKey { get; set; }

        /// <summary>
        /// User's access token
        /// </summary>
        [Required]
        public string Authorization { get; set; }

        /// <summary>
        /// User's refresh token
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}
