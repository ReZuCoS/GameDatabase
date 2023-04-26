using System.Text.Json.Serialization;

namespace Shared.Model
{
    /// <summary>
    /// Game tag class
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Game tag class
        /// </summary>
        public Tag()
        {
            Games = new HashSet<Game>();
        }

        /// <summary>
        /// Game tag ID
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// Game tag name
        /// </summary>
        public string Name { get; set; } = default!;

        [JsonIgnore]
        public virtual ICollection<Game> Games { get; set; }
    }
}
