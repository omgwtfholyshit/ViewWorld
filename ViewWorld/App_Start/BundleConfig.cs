using System.Web;
using System.Web.Optimization;
using ViewWorld.App_Start;
namespace ViewWorld
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //发布时打开
            bundles.UseCdn = false;
            BundleTable.EnableOptimizations = true;

            var jQuery = new ScriptBundle("~/bundles/jquery", "//cdn.bootcss.com/jquery/3.1.1/jquery.min.js").Include(
                        "~/Scripts/jquery-{version}.min.js");
            jQuery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jQuery);
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));


            #region 后端
            //var react = new ScriptBundle("~/bundles/react", "https://unpkg.com/react@15.3.1/dist/react.min.js").Include("~/Lib/React/react.js");
            //react.CdnFallbackExpression = "window.react";
            //bundles.Add(react);
            //var reactDom = new ScriptBundle("~/bundles/reactDom", "https://unpkg.com/react-dom@15.3.1/dist/react-dom.min.js").Include("~/Lib/React/react-dom.js");
            //reactDom.CdnFallbackExpression = "window.reactDom";
            //bundles.Add(reactDom);
            var semantic = new ScriptBundle("~/bundles/semantic", "https://cdn.jsdelivr.net/semantic-ui/2.2.4/semantic.min.js").Include("~/Scripts/semantic.min.js");
            semantic.CdnFallbackExpression = "window.semantic";
            bundles.Add(semantic);
            var semanticCss = new StyleBundle("~/Content/semantic", "https://cdn.jsdelivr.net/semantic-ui/2.2.4/semantic.min.css")
                .IncludeFallback("~/Content/semantic.css", "ui grid", "margin-top","-14px");
            bundles.Add(semanticCss);
            
            #endregion
        }
    }
}
