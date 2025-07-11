using Microsoft.EntityFrameworkCore;
using MusiCan.Server.Data;

namespace MusiCan.Server.DatabaseContext
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
        /// Linking 
        /// n:n Beziehung zwischen Nutzer und Musik
        /// 1:1 Beziehung zwischen Musik und Composer
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

            //modelBuilder.Entity<Composer>()
            //    .HasOne(c => c.User)
            //    .WithOne(u => u.Composer)
            //    .HasForeignKey<Composer>(c => c.UserId); // Typ setzten, da beide UserId haben

        }
    }
}
