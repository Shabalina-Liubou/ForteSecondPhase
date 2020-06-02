using System;

namespace DemoOybek.DomainObjects
{
    public enum Roles
    {
        Admin,
        User
    }

    public class User
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public DateTime DateCreated { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool HasAcceptedTerms { get; set; }

    }
}
