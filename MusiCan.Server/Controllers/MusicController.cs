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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusiCan.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController(IMusicService musicService, IProfileService profileService) : ControllerBase
    {
        private readonly IMusicService _musicService = musicService;
        private readonly IProfileService _profileService = profileService;

        [HttpGet("randomMusic")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> GetRandomMusic()
        {
            try
            {
                List<Music> musics = await _musicService.GetRandomMusicAsync();

                List<DisplayMusic> response = [.. musics.Select(music => new DisplayMusic
                {
                    Id = music.MusicId,
                    Title = music.Title,
                    Composer = music.Composer,
                    ContentType = music.ContentType,
                    FileData = music.FileData,
                    Publication = music.Publication,
                    Genre = music.Genre,
                    Timestamp = music.Timestamp,
                    Owner = music.UserMusics
                    .Where(um => um.Access == Access.Owner)
                    .Select(userMusic => new MusicOwner
                    {
                        Id = userMusic.User.Composer != null ? userMusic.User.Composer.Id: userMusic.UserId,
                        Name = userMusic.User.Composer != null ? userMusic.User.Composer.ArtistName : userMusic.User.Name,
                        isComposer = userMusic.User.Composer != null
                    })
                    .FirstOrDefault()
                })];

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during RandomMusic: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        [HttpGet("ownMusic")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> GetOwnMusic()
        {
            try
            {
                #region User
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error during OwnMusic: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error during OwnMusic: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error during OwnMusic of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                User? user = await _profileService.GetUserByIdAsync(user_id);

                if (user == null)
                {
                    Log.Warning($"Error during OwnMusic: user {user_id} no found.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }
                #endregion

                List<Music> musics = await _musicService.GetMusicByUserIdAsync(user_id);

                MusicOwner owner = new()
                {
                    Id = user.Composer != null ? user.Composer.Id : user.UserId,
                    Name = user.Composer != null ? user.Composer.ArtistName : user.Name,
                    isComposer = user.Composer != null
                };

                List<DisplayMusic> response = [.. musics.Select(music => new DisplayMusic
                {
                    Id = music.MusicId,
                    Title = music.Title,
                    Composer = music.Composer,
                    ContentType = music.ContentType,
                    FileData = music.FileData,
                    Publication = music.Publication,
                    Genre = music.Genre,
                    Timestamp = music.Timestamp,
                    Owner = owner
                })];

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during OwnMusic: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        [HttpGet("music")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> GetMusic()
        {
            try
            {
                #region User
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error during Music: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error during Music: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error during Music of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                User? user = await _profileService.GetUserWithMusicByIdAsync(user_id);

                if (user == null)
                {
                    Log.Warning($"Error during Music: user {user_id} no found.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }
                #endregion

                List<DisplayMusic> response = [.. user.UserMusics.Select(userMusic => new DisplayMusic
                {
                    Id = userMusic.Music.MusicId,
                    Title = userMusic.Music.Title,
                    Composer = userMusic.Music.Composer,
                    ContentType = userMusic.Music.ContentType,
                    FileData = userMusic.Music.FileData,
                    Publication = userMusic.Music.Publication,
                    Genre = userMusic.Music.Genre,
                    Timestamp = userMusic.Music.Timestamp,
                    Owner = userMusic.Music.UserMusics
                    .Where(um => um.Access == Access.Owner)
                    .Select(userMusic => new MusicOwner
                    {
                        Id = userMusic.User.Composer != null ? userMusic.User.Composer.Id : userMusic.UserId,
                        Name = userMusic.User.Composer != null ? userMusic.User.Composer.ArtistName : userMusic.User.Name,
                        isComposer = userMusic.User.Composer != null
                    })
                    .FirstOrDefault()
                })];

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Music: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        [HttpPost("music")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> PostMusic([FromForm] MusicRequest request) // FromForm !!!
        {
            try
            {
                #region User
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error when creating Music: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error when creating Music: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error when creating Music of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                User? user = await _profileService.GetUserByIdAsync(user_id);

                if (user == null)
                {
                    Log.Warning($"Error when creating Music: user {user_id} no found.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }
                #endregion

                using (var memoryStream = new MemoryStream())
                {
                    await request.file.CopyToAsync(memoryStream);
                    request.file_b = memoryStream.ToArray();
                }

                if (request.file_b == null)
                {
                    return Conflict("Could not create music.");
                }

                Music music = new(request, user);

                if (!await _musicService.CreateMusicAsync(music))
                {
                    return Conflict("Could not create music.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"Error when creating Music: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        [HttpPut("music")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> UpdateMusic([FromBody] MusicRequest request)
        {
            try
            {
                #region User
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error when updating Music: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error when updating Music: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error when updating Music of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                User? user = await _profileService.GetUserByIdAsync(user_id);

                if (user == null)
                {
                    Log.Warning($"Error when updating Music: user {user_id} no found.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }
                #endregion

                if (request.id == null || request.id == Guid.Empty)
                {
                    return Conflict("Missing music id.");
                }

                (Music? music, string updateError) = await _musicService.UpdateMusicAsync(request);

                if (music == null)
                {
                    Log.Warning($"Error when updating Music: user {user_id} no found.");
                    return Conflict(updateError); // 409 > Conflict
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"Error when updating Music: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        [HttpDelete("music/{id}")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> DeleteMusic(Guid id)
        {
            try
            {
                #region User
                if (HttpContext.User.Identity is not ClaimsIdentity identity)
                {
                    Log.Warning($"Error when deleting Music: HttpContext.User.Identity is not ClaimsIdentity (invalid token).");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                IEnumerable<Claim> userClaims = identity.Claims;

                string? user_id_s = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (user_id_s == null)
                {
                    Log.Warning($"Error when deleting Music: invalid token.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                if (!Guid.TryParse(user_id_s, out Guid user_id))
                {
                    Log.Warning($"Error when deleting Music of user {user_id_s}: could not parse user id.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }

                User? user = await _profileService.GetUserByIdAsync(user_id);

                if (user == null)
                {
                    Log.Warning($"Error when deleting Music: user {user_id} no found.");
                    InvalidTokenErrorResponse error = new();
                    return StatusCode(error.StatusCode, error.Message);
                }
                #endregion

                if (id == Guid.Empty)
                {
                    return Conflict("Missing music id.");
                }

                if (!await _musicService.DeleteMusicByIdAsync(id))
                {
                    return Conflict("Could not delete music.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"Error when deleting Music: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }
    }
}
