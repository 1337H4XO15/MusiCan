using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Helper
{
    public class RegistrationRequest
    {
        public required string name { get; set; }

        public required string password { get; set; }

        public required string email { get; set; }

        public required bool isComposer { get; set; }
    }

    public class LoginRequest
    {
        public required string nameOrMail { get; set; }

        public required string password { get; set; }

        public bool remember { get; set; }

    }

    public class ProfileRequest
    {
        public required string name { get; set; }

        public required string password { get; set; }

        public required string email { get; set; }

        public required bool isComposer { get; set; }

        public byte[]? profileImage { get; set; }

        public string? profileImageContentType { get; set; }

        public DateTime? birthYear { get; set; }

        public string? genre { get; set; }

        public string? country { get; set; }

        public string? description { get; set; }

    }

    public class MusicRequest
    {
        public Guid? id { get; set; }
        public required string title { get; set; }

        public required string author { get; set; }
        
        public string? releaseYear { get; set; }

        public string? genre { get; set; }

        public required string mimetype { get; set; }

        [FromForm]
        public required IFormFile file { get; set; }

        public byte[]? file_b { get; set; }
    }
}
