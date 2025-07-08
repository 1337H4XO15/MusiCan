using MusiCan.Server.Helper;
using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Data
{
    public class UserMusic
    {
        [Key]
        public Guid LinkId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid MusicId { get; set; }
        public Music Music { get; set; }

        public Access Access { get; set; }
    }
}
