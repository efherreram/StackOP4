using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Entities
{
    public class Question : IEntity
    {
        public Guid Id { get; private set; }

        public Question()
        {
            Id = Guid.NewGuid();
        }

        public int Votes { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public  Account Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
