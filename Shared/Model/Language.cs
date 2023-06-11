using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// User interface language class
    /// </summary>
    public class Language
    {
        /// <summary>
        /// User interface language class
        /// </summary>
        public Language()
        {
            Users = new HashSet<User>();
            GameStatuses = new HashSet<GameStatus>();
            GameTranslations = new HashSet<GameTranslation>();
        }

        /// <summary>
        /// Language key, e.g. "EN"
        /// </summary>
        public string Key { get; private set; } = "EN";

        /// <summary>
        /// Language name, e.g. "English"
        /// </summary>
        public string Name { get; private set; } = "English";

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<GameStatus> GameStatuses { get; set; }

        [JsonIgnore]
        public virtual ICollection<GameTranslation> GameTranslations { get; set; }
    }
}
