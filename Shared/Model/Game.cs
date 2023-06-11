using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// Game database game class
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Game database game class
        /// </summary>
        public Game()
        {
            GameTranslations = new HashSet<GameTranslation>();
            UserGames = new HashSet<UserGame>();
            Genres = new HashSet<Genre>();
            Tags = new HashSet<Tag>();
        }

        /// <summary>
        /// Game ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Game image vertical path
        /// </summary>
        public string ImageVertical { get; set; } = default!;

        /// <summary>
        /// Game image horizontal path
        /// </summary>
        public string ImageHorizontal { get; set; } = default!;

        /// <summary>
        /// Game average playtime in hours
        /// </summary>
        public short AvgPlaytimeInHours { get; set; } = default!;

        /// <summary>
        /// Game release date
        /// </summary>
        public DateTime ReleaseDate { get; set; } = default!;

        [JsonIgnore]
        public virtual ICollection<GameTranslation> GameTranslations { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGame> UserGames { get; set; }

        [JsonIgnore]
        public virtual ICollection<Genre> Genres { get; set; }

        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
