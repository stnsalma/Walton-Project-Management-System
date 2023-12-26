using System.Web;
using System.Web.Optimization;

namespace ProjectManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*", "~/Scripts/moment.min.js", "~/Scripts/moment-timezone.js",
                        "~/Scripts/jquery.dataTables.min.js", "~/Scripts/dataTables.bootstrap.min.js", "~/Scripts/dataTables.select.min.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/customcss").Include(
                      "~/Content/site.css",
                      "~/Content/font-awesome/css/font-awesome.min.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/timepickerdotco/jquery.timepicker.min.css"));
            #region CustomJavaScripts
            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                "~/Scripts/sindresorhus_multiline/browser.js",
                "~/Scripts/timepickerdotco/jquery.timepicker.min.js"

                ));
            #endregion


            #region LayoutMetronicTemplate

            var metronicStyles = new StyleBundle("~/bundles/metroniccss");
            metronicStyles.Include("~/assets/googleapiscss.css",
                "~/Content/font-awesome/css/font-awesome.min.css",
                "~/assets/global/plugins/simple-line-icons/simple-line-icons.min.css",
                "~/assets/global/plugins/bootstrap/css/bootstrap.min.css",
                "~/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                "~/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.css",
                "~/assets/global/plugins/morris/morris.css",
                "~/assets/global/plugins/fullcalendar/fullcalendar.min.css",
                "~/assets/global/plugins/jqvmap/jqvmap/jqvmap.css",
                "~/assets/global/css/components.min.css",
                "~/assets/global/css/plugins.min.css",
                "~/assets/layouts/layout4/css/layout.min.css",
                "~/assets/layouts/layout4/css/themes/light.min.css",
                "~/assets/layouts/layout4/css/custom.min.css",
                "~/assets/global/plugins/icheck/skins/all.css");
            bundles.Add(metronicStyles);

            var metronicScripts = new ScriptBundle("~/bundles/metronicjs");
            metronicScripts.Include(
                "~/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/assets/global/plugins/js.cookie.min.js",
                "~/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                "~/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/assets/global/plugins/jquery.blockui.min.js",
                "~/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                "~/assets/global/plugins/moment.min.js",
                "~/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.js",
                "~/assets/global/plugins/morris/morris.min.js",
                "~/assets/global/plugins/morris/raphael-min.js",
                "~/assets/global/plugins/counterup/jquery.waypoints.min.js",
                "~/assets/global/plugins/counterup/jquery.counterup.min.js",
                "~/assets/global/plugins/amcharts/amcharts/amcharts.js",
                "~/assets/global/plugins/amcharts/amcharts/serial.js",
                "~/assets/global/plugins/amcharts/amcharts/pie.js",
                "~/assets/global/plugins/amcharts/amcharts/radar.js",
                "~/assets/global/plugins/amcharts/amcharts/themes/light.js",
                "~/assets/global/plugins/amcharts/amcharts/themes/patterns.js",
                "~/assets/global/plugins/amcharts/amcharts/themes/chalk.js",
                "~/assets/global/plugins/amcharts/ammap/ammap.js",
                "~/assets/global/plugins/amcharts/ammap/maps/js/worldLow.js",
                "~/assets/global/plugins/amcharts/amstockcharts/amstock.js",
                "~/assets/global/plugins/fullcalendar/fullcalendar.min.js",
                "~/assets/global/plugins/horizontal-timeline/horozontal-timeline.min.js",
                "~/assets/global/plugins/flot/jquery.flot.min.js",
                "~/assets/global/plugins/flot/jquery.flot.resize.min.js",
                "~/assets/global/plugins/flot/jquery.flot.categories.min.js",
                "~/assets/global/plugins/jquery-easypiechart/jquery.easypiechart.min.js",
                "~/assets/global/plugins/jquery.sparkline.min.js",
                //  "~/assets/global/plugins/jqvmap/jqvmap/maps/jquery.vmap.russia.js",
                //   "~/assets/global/plugins/jqvmap/jqvmap/maps/jquery.vmap.world.js",
                //     "~/assets/global/plugins/jqvmap/jqvmap/maps/jquery.vmap.europe.js",
                //    "~/assets/global/plugins/jqvmap/jqvmap/maps/jquery.vmap.germany.js",
                //  "~/assets/global/plugins/jqvmap/jqvmap/maps/jquery.vmap.usa.js",
                //   "~/assets/global/plugins/jqvmap/jqvmap/data/jquery.vmap.sampledata.js",
                "~/assets/global/scripts/app.min.js",
                "~/assets/pages/scripts/dashboard.min.js",
                "~/assets/layouts/layout4/scripts/layout.min.js",
                "~/assets/layouts/layout4/scripts/demo.min.js",
                "~/assets/layouts/global/scripts/quick-sidebar.min.js",
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/dataTables.buttons.min.js",
                "~/Scripts/jszip.min.js",
                "~/Scripts/pdfmake.min.js",
                "~/Scripts/vfs_fonts.js",
                "~/Scripts/buttons.html5.min.js",
                "~/Scripts/buttons.print.min.js"
                );
            bundles.Add(metronicScripts);

            #endregion

            #region jQueryUnobustrusivVliadate
            bundles.Add(new ScriptBundle("~/bundles/jqueryunobstrusive").Include(
                //"~/Scripts/jquery.unobtrusive-ajax.min.js",
                //"~/Scripts/jquery.validate.min.js",
                "~/Scripts/required_if.js",
                 "~/Scripts/jquery.unobtrusive-ajax.min.js",
                 "~/Scripts/commonAjax/commonJqueryAjax.js"));

            #endregion

            #region dateTimePicker
            bundles.Add(new ScriptBundle("~/bundles/datepickerjs").Include(
          "~/Scripts/datetime/zebra_datepicker.js", "~/Scripts/datetime/core.js"));

            bundles.Add(new StyleBundle("~/bundles/datepickercss").Include(
                     "~/Content/datetime/default.css", "~/Content/datetime/metallic.css"));
            #endregion

            #region chosen
            bundles.Add(new StyleBundle("~/bundles/chosencss").Include(
         "~/Content/chosen/chosen.css",
         "~/Content/chosen/docsupport/prism.css",
         "~/Content/chosen/docsupport/style.css",
         "~/Content/alertify/alertify.css",
         "~/Content/alertify/default.css",
         "~/Content/apps/todo-2.min.css",
         "~/Content/apps/buttons.dataTables.min.css"
         ));

            bundles.Add(new ScriptBundle("~/bundles/chosenjs").Include(
                     "~/Scripts/chosen/chosen.jquery.js",
                     "~/Scripts/chosen/prism.js",
                     "~/Scripts/alertify/alertify.min.js"
                     ));
            #endregion

            #region footable

            bundles.Add(new StyleBundle("~/bundles/footablecss").Include("~/Content/footable.bootstrap.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/footablejs").Include("~/Scripts/footable.min.js"));

            #endregion

        }
    }
}
