
namespace City_Bus_Management_System.Factories
{
    public class AuthModelFactory
    {
        public AuthModel CreateAuthModel(string id, string username, string email, DateTime expiresOn,List<string> roles, string JWTSecurityToken,string refreshToken,DateTime Expires,string Message = "")
        {
            return new AuthModel()
            {
                Id = id,
                Username = username,
                Email = email,
                IsAuthenticated = true,
                //ExpiresOn = expiresOn,
                Roles = roles,
                Token = JWTSecurityToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = Expires,
            };
        }
    }
}
