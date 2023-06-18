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
        public UserDto() { }

        /// <summary>
        /// Game database user data transfer object
        /// </summary>
        public UserDto(string accessToken, string profileImage)
        {
            AccessToken = accessToken;
            ProfileImage = profileImage;
        }

        /// <summary>
        /// User access token
        /// </summary>
        public string AccessToken { get; set; } = default!;

        /// <summary>
        /// User profile link
        /// </summary>
        public string? ProfileImage { get; set; }
    }
}
