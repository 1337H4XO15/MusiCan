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
        /// gibt den Nutzer mit passender Nutzer ID zurück
        /// </summary>
        /// <param name="id">Nutzer ID</param>
        /// <returns>Nutzer</returns>
        Task<User?> GetUserByIdAsync(Guid id);
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

    }

}
