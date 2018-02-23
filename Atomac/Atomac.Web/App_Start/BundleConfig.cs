using System.Web;
using System.Web.Optimization;

namespace Atomac.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));
            
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/globalstyle")
                .Include("~/Content/bootstrap.min.css", "~/Content/global-style.css"));
					  
			bundles.Add(new StyleBundle("~/Content/datatables")
                .Include("~/Content/juery.dataTables.min.css", "~/Content/dataTables.bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables")
                .Include("~/Scripts/jquery.dataTables.min.js", "~/Scripts/dataTables.bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/timer")
                .Include("~/Scripts/easytimer.js"));

            bundles.Add(new StyleBundle("~/Content/chessboard")
                .Include("~/Content/chessboard-0.3.0.css"));

            bundles.Add(new ScriptBundle("~/bundles/chessboard")
                .Include("~/Scripts/chess.js", "~/Scripts/chessboard-0.3.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/chessboardgameplay")
                .Include("~/Scripts/main-chess-board.js", "~/Scripts/side-chess-board.js"));

            bundles.Add(new ScriptBundle("~/Content/gameplace")
                .Include("~/Content/game-place.css"));

            bundles.Add(new ScriptBundle("~/bundles/shop")
                .Include("~/Scripts/shop.js"));
        }
    }
}
