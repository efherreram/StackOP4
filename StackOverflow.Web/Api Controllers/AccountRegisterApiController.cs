using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using StackOverflow.Data;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;

namespace StackOverflow.Web.Api_Controllers
{
    public class AccountRegisterApiController : ApiController
    {
        //
        // GET: /AccountRegisterApi/
        public HttpResponseMessage PostRegister(AccountRegisterModel model)
        {
            if (!model.Email.IsNullOrWhiteSpace() && !model.Password.IsNullOrWhiteSpace()
                && !model.ConfirmPassword.IsNullOrWhiteSpace() && !model.Surname.IsNullOrWhiteSpace()
                && !model.Name.IsNullOrWhiteSpace() && model.ConfirmPassword == model.Password)
            {
                var context = new StackOverflowContext();
                var account = new Account
                {
                    CreationDate = DateTime.Now,
                    Email = model.Email,
                    IsVerified = true,
                    LastLogDate = DateTime.Now,
                    Name = model.Name + " " + model.Surname,
                    Password = model.Password,
                    ViewsToProfile = 0
                };

                context.Accounts.Add(account);
                context.SaveChanges();

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created, model);
                return response;

            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));

        }
	}
}