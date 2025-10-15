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
    public class UOMTypesController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        // GET: UOMTypes
        public ActionResult Index()
        {
            return View(db.UOMTypes.ToList());
        }

        // GET: UOMTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UOMType uOMType = db.UOMTypes.Find(id);
            if (uOMType == null)
            {
                return HttpNotFound();
            }
            return View(uOMType);
        }

        // GET: UOMTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UOMTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UOMTypeID,UOMTypeDescription,IsDecimal,IsActive")] UOMType uOMType)
        {
            if (ModelState.IsValid)
            {
                db.UOMTypes.Add(uOMType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uOMType);
        }

        // GET: UOMTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UOMType uOMType = db.UOMTypes.Find(id);
            if (uOMType == null)
            {
                return HttpNotFound();
            }
            return View(uOMType);
        }

        // POST: UOMTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UOMTypeID,UOMTypeDescription,IsDecimal,IsActive")] UOMType uOMType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uOMType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uOMType);
        }

        // GET: UOMTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UOMType uOMType = db.UOMTypes.Find(id);
            if (uOMType == null)
            {
                return HttpNotFound();
            }
            return View(uOMType);
        }

        // POST: UOMTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UOMType uOMType = db.UOMTypes.Find(id);
            db.UOMTypes.Remove(uOMType);
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
