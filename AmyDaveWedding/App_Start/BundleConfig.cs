using System.Web;
using System.Web.Optimization;

namespace AmyDaveWedding
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // JavaScript:

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/vendorScripts").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-route.js",
                      "~/Scripts/angular-animate.js"));

            bundles.Add(new ScriptBundle("~/bundles/siteScripts").Include(
                      "~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/songApp").Include(
                      "~/app/songApp.js",
                      "~/app/directives.js"));

            bundles.Add(new ScriptBundle("~/bundles/weddingJavaScript").Include(
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/jquery.smoothscroll.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/wedding-home.js"));

            // CSS:

            bundles.Add(new StyleBundle("~/Content/weddingCss").Include(
                      "~/Content/theme-fonts.css",
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/theme.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site-fonts.css",
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/toastr.css",
                      "~/Content/site.css"));
        }
    }
}
