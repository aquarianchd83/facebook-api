using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using FB_api.Filters;
using FB_api.Models;
using Facebook;
using System.Net;
using System.IO;

namespace FB_api.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(OAuthWebSecurity.RegisteredClientData);
        }

        [HttpPost]
        public void LogOff()
        {
            //todo: Need to logout from the social media site as well
            Helpers.StaticValue.Profile = null;
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));

            if (result.IsSuccessful && result.ExtraData != null)
            {
                Models.Profile profile = new Models.Profile
                {
                    AccessToken = result.ExtraData["accesstoken"],
                    Name = result.ExtraData["name"],
                    Id = result.ExtraData["id"],
                    Gender = result.ExtraData["gender"]
                };

                Helpers.StaticValue.Profile = profile;
                FormsAuthentication.SetAuthCookie(profile.Name, true);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            //if (result.IsSuccessful)
            //{
            //    //var client = new FacebookClient(result.ExtraData["accesstoken"]);

            //    //dynamic fbresult = client.Get("me/friends");

            //    //string graphApiMe = "https://graph.facebook.com/me?access_token=" + result.ExtraData["accesstoken"];
            //    //GetHTML(graphApiMe);

            //    Session["accesstoken"] = result.ExtraData["accesstoken"];

            //    //return fbresult["data"].ToString();
            //    return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            //}
        }
        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
