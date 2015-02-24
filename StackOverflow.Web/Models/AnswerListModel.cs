using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class AnswerListModel
    {

        public string OwnerName { get; set; }
        public int Votes { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid OwnerId { get; set; }
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }

        public string AnswerCount { get; set; }
        public string BestAnswer { get; set; }

    }
}