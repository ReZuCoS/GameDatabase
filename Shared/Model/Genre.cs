using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// Game genre class
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Game genre class
        /// </summary>
        public Genre()
        {
            Games = new HashSet<Game>();
        }

        /// <summary>
        /// Game genre ID
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// Game genre name
        /// </summary>
        public string Name { get; private set; } = default!;

        [JsonIgnore]
        public virtual ICollection<Game> Games { get; set; }
    }
}
