namespace API.Controllers
{
    /// <summary>
    /// User totp access storage
    /// </summary>
    public class UserTotpAccess
    {
        /// <summary>
        /// User totp access storage constructor
        /// </summary>
        public UserTotpAccess(string accessToken, string totpKey)
        {
            AccessToken = accessToken;
            TotpKey = totpKey;
        }

        /// <summary>
        /// User's totp key
        /// </summary>
        public string TotpKey { get; set; }

        /// <summary>
        /// User's access token
        /// </summary>
        public string AccessToken { get; set; }
    }
}
