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
using HomeFoodies.Models.CustomModels;

namespace HomeFoodies.Controllers
{
    public class OrdersController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.StatusCode);
            if (User.Identity.Name == "Administrator")
                return View(orders.ToList().OrderByDescending(o => o.OrderID));
            else if (Session["LoggedInSupplier"] != null)
            {
                Helpers.SessionData _Session = (Helpers.SessionData)Session["LoggedInSupplier"];
                if (_Session != null)
                {
                    List<Order> lstOrders = new List<Order>();
                    orders.ToList().ForEach(e =>
                    {
                        e.OrderDetails.ToList().ForEach(ee =>
                        {
                            if (ee.Item.SupplierID == _Session.LoggedInUser.SupplierID)
                                lstOrders.Add(e);
                        });
                    });
                    return View(lstOrders.OrderByDescending(o => o.OrderID));
                }
            }
            else
                RedirectToAction("Index", "Home");

            return View();
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CutomerName");
            ViewBag.CurrentStatusID = new SelectList(db.StatusCodes, "StatusID", "StatusDescription");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderNo,OrderDate,CustomerID,CurrentStatusID,TotalOrderAmount,CommisionPercentage,IsCustomerConfirmed,IsSupplierConfirmed,VerificationCode")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CutomerName", order.CustomerID);
            ViewBag.CurrentStatusID = new SelectList(db.StatusCodes, "StatusID", "StatusDescription", order.CurrentStatusID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            Order _Order = db.Orders.Find(id);
            if (_Order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CutomerName", _Order.CustomerID);
            ViewBag.CurrentStatusID = new SelectList(db.StatusCodes, "StatusID", "StatusDescription", _Order.CurrentStatusID);
            return View(_Order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,OrderNo,OrderDate,CustomerID,CurrentStatusID,TotalOrderAmount,CommisionPercentage,IsCustomerConfirmed,IsSupplierConfirmed,VerificationCode")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CutomerName", order.CustomerID);
            ViewBag.CurrentStatusID = new SelectList(db.StatusCodes, "StatusID", "StatusDescription", order.CurrentStatusID);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmFromSupplier([Bind(Include = "OrderID,IsSupplierConfirmed")] Order order)
        {
            if (order.IsSupplierConfirmed)
                db.SP_Order_UpdateStatus((int)Helpers.StatusCodes.SupplierVerified, order.OrderID);

            return RedirectToAction("Index", "Orders");
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public Order CreateNewOrder()
        {
            Order result = new Order();

            result.OrderNo = db.SP_Order_GetMaxOrerNo().ToList().FirstOrDefault();
            result.CommisionPercentage = 15;
            result.CurrentStatusID = (int)Helpers.StatusCodes.InProgress;
            result.Customer = new Customer();
            result.CustomerID = 0;
            result.IsCustomerConfirmed = false;
            result.IsSupplierConfirmed = false;
            result.OrderDate = DateTime.Now;
            result.OrderID = 0;
            result.TotalOrderAmount = 0;

            return result;
        }

        public ActionResult PlaceOrder(Order order)
        {
            try
            {
                ViewBag.PlaceOrderReturn = "";
                if (Session["UserOrder"] != null)
                {
                    if (!String.IsNullOrEmpty(order.Customer.CustomerPhone))
                    {
                        Order _Order = (Order)Session["UserOrder"];

                        Customer _Customer = new CustomersController().GetByPhone(order.Customer.CustomerPhone);
                        if (_Customer != null)
                        {
                            _Order.CustomerID = _Customer.CustomerID;
                            _Order.Customer = _Customer;
                        }
                        else
                        {
                            order.Customer.CustomerPhone = Helpers.Helper.FormatMobileNumber(order.Customer.CustomerPhone);
                            _Order.Customer = order.Customer;
                        }
                        _Order.VerificationCode = new Random().Next(111111, 999999).ToString();
                        _Order.TotalOrderAmount = _Order.OrderDetails.ToList().Sum(s => s.OrderPrice * s.OrderQty);

                        Session["UserOrder"] = _Order;
                        Helpers.Helper.SendSMSByGCM(order.Customer.CutomerName, order.Customer.CustomerPhone, 5, "The Verification Code For Your Order " + _Order.OrderNo + " is " + _Order.VerificationCode);
                        Session["OrderStatus"] = "CodeSent";
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Search", "Products", new { itemcategoryid = ((UserSearch)Session["UserSearch"]).ItemCategoryId, supplierid = ((UserSearch)Session["UserSearch"]).SupplierId, supplierregion = ((UserSearch)Session["UserSearch"]).SupplierRegion, itemname = ((UserSearch)Session["UserSearch"]).ItemName });
        }

        public ActionResult SaveOrder(Order order)
        {
            try
            {
                ViewBag.PlaceOrderReturn = "";
                if (Session["UserOrder"] != null)
                {
                    if (!String.IsNullOrEmpty(order.VerificationCode))
                    {
                        Order _Order = (Order)Session["UserOrder"];

                        if (order.VerificationCode == _Order.VerificationCode)
                        {
                            Helpers.Helper.SendSMSByGCM(_Order.Customer.CutomerName, _Order.Customer.CustomerPhone, 3, _Order.OrderNo);

                            Dictionary<string, string> msgData = new Dictionary<string, string>();
                            _Order.OrderDetails.ToList().ForEach(e =>
                            {
                                if (!msgData.ContainsKey(e.Item.SupplierID.ToString()))
                                    msgData.Add(e.Item.SupplierID.ToString(), e.Item.ItemName);
                                else
                                {
                                    string dataValue = "";
                                    if (msgData.TryGetValue(e.Item.SupplierID.ToString(), out dataValue))
                                        msgData[e.Item.SupplierID.ToString()] = String.IsNullOrEmpty(dataValue) ? e.Item.ItemName : dataValue + ", " + e.Item.ItemName;
                                }
                            });

                            foreach (var item in msgData)
                            {
                                Supplier supplier = db.Suppliers.Find(Convert.ToInt32(item.Key));
                                if (supplier != null)
                                    Helpers.Helper.SendSMSByGCM(supplier.FullName, supplier.ContactNumber, 4, "You have a new Order " + _Order.OrderNo + " for " + item.Value + ". Login to activate!");
                            }

                            _Order.OrderDetails.ToList().ForEach(e => e.Item = null);
                            _Order.TotalOrderAmount = _Order.OrderDetails.ToList().Sum(s => s.OrderPrice * s.OrderQty);

                            db.Orders.Add(_Order);
                            db.SaveChanges();

                            db.SP_Order_UpdateStatus((int)Helpers.StatusCodes.CustomerVerified, _Order.OrderID);

                            _Order = new Order();
                            Session["UserOrder"] = null;
                            Session["OrderStatus"] = "OrderSuccessfully";
                        }
                        else
                        {
                            Session["OrderStatus"] = "WrongCodeEntered";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Search", "Products", new { itemcategoryid = ((UserSearch)Session["UserSearch"]).ItemCategoryId, supplierid = ((UserSearch)Session["UserSearch"]).SupplierId, supplierregion = ((UserSearch)Session["UserSearch"]).SupplierRegion, itemname = ((UserSearch)Session["UserSearch"]).ItemName });
        }

        public JsonResult ClearOrderSession(string key)
        {
            Session["UserOrder"] = null;
            Session[key] = null;

            return this.Json(new { success = true });
        }

        public ActionResult OrderCommission()
        {
            List<OrdersCommission> _Commission = new List<OrdersCommission>();
            Int32 SupplierID = 0;
            if (User.Identity.Name == "Administrator")
                SupplierID = 1;
            else if (Session["LoggedInSupplier"] != null)
            {
                Helpers.SessionData _Session = (Helpers.SessionData)Session["LoggedInSupplier"];
                if (_Session != null)
                    SupplierID = Convert.ToInt32(_Session.LoggedInUser.SupplierID);
            }
            else RedirectToAction("Index", "Home");

            _Commission = db.SP_Orders_Commission(SupplierID).ToList();
            _Commission.ForEach(e =>
            {
                double CommissionAmt = (e.CommisionPercentage / 100) * e.TotalOrderAmount;
                e.SellerCost = Convert.ToDecimal(e.TotalOrderAmount - CommissionAmt);
                e.CompanyCommission = Convert.ToDecimal(e.TotalOrderAmount) - Convert.ToDecimal(e.SellerCost);
            });

            return View("~/Views/Orders/Commission.cshtml", _Commission);
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
