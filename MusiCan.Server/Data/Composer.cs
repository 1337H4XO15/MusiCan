using MusiCan.Server.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusiCan.Server.Data
{
    public class Composer 
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [MaxLength(32)]
        public string ArtistName {  get; set; }

        [MaxLength(32)]
        public string Genre { get; set; }

        public DateTime BirthYear { get; set; }

        [MaxLength(32)]
        public string Country { get; set; }

        public byte[]? ProfileImage { get; set; }

        public string? ProfileImageContentType { get; set; }

        public string? Description { get; set; }

        public User User { get; set; }

        // Parameterloser Konstruktor für EF Core
        public Composer() { }
    }
}
