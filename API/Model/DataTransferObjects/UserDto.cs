using Shared.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Model.DataTransferObjects
{
    /// <summary>
    /// Game database user data transfer object
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Game database user data transfer object
        /// </summary>
        public UserDto(User user)
        {
            Login = user.Login;
            ProfileImage = user.ProfileImage;
            LanguageKey = user.LanguageKey;
        }

        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; } = default!;

        /// <summary>
        /// User profile link
        /// </summary>
        public string? ProfileImage { get; set; }

        /// <summary>
        /// User preferred language key.
        /// Default = "EN"
        /// </summary>
        [Required]
        [DefaultValue("EN")]
        public string LanguageKey { get; set; }
    }
}
