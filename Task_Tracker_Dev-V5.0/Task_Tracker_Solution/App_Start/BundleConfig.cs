using System.Web;
using System.Web.Optimization;

namespace Task_Tracker_Solution
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));            

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/Workflow").Include(
                      "~/Areas/Master/Views/Workflow/WorkflowJS.js"));

            bundles.Add(new ScriptBundle("~/bundles/TaskCommonJS").Include(
                        "~/Areas/Tasks/Views/JS/TaskCommon.js"));
            bundles.Add(new ScriptBundle("~/bundles/TaskViewJS").Include(
                          "~/Areas/Tasks/Views/JS/TaskStartStop.js"
                        , "~/Areas/Tasks/Views/JS/TaskTreeView.js"
                        , "~/Content/bootstrap-treeview.js"
                        , "~/Content/jquery-base64-master/jquery.base64.js"
                        , "~/Content/jquery-base64-master/jquery.base64.min.js"));

            bundles.Add(new StyleBundle("~/bundles/TaskTreeCSS").Include(
                        "~/Content/bootstrap-treeview.css"));
            bundles.Add(new ScriptBundle("~/bundles/TaskListJS").Include(
                        "~/Areas/Tasks/Views/JS/TaskStartStop.js"
                        , "~/Content/jquery-base64-master/jquery.base64.min.js"));
        }
    }
}
