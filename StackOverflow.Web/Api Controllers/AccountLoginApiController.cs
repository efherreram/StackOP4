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
using StackOverflow.Web.Models;

namespace StackOverflow.Web.Api_Controllers
{
    public class AccountLoginApiController : ApiController
    {
        //
        // GET: /AccountApi/
        public HttpResponseMessage PostLogin(AccountLoginModel model)
        {
            if (!model.Email.IsNullOrWhiteSpace() && !model.Password.IsNullOrWhiteSpace())
            {
                var context = new StackOverflowContext();
                var account =
                    context.Accounts.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if (account != null)
                {
                    HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created, model);
                    return response;
                }
            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
        }
	}
}