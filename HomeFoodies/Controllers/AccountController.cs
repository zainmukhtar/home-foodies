using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using HomeFoodies.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core.Objects;

namespace HomeFoodies.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public ApplicationUserManager AppUserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager AppSignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        /// <summary>
        /// Post Account/Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginUser model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                HomeFoodiesEntities _entity = new HomeFoodiesEntities();
                ObjectResult<LoginUserGetLogin> result = _entity.SP_LoginUserGetLogin(model.UserEmail);

                if (result != null)
                {
                    LoginUserGetLogin currentUser = result.First();
                    model.UserID = currentUser.UserID;

                    if (model.UserID > 0)
                    {
                        if (model.UserPassword == currentUser.UserPassword)
                        {                            
                            var identity = await AppUserManager.FindByIdAsync(model.UserEmail);
                            if (identity != null)
                            {
                                AppSignInManager.SignIn(identity, isPersistent: true, rememberBrowser: true);
                                identity.LoggedInUser = new LoginUser();
                                identity.LoggedInUser.UserID = currentUser.UserID;
                                identity.LoggedInUser.UserEmail = currentUser.UserEmail;
                                
                            }
                            if (!string.IsNullOrEmpty(returnUrl))
                                return RedirectToLocal(returnUrl);
                            else
                                return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.LoggedInUserType = model.CurrentStatusID;
                            ViewBag.ValidLogin = false;
                            ModelState.AddModelError("", "Password is wrong");
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewBag.ValidLogin = false;
                        ModelState.AddModelError("", "Invalid User ID");
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}