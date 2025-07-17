using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MusiCan.Server.Data
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }


    /// <summary>
    /// https://www.c-sharpcorner.com/article/implementing-jwt-refresh-tokens-in-net-8-0/
    /// </summary>
    public static class TokenUtils
    {
        /// <summary>
        /// generiert ein JsonWebToken 
        /// </summary>
        /// <param Name="user">Nutzer</param>
        /// <param Name="jwt">JsonWebToken Einstellungen</param>
        /// <param Name="expiration_time">Token Auslaufzeit in Minuten</param>
        /// <returns>JsonWebToken</returns>
        public static (string, DateTime) GenerateAccessToken(User user, Jwt jwt, int expiration_time)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwt.Key);
            DateTime expire = DateTime.UtcNow.AddMinutes(expiration_time);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.GetDisplayName()),
                }),
                Expires = expire,
                Audience = jwt.Audience,
                Issuer = jwt.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expire);
        }
    }
}
