using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI.WebControls;
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
            var ans = context.Answers.Include(r => r.Owner).Include(r => r.QuestionReference);

            ans = ans.OrderByDescending(x => x.IsBestAnswer).ThenByDescending(y => y.Votes)
                   .ThenByDescending(z => z.CreationDate);           

            foreach (Answer q in ans)
            {
                if (Guid.Parse(id) != q.QuestionReference.Id)
                    continue;

                var model = new AnswerListModel();

                model.AnswerCount = "Answer " + (count++);
                if (q.IsBestAnswer) model.BestAnswer = "Best Answer";
                else model.BestAnswer = "";
                model.CreationDate = q.CreationDate;
                model.OwnerName = q.Owner.Name;
                model.Votes = q.Votes;
                model.OwnerId = q.Owner.Id;
                model.AnswerId = q.Id;
                model.QuestionId = q.QuestionReference.Id;
                model.AnswerText = q.AnswerText;
                model.QuestionOwnerId = q.QuestionReference.Owner.Id;
                model.UserHasVoted = AnswerHasBeenVoted(model.AnswerId);
                models.Add(model);   
            }

            context.SaveChanges();
            return PartialView(models);
        }

        public ActionResult RecentAnswers(Guid ownerId)
        {
            IList<AnswerListModel> models = new ListStack<AnswerListModel>();
            var context = new StackOverflowContext();
            var ans = context.Answers.Include(r => r.Owner).Include(r => r.QuestionReference);

            ans = ans.OrderByDescending(z => z.CreationDate);
            int cont = 0;
            foreach (Answer q in ans)
            {
                if (ownerId != q.Owner.Id)
                    continue;
                if (cont >= 5)
                    break;
                var model = new AnswerListModel();
                if (q.IsBestAnswer) model.BestAnswer = "Best Answer";
                else model.BestAnswer = "";
                model.CreationDate = q.CreationDate;
                model.OwnerName = q.Owner.Name;
                model.Votes = q.Votes;
                model.OwnerId = q.Owner.Id;
                model.AnswerId = q.Id;
                model.QuestionId = q.QuestionReference.Id;
                model.AnswerText = q.AnswerText;
                model.QuestionOwnerId = q.QuestionReference.Owner.Id;
                models.Add(model);
                cont++;
            }
            return PartialView(models);
        }
        private bool HasBest(IEnumerable<Answer> ans)
        {
            return ans.Any(x => x.IsBestAnswer);
        }

        private bool HasPoints(IEnumerable<Answer> ans)
        {
            return ans.Any(x => x.Votes>0);
        }

        public bool AnswerHasBeenVoted(Guid ansId)
        {
            var context = new StackOverflowContext();
            var votes = context.Votes;
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);

                if (Enumerable.Any(votes, v => v.AccountReference == ownerId && v.ReferenceToQuestionOrAnswer == ansId))
                {
                    return true;
                }

            }

            return false;
        }

        [Authorize]
        public ActionResult AddNewAnswer()
        {
            AddNewAnswerModel model = new AddNewAnswerModel();
            return PartialView(model);
        }

    
        public ActionResult AddAnswer(string desc)
        {
            var QuestionId = Guid.Parse(Session["CurrentQ"].ToString());
            if (desc.Split().Length < 5 || desc.Length < 50)
            {
                return RedirectToAction("QuestionDetails", "Question", new { id = QuestionId, errorMessage="Description Must Contain At Least 50 Characters and 5 Words"});
            }
            if (ModelState.IsValid)
            {          
                Guid id = Guid.Parse(QuestionId.ToString());
                var context = new StackOverflowContext();
                var newAnswer = new Answer();
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                    Guid ownerId = Guid.Parse(ticket.Name);
                    newAnswer.Votes = 0;
                    newAnswer.AnswerText = desc;
                    newAnswer.Owner = context.Accounts.FirstOrDefault(x => x.Id == ownerId);
                    newAnswer.ModificationDate = DateTime.Now;
                    newAnswer.CreationDate = DateTime.Now;
                    newAnswer.IsBestAnswer = false;
                    newAnswer.QuestionReference = context.Questions.Find(id);
                    newAnswer.NumberOfViews = 0;
                    context.Answers.Add(newAnswer);
                    context.SaveChanges();
                }
                return RedirectToAction("QuestionDetails","Question", new{id = QuestionId});
            }
            return RedirectToAction("QuestionDetails","Question",new{id=QuestionId});
        }


        public ActionResult AnswerDetails(Guid id)
        {
            var context = new StackOverflowContext();
            var answer = context.Answers.Include(r => r.Owner).Include(x => x.QuestionReference).Include(m=>m.QuestionReference.Owner).SingleOrDefault(z => z.Id == id);
            AnswerDetailModel model = _mappingEngine.Map<Answer, AnswerDetailModel>(answer);
            model.AnswerId = id;
            model.QuestionId = answer.QuestionReference.Id;
            model.OwnerId = answer.Owner.Id;
            model.QuestionOwnerId = answer.QuestionReference.Owner.Id;
            model.BestAnswer = (answer.IsBestAnswer ? "yes" : "no");
            return View(model);
        }

        [Authorize]
        public ActionResult LikeAnswer(Guid id, Guid qId)
        {
            var context = new StackOverflowContext();
            var answer = context.Answers.Find(id);
            var votes = context.Votes;
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);
                
                if (Enumerable.Any(votes, v => v.AccountReference == ownerId && v.ReferenceToQuestionOrAnswer==id))
                {
                    return RedirectToAction("QuestionDetails", "Question", new { id = qId });
                }

                votes.Add(new Vote() {AccountReference = ownerId, ReferenceToQuestionOrAnswer = id});
            }
            

            answer.Votes++;
            //context.Answers.Find(id).Votes++;

            context.SaveChanges();

            return RedirectToAction("QuestionDetails","Question", new{id = qId});
        }

        [Authorize]
        public ActionResult DisLikeAnswer(Guid id, Guid qId)
        {
            var context = new StackOverflowContext();
            var answer = context.Answers.Find(id);
            var votes = context.Votes;
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);

                if (Enumerable.Any(votes, v => v.AccountReference == ownerId && v.ReferenceToQuestionOrAnswer == id))
                {
                    return RedirectToAction("QuestionDetails", "Question", new { id = qId });
                }
            }


            answer.Votes--;
            //context.Answers.Find(id).Votes++;

            context.SaveChanges();

            return RedirectToAction("QuestionDetails", "Question", new { id = qId });
        }

        
        public ActionResult SetAsBestAnswer(Guid id, Guid qId)
        {
            var context = new StackOverflowContext();
            var answers = context.Answers.Include(r=>r.QuestionReference).Include(s=>s.Owner);
            var answer = answers.FirstOrDefault(x => x.Id == id);
            var question = context.Questions.Include(r => r.Owner).SingleOrDefault(s => s.Id == answer.QuestionReference.Id);
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);
                if(question.Owner.Id != ownerId)
                    return RedirectToAction("QuestionDetails","Question", new {id = qId});
            }

            if (answer.IsBestAnswer)
            {
                answer.IsBestAnswer = false;
            }
            else
            {
                foreach (Answer ans in answers)
                {
                    if (ans.QuestionReference.Id != qId)
                    {
                        continue;
                    }

                    if (ans.Id == id)
                        ans.IsBestAnswer = true;
                    else
                        ans.IsBestAnswer = false;
                }
            }

            context.SaveChanges();
            return RedirectToAction("QuestionDetails","Question", new {id = qId});

        }

        public ActionResult DeleteAnswer(Guid id, Guid qId)
        {
            var context = new StackOverflowContext();
            var answer = context.Answers.Include(x=>x.Owner).Include(y=>y.QuestionReference).FirstOrDefault(z=>z.Id==id);
            context.Answers.Remove(answer);
            context.SaveChanges();

            return RedirectToAction("QuestionDetails","Question", new {id = qId});
        }

    }
}