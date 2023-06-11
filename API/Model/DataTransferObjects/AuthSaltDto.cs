namespace API.Model.DataTransferObjects
{
    /// <summary>
    /// Auth salt container data transfer object
    /// </summary>
    public class AuthDataDto
    {
        /// <summary>
        /// Auth salt construction
        /// </summary>
        /// <param name="saltLength">Password salt length</param>
        /// <param name="challengeLength">Challenge length</param>
        public AuthDataDto(int saltLength = 64, int challengeLength = 64)
        {
            Salt = HashGenerator.GenerateSalt(saltLength);
            Challenge = HashGenerator.GenerateSalt(challengeLength);
        }
        /// <summary>
        /// Password salt to get hash (H)
        /// H = Sha256(password, passwordSalt)
        /// </summary>
        public string Salt { get; set; } = default!;
        /// <summary>
        /// Challenge to get hash secret (Hs)
        /// Hs = Sha256(H, hashSalt)
        /// </summary>
        public string? Challenge { get; set; } = default!;
    }
}
