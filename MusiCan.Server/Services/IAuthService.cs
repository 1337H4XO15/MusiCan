using Microsoft.EntityFrameworkCore;
using MusiCan.Server.Data;
using MusiCan.Server.DatabaseContext;
using MusiCan.Server.Helper;
using Serilog;

namespace MusiCan.Server.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// überprüft ob ein Nutzer mit diesem Nutzername bereits vorhanden ist
        /// </summary>
        /// <param name="user_name">Nutzername</param>
        /// <returns>True wenn ein Nutzer diesen Nutzername hat</returns>
        Task<bool> CheckUserNameAsync(string username);
        /// <summary>
        /// überprüft ob ein Nutzer mit dieser email bereits vorhanden ist
        /// </summary>
        /// <param name="mail">email</param>
        /// <returns>True wenn ein Nutzer diesen email hat</returns>
        Task<bool> CheckUserMailAsync(string mail);
		/// <summary>
		/// Nutzer neu Erstellen
		/// </summary>
		/// <param name="request">Nutzer / Künstlerprofil</param>
		/// <returns>Nutzer oder Error</returns>
		Task<(User?, string)> CreateUserAsync(ProfileRequest request);
        /// <summary>
        /// Nutzer aus User Datenbank auslesen und password DeviceHash überprüfen
        /// </summary>
        /// <param name="namemail">Nutzername oder email</param>
        /// <param name="password">password DeviceHash</param>
        /// <returns>Nutzer</returns>
        Task<User?> AuthenticateAsync(string namemail, string password);
    }

    public class AuthService(DataContext dataContext) : IAuthService
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<bool> CheckUserNameAsync(string username)
        {
            IEnumerable<User> users = await _dataContext.Users
             .Where(u => u.Name == username)
             .AsNoTracking()
             .ToListAsync();

            if (users.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CheckUserMailAsync(string mail)
        {
            IEnumerable<User> users = await _dataContext.Users
             .Where(u => u.EMail == mail)
             .AsNoTracking()
             .ToListAsync();

            if (users.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<(User?, string)> CreateUserAsync(ProfileRequest request)
        {
            var transaction = _dataContext.Database.BeginTransaction();

            try
            {
                string password = SecretHasher.Hash(request.password);

                User user = new(request.name, password, request.email, Roles.Nutzer);

                if (request.isComposer)
                {
                    user.Role = Roles.Kuenstler;
					user.Composer = new Composer { UserId = user.UserId };

					if (string.IsNullOrEmpty(request.genre) || string.IsNullOrEmpty(request.country)
		                || string.IsNullOrEmpty(request.birthYear))
					{
						return (null, "Nutzer konnte nicht erstellt werden, Komponisten Attribute fehlen.");
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

				_dataContext.Add(user);

                await _dataContext.SaveChangesAsync();

				await transaction.CommitAsync();

				Log.Information($"User {user.UserId} created successfully.");

				return (user, "");
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				Log.Error($"Error while creating user {ex}");
			}
			return (null, "Nutzer konnte nicht erstellt werden.");
		}

        public async Task<User?> AuthenticateAsync(string namemail, string password)
        {
            try
            {
                User? user;

                user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Name == namemail);

                if (user == null)
                {
                    user = await _dataContext.Users.FirstOrDefaultAsync(u => u.EMail == namemail);
                }

                if (user == null)
                {
                    Log.Error("User is null!");
                    return null; // Authentication fehlgeschlagen
                }

                if (!SecretHasher.VerifyHash(password, user.Password))
                {
                    Log.Error("Invalid password hash!");
                    return null; // Authentication fehlgeschlagen
                }

                return user; // Authentication erfolgreich
            }
            catch (Exception ex)
            {
                Log.Error($"Error during Login->AuthenticateAsync of user {namemail}: {ex}");
                return null; // schreiben in DB fehlgeschlagen
            }
        }
    }

}
