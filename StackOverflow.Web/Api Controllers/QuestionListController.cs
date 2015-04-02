using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using StackOverflow.Data;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;
using System.Data.Entity;

namespace StackOverflow.Web.Api_Controllers
{
    public class QuestionListController : ApiController
    {
        // GET: QuestionList

        public IEnumerable<QuestionListModel> Get()
        {
            List<QuestionListModel> models = new List<QuestionListModel>();
            var context = new StackOverflowContext();
            var questions = context.Questions.Include(x=>x.Owner);

            foreach (var q in questions)
            {
                models.Add(new QuestionListModel
                {
                    CreationTime = q.CreationDate,
                    OwnerId = q.Owner.Id,
                    OwnerName = q.Owner.Name,
                    QuestionId = q.Id,
                    QuestionPreview = q.Description,
                    Title = q.Title,
                    Views = q.NumberOfViews,
                    Votes = q.Votes
                });
            }

            return models;
        } 
    }
}