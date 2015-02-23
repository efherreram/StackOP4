using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class QuestionDetailsModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }

        public Guid QuestionId { get; set; }
    }
}