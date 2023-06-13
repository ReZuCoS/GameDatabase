using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// Game database user class
    /// </summary>
    public class User
    {
        /// <summary>
        /// Game database user class
        /// </summary>
        public User()
        {
            UserGames = new HashSet<UserGame>();
        }

        /// <summary>
        /// User login
        /// </summary>
        [Required]
        [DefaultValue("login")]
        public string Login { get; set; } = default!;

        /// <summary>
        /// User password.
        /// Field must match format: SHA256([password][salt])
        /// </summary>
        [Required]
        [MinLength(64)]
        [MaxLength(64)]
        [DefaultValue("5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8")]
        public string Password { get; set; } = default!;

        /// <summary>
        /// Password hash salt
        /// </summary>
        [JsonIgnore]
        public string Salt { get; set; } = "";

        /// <summary>
        /// User TOTP (Time-based ope-time password) secret key
        /// </summary>
        [JsonIgnore]
        public string? TotpKey { get; set; }

        /// <summary>
        /// User TOTP (Time-based ope-time password) recovery codes
        /// </summary>
        [JsonIgnore]
        public string[]? TotpRecoveries { get; set; }

        /// <summary>
        /// User profile link
        /// </summary>
        [DefaultValue(null)]
        public string? ProfileImage { get; set; }

        /// <summary>
        /// User preferred language key.
        /// Default = "EN"
        /// </summary>
        [Required]
        [DefaultValue("EN")]
        public string LanguageKey { get; set; } = "EN";
        
        [JsonIgnore]
        public virtual Language? Language { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGame> UserGames { get; set; }
    }
}
