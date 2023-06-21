using Shared.Model;

namespace API.Utils.Interfaces
{
    public interface IJwtHandler
    {
        public string? GetContextClaim(string key);
        public string GetRefreshToken();
        public string GetUserToken(User user);
    }
}
