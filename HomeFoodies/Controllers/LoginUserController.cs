using HomeFoodies.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeFoodies.Controllers
{
    public class LoginUserController : Controller
    {
        // GET: LoginUser
        public ActionResult Index()
        {
            return View();
        }

        public Boolean VerifyCode(LoginUser model)
        {
            Boolean result = false;

            HomeFoodiesEntities _entity = new HomeFoodiesEntities();
            ObjectResult<LoginUserVerifyCode> resultSP = _entity.SP_LoginUserVerifyCode(model.VerificationCode, model.UserID);
            List<LoginUserVerifyCode> lstResult = resultSP.ToList();

            if (lstResult.Count > 0)
            {
                LoginUserVerifyCode currentUser = lstResult.FirstOrDefault();
                if (currentUser.UserID == model.UserID && currentUser.VerificationCode == model.VerificationCode)
                {
                    result = true;
                    currentUser.CurrentStatusID = (int)Helpers.StatusCodes.InActive;
                    new HomeFoodiesEntities().sp_LoginUser_UpdateStatus(currentUser.UserID, (int)Helpers.StatusCodes.InActive);
                    ViewBag.CurrentSignUpStatus = "SetPassword";
                }
            }

            return result;
        }

        public Boolean SetPassword(LoginUser model)
        {
            Boolean result = false;

            HomeFoodiesEntities _entity = new HomeFoodiesEntities();
            int resultSP = _entity.SP_LoginUserSetPassword(model.UserID, model.UserPassword);

            if (resultSP != null)
            {
                if (resultSP > 0)
                {
                    result = true;
                    ViewBag.CurrentSignUpStatus = "Active";
                }
            }

            return result;
        }


        public ActionResult Login(LoginUser model)
        {

            return View();
        }
    }
}