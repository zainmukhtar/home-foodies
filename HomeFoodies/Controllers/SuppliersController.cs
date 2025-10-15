using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomeFoodies.Models;
using System.Data.Entity.Core.Objects;

namespace HomeFoodies.Controllers
{
    public class SuppliersController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        // GET: Suppliers
        public ActionResult Index()
        {
            if (User.Identity.Name != "Administrator")
                return RedirectToAction("Index", "Home");

            var suppliers = db.Suppliers.Include(s => s.LoginUser);
            return View(suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            ViewBag.LinkedLoginUserID = new SelectList(db.LoginUsers, "UserID", "UserEmail");
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierID,FullName,CompanyName,SupplierAddress,SupplierRegion,SupplierCity,LinkedLoginUserID,ContactNumber,SupplierEmail")] Supplier supplier)
        {
            //var userWithSameEmail = db.Suppliers.Where(model => model.SupplierEmail == supplier.SupplierEmail).SingleOrDefault();
            if (ModelState.IsValid)
            {
                HomeFoodiesEntities _entity = new HomeFoodiesEntities();
                ObjectResult<LoginUserGetLogin> resultSP = _entity.SP_LoginUserGetLogin(supplier.SupplierEmail);

                if (resultSP != null)
                {
                    List<LoginUserGetLogin> lstResult = resultSP.ToList();

                    if (lstResult.Count() > 0)
                    {
                        LoginUserGetLogin currentUser = lstResult.FirstOrDefault();

                        if (currentUser.UserID > 0)
                        {
                            supplier.SupplierID = Convert.ToInt32(currentUser.SupplierID);
                            supplier.LinkedLoginUserID = currentUser.UserID;
                            supplier.LoginUser = new LoginUser();
                            supplier.LoginUser.UserID = currentUser.UserID;
                            supplier.LoginUser.UserEmail = currentUser.UserEmail;
                            supplier.LoginUser.UserPassword = currentUser.UserPassword;
                            supplier.LoginUser.CurrentStatusID = currentUser.CurrentStatusID;
                            supplier.LoginUser.VerificationCode = currentUser.VerificationCode;
                            supplier.LoginUser.CreatedOn = currentUser.CreatedOn;

                            if (currentUser.CurrentStatusID == 3)
                            {
                                ViewBag.CurrentSignUpStatus = "VerificationSent";
                                supplier.LoginUser.VerificationCode = "";
                                ViewBag.MessageToShow = "Verifcation has been sent!";
                            }
                            else if (currentUser.CurrentStatusID == 2)
                            {
                                ViewBag.CurrentSignUpStatus = "SetPassword";
                                supplier.LoginUser.UserPassword = "";
                                ViewBag.MessageToShow = "Please set your password!";
                            }
                            else if (currentUser.CurrentStatusID == 1)
                            {
                                ViewBag.CurrentSignUpStatus = "Signed Up";
                                supplier.LoginUser.UserPassword = "";
                                ViewBag.MessageToShow = "Already Registerd! Kindly Login";
                            }

                            return View("~/Views/Client/Home/Index.cshtml", supplier);
                        }
                    }
                }


                db.Suppliers.Add(supplier);

                LoginUser objLoginUser = new LoginUser();
                objLoginUser.UserID = 0;
                objLoginUser.UserEmail = supplier.SupplierEmail;
                objLoginUser.UserPassword = "787hah*&%";
                objLoginUser.CurrentStatusID = (int)Helpers.StatusCodes.VerificationSent;
                objLoginUser.VerificationCode = new Random().Next(111111, 999999).ToString();
                objLoginUser.CreatedOn = DateTime.Now;
                db.LoginUsers.Add(objLoginUser);

                //db.SaveChanges();

                if (supplier.LoginUser == null)
                    supplier.LoginUser = new LoginUser();
                supplier.LoginUser = objLoginUser;

                db.SaveChanges();

                ViewBag.CurrentSignUpStatus = "VerificationSent";
                Helpers.Helper.SendSMSByGCM(supplier.FullName, supplier.ContactNumber, 1, supplier.LoginUser.VerificationCode);
            }


            supplier.LoginUser.VerificationCode = "";
            return View("~/Views/Client/Home/Index.cshtml", supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.LinkedLoginUserID = new SelectList(db.LoginUsers, "UserID", "UserEmail", supplier.LinkedLoginUserID);
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierID,FullName,CompanyName,SupplierAddress,SupplierRegion,SupplierCity,LinkedLoginUserID,ContactNumber,SupplierEmail")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LinkedLoginUserID = new SelectList(db.LoginUsers, "UserID", "UserEmail", supplier.LinkedLoginUserID);
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyCode(Supplier supplier)
        {
            LoginUserController _loginController = new LoginUserController();
            if (_loginController.VerifyCode(supplier.LoginUser))
            {
                ViewBag.CurrentSignUpStatus = "SetPassword";
                supplier.LoginUser.UserPassword = "";
                return View("~/Views/Client/Home/Index.cshtml", supplier);
            }
            else
            {
                ViewBag.MessageToShow = "Wrong Code!";
                ViewBag.CurrentSignUpStatus = "VerificationSent";
            }

            supplier.LoginUser.VerificationCode = "";
            return View("~/Views/Client/Home/Index.cshtml", supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPassword(Supplier supplier)
        {
            LoginUserController _loginController = new LoginUserController();
            Boolean IsPasswordSet = _loginController.SetPassword(supplier.LoginUser);
            if (IsPasswordSet)
            {
                ViewBag.CurrentSignUpStatus = "Active";
                String EmailBody = "Hi " + supplier.FullName + ","
                                    + System.Environment.NewLine
                                    + System.Environment.NewLine
                                    + "We welcome you to the Home Foodies Online portal."
                                    + System.Environment.NewLine
                                    + System.Environment.NewLine
                                    + "Warm Regards,"
                                    + System.Environment.NewLine
                                    + "The Team";
                Helpers.Helper.SendEmail(supplier.LoginUser.UserEmail, "Welcome to HomeFoodies", EmailBody);
                return View("~/Views/Client/Home/Index.cshtml", supplier);
            }
            return View("~/Views/Client/Home/Index.cshtml", supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendCodeAgain(Supplier supplier)
        {
            supplier = db.Suppliers.Find(supplier.SupplierID);
            LoginUser user = db.LoginUsers.Find(supplier.LinkedLoginUserID);

            if (supplier != null && user != null)
            {
                Helpers.Helper.SendSMSByGCM(supplier.FullName, supplier.ContactNumber, 1, user.VerificationCode);
                ViewBag.MessageToShow = "Sent Successfully!";
            }
            else
                ViewBag.MessageToShow = "Problem in Sending Code!";

            ViewBag.CurrentSignUpStatus = "VerificationSent";
            supplier.LoginUser.VerificationCode = "";
            return View("~/Views/Client/Home/Index.cshtml", supplier);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
