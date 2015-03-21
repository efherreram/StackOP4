using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;
using Antlr.Runtime.Misc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using StackOverflow.Data;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;

namespace StackOverflow.Web.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly IMappingEngine _mappingEngine;

        public QuestionController(IMappingEngine mappingEngine)
        {
            _mappingEngine = mappingEngine;
        }
        
        // GET: /Question/
        [AllowAnonymous]
        public ActionResult Index(int start = 0)
        {
            IList<QuestionListModel> models = new ListStack<QuestionListModel>();
            var context = new StackOverflowContext();
            var que = context.Questions.Include(r =>r.Owner).ToList();
            //foreach (Question q in que)
            //{
            //    QuestionListModel model = new QuestionListModel();
            //    model.Title = q.Title;
            //    model.Votes = q.Votes;
            //    model.CreationTime = q.CreationDate;
            //    model.OwnerName = q.Owner.Name;
            //    model.QuestionId = q.Id;
            //    model.OwnerId = q.Owner.Id;
            //    models.Add(model);
            //}
            int i;
            for (i = start; i < start+6; i++)
            {
                if (i == que.Count)
                {
                    break;
                }
                QuestionListModel model = new QuestionListModel();
                model.Title = que.ElementAt(i).Title;
                model.Votes = que.ElementAt(i).Votes;
                model.CreationTime = que.ElementAt(i).CreationDate;
                model.OwnerName = que.ElementAt(i).Owner.Name;
                model.QuestionId = que.ElementAt(i).Id;
                model.OwnerId = que.ElementAt(i).Owner.Id;
                model.QuestionPreview = que.ElementAt(i).Description.Substring(0, 2) + "...";
                models.Add(model);
            }
            start = i;
            ViewData["start"] = start.ToString();
            return View(models);        
        }

        public ActionResult IndexAddQuestion()
        {
            return View(new AddNewQuestionModel());
        }

        [HttpPost]
        public ActionResult IndexAddQuestion(AddNewQuestionModel model)
        {
            if (ModelState.IsValid)
            {
                 var context = new StackOverflowContext();
                 var newQuestion = _mappingEngine.Map<AddNewQuestionModel, Question>(model);
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                    Guid ownerId = Guid.Parse(ticket.Name);
                    newQuestion.Votes = 0;
                    newQuestion.Owner = context.Accounts.FirstOrDefault(x => x.Id == ownerId);
                    newQuestion.ModificationDate = DateTime.Now;
                    newQuestion.CreationDate = DateTime.Now;
                    context.Questions.Add(newQuestion);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            return View(model);
        
        }

        [AllowAnonymous]
        public ActionResult QuestionDetails(Guid id)
        {
            var context = new StackOverflowContext();
            var question = context.Questions.Find(id);
            QuestionDetailsModel details = new QuestionDetailsModel();
            details.Description = question.Description;
            details.Title = question.Title;
            details.Score = question.Votes;
            details.QuestionId = id;
            
            return View(details);
        
        }

        [Authorize]
        public ActionResult LikeQuestion(Guid id)
        {
            var context = new StackOverflowContext();
            var question = context.Answers.Find(id);
            var votes = context.Votes;
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);

                if (Enumerable.Any(votes, v => v.AccountReference == ownerId && v.ReferenceToQuestionOrAnswer == id))
                {
                    return RedirectToAction("QuestionDetails", "Question", new { id = id });
                }

                votes.Add(new Vote() { AccountReference = ownerId, ReferenceToQuestionOrAnswer = id });
            }


            question.Votes++;
            //context.Answers.Find(id).Votes++;

            context.SaveChanges();

            return RedirectToAction("QuestionDetails", "Question", new { id = id });
 
        }

        [Authorize]
        public ActionResult DisLikeQuestion(Guid id)
        {
            var context = new StackOverflowContext();
            var question = context.Answers.Find(id);
            var votes = context.Votes;
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);

                if (Enumerable.Any(votes, v => v.AccountReference == ownerId && v.ReferenceToQuestionOrAnswer == id))
                {
                    return RedirectToAction("QuestionDetails", "Question", new { id = id });
                }

                votes.Add(new Vote() { AccountReference = ownerId, ReferenceToQuestionOrAnswer = id });
            }


            question.Votes--;
            //context.Answers.Find(id).Votes++;

            context.SaveChanges();

            return RedirectToAction("QuestionDetails", "Question", new { id = id });
 
        }

        public ActionResult DeleteQuestion(Guid id)
        {
            var context = new StackOverflowContext();
            var question = context.Questions.Find(id);

            var answers = context.Answers.Include(r => r.QuestionReference);
            foreach (Answer ans in answers)
            {
                if (ans.QuestionReference.Id == id)
                {
                    context.Answers.Remove(ans);
                }
            }

            context.Questions.Remove(question);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}