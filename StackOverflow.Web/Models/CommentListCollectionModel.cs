using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class CommentListCollectionModel
    {

        public IList<CommentListModel> Comments { get; set; }
        public Guid QuestionReference { get; set; }
        public Guid ParentReference { get; set; }
    }
}