using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
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
using StackOverflow.Domain.Services;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
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
                var account = context.Accounts.FirstOrDefault(x => x.Email == model.Email);
                if (account != null)
                {
                    model.ErrorMessage = "Email Already Used";
                    return View(model);
                }
                account = _mappingEngine.Map<AccountRegisterModel, Account>(model);
                account.Name += (" " + model.Surname);
                account.CreationDate = DateTime.Now;
                account.LastLogDate = DateTime.Now;
                account.ViewsToProfile = 0;
                account.IsVerified = false;
                context.Accounts.Add(account);
                context.SaveChanges();

                string host = Request.Url.GetLeftPart(UriPartial.Authority);
                string message;
                if (host.Contains("localhost:"))
                {
                    message = "Click to Confirm: " + host + "/Account/ConfirmRegister?id=" + account.Id;
                }
                else
                {
                    message = "Click to Confirm: " + "http://stackop4.apphb.com/Account/ConfirmRegister?id=" + account.Id;
                }
                new MailService().SendMail(model.Email, message);

                return RedirectToAction("Login", new AccountLoginModel());
            }
            return View(model);
        }

        public ActionResult ConfirmRegister(string id)
        {
            var context = new StackOverflowContext();
            Guid s = Guid.Parse(id);
            var account = context.Accounts.FirstOrDefault(x=>x.Id == s);
            //context.Accounts.FirstOrDefault(x => x.Id == s);
            account.IsVerified = true;
            TempData["Success"] = "Welcome <strong>" + account.Name + "</strong>!";

            context.SaveChanges();
            
            return RedirectToAction("Login");
        }

        public ActionResult Login(string email)
        {
            var model = new AccountLoginModel();
            Session["Strikes"] = 0;
            if (TempData["Error"] != null)
                model.ErrorMessage = TempData["Error"].ToString();
            if(TempData["Success"] != null)
                model.SuccessMessage = TempData["Success"].ToString();

            return View(model);
        }

        [HttpPost]
        public ActionResult Login(AccountLoginModel model)
        {
            
            if (ModelState.IsValid)
            {
                if (int.Parse(Session["Strikes"].ToString()) >= 3)
                {
                    RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

                    if (String.IsNullOrEmpty(recaptchaHelper.Response))
                    {
                        ModelState.AddModelError("", "Captcha answer cannot be empty.");
                        return View(model);
                    }

                    RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

                    if (recaptchaResult != RecaptchaVerificationResult.Success)
                    {
                        ModelState.AddModelError("", "Incorrect captcha answer.");
                    }
                }
                if (TempData["Success"] != null)
                    model.SuccessMessage = TempData["Success"].ToString();
                if (TempData["Error"] != null)
                    model.ErrorMessage = TempData["Error"].ToString();

                var context = new StackOverflowContext();
                var account = context.Accounts.FirstOrDefault(x => x.Email == model.Email);
                if (account != null)
                {
                    if (!account.IsVerified)
                    {
                        model.ErrorMessage = "Account Has not Been Verified";
                        return View(model);
                    }

                    if (account.Password != model.Password)
                    {
                        SendWarningEmail(model.Email);
                        model.ErrorMessage = "Wrong Password";
                        
                        Session["Strikes"] = int.Parse(Session["Strikes"].ToString())+1;
                        model.MistakesWereMade = int.Parse(Session["Strikes"].ToString());
                        return View(model);
                    }
                    FormsAuthentication.SetAuthCookie(account.Id.ToString(), false);
                    return RedirectToAction("Index", "Question");
                }
            }
            model.ErrorMessage = "Account not Found";
            
            Session["Strikes"] = int.Parse(Session["Strikes"].ToString()) + 1;
            model.MistakesWereMade = int.Parse(Session["Strikes"].ToString());
            return View(model);
        }

        private void SendWarningEmail(string email)
        {
            string message = "<strong>It appears someone tried, unsuccesfully, to enter your account</strong>";
            new MailService().SendMail(email, message);
        }

        public ActionResult Logout()
        {
            var context = new StackOverflowContext();
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                Guid userId = Guid.Parse(ticket.Name);
                context.Accounts.Find(userId).LastLogDate = DateTime.Now;
                context.SaveChanges();
            }
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
            

            var context = new StackOverflowContext();
            var account = context.Accounts.FirstOrDefault(x => x.Email == model.email);

            if (account == null)
            {
                model.Error = String.Format("Email <strong>{0}</strong> Does Not Exist", model.email);
                model.email = "";
                return View(model);
            }
            
            //    return RedirectToAction("PasswordDisplay",new{password = account.Password});
            string host = Request.Url.GetLeftPart(UriPartial.Authority);
            string message;
            if (host.Contains("localhost:"))
            {
                message = "Click to get your Password: " + host + "/Account/ChangePassword?id=" + account.Id;
            }
            else
            {
                message = "Click to get your Password: " + "http://stackop4.apphb.com/Account/ChangePassword?id=" + account.Id;
            }
            new MailService().SendMail(model.email,message);
            model = new PasswordRecovery
            {
                Success = "An Email Has Been Sent With Instructions to Recover your Password",
                email = ""
            };
            return View(model);
        }

        public ActionResult ChangePassword(string id)
        {
            ChangePasswordModel model = new ChangePasswordModel() {OwnerId = Guid.Parse(id)};
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var context = new StackOverflowContext();
                var account = context.Accounts.Find(model.OwnerId);
                account.Password = model.Password;
                context.SaveChanges();
                TempData["Success"] = "Your Password Has Been Updated Successfully!";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public ActionResult ProfileView(ProfileModel model, Guid id)
        {
            var context = new StackOverflowContext();
            var account = context.Accounts.Find(id);

            model.Email = account.Email;
            model.UserName = account.Name;
            model.CreationDate = account.CreationDate;
            model.Views = ++(account.ViewsToProfile);
            model.LastLogTime = account.LastLogDate;
            model.UserId = account.Id;
            context.SaveChanges();
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
                model.CreationDate = context.Accounts.FirstOrDefault(x => x.Id == UserId).CreationDate;
                model.LastLogTime = context.Accounts.FirstOrDefault(x => x.Id == UserId).LastLogDate;
                model.Views = context.Accounts.FirstOrDefault(x => x.Id == UserId).ViewsToProfile;
                return RedirectToAction("ProfileView", new { id = UserId });
            }
            return RedirectToAction("Index", "Question");
        }

        public ActionResult PasswordDisplay(string id)
        {
            PasswordDisplayModel model = new PasswordDisplayModel();
            var context = new StackOverflowContext();
            var account = context.Accounts.Find(Guid.Parse(id));
            model.Password = account.Password;

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
