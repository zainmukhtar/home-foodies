using HomeFoodies.Models;
using HomeFoodies.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HomeFoodies.Controllers
{
    public class ProductsController : Controller
    {
        private HomeFoodiesEntities db = new HomeFoodiesEntities();

        public ActionResult Index()
        {
            List<ProductsSearch> ProductsModel = SearchProducts(0, 0, "", "");
            return View("~/Views/Products/Index.cshtml", ProductsModel);
        }

        public ActionResult Search(int? itemcategoryid, int? supplierid, string supplierregion, string itemname)
        {
            if (itemcategoryid == null && supplierid == null && String.IsNullOrEmpty(supplierregion) && String.IsNullOrEmpty(itemname))
            {
                return Index();
            }

            Int32 pItemCategoryID = 0;
            Int32.TryParse(itemcategoryid == null ? "0" : itemcategoryid.ToString(), out pItemCategoryID);

            Int32 pSupplierID = 0;
            Int32.TryParse(supplierid == null ? "0" : supplierid.ToString(), out pSupplierID);

            String pSupplierRegion = String.IsNullOrEmpty(supplierregion) ? "" : supplierregion;
            ViewBag.SupplierRegion = pSupplierRegion;
            String pItemName = String.IsNullOrEmpty(itemname) ? "" : itemname;

            List<ProductsSearch> ProductsModel = SearchProducts(pItemCategoryID, pSupplierID, pSupplierRegion, pItemName);
            return View("~/Views/Products/Index.cshtml", ProductsModel);
        }

        public ActionResult ShowCategories(string supplierregion)
        {
            if (String.IsNullOrEmpty(supplierregion))
            {

            }

            String pSupplierRegion = String.IsNullOrEmpty(supplierregion) ? "" : supplierregion;

            ObjectResult<ProductsCategories> resultSP = db.SP_Products_GetItemCategories(pSupplierRegion);
            List<ProductsCategories> lstCategories = resultSP.ToList();

            return PartialView("~/Views/Client/Shared/_SidebarPartial.cshtml", lstCategories);
        }

        public ActionResult ShowCurrentOrder()
        {
            Order _Order = new Order();
            _Order.Customer = new Customer() { CutomerName = "", CustomerAddress = "", CustomerPhone = "", CustomerID = 0 };
            if (Session["UserOrder"] != null)
                _Order = (Order)Session["UserOrder"];

            return PartialView("~/Views/Client/Shared/_RightSidebarPartial.cshtml", _Order);
        }

        private List<ProductsSearch> SearchProducts(Int32 pItemCategoryID, Int32 pSupplierID, String pSupplierRegion, String pItemName)
        {
            List<ProductsSearch> result = new List<ProductsSearch>();
            try
            {
                UserSearch _UserSearch = new UserSearch();
                _UserSearch.SupplierRegion = pSupplierRegion;
                _UserSearch.ItemCategoryId = pItemCategoryID;
                _UserSearch.SupplierId = pSupplierID;
                _UserSearch.ItemName = pItemName;
                Session["UserSearch"] = _UserSearch;

                ObjectResult<ProductsSearch> resultSP = db.SP_Products_Search(pItemCategoryID, pSupplierID, pSupplierRegion, pItemName);
                result = resultSP.ToList();
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public ActionResult MainSearch()
        {
            return PartialView("~/Views/Client/Shared/_MainSearchPartial.cshtml");
        }

      
        public ActionResult LatestItems()
        {

            ObjectResult<LatestItems> resultSP = db.SP_Products_GetLatest();
            List<LatestItems> lstLatestItems = resultSP.ToList();

            return PartialView("~/Views/Client/Shared/_LatestItemsPartial.cshtml", lstLatestItems);
        }
    }
}