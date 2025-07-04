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

    }
}
