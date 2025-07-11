using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MusiCan.Server.Data;
using MusiCan.Server.DatabaseContext;
using MusiCan.Server.Helper;
using MusiCan.Server.Services;
using Serilog;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace MusiCan.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("profile")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error during Profile of unknown user: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error during Profile of unknown user: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error during Profile of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                User? user = await _profileService.GetUserByIdAsync(user_id);

                if (user == null)
                {
                    Log.Warning($"Error during Profile of user {user_id_s}: user {user_id} no found.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                // normaler Nutzer
                ProfileResponse response = new ProfileResponse
                {
                    Name = user.Name,
                    Mail = user.EMail,
                    Role = user.Role,
                    //ProfileImage = user.ProfileImage,
                    //ProfileImageContentType = user.ProfileImageContentType
                };

                // Künstler
                if (user.Role == Roles.Kuenstler && user.Composer != null)
                {
                    response.BirthYear = user.Composer?.BirthYear;
                    response.Genre = user.Composer?.Genre;
                    response.Country = user.Composer?.Country;
                    response.Discription = user.Composer?.Discription;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Profile of unknown user: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }
    }
}
