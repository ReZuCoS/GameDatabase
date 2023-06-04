using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            Language = new Language();
            UserGames = new HashSet<UserGame>();
        }

        /// <summary>
        /// User ID
        /// </summary>
        public int ID { get; private set; }
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
        public string? Salt { get; private set; }
        /// <summary>
        /// User TOTP (Time-based ope-time password) secret key
        /// </summary>
        [JsonIgnore]
        public string? TotpKey { get; private set; }
        /// <summary>
        /// User TOTP (Time-based ope-time password) recovery codes
        /// </summary>
        [JsonIgnore]
        public string[]? TotpRecoveries { get; private set; }
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
        public string LanguageKey { get; set; }
        /// <summary>
        /// User preferred language.
        /// Default = "EN"
        /// </summary>
        [JsonIgnore]
        public virtual Language Language { get; set; } = new();

        [JsonIgnore]
        public virtual ICollection<UserGame> UserGames { get; set; }
    }
}
