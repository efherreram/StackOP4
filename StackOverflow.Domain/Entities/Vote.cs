using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Entities
{
    public class Vote : IEntity
    {
        public Guid Id { get; private set; }

        public Vote()
        {
            Id = Guid.NewGuid();
        }

        public Guid AccountReference { get; set; }
        public Guid ReferenceToQuestionOrAnswer { get; set; }
    }
}
