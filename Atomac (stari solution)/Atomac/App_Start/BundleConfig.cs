﻿using System.Web;
using System.Web.Optimization;

namespace Atomac
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

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

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                        "~/Content/juery.dataTables.min.css",
                        "~/Content/dataTables.bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/chessboard").Include(
                        "~/Content/chessboard-0.3.0.css"));

            bundles.Add(new ScriptBundle("~/bundles/chessboard").Include(
                        "~/Scripts/chess.js",
                        "~/Scripts/chessboard-0.3.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/chessboardGamePlay").Include(
                "~/Scripts/main-chess-board.js",
                "~/Scripts/side-chess-board.js"));

            bundles.Add(new ScriptBundle("~/Content/gamePlace").Include(
                "~/Content/GamePlace.css"));


            bundles.Add(new ScriptBundle("~/bundles/shop").Include(
            "~/Scripts/Shop.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/timer").Include(
                "~/Scripts/easytimer.js"));
            
        }
    }
}
