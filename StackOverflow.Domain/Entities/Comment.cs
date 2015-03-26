using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Entities
{
    public class Comment :IEntity
    {
        public Guid Id { get; private set; }

        public string Description { get; set; }
        public Guid ReferenceToQuestionOrAnswer { get; set; }
        public Guid QuestionReference { get; set; }
        public virtual Account Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public int Votes { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid();
        }
    }
}
