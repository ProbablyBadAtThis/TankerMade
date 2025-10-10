using System;

namespace TankerMade.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Protected constructor for Entity Framework
        protected User() { }

        // Main constructor for creating new users
        public User(Guid id, string username, string email, string role = "User")
        {
            Id = id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Role = role;
            CreatedAt = DateTime.UtcNow;
            LastLoginAt = null; // Set when user first logs in
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void UpdateRole(string newRole)
        {
            Role = newRole ?? throw new ArgumentNullException(nameof(newRole));
        }
    }
}
