using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// Game translations class
    /// </summary>
    public class GameTranslation
    {
        /// <summary>
        /// Game translations class
        /// </summary>
        public GameTranslation()
        {
            Game = new Game();
        }

        /// <summary>
        /// Game ID
        /// </summary>
        public int GameID { get; set; } = default!;

        /// <summary>
        /// Game title translation
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// Game description translation
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Game translation language key
        /// Default = "EN"
        /// </summary>
        [JsonIgnore]
        public string LanguageKey { get; set; } = "EN";

        /// <summary>
        /// Game translation language
        /// Default = "EN"
        /// </summary>
        public virtual Language? Language { get; set; }

        [JsonIgnore]
        public virtual Game Game { get; set; }
    }
}
