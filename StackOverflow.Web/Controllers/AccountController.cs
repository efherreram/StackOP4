using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Autofac.Core;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using RestSharp;
using StackOverflow.Data;
using StackOverflow.Domain.Entities;
using StackOverflow.Web.Models;
using HttpCookie = System.Web.HttpCookie;

namespace StackOverflow.Web.Controllers
{
    public class AccountController : Controller
    {
        readonly IMappingEngine _mappingEngine;
        readonly IReadOnlyRepository _readOrWriteRepository;

        public AccountController(IMappingEngine mappingEngine, IReadOnlyRepository readOrWriteRepository)
        {
            _mappingEngine = mappingEngine;
            _readOrWriteRepository = readOrWriteRepository;
        }

        public ActionResult Register()
        {
            return View(new AccountRegisterModel());
        }

        [HttpPost]
        public ActionResult Register(AccountRegisterModel model)
        {
            if (ModelState.IsValid)
            {

                var context = new StackOverflowContext();
                var account = _mappingEngine.Map<AccountRegisterModel, Account>(model);
                context.Accounts.Add(account);
                context.SaveChanges();

                return RedirectToAction("Login", new AccountLoginModel());
            }
            return View(model);
        }

        public ActionResult Login(string email)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var context = new StackOverflowContext();
                var account = context.Accounts.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (account != null)
                {
                    FormsAuthentication.SetAuthCookie(account.Id.ToString(), false);
                    return RedirectToAction("Index", "Question");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult PasswordRecovery()
        {
            return View(new PasswordRecovery());
        }

        [HttpPost]
        public ActionResult PasswordRecovery(PasswordRecovery model)
        {
            SmtpClient mailClient = new SmtpClient();
            mailClient.Host = "smtp.gmail.com";
            mailClient.Port = 25;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.EnableSsl = true;
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = new NetworkCredential("mywebsmpt@gmail.com","KongKollective" );

            var context = new StackOverflowContext();
            var account = context.Accounts.FirstOrDefault(x => x.Email == model.email);

            if (account == null)
            {
                return View();
            }
            
            try
            {
                string email = account.Email;
                var mensaje = new MailMessage("mywebsmpt@gmail.com", email);
                mensaje.Subject = "Password Recovery";
                mensaje.Body = account.Password;
                mailClient.Send(mensaje);
            }
            catch (Exception ex)
            {
                
            }
            //if(account != null)
            //    return RedirectToAction("PasswordDisplay",new{password = account.Password});

            return RedirectToAction("Login");
        }

        public ActionResult ProfileView(ProfileModel model, Guid id)
        {
            var context = new StackOverflowContext();
            var account = context.Accounts.Find(id);

            model.Email = account.Email;
            model.UserName = account.Name;
            

            return View(model);
        }

        public ActionResult MyProfile()
        {
            var context = new StackOverflowContext();
            ProfileModel model = new ProfileModel();
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid UserId = Guid.Parse(ticket.Name);
                model.Email = context.Accounts.FirstOrDefault(x => x.Id == UserId).Email;
                model.UserName = context.Accounts.FirstOrDefault(x => x.Id == UserId).Name;

                return RedirectToAction("ProfileView", new { id = UserId });
            }
            return RedirectToAction("Index", "Question");
        }

        public ActionResult PasswordDisplay(string password)
        {
            PasswordDisplayModel model = new PasswordDisplayModel();

            model.Password = password;

            return View(model);
        }

        public IHtmlString IsOwner(Guid id)
        {
            var context = new StackOverflowContext();
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid ownerId = Guid.Parse(ticket.Name);

                if (id == ownerId)
                {
                    return new HtmlString("Yes");
                }
            }
            return new HtmlString("No");
        }
    }
}
