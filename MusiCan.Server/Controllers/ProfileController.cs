using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusiCan.Server.Data;
using MusiCan.Server.Helper;
using MusiCan.Server.Services;
using Serilog;
using System.Security.Claims;

namespace MusiCan.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        private readonly IProfileService _profileService = profileService;

        /// <summary>
        /// Http Get Anfrage um eigenes Nutzer- / Künstlerprofil abzufragen
        /// </summary>
        /// <returns>Nutzer- oder Künstlerprofil</returns>
        [HttpGet("profile")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                #region User
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
                #endregion

                // normaler Nutzer
                ProfileResponse response = new()
                {
                    Name = user.Name,
                    Mail = user.EMail,
                    Role = user.Role
                };

                // Künstler
                if (user.Role == Roles.Kuenstler && user.Composer != null)
                {
                    response.BirthYear = user.Composer?.BirthYear;
                    response.Genre = user.Composer?.Genre;
                    response.Country = user.Composer?.Country;
                    response.Description = user.Composer?.Description;
                    response.ProfileImage = user.Composer?.ProfileImage != null ? Convert.ToBase64String(user.Composer?.ProfileImage) : "";
                    response.ProfileImageContentType = user.Composer?.ProfileImageContentType;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Profile of unknown user: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        /// <summary>
        /// Http Put Anfrage um eigenes Nutzer- / Künstlerprofil zu ändern
        /// </summary>
        /// <param name="profile">Nutzer- / Künstlerprofil</param>
        /// <returns>Nutzer- oder Künstlerprofil</returns>
        [HttpPut("profile")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> PutProfile([FromForm] ProfileRequest profile)
        {
            try
            {
                #region User
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error when updating Profile of unknown user: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error when updating Profile of unknown user: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error when updating Profile of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (profile.profileImage != null && !string.IsNullOrEmpty(profile.mimetype))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await profile.profileImage.CopyToAsync(memoryStream);
                        profile.profileImage_b = memoryStream.ToArray();
                    }

                    if (profile.profileImage_b == null)
                    {
                        return Conflict("Could not create music.");
                    }
                }

                (User? user, string updateError) = await _profileService.UpdateUserAsync(user_id, profile);

                if (user == null)
                {
                    Log.Warning($"Error when updating Profile of user {user_id_s}: user {user_id} no found.");
                    return Conflict(updateError); // 409 > Conflict
                }
                #endregion

                // normaler Nutzer
                ProfileResponse response = new()
                {
                    Name = user.Name,
                    Mail = user.EMail,
                    Role = user.Role
                };

                // Künstler
                if (user.Role == Roles.Kuenstler && user.Composer != null)
                {
                    response.BirthYear = user.Composer?.BirthYear;
                    response.Genre = user.Composer?.Genre;
                    response.Country = user.Composer?.Country;
                    response.Description = user.Composer?.Description;
                    response.ProfileImage = user.Composer?.ProfileImage != null ? Convert.ToBase64String(user.Composer?.ProfileImage) : "";
                    response.ProfileImageContentType = user.Composer?.ProfileImageContentType;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error when updating Profile of unknown user: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        /// <summary>
        /// Http Get Anfrage um alle Künstlerprofile abzufragen
        /// </summary>
        /// <returns>Liste der Künstlerprofile</returns>
        [HttpGet("composers")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> GetComposers()
        {
            try
            {
                List<Composer> composers = await _profileService.GetAllComposerAsync();

                List<DisplayComposer> response = [.. composers.Select(composer => new DisplayComposer
                {
                    Id = composer.Id,
                    ArtistName = composer.ArtistName,
                    Genre = composer.Genre,
                    BirthYear = composer.BirthYear,
                    Country = composer.Country,
                    Description = composer.Description,
                    ProfileImage = composer.ProfileImage != null ? Convert.ToBase64String(composer.ProfileImage) : "",
                    ProfileImageContentType = composer.ProfileImageContentType
                })];

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Composers: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        /// <summary>
        /// Http Get Anfrage um ein Künstlerprofil abzufragen
        /// </summary>
        /// <param name="id">Künstler ID</param>
        /// <returns>Künstlerprofil</returns>
        [HttpGet("composer/{id}")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> GetComposer(Guid id)
        {
            try
            {
                Composer? composer = await _profileService.GetComposerByIdAsync(id);

                if (composer == null)
                {
                    return NotFound("Composer not found.");
                }

                DisplayComposer response = new()
                {
                    Id = composer.Id,
                    ArtistName = composer.ArtistName,
                    Genre = composer.Genre,
                    BirthYear = composer.BirthYear,
                    Country = composer.Country,
                    Description = composer.Description,
                    ProfileImage = composer.ProfileImage != null ? Convert.ToBase64String(composer.ProfileImage) : "",
                    ProfileImageContentType = composer.ProfileImageContentType
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Composer: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }
    }
}
