using System.Web;
using System.Web.Optimization;

namespace HomeFoodies
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/jquery.slimscroll.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/other").Include(
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/bootstrap-hover-dropdown.min.js",
                        "~/Scripts/jquery.blockui.min.js",
                        "~/Scripts/jquery.cokie.min.js",
                        "~/Scripts/jquery.uniform.min.js",
                        "~/Scripts/bootstrap-switch.min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/morris.min.js",
                        "~/Scripts/raphael-min.js",
                        "~/Scripts/jquery.sparkline.min.js",
                        "~/Scripts/metronic.js",
                        "~/Scripts/layout.js",
                        "~/Scripts/demo.js",
                        "~/Scripts/index3.js",
                        "~/Scripts/tasks.js"
                        ));
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate.min.js",
                            "~/Scripts/jquery.validate.unobtrusive.min.js"
                            ));
      
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/plugins/bootstrap/bootstrap.js",
                      "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css")
                    //.IncludeDirectory("~/Content", "*.css", true));

            bundles.Add(new StyleBundle("~/Content/fontcss")
                    .Include("~/Content/font-awesome.min.css",
                    "~/Content/simple-line-icons.css"
                            ));

            bundles.Add(new StyleBundle("~/Content/css")
                    .IncludeDirectory("~/Content", "*.css", true));

            bundles.Add(new ScriptBundle("~/Scripts/js")
                    .IncludeDirectory("~/Scripts", "*.js", true));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
            bundles.IgnoreList.Clear();
        }
    }}
