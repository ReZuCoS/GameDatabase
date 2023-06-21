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
        public UserDto(string authorization, string refreshToken, string profileImage)
        {
            RefreshToken = refreshToken;
            ProfileImage = profileImage;
            Authorization = authorization;
        }

        /// <summary>
        /// User's access token
        /// </summary>
        public string Authorization { get; set; } = default!;

        /// <summary>
        /// User's refresh token
        /// </summary>
        public string RefreshToken { get; set; } = default!;

        /// <summary>
        /// User profile link
        /// </summary>
        public string? ProfileImage { get; set; }
    }
}
