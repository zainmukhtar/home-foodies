using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomeFoodies.Models;
using HomeFoodies.Models.CustomModels;

namespace HomeFoodies.Controllers
{
    public class OrderDetailsController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        // GET: OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.Item).Include(o => o.Order);
            return View(orderDetails.ToList());
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item OrderItem = db.Items.Find(id);
            if (OrderItem == null)
            {
                return HttpNotFound();
            }
            OrderDetail orderDetail = new OrderDetail();
            Order _Order = new Order();
            if (Session["UserOrder"] != null)
            {
                _Order = (Order)Session["UserOrder"];
                orderDetail.Order = _Order;
            }
            orderDetail.Item = OrderItem;
            orderDetail.ItemID = OrderItem.ItemID;
            orderDetail.OrderPrice = OrderItem.ItemPrice;
            orderDetail.Order = _Order;
            ViewBag.UOMTypeID = new SelectList(db.UOMTypes, "UOMTypeID", "UOMTypeDescription", orderDetail.Item.UOMTypeID);
            return View(orderDetail);
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DetailID,OrderID,ItemID,OrderQty,OrderPrice")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                Order _Order = new Order();
                if (Session["UserOrder"] == null)
                {
                    _Order = new OrdersController().CreateNewOrder();
                    Session["UserOrder"] = _Order;
                }
                else
                {
                    _Order = (Order)Session["UserOrder"];
                }
                Item OrderItem = db.Items.Find(orderDetail.ItemID);
                orderDetail.Item = OrderItem;
                UOMType ItemUOM = db.UOMTypes.Find(OrderItem.UOMTypeID);
                orderDetail.Item.UOMType = ItemUOM;
                orderDetail.Total = orderDetail.OrderPrice * orderDetail.OrderQty;
                _Order.OrderDetails.Add(orderDetail);
                Session["UserOrder"] = _Order;

                if ((UserSearch)Session["UserSearch"] != null)
                    return RedirectToAction("Search", "Products", new { itemcategoryid = ((UserSearch)Session["UserSearch"]).ItemCategoryId, supplierid = ((UserSearch)Session["UserSearch"]).SupplierId, supplierregion = ((UserSearch)Session["UserSearch"]).SupplierRegion, itemname = ((UserSearch)Session["UserSearch"]).ItemName });
                else return RedirectToAction("Search", "Products");
            }

            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemName", orderDetail.ItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNo", orderDetail.OrderID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemName", orderDetail.ItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNo", orderDetail.OrderID);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DetailID,OrderID,ItemID,OrderQty,OrderPrice")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemName", orderDetail.ItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNo", orderDetail.OrderID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
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
