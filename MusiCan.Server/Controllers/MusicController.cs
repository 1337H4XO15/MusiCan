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
    public class MusicController(IMusicService musicService, IProfileService profileService) : ControllerBase
    {
        private readonly IMusicService _musicService = musicService;
        private readonly IProfileService _profileService = profileService;

        /// <summary>
        /// Http Get Anfrage um 10 zufällige Musikstücke abzufragen, die öffentlich zugänglich sind
        /// </summary>
        /// <returns>Liste der zufälligen Musikstücke</returns>
        [HttpGet("randomMusic")]
        [AllowAnonymous]
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
                    FileData = Convert.ToBase64String(music.FileData),
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

        /// <summary>
        /// Http Get Anfrage um eigene Musikstücke abzufragen
        /// </summary>
        /// <returns>Liste der eigenen Musikstücke</returns>
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
                    FileData = Convert.ToBase64String(music.FileData),
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

        /// <summary>
        /// Http Get Anfrage um öffentliche Musikstücke abzufragen
        /// </summary>
        /// <returns>Liste der öffentlichen Musikstücke</returns>
        [HttpGet("music")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMusic()
        {
            try
            {
                List<Music> publicMusic = await _musicService.GetPublicMusicAsync();

                List<DisplayMusic> response = [.. publicMusic
                    .Select(music => new DisplayMusic
                {
                    Id = music.MusicId,
                    Title = music.Title,
                    Composer = music.Composer,
                    ContentType = music.ContentType,
                    FileData = Convert.ToBase64String(music.FileData),
                    Publication = music.Publication,
                    Genre = music.Genre,
                    Timestamp = music.Timestamp,
                    Owner = music.UserMusics
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

        /// <summary>
        /// Http Get Anfrage um Musikstück abzufragen
        /// </summary>
        /// <param name="id">Musikstück ID</param>
        /// <returns>Musikstück</returns>
        [HttpGet("music/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMusic(Guid id)
        {
            try
            {
                Guid? user_id = null;

                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    string? user_id_s = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (user_id_s != null && Guid.TryParse(user_id_s, out Guid parsedUserId))
                    {
                        user_id = parsedUserId;
                    }
                }

                Music? music = await _musicService.GetMusicByIdAsync(id, user_id);

                if (music == null)
                {
                    return NotFound("Music not found.");
                }

                DisplayMusic response = new()
                {
                    Id = music.MusicId,
                    Title = music.Title,
                    Composer = music.Composer,
                    ContentType = music.ContentType,
                    FileData = Convert.ToBase64String(music.FileData),
                    Publication = music.Publication,
                    Genre = music.Genre,
                    Timestamp = music.Timestamp,
                    Owner = music.UserMusics
                    .Where(um => um.Access == Access.Owner)
                    .Select(userMusic => new MusicOwner
                    {
                        Id = userMusic.User.Composer != null ? userMusic.User.Composer.Id : userMusic.UserId,
                        Name = userMusic.User.Composer != null ? userMusic.User.Composer.ArtistName : userMusic.User.Name,
                        isComposer = userMusic.User.Composer != null
                    })
                    .FirstOrDefault()
                };


                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Music: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }

        /// <summary>
        /// Http Post Anfrage um Musikstück hochzuladen
        /// </summary>
        /// <returns>Statuscode 200</returns>
        [HttpPost("music")]
        [Authorize(Policy = "NotBanned")]
        public async Task<IActionResult> PostMusic([FromForm] MusicRequest request)
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

        /// <summary>
        /// Http Put Anfrage um Musikstück zu ändern
        /// </summary>
        /// <returns>Statuscode 200</returns>
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

        /// <summary>
        /// Http Delete Anfrage um Musikstücke zu löschen
        /// </summary>
        /// <returns>Liste der eigenen Musikstücke</returns>
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
                    FileData = Convert.ToBase64String(music.FileData),
                    Publication = music.Publication,
                    Genre = music.Genre,
                    Timestamp = music.Timestamp,
                    Owner = owner
                })];

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error when deleting Music: {ex}");
                return StatusCode(500, "Something unexpected happend");
            }
        }
    }
}
