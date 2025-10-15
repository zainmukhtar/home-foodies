using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomeFoodies.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace HomeFoodies.Controllers
{
    public class ItemsController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Category).Include(i => i.Supplier).Include(i => i.UOMType);
            if (User.Identity.Name == "Administrator")
                return View(items.ToList());
            else if (Session["LoggedInSupplier"] != null)
            {
                Helpers.SessionData _Session = (Helpers.SessionData)Session["LoggedInSupplier"];
                if (_Session != null)
                    return View(items.Where(w => w.SupplierID == _Session.LoggedInUser.SupplierID).ToList());
            }
            else
                RedirectToAction("Index", "Home");

            return View();
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            if (Session["LoggedInSupplier"] != null)
            {
                Helpers.SessionData _Session = (Helpers.SessionData)Session["LoggedInSupplier"];

                if (_Session != null)
                {
                    ViewBag.ItemCategoryID = new SelectList(db.Categories, "ItemCategoryID", "ItemCategoryDesc");
                    ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "FullName", _Session.LoggedInUser.SupplierID);
                    ViewBag.UOMTypeID = new SelectList(db.UOMTypes, "UOMTypeID", "UOMTypeDescription");
                }
                else
                    return View("~/Views/Client/Home/Index.cshtml");
            }
            else
                return View("~/Views/Client/Home/Index.cshtml");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,ItemCategoryID,ItemName,ItemDescription,IsActive,ItemPrice,UOMTypeID,SupplierID,MaxOrderLimit,ImagePath")] Item item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string tmpPath = "";
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var itemImage = Request.Files[i];
                        if (itemImage != null && itemImage.ContentLength > 0)
                        {
                            string pic = DateTime.Now.ToString("ddMMyyyyhhmmss") + "_" + System.IO.Path.GetFileName(itemImage.FileName);
                            tmpPath = System.IO.Path.Combine(Server.MapPath("~/img/tmp"), pic);
                            itemImage.SaveAs(tmpPath);
                            break;
                        }
                    }
                    item.ImagePath = "";
                    db.Items.Add(item);

                    db.SaveChanges();

                    if (item.ItemID > 0)
                    {
                        string newPath = System.IO.Path.Combine(Server.MapPath("~/img/products"), item.ItemID.ToString() + ".png");
                        System.IO.File.Copy(tmpPath, newPath);
                        item.ImagePath = "~/img/products/" + item.ItemID.ToString() + ".png";
                        db.SaveChanges();
                        System.IO.File.Delete(tmpPath);
                    }                    
                }
                catch (Exception ex)
                {

                }
                return RedirectToAction("Index");
            }

            ViewBag.ItemCategoryID = new SelectList(db.Categories, "ItemCategoryID", "ItemCategoryDesc", item.ItemCategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "FullName", item.SupplierID);
            ViewBag.UOMTypeID = new SelectList(db.UOMTypes, "UOMTypeID", "UOMTypeDescription", item.UOMTypeID);
            return View(item);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemCategoryID = new SelectList(db.Categories, "ItemCategoryID", "ItemCategoryDesc", item.ItemCategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "FullName", item.SupplierID);
            ViewBag.UOMTypeID = new SelectList(db.UOMTypes, "UOMTypeID", "UOMTypeDescription", item.UOMTypeID);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemID,ItemCategoryID,ItemName,ItemDescription,IsActive,ItemPrice,UOMTypeID,SupplierID,MaxOrderLimit,ImagePath")] Item item)
        {
            try
            {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var itemImage = Request.Files[i];
                    if (itemImage != null && itemImage.ContentLength > 0)
                    {
                        itemImage.SaveAs(Server.MapPath(item.ImagePath));
                        break;
                    }
                }

                if (String.IsNullOrEmpty(item.ImagePath))
                {
                    item.ImagePath = "~/img/products/default.png";
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemCategoryID = new SelectList(db.Categories, "ItemCategoryID", "ItemCategoryDesc", item.ItemCategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "FullName", item.SupplierID);
            ViewBag.UOMTypeID = new SelectList(db.UOMTypes, "UOMTypeID", "UOMTypeDescription", item.UOMTypeID);

            }
            catch (Exception ex)
            {

                throw;
            }
            return View(item);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/img/tmp"), pic);
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

            }
            // after successfully uploading redirect the user
            return View();
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
