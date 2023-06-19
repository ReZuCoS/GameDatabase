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
        public UserDto(string authorization, string profileImage)
        {
            Authorization = authorization;
            ProfileImage = profileImage;
        }

        /// <summary>
        /// User access token
        /// </summary>
        public string Authorization { get; set; } = default!;

        /// <summary>
        /// User profile link
        /// </summary>
        public string? ProfileImage { get; set; }
    }
}
