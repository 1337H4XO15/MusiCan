using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusiCan.Server.Helper
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

    public class RegistrationErrorResponse : ErrorResponse
    {
        public RegistrationErrorResponse()
            : base("Error during registration", 401)
        {
        }

        public RegistrationErrorResponse(string msg)
            : base(msg, 401)
        {
        }
    }

    public class LoginErrorResponse : ErrorResponse
    {
        public LoginErrorResponse()
            : base("Error during login", 401)
        {
        }

        public LoginErrorResponse(string msg)
            : base(msg, 401)
        {
        }
    }

    public class InvalidTokenErrorResponse : ErrorResponse
    {
        public InvalidTokenErrorResponse()
            : base("Invalid Token", 498)
        {
        }

        public InvalidTokenErrorResponse(string msg)
            : base(msg, 498)
        {
        }
    }

    public class AuthResponse
    {
        public string AuthToken { get; set; }

        public string Name { get; set; }
        
        public DateTime ExpireTime { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ProfileResponse
    {
        public string Name { get; set; }
        public string Mail { get; set; }
        public Roles Role { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? ProfileImageContentType { get; set; }
        public DateTime? BirthYear { get; set; }
        public string? Genre { get; set; }
        public string? Country { get; set; }
        public string? Discription { get; set; }
    }

}

