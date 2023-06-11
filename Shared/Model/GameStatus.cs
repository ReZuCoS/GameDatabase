using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// Game database game status class
    /// </summary>
    public class GameStatus
    {
        /// <summary>
        /// Game database game status class
        /// </summary>
        public GameStatus()
        {
            UserGames = new HashSet<UserGame>();
        }

        /// <summary>
        /// Game status entry ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Game status entry name
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Game status language key.
        /// Default = "EN"
        /// </summary>
        [JsonIgnore]
        public string LanguageKey { get; set; } = "EN";

        /// <summary>
        /// Game status language.
        /// Default = "EN"
        /// </summary>
        public virtual Language? Language { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGame> UserGames { get; set; }
    }
}
