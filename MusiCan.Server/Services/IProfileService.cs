﻿using Microsoft.EntityFrameworkCore;
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

    public class ProfileService(DataContext dataContext) : IProfileService
    {
        private readonly DataContext _dataContext = dataContext;

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

        public async Task<(User?, string)> UpdateUserAsync(Guid userId, ProfileRequest request)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _dataContext.Users
                    .Include(u => u.Composer)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return (null, "Nutzer nicht gefunden.");
                }

                // Nutzer
                user.Name = request.name;
                user.EMail = request.email;
                user.Password = SecretHasher.Hash(request.password);

                if (user.Role == Roles.Kuenstler && !request.isComposer)
                {
                    user.Composer = null;
                }

                if (user.Role != Roles.Admin && user.Role != Roles.Banned)
                {
                    user.Role = request.isComposer ? Roles.Kuenstler : Roles.Nutzer;
                }
                user.UpdatedAt = DateTime.UtcNow;

                // Komponist
                if (request.isComposer)
                {
                    if (user.Composer == null)
                    {
                        user.Composer = new Composer { UserId = userId };
                    }

                    if (string.IsNullOrEmpty(request.genre) || string.IsNullOrEmpty(request.country) 
                        || string.IsNullOrEmpty(request.birthYear))
					{
						return (null, "Nutzer konnte nicht bearbeitet werden, Komponisten Attribute fehlen.");
					}

					user.Composer.ArtistName = request.name;
                    user.Composer.Genre = request.genre;
                    user.Composer.Country = request.country;
                    user.Composer.Description = request.description;
                    if (request.profileImage != null && !string.IsNullOrEmpty(request.mimetype))
                    {
                        user.Composer.ProfileImage = request.profileImage_b;
                        user.Composer.ProfileImageContentType = request.mimetype;
                    }
                    if (int.TryParse(request.birthYear, out int year))
                    {
                        user.Composer.BirthYear = new DateTime(year, 1, 1);
                    }
                    else
                    {
                        user.Composer.BirthYear = DateTime.MinValue;
                    }
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
            return (null, "Server Fehler.");
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
