using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HomeFoodies
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductsSearch",
                url: "{controller}/{action}/{itemcategoryid}/{supplierid}/{supplierregion}/{itemname}",
                defaults: new { controller = "Products", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductsCategories",
                url: "{controller}/{action}/{supplierregion}",
                defaults: new { controller = "Products", action = "ShowCategories", id = UrlParameter.Optional }
            );
        }
    }
}
