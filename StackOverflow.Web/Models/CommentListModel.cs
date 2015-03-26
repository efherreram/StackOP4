using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class CommentListModel
    {

        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid OwnerIf { get; set; }
        public string OwnerName { get; set; }
        public Guid ReferenceToQuestionOrAnswer { get; set; }
        public Guid ReferenceToQuestion { get; set; }
    }
}