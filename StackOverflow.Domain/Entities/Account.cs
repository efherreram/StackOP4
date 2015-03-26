using System;
using StackOverflow.Domain.Entities;

namespace StackOverflow.Domain.Entities
{
    public class Account : IEntity
    {
        public Guid Id { get; private set ; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsVerified { get; set; }
        public int ViewsToProfile { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLogDate { get; set; }
        public Account()
        {
            Id = Guid.NewGuid();
        }

        
    }
}