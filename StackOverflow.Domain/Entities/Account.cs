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
        public Account()
        {
            Id = Guid.NewGuid();
        }

        
    }
}