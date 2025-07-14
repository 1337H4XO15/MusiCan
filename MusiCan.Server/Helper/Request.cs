using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Helper
{
    public class RegistrationRequest
    {
        public required string name { get; set; }

        public required string password { get; set; }

        public required string email { get; set; }

        public required bool iscomposer { get; set; }
    }

    public class LoginRequest
    {
        public required string nameormail { get; set; }

        public required string password { get; set; }

        public bool remember { get; set; }

    }

    public class ProfileRequest
    {
        public required string name { get; set; }

        public required string password { get; set; }

        public required string email { get; set; }

        public required bool iscomposer { get; set; }

        public byte[]? profileimage { get; set; }

        public string? profileimagecontenttype { get; set; }

        public DateTime? birthyear { get; set; }

        public string? genre { get; set; }

        public string? country { get; set; }

        public string? description { get; set; }

    }

    public class MusicRequest
    {
        public Guid? id { get; set; }
        public required string title { get; set; }

        public required string composer { get; set; }
        
        public DateTime? releaseyear { get; set; }

        public string? genre { get; set; }

        public required string mimetype { get; set; }

        public required byte[] file { get; set; }
    }

    public class MusicIdRequest
    {
        public required Guid id { get; set; }
    }

    public class ComposerIdRequest
    {
        public required Guid id { get; set; }
    }
}
