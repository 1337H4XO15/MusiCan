using Microsoft.EntityFrameworkCore;
using MusiCan.Server.Data;

namespace MusiCan.DatabaseContext
{
    public class DataContext : DbContext
    {
        private readonly DbContext _dataContext;
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Music> Musics { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            _dataContext = this;
        }

        /// <summary>
        /// liest Nutzer aus User Datenbank anhand der Nutzer Id aus
        /// </summary>
        /// <param name="userId">Nutzer Guid</param>
        /// <returns>Nutzer</returns>
        public async Task<User?> GetByIdAsync(ushort userId)
        {
            return await _dataContext.Set<User>().FindAsync(userId);
        }


        /// <summary>
        /// Linking n:n Beziehung zwischen Nutzer und Musik
        /// </summarydotnet ef migrations add>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMusic>()
                .HasKey(um => new { um.UserId, um.MusicId });

            modelBuilder.Entity<UserMusic>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMusics)
                .HasForeignKey(um => um.UserId);

            modelBuilder.Entity<UserMusic>()
                .HasOne(um => um.Music)
                .WithMany(m => m.UserMusics)
                .HasForeignKey(um => um.MusicId);
        }
    }
}
