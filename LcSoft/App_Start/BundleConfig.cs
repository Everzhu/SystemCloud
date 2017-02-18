using System.Web;
using System.Web.Optimization;

namespace XkSystem
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                      "~/Scripts/bootstrap-datetimepicker.js",
                      "~/Scripts/bootstrap-datetimepicker.zh-CN.js",
                      "~/Scripts/bootstrap-select.js",
                      "~/Scripts/bootstrap3-typeahead.js",
                      "~/Scripts/jquery.ztree.all-3.5.js",
                      "~/Scripts/mergetable.js",
                      "~/Scripts/scriptfix.js")); 

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/bootstrap-select.css",
                      "~/Content/zTree.css",
                      "~/Content/zTree.theme.metro.css",
                      "~/Content/site.css"));

            // 将 EnableOptimizations 设为 false 以进行调试。有关详细信息，默认为false
            // 请访问 http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}
