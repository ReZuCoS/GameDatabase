using System.ComponentModel.DataAnnotations;

namespace API.Model.DataTransferObjects
{
    /// <summary>
    /// Server salt container data transfer object
    /// </summary>
    public class ServerSecretDto
    {
        /// <summary>
        /// Server salt construction
        /// </summary>
        public ServerSecretDto() { }

        /// <summary>
        /// Server salt construction
        /// </summary>
        /// <param name="salt">User salt</param>
        /// <param name="challenge">Challenge string</param>
        public ServerSecretDto(string salt, string challenge)
        {
            Salt = salt;
            Challenge = challenge;
        }

        /// <summary>
        /// Password salt to get hash
        /// </summary>
        [Required]
        public string Salt { get; set; } = default!;

        /// <summary>
        /// Server challenge
        /// </summary>
        [Required]
        public string Challenge { get; set; } = default!;
    }
}
