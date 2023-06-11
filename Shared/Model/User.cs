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
        public string Login { get; set; } = default!;

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// Password hash salt
        /// </summary>
        [JsonIgnore]
        public string? Salt { get; set; }

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
