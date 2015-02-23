using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Entities
{
    public class Answer : IEntity
    {
        public Guid Id { get; private set; }

        public Answer()
        {
            Id = Guid.NewGuid();
        }

        public string AnswerText { get; set; }
        public int Votes { get; set; }
        public Account Owner { get; set; }
        public Question QuestionReference { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
