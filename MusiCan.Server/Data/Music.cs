using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Data
{
    public class Music
    {
        [Key]
        public Guid MusicId { get; set; }

        [MaxLength(128)]
        public string Composer { get; set; }

        public string ContentType { get; set; }

        public byte[] FileData { get; set; }

        public DateTime Publication { get; set; }

        [MaxLength(64)]
        public string Genre { get; set; }

        public DateTime Timestamp { get; set; }

        public ICollection<UserMusic> UserMusics { get; set; }

        // Parameterloser Konstruktor für EF Core
        public Music() { }
    }
}
