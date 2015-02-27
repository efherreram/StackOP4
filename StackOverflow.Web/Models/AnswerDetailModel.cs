using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class AnswerDetailModel
    {

        public string AnswerText { get; set; }
        public int Votes { get; set; }
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid OwnerId { get; set; }
    }
}