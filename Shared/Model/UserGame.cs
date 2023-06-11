using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// User game class
    /// </summary>
    public class UserGame
    {
        /// <summary>
        /// User game class
        /// </summary>
        public UserGame()
        {
            GameStatus = new GameStatus();
            Game = new Game();
            User = new User();
        }

        /// <summary>
        /// Game ID
        /// </summary>
        public int GameID { get; set; } = default!;

        /// <summary>
        /// User ID
        /// </summary>
        public string UserLogin { get; set; } = default!;

        /// <summary>
        /// User game status
        /// </summary>
        public int Status { get; set; } = default!;

        /// <summary>
        /// User's custom game vertical image location
        /// </summary>
        public string CustomImageVertical { get; set; } = default!;

        /// <summary>
        /// User's custom game horizontal image location
        /// </summary>
        public string CustomImageHorizontal { get; set; } = default!;

        /// <summary>
        /// User's game rate
        /// </summary>
        public short? UserRate { get; set; }

        [JsonIgnore]
        public virtual GameStatus GameStatus { get; set; }

        [JsonIgnore]
        public virtual Game Game { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
