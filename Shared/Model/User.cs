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
        public string Login { get; set; } = default!;
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; } = default!;
        /// <summary>
        /// Password hash salt
        /// </summary>
        public string Salt { get; set; } = default!;
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = default!;
        /// <summary>
        /// User profile link
        /// </summary>
        public string? ProfileImage { get; set; }
        /// <summary>
        /// User verification status
        /// </summary>
        public bool IsVerified { get; set; } = false;
        /// <summary>
        /// User preferred language key.
        /// Default = "EN"
        /// </summary>
        [JsonIgnore]
        public string LanguageKey { get; set; } = "EN";
        /// <summary>
        /// User preferred language.
        /// Default = "EN"
        /// </summary>
        public virtual Language Language { get; set; } = default!;

        [JsonIgnore]
        public virtual ICollection<UserGame> UserGames { get; set; }
    }
}
