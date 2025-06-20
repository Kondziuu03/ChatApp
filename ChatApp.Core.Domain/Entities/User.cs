﻿namespace ChatApp.Core.Domain.Entities
{
    public class User
    {
        public User(string username, string password)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
