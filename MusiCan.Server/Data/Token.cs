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
        /// <summary>ein Tag in Minuten</summary>
        public const int expiration_time = 1440;

        /// <summary>
        /// generiert ein JsonWebToken 
        /// </summary>
        /// <param name="user">Nutzer</param>
        /// <param name="jwt">JsonWebToken Einstellungen</param>
        /// <returns>JsonWebToken</returns>
        public static (string, DateTime) GenerateAccessToken(User user, Jwt jwt)
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
