using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Antlr.Runtime.Misc;
using StackOverflow.Data;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;
using System.Data.Entity;

namespace StackOverflow.Web.Controllers
{
    public class CommentController : Controller
    {
        //
        // GET: /Comment/
        public ActionResult CommentList(Guid id, Guid qId)
        {
            var context = new StackOverflowContext();
            var m = new CommentListCollectionModel()
            {
                Comments = new ListStack<CommentListModel>(),
                ParentReference = id, QuestionReference = qId
            };
            
            IList<CommentListModel> models = new ListStack<CommentListModel>();
            
            var comments = context.Comments.Include(x => x.Owner);
            foreach (Comment com in comments)
            {
                if (com.ReferenceToQuestionOrAnswer == id)
                {
                    var c = new CommentListModel
                    {
                        CreationDate = com.CreationDate,
                        Description = com.Description,
                        OwnerIf = com.Owner.Id,
                        ReferenceToQuestionOrAnswer = com.ReferenceToQuestionOrAnswer,
                        ReferenceToQuestion = com.QuestionReference,
                        OwnerName = com.Owner.Name
                    };
                    m.Comments.Add(c);
                    
                }
            }
            return PartialView(m);
        }


        public ActionResult AddNewComment(Guid parentId, string description,Guid qId)
        {
            if (description == "undefined")
            {
                return RedirectToAction("QuestionDetails", "Question", new { id = qId });
            }
            var comment = new Comment();
            var context = new StackOverflowContext();
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);
                comment.CreationDate = DateTime.Now;
                comment.Description = description;
                comment.Owner = context.Accounts.Find(ownerId);
                comment.ReferenceToQuestionOrAnswer = parentId;
                comment.Votes = 0;
                comment.QuestionReference = qId;
                context.Comments.Add(comment);
                context.SaveChanges();
            }
            return RedirectToAction("QuestionDetails", "Question",new{id=qId});
        }
        
	}
}