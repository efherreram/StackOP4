﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
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
        public ActionResult Index(int start = 0, int type=0)
        {            
            IList<QuestionListModel> models = new ListStack<QuestionListModel>();
            var context = new StackOverflowContext();
            var context2 = new StackOverflowContext();
            List<Question> que = new List<Question>();

            switch (type)
            {
                case 0:
                    que = context.Questions.Include(r => r.Owner)
                .OrderByDescending(x => x.CreationDate).ToList();
                    break;
                case 1:
                    que = context.Questions.Include(r => r.Owner)
                .OrderByDescending(x => x.NumberOfViews).ToList();
                    break;
                case 2:
                    que = context.Questions.Include(r => r.Owner)
                .OrderByDescending(x => x.Votes).ToList();
                    break;
                case 3:
                    que = context.Questions.Include(r => r.Owner)
                .OrderByDescending(x => x.CreationDate).ToList();
                    break;
            }
            
            var hasAvailable = true;
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
            for (i = 0; i < start+25; i++)
            {
                if (i == que.Count)
                {
                    hasAvailable = false;
                    break;
                }
                QuestionListModel model = new QuestionListModel();
                model.Title = que.ElementAt(i).Title;
                model.Votes = que.ElementAt(i).Votes;
                model.CreationTime = que.ElementAt(i).CreationDate;
                model.OwnerName = que.ElementAt(i).Owner.Name;
                model.QuestionId = que.ElementAt(i).Id;
                model.OwnerId = que.ElementAt(i).Owner.Id;
                model.Views = que.ElementAt(i).NumberOfViews;
                var trimmed = que.ElementAt(i).Description.Trim();
                var substring = trimmed.Substring(0, Math.Min(10, trimmed.Length));
                model.AnswerCount = AnswerCount(model.QuestionId);
                model.QuestionPreview = substring + "...";
                models.Add(model);
            }
            start = i;
            ViewData["start"] = start.ToString();
            ViewData["hasAvailable"] = hasAvailable;
            return View(models);        
        }

        private int AnswerCount(Guid qId)
        {
            var context = new StackOverflowContext();
            var count= context.Answers.Include(r => r.QuestionReference).Count(a => a.QuestionReference.Id == qId);
            return count;
        }

        public ActionResult IndexAddQuestion()
        {
            return View(new AddNewQuestionModel());
        }

        public ActionResult RecentQuestions(Guid ownerId)
        {
            IList<QuestionListModel> models = new ListStack<QuestionListModel>();
            var context = new StackOverflowContext();
            var que = context.Questions.Include(r => r.Owner).OrderByDescending(y => y.CreationDate).ToList();
            int i, count = 0;
            for (i = 0; i < que.Count ; i++)
            {
                if (count>=5 || que.ElementAt(i).Owner.Id != ownerId)
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
                model.Views = que.ElementAt(i).NumberOfViews;
                var trimmed = que.ElementAt(i).Description.Trim();
                var substring = trimmed.Substring(0, Math.Min(10, trimmed.Length));
                model.QuestionPreview = substring+"...";
                count++;
                models.Add(model);
            }
            return PartialView(models);
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
        public ActionResult QuestionDetails(Guid id, string errorMessage=null)
        {
            var context = new StackOverflowContext();
            var context2 = new StackOverflowContext();
            var question = context.Questions.Find(id);
            QuestionDetailsModel details = new QuestionDetailsModel();
            details.Description = question.Description;
            details.Title = question.Title;
            details.Score = question.Votes;
            details.QuestionId = id;
            details.ErrorMessage = errorMessage;
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);
                details.UserHasVoted =
                    (context2.Votes.FirstOrDefault(
                        x => x.AccountReference == ownerId && x.ReferenceToQuestionOrAnswer == id) != null);
            }
            else {details.UserHasVoted = false;}
            ++(question.NumberOfViews);
            Session["CurrentQ"] = question.Id;
            context.SaveChanges();
            ViewData["qId"] = id;
            return View(details);
        
        }

        [Authorize]
        public ActionResult LikeQuestion(Guid id)
        {
            var context = new StackOverflowContext();
            var question = context.Questions.Find(id);
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