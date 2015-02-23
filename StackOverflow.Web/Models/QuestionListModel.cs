using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class QuestionListModel
    {
        public string Title { get; set; }
        public int Votes { get; set; }
        public DateTime CreationTime { get; set; }
        public string OwnerName { get; set; }

        public Guid OwnerId { get; set; }
        public Guid QuestionId { get; set; }
    }
}