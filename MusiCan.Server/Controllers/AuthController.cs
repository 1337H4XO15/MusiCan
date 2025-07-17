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
        /// <param Name="reg">Name, password, email und isComposer</param>
        /// <returns>JsonWebToken, Nutzername, Ablaufdatum</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest reg)
        {
            try
            {
                if (await _authService.CheckUserNameAsync(reg.name))
                {
                    Log.Warning($"Error during Registration of user {reg.name}: user name is already registrated.");
                    RegistrationErrorResponse error = new("user name is already registrated");
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (await _authService.CheckUserMailAsync(reg.email))
                {
                    Log.Warning($"Error during Registration of user {reg.name}: user mail is already registrated.");
                    RegistrationErrorResponse error = new("user mail is already registrated");
                    return StatusCode(error.StatusCode, error.Message);
                }

                //string hashedPW = SecretHasher.GenerateSaltedPassword(reg.password, reg.Salt);
                //password is already Hashed by the client on Registration
                User? user = await _authService.CreateUserAsync(reg.name, reg.password, reg.email, reg.isComposer ? Roles.Kuenstler : Roles.Nutzer);
                if (user == null)
                {
                    Log.Warning($"Error during Registration of user {reg.name}: user not created.");
                    RegistrationErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
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
                RegistrationErrorResponse error = new();
                return StatusCode(error.StatusCode, error.Message);
            }
        }

        /// <summary>
        /// Http Post Anfrage um einen Nutzer einzuloggen 
        /// </summary>
        /// <param Name="login">nam, password, remember</param>
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
                    LoginErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (user.Role == Roles.Banned)
                {
                    Log.Warning($"Error during Login of user {user.UserId}: user banned.");
                    LoginErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
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
                LoginErrorResponse error = new();
                return StatusCode(error.StatusCode, error.Message);
            }
        }
    }
}
