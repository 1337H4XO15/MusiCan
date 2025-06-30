using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusiCan.Server.DatabaseContext;
using MusiCan.Server.Data;
using MusiCan.Server.Helper;
using MusiCan.Server.Services;
using Serilog;

namespace MusiCan.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOptions<Jwt> _jwt;
        private readonly DataContext _dataContext;

        public AuthController(IAuthService authService, IOptions<Jwt> jtw, DataContext dataContext)
        {
            _authService = authService;
            _jwt = jtw;
            _dataContext = dataContext;
        }

        /// <summary>
        /// Http Get Anfrage um einen Nutzer zu registrieren 
        /// </summary>
        /// <param name="reg">Name, Password, DeviceHash und Salt</param>
        /// <returns>JsonWebToken und Product als Response</returns>
        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> Registration(RegistrationRequest reg)
        {
            try
            {
                if (!SecretHasher.IsHash(reg.Password))
                {
                    Log.Warning($"Error during Registration of user {reg.Name}: password is not a hash.");
                    RegistrationErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (await _authService.CheckUserNameAsync(reg.Name))
                {
                    Log.Warning($"Error during Registration of user {reg.Name}: user name is already registrated.");
                    RegistrationErrorResponse error = new("user name is already registrated");
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (await _authService.CheckUserMailAsync(reg.EMail))
                {
                    Log.Warning($"Error during Registration of user {reg.Name}: user mail is already registrated.");
                    RegistrationErrorResponse error = new("user mail is already registrated");
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Enum.TryParse<Roles>(reg.Role, true, out var role))
                {
                    Log.Warning($"Error during Registration of user {reg.Name}: not a valid role selected.");
                    RegistrationErrorResponse error = new("not a valid role selected");
                    return StatusCode(error.StatusCode, error.Message);
                }

                //string hashedPW = SecretHasher.GenerateSaltedPassword(reg.Password, reg.Salt);
                //Password is already Hashed by the client on Registration
                User? user = await _authService.CreateUserAsync(reg.Name, reg.Password, reg.EMail, role);
                if (user == null)
                {
                    Log.Warning($"Error during Registration of user {reg.Name}: user not created.");
                    RegistrationErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                string accessToken = TokenUtils.GenerateAccessToken(user, _jwt.Value);

                DefaultResponse response = new DefaultResponse
                {
                    AuthToken = accessToken
                };

                return Ok(response.ToString());
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Registration of unknown user: {ex}");
                RegistrationErrorResponse error = new();
                return StatusCode(error.StatusCode, error.Message);
            }
        }

        /// <summary>
        /// Http Get Anfrage um einen Nutzer einzuloggen 
        /// </summary>
        /// <param name="login">Name und Password</param>
        /// <returns>JsonWebToken als Response</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Helper.LoginRequest login)
        {
            try
            {
                User? user = await _authService.AuthenticateAsync(login.Name, login.Password, login.EMail);
                if (user == null)
                {
                    Log.Warning($"Error during Login of user {login.Name ?? login.EMail}: authentication failed.");
                    LoginErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (user.Role == Roles.Banned)
                {
                    Log.Warning($"Error during Login of user {user.UserId}: user banned.");
                    LoginErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                string accessToken = TokenUtils.GenerateAccessToken(user, _jwt.Value);

                DefaultResponse response = new DefaultResponse
                {
                    AuthToken = accessToken
                };

                return Ok(response.ToString());
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
