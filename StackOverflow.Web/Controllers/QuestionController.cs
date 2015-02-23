﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
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
        public ActionResult Index()
        {
            IList<QuestionListModel> models = new ListStack<QuestionListModel>();
            var context = new StackOverflowContext();
            var que = context.Questions.Include(r =>r.Owner);
            foreach (Question q in que)
            {
                QuestionListModel model = new QuestionListModel();
                model.Title = q.Title;
                model.Votes = q.Votes;
                model.CreationTime = q.CreationDate;
                model.OwnerName = q.Owner.Name;
                model.QuestionId = q.Id;
                model.OwnerId = q.Owner.Id;
                models.Add(model);
            }
            
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

            context.Questions.Find(id).Votes++;

            context.SaveChanges();

            return RedirectToAction("QuestionDetails", new { id = id });
        }

        [Authorize]
        public ActionResult DisLikeQuestion(Guid id)
        {
            var context = new StackOverflowContext();

            context.Questions.Find(id).Votes--;

            context.SaveChanges();

            return RedirectToAction("QuestionDetails", new { id = id });
        }
	}
}