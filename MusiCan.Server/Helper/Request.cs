using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Helper
{
    public class RegistrationRequest
    {
        public required string Name { get; set; }

        public required string Password { get; set; }

        public required string EMail { get; set; }

        public required string Role { get; set; }
    }

    public class LoginRequest
    {
        public required string Name { get; set; }

        public required string Password { get; set; }

        public required string EMail { get; set; }

    }
}
