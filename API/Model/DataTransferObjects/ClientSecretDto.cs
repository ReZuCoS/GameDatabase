using System.ComponentModel.DataAnnotations;

namespace API.Model.DataTransferObjects
{
    /// <summary>
    /// Client secret data transfer object
    /// </summary>
    public class ClientSecretDto
    {
        /// <summary>
        /// Client secret data transfer object 
        /// </summary>
        public ClientSecretDto() { }

        /// <summary>
        /// User login
        /// </summary>
        [Required]
        public string Login { get; set; } = default!;

        /// <summary>
        /// Must match format: SHA256([SHA256([password][salt])][server_challenge][client_challenge])
        /// </summary>
        [Required]
        public string HashSecret { get; set; } = default!;

        /// <summary>
        /// Client challenge
        /// </summary>
        [Required]
        public string Challenge { get; set; } = default!;
    }
}
