using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Antlr.Runtime.Misc;
using AutoMapper;
using StackOverflow.Data;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;

namespace StackOverflow.Web.Controllers
{
    public class AnswerController : Controller
    {
        private readonly IMappingEngine _mappingEngine;

        public AnswerController(IMappingEngine mappingEngine)
        {
            _mappingEngine = mappingEngine;
        }

        //
        // GET: /Answer/
        public ActionResult AnswerIndex(string id)
        {
            TempData["QuestionRef"] = id;
            int count = 1;
            IList<AnswerListModel> models = new ListStack<AnswerListModel>();
            var context = new StackOverflowContext();
            var ans = context.Answers.Include(r => r.Owner).Include(r=>r.QuestionReference);
            foreach (Answer q in ans)
            {
                if (Guid.Parse(id) != q.QuestionReference.Id)
                    continue;

                var model = new AnswerListModel();

                model.AnswerCount = "Answer " + (count++);
                model.CreationDate = q.CreationDate;
                model.OwnerName = q.Owner.Name;
                model.Votes = q.Votes;
                model.AnswerId = q.Id;
                models.Add(model);               
            }

            return View(models);
        }

        [Authorize]
        public ActionResult AddNewAnswer()
        {
            AddNewAnswerModel model = new AddNewAnswerModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewAnswer(AddNewAnswerModel model)
        {
            var QuestionId = TempData["QuestionRef"];
            Guid id = Guid.Parse(QuestionId.ToString());
            if (ModelState.IsValid)
            {
                var context = new StackOverflowContext();
                var newAnswer = _mappingEngine.Map<AddNewAnswerModel, Answer>(model);
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                    Guid ownerId = Guid.Parse(ticket.Name);
                    newAnswer.Votes = 0;
                    newAnswer.AnswerText = model.Description;
                    newAnswer.Owner = context.Accounts.FirstOrDefault(x => x.Id == ownerId);
                    newAnswer.ModificationDate = DateTime.Now;
                    newAnswer.CreationDate = DateTime.Now;
                    newAnswer.QuestionReference = context.Questions.Find(id);
                    context.Answers.Add(newAnswer);
                    context.SaveChanges();
                }
                return RedirectToAction("AnswerIndex", new{id = QuestionId});
            }
            return View();
        }


        public ActionResult AnswerDetails(Guid id)
        {
            var context = new StackOverflowContext();
            var answer = context.Answers.Find(id);
            AnswerDetailModel model = _mappingEngine.Map<Answer, AnswerDetailModel>(answer);
            model.AnswerId = id;
            return View(model);
        }

        [Authorize]
        public ActionResult LikeAnswer(Guid id)
        {
            var context = new StackOverflowContext();

            context.Answers.Find(id).Votes++;

            context.SaveChanges();

            return RedirectToAction("AnswerDetails", new{id = id});
        }

        [Authorize]
        public ActionResult DisLikeAnswer(Guid id)
        {
            var context = new StackOverflowContext();

            context.Answers.Find(id).Votes--;

            context.SaveChanges();

            return RedirectToAction("AnswerDetails", new { id = id });
        }

    }


}