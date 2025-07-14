using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using MusiCan.Server.Data;
using MusiCan.Server.DatabaseContext;
using MusiCan.Server.Helper;
using Serilog;

namespace MusiCan.Server.Services
{
    public interface IProfileService
    {
        /// <summary>
        /// gibt den Nutzer mit passender Nutzer ID zurück, läd auch den Komponisten, wenn vorhanden
        /// </summary>
        /// <param name="id">Nutzer ID</param>
        /// <returns>Nutzer</returns>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// gibt den Nutzer mit passender Nutzer ID zurück, läd auch den Komponisten und Musik, wenn vorhanden
        /// </summary>
        /// <param name="id">Nutzer ID</param>
        /// <returns>Nutzer</returns>
        Task<User?> GetUserWithMusicByIdAsync(Guid id);

        /// <summary>
        /// Bearbeitet einen Nutzer und gibt diesen dann zurück
        /// </summary>
        /// <param name="userId">Nutzer ID</param>
        /// <param name="request">neue Profil Daten</param>
        /// <returns>Nutzer oder Error</returns>
        Task<(User?, string)> UpdateUserAsync(Guid userId, ProfileRequest request);

        /// <summary>
        /// gibt den Komponist mit passender Komponist ID zurück
        /// </summary>
        /// <param name="id">Komponist ID</param>
        /// <returns>Komponist</returns>
        Task<Composer?> GetComposerByIdAsync(Guid id);

        /// <summary>
        /// gibt alle Komponist zurück
        /// </summary>
        /// <returns>Liste mit Komponisten</returns>
        Task<List<Composer>> GetAllComposerAsync();
    }

    public class ProfileService : IProfileService
    {
        private readonly DataContext _dataContext;

        public ProfileService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _dataContext.Users
                    .Include(u => u.Composer)
                    .FirstOrDefaultAsync(u => u.UserId == id);
            }
            catch (Exception ex)
            {
                Log.Error($"Error while retrieving user {ex}");
                return null; // DB error
            }
        }

        public async Task<User?> GetUserWithMusicByIdAsync(Guid id)
        {
            try
            {
                return await _dataContext.Users
                    .Include(u => u.Composer)
                    .Include(u => u.UserMusics)
                        .ThenInclude(u => u.Music)
                    .FirstOrDefaultAsync(u => u.UserId == id);
            }
            catch (Exception ex)
            {
                Log.Error($"Error while retrieving user {ex}");
                return null; // DB error
            }
        }

        public async Task<(User?, string)> UpdateUserAsync(Guid userId, ProfileRequest request)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _dataContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return (null, "User not found.");
                }

                // Nutzer
                user.Name = request.name;
                user.EMail = request.email;
                if (user.Role != Roles.Admin && user.Role != Roles.Banned)
                {
                    user.Role = request.iscomposer ? Roles.Kuenstler : Roles.Nutzer;
                }
                user.UpdatedAt = DateTime.UtcNow;

                // Komponist
                if (request.iscomposer)
                {
                    if (user.Composer == null)
                    {
                        user.Composer = new Composer();
                    }

                    if (string.IsNullOrEmpty(request.genre) || string.IsNullOrEmpty(request.country) 
                        || string.IsNullOrEmpty(request.description) || !request.birthyear.HasValue)
                    {
                        return (null, "Missing Composer Attributes.");
                    }

                    user.Composer.Genre = request.genre;
                    user.Composer.Country = request.country;
                    user.Composer.Description = request.description;
                    user.Composer.BirthYear = request.birthyear ?? DateTime.MinValue; // Der Fall tritt nicht ein, wird vorher geprüft
                }

                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                Log.Information($"User {userId} updated successfully.");

                return (user, "");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"Error while updating user {ex}");
            }
            return (null, "Something unexpected happend.");
        }

        public async Task<Composer?> GetComposerByIdAsync(Guid id)
        {
            try
            {
                return await _dataContext.Composers
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Log.Error($"Error while retrieving composer {ex}");
                return null; // DB error
            }
        }

        public async Task<List<Composer>> GetAllComposerAsync()
        {
            return await _dataContext.Composers.ToListAsync();
        }
    }

}
