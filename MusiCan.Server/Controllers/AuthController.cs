using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusiCan.Server.Data;
using MusiCan.Server.Helper;
using MusiCan.Server.Services;
using Serilog;

namespace MusiCan.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService, IOptions<Jwt> jtw) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IOptions<Jwt> _jwt = jtw;

        /// <summary>
        /// Http Post Anfrage um einen Nutzer zu registrieren 
        /// </summary>
        /// <param name="reg">Name, password, email und isComposer</param>
        /// <returns>JsonWebToken, Nutzername, Ablaufdatum</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromForm] ProfileRequest reg)
        {
            try
            {
                if (await _authService.CheckUserNameAsync(reg.name))
                {
                    Log.Warning($"Error during Registration of user {reg.name}: user name is already registrated.");
                    return Unauthorized("Nutzername wird bereits verwendet.");
                }

                if (await _authService.CheckUserMailAsync(reg.email))
                {
                    Log.Warning($"Error during Registration of user {reg.name}: user mail is already registrated.");
                    return Unauthorized("E-Mail wird bereits verwendet.");
                }

                if (reg.isComposer && reg.profileImage != null && !string.IsNullOrEmpty(reg.mimetype))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await reg.profileImage.CopyToAsync(memoryStream);
                        reg.profileImage_b = memoryStream.ToArray();
                    }

                    if (reg.profileImage_b == null)
                    {
                        return Conflict("Nutzer konnte nicht erstellt werden, Profilbild ungültig.");
                    }
                }

                (User? user, string createError) = await _authService.CreateUserAsync(reg);

                if (user == null)
                {
                    Log.Warning($"Error during Registration of user {reg.name}: user not created.");
                    return Unauthorized("Nutzer konnte nicht erstellt werden.");
                }

                (string accessToken, DateTime expire) = TokenUtils.GenerateAccessToken(user, _jwt.Value, 60); // 1 Stunde

                AuthResponse response = new()
                {
                    AuthToken = accessToken,
                    Name = user.Name,
                    ExpireTime = expire
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Registration of unknown user: {ex}");
                return Unauthorized("Server Fehler.");
            }
        }

        /// <summary>
        /// Http Post Anfrage um einen Nutzer einzuloggen 
        /// </summary>
        /// <param name="login">name, password, remember</param>
        /// <returns>JsonWebToken, Nutzername, Ablaufdatum</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            try
            {
                User? user = await _authService.AuthenticateAsync(login.nameOrMail, login.password);
                if (user == null)
                {
                    Log.Warning($"Error during Login of user {login.nameOrMail}: authentication failed.");
                    return Unauthorized("Anmeldung fehlgeschlagen, Name oder Passwort falsch.");
                }

                if (user.Role == Roles.Banned)
                {
                    Log.Warning($"Error during Login of user {user.UserId}: user banned.");
                    return Unauthorized("Anmeldung fehlgeschlagen, Nutzer gebannt.");
                }

                // 1 Woche oder 1 Stunde
                (string accessToken, DateTime expire) = TokenUtils.GenerateAccessToken(user, _jwt.Value, login.remember ? 10080 : 60);

                AuthResponse response = new()
                {
                    AuthToken = accessToken,
                    Name = user.Name,
                    ExpireTime = expire
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Login of unknown user: {ex}");
                return Unauthorized("Server Fehler.");
            }
        }
    }
}
