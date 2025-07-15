using MusiCan.Server.Helper;
using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Data
{
    public class Music
    {
        [Key]
        public Guid MusicId { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        [MaxLength(128)]
        public string Composer { get; set; }

        public string ContentType { get; set; }

        public byte[] FileData { get; set; }

        public DateTime? Publication { get; set; }

        [MaxLength(64)]
        public string? Genre { get; set; }

        public DateTime Timestamp { get; set; }

        public ICollection<UserMusic> UserMusics { get; set; }

        public bool Public { get; set; }

        // Parameterloser Konstruktor für EF Core
        public Music() { }

        public Music(MusicRequest request, User user)
        {
            Guid musicId = Guid.NewGuid();
            MusicId = musicId;
            Title = request.title;
            Composer = request.author;
            ContentType = request.mimetype;
            FileData = request.file_b;
            if (int.TryParse(request.releaseYear, out int year))
            {
                Publication = new DateTime(year, 1, 1);
            }
            Genre = request.genre;
            Timestamp = DateTime.Now;
            UserMusics = [ new UserMusic
            {
                LinkId = Guid.NewGuid(),
                UserId = user.UserId,
                MusicId = musicId,
                Access = Access.Owner
            } ];
            Public = user.Composer != null;
        }
    }
}
