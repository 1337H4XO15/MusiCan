using Microsoft.EntityFrameworkCore;
using MusiCan.Server.Data;
using MusiCan.Server.DatabaseContext;
using MusiCan.Server.Helper;
using Serilog;
using System.Data;

namespace MusiCan.Server.Services
{
    public interface IMusicService
    {
        /// <summary>
        /// Erstellt einen Musik Eintrag
        /// </summary>
        /// <param name="music">Musik Eintrag</param>
        /// <returns>True bei Erfolg</returns>
        Task<bool> CreateMusicAsync(Music music);

        /// <summary>
        /// Bearbeitet einen Musik Eintrag
        /// </summary>
        /// <param name="musicId">Musik ID</param>
        /// <param name="request">neue Musik Daten</param>
        /// <returns>Musik oder Error</returns>
        Task<(Music?, string)> UpdateMusicAsync(MusicRequest request);

        /// <summary>
        /// Löscht einen Musik Eintrag
        /// </summary>
        /// <param name="musicId">Musik Eintrag ID</param>
        /// <returns></returns>
        Task<bool> DeleteMusicByIdAsync(Guid musicId);

        /// <summary>
        /// Holt zufällig 9 Musik Einträge, die öffentlich zugänglich sind
        /// </summary>
        /// <returns>Liste mit den Musik Einträgen</returns>
        Task<List<Music>> GetRandomMusicAsync();

        /// <summary>
        /// Holt öffentliche Musik Einträge
        /// </summary>
        /// <returns>Liste mit den Musik Einträgen</returns>
        Task<List<Music>> GetPublicMusicAsync();

        /// <summary>
        /// Holt alle Musik Einträge des Nutzers 
        /// </summary>
        /// <param name="userId">Nutzer ID</param>
        /// <returns>Liste mit den Musik Einträgen</returns>
        Task<List<Music>> GetMusicByUserIdAsync(Guid userId);

        /// <summary>
        /// Holt Musik Einträge mit, wenn Nutzer Nutzer gültig
        /// </summary>
        /// <param name="musicId">Musik Eintrag ID</param>
        /// <param name="userId">Nutzer ID</param>
        /// <returns>Liste mit den Musik Einträgen</returns>
        Task<Music?> GetMusicByIdAsync(Guid musicId, Guid? userId);
    }

    public class MusicService(DataContext dataContext) : IMusicService
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<bool> CreateMusicAsync(Music music)
        {
            var transaction = _dataContext.Database.BeginTransaction();

            try
            {
                _dataContext.Add(music);

                await _dataContext.SaveChangesAsync();

                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error($"Error while creating music {ex}");
                return false; // schreiben in DB fehlgeschlagen
            }
        }

        public async Task<(Music?, string)> UpdateMusicAsync(MusicRequest request)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (request.id == Guid.Empty)
                {
                    return (null, "Musik ID fehlt.");
                }

                var music = await _dataContext.Musics
                    .FirstOrDefaultAsync(m => m.MusicId == request.id);

                if (music == null)
                {
                    return (null, "Musik nicht gefunden.");
                }


                if (string.IsNullOrEmpty(request.title) || string.IsNullOrEmpty(request.author)
                    || string.IsNullOrEmpty(request.mimetype) || request.file.Length == 0)
                {
                    return (null, "Musik Attribute fehlen.");
                }

                music.Title = request.title;
                music.Composer = request.author; // owner?
                if (int.TryParse(request.releaseYear, out int year))
                {
                    music.Publication = new DateTime(year, 1, 1);
                }
                music.Genre = request.genre;
                music.ContentType = request.mimetype;

                using (var memoryStream = new MemoryStream())
                {
                    await request.file.CopyToAsync(memoryStream);
                    music.FileData = memoryStream.ToArray();
                }
                music.Timestamp = DateTime.Now;

                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                Log.Information($"Music {request.id} updated successfully.");

                return (music, "");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"Error while updating music {ex}");
            }
            return (null, "Server Fehler.");
        }

        public async Task<bool> DeleteMusicByIdAsync(Guid musicId)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                var music = await _dataContext.Musics
                    .FirstOrDefaultAsync(m => m.MusicId == musicId);

                if (music == null)
                {
                    return false;
                }

                _dataContext.Musics.Remove(music);

                await _dataContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"Error while deleting music {ex}");
            }

            return false;
        }

        public async Task<List<Music>> GetRandomMusicAsync()
        {
            var music = await _dataContext.Musics
                .Include(m => m.UserMusics)
                    .ThenInclude(um => um.User)
                        .ThenInclude(u => u.Composer)
                .Where(m => m.Public)
                .ToListAsync();

            return music
                .OrderBy(m => Guid.NewGuid())
                .Take(9).ToList();
        }

        public async Task<List<Music>> GetPublicMusicAsync()
        {
            return await _dataContext.Musics
                .Include(m => m.UserMusics)
                    .ThenInclude(um => um.User)
                        .ThenInclude(u => u.Composer)
                .Where(m => m.Public)
                .ToListAsync();
        }

        public async Task<List<Music>> GetMusicByUserIdAsync(Guid userId)
        {
            return await _dataContext.Musics
                .Include(m => m.UserMusics)
                    .ThenInclude(um => um.User)
                .Where(m => m.UserMusics.Any(um => um.Access == Access.Owner && um.UserId == userId))
                .ToListAsync();
        }

        public async Task<Music?> GetMusicByIdAsync(Guid musicId, Guid? userId)
        {
            return await _dataContext.Musics
                .Include(m => m.UserMusics)
                    .ThenInclude(um => um.User)
                .FirstOrDefaultAsync(m =>
                    m.MusicId == musicId &&
                    (
                        m.Public ||
                        (userId != null && m.UserMusics.Any(um => um.Access == Access.Owner && um.UserId == userId))
                    )
                );
        }
    }

}
