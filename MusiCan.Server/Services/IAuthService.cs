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
        /// <param Name="user_name">Nutzername</param>
        /// <returns>True wenn ein Nutzer diesen Nutzername hat</returns>
        Task<bool> CheckUserNameAsync(string username);
        /// <summary>
        /// überprüft ob ein Nutzer mit dieser email bereits vorhanden ist
        /// </summary>
        /// <param Name="mail">email</param>
        /// <returns>True wenn ein Nutzer diesen email hat</returns>
        Task<bool> CheckUserMailAsync(string mail);
        /// <summary>
        /// Nutzer neu Erstellen, wenn DeviceHash noch nicht verwendet
        /// </summary>
        /// <param Name="username">Nutzername</param>
        /// <param Name="password">password</param>
        /// <param Name="mail">email</param>
        /// <param Name="role">Rolle</param>
        /// <returns>Nutzer</returns>
        Task<User?> CreateUserAsync(string username, string password, string mail, Roles role);
        /// <summary>
        /// Nutzer aus User Datenbank auslesen und password DeviceHash überprüfen
        /// </summary>
        /// <param Name="namemail">Nutzername oder email</param>
        /// <param Name="password">password DeviceHash</param>
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

        public async Task<User?> CreateUserAsync(string username, string password, string mail, Roles role)
        {
            var transaction = _dataContext.Database.BeginTransaction();

            try
            {
                string hash = SecretHasher.Hash(password);

                User user = new(username, hash, mail, role);
                _dataContext.Add(user);

                await _dataContext.SaveChangesAsync();

                transaction.Commit();

                return user;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error($"Error while creating user {ex}");
                return null; // schreiben in DB fehlgeschlagen
            }      
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
