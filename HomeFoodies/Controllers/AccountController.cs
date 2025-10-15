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
        public async Task<ActionResult> Login(Supplier supplierModel, string returnUrl)
        {
            LoginUser model = supplierModel.LoginUser;

            if (supplierModel.LoginUser != null)
            {
                HomeFoodiesEntities _entity = new HomeFoodiesEntities();
                ObjectResult<LoginUserGetLogin> result = _entity.SP_LoginUserGetLogin(model.UserEmail);

                List<LoginUserGetLogin> lstResult = result.ToList();
                if (lstResult.Count > 0)
                {
                    LoginUserGetLogin currentUser = lstResult.FirstOrDefault();
                    model.UserID = currentUser.UserID;

                    if (model.UserID > 0)
                    {
                        if (currentUser.CurrentStatusID != (int)Helpers.StatusCodes.Active)
                        {
                            ViewBag.Message = "You are not uthorized to login, Please contact administrator!";
                            ViewBag.CurrentSignUpStatus = "InActiveUser";
                            return View("~/Views/Client/Home/Index.cshtml");
                        }
                        else if (model.UserPassword == currentUser.UserPassword)
                        {
                            var identity = await AppUserManager.FindByIdAsync(model.UserEmail);
                            if (identity != null)
                            {
                                AppSignInManager.SignIn(identity, isPersistent: true, rememberBrowser: true);
                                identity.Id = currentUser.UserID.ToString();
                                identity.UserName = currentUser.FullName;
                                identity.SupplierId = currentUser.SupplierID.ToString();

                                Helpers.SessionData _SessionData = new Helpers.SessionData();
                                _SessionData.LoggedInUser = currentUser;

                                Session["LoggedInSupplier"] = _SessionData;
                            }
                            ViewBag.Message = "";
                            if (!string.IsNullOrEmpty(returnUrl))
                                return RedirectToLocal(returnUrl);
                            else
                                return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.LoggedInUserType = model.CurrentStatusID;
                            ViewBag.ValidLogin = false;
                            //ModelState.AddModelError("", "Password is wrong");
                            ViewBag.CurrentSignUpStatus = "WrongPassord";
                            ViewBag.Message = "Wrong Password!";
                            return View("~/Views/Client/Home/Index.cshtml");
                        }
                    }
                }
                else
                {
                    ViewBag.ValidLogin = false;
                    //ModelState.AddModelError("", "Invalid User ID");
                    ViewBag.CurrentSignUpStatus = "InvalidUser";
                    ViewBag.Message = "Invalid User ID";
                    return View("~/Views/Client/Home/Index.cshtml");
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