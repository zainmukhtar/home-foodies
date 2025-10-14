using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomeFoodies.Models;

namespace HomeFoodies.Controllers
{
    public class SuppliersController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        // GET: Suppliers
        public ActionResult Index()
        {
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
            if (ModelState.IsValid)
            {
                db.Suppliers.Add(supplier);

                LoginUser objLoginUser = new LoginUser();
                objLoginUser.UserID = 0;
                objLoginUser.UserEmail = supplier.ContactNumber;
                objLoginUser.UserPassword = "787hah*&%";
                objLoginUser.CurrentStatusID = (int)Helpers.StatusCodes.VerificationSent;
                objLoginUser.VerificationCode = new Random().Next(111111, 999999).ToString();
                objLoginUser.CreatedOn = DateTime.Now;
                db.LoginUsers.Add(objLoginUser);

                db.SaveChanges();

                supplier.LinkedLoginUserID = objLoginUser.UserID;

                db.SaveChanges();

                ViewBag.val = "sent";
            }

            ViewBag.LinkedLoginUserID = new SelectList(db.LoginUsers, "UserID", "UserEmail", supplier.LinkedLoginUserID);
            Redirect("~/Views/Client/Home/Index.cshtml");
            return View("~/Views/Client/Home/Index.cshtml");
            //return View(supplier);
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
