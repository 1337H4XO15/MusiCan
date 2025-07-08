using MusiCan.Server.Helper;
using System.ComponentModel.DataAnnotations;

namespace MusiCan.Server.Data
{
    public class User 
    {
        [Key]
        public Guid UserId { get; set; }

        [MaxLength(32)]
        public string Name {  get; set; }

        [MaxLength(128)]
        [StringLength(128)]
        public string Password { get; set; }

        [MaxLength(128)]
        public string EMail { get; set; }

        public Roles Role { get; set; }

        public ICollection<UserMusic> UserMusics { get; set; }

        // Parameterloser Konstruktor für EF Core
        public User() { }

        public User(string name, string password, string email, Roles role)
        {
            Name = name;
            Password = password;
            EMail = email;
            Role = role;
        }
    }
}
