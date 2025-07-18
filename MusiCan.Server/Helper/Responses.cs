﻿namespace MusiCan.Server.Helper
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ErrorResponse()
        {
        }

        public ErrorResponse(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }

    public class InvalidTokenErrorResponse : ErrorResponse
    {
        public InvalidTokenErrorResponse()
            : base("Ungültiger Nutzertoken.", 401)
        {
        }

        public InvalidTokenErrorResponse(string msg)
            : base(msg, 401)
        {
        }
    }

    public class AuthResponse
    {
        public string AuthToken { get; set; }

        public string Name { get; set; }
        
        public DateTime ExpireTime { get; set; }
    }

    public class ProfileResponse
    {
        public string Name { get; set; }
        public string Mail { get; set; }
        public Roles Role { get; set; }
        public string? ProfileImage { get; set; }
        public string? ProfileImageContentType { get; set; }
        public DateTime? BirthYear { get; set; }
        public string? Genre { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
    }

    public class DisplayComposer
    {
        public Guid Id { get; set; }

        public string ArtistName { get; set; }

        public string Genre { get; set; }

        public DateTime BirthYear { get; set; }

        public string Country { get; set; }

        public string? Description { get; set; }
        public string? ProfileImage { get; set; }

        public string? ProfileImageContentType { get; set; }
    }

    public class MusicOwner
    {
        public Guid Id { get; set; }

        public string Name { get;  set; }

        public bool isComposer { get; set; }
    }

    public class DisplayMusic
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Composer { get; set; }

        public string ContentType { get; set; }

        public string FileData { get; set; }

        public DateTime? Publication { get; set; }

        public string? Genre { get; set; }

        public DateTime Timestamp { get; set; }

        public MusicOwner? Owner { get; set; }
    }
}

