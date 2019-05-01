using System;
using System.Web.Optimization;
using Eli.Common;

namespace LeonardCRM.Web
{
    public class BundleConfig
    {

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
#if DEBUG
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);
#endif
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        Constant.ANGULAR_APP_ROOT + "vendor/jquery/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        Constant.ANGULAR_APP_ROOT + "vendor/jquery/jquery-ui-{version}.min.js"));

            //admin script
            var pluginBundle = new ScriptBundle("~/ScriptPlugin")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/jquery/jquery.mousewheel.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/lodash/lodash.min.js")//Javascript Extention
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/angular-dragdrop.min.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/sortable.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/select2.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/bootstrap-timepicker.min.js")
              .Include(Constant.ANGULAR_APP_ROOT + "tinymceui/tinymce.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/bootstrap/bootstrap-colorpicker-module.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/jquery.scrollbar.min.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/ui-utils-ieshiv.min.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/ui-utils.min.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/timePicker/ngTimepicker.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/bootstrap/bootstrap.js")              
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/jquery/alertify.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/bootstrap/ui-bootstrap-tpls-1.1.2.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/input-mask/jquery.inputmask.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/input-mask/jquery.inputmask.date.extensions.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/input-mask/jquery.inputmask.phone.extensions.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/input-mask/jquery.inputmask.extensions.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/loading-bar/loading-bar.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/jquery.signaturepad.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/timePicker/jquery.timepicker.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/uiSelect/select.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/webcam.js")
              .Include(Constant.ANGULAR_APP_ROOT + "vendor/others/angular-promise-buttons.min.js");

            bundles.Add(pluginBundle);

            var angularBundle = new ScriptBundle("~/script")
                .Include(Constant.ANGULAR_APP_ROOT + "app/main.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/appConfig.js")
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "app/beans", "*.js", searchSubdirectories: true)
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "app/services", "*.js", searchSubdirectories: true)
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "app/filters", "*.js", searchSubdirectories: true)
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "app/directives", "*.js", searchSubdirectories: true)
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "app/controllers", "*.js", searchSubdirectories: true);
            bundles.Add(angularBundle);

            //bundle css for admin
            var contentBundle = new StyleBundle("~/Content/style")
                .Include(Constant.ContentStyleRoot + "themes/smoothness/jquery-ui.css")
                .Include(Constant.ContentStyleRoot + "vendor/bootstrap/colorpicker.css")
                .Include(Constant.ContentStyleRoot + "vendor/uploadfile/jquery.fileupload.css")
                .Include(Constant.ContentStyleRoot + "vendor/uploadfile/jquery.fileupload-ui.css")
                .Include(Constant.ContentStyleRoot + "vendor/alertify/alertify.core.css")
                .Include(Constant.ContentStyleRoot + "vendor/alertify/alertify.default.css")
                .Include(Constant.ContentStyleRoot + "vendor/scrollbar/jquery.scrollbar.css")
                .Include(Constant.ContentStyleRoot + "dragtable.css")
                .Include(Constant.ContentStyleRoot + "Site.css")
                .Include(Constant.ContentStyleRoot + "ng-table.css")
                .Include(Constant.ContentStyleRoot + "ngTimepicker.css")
                .Include(Constant.ContentStyleRoot + "vendor/loading-bar/loading-bar.css")
                .Include(Constant.ContentStyleRoot + "vendor/signaturepad/jquery.signaturepad.css")
                .Include(Constant.ContentStyleRoot + "jquery.timepicker.css")                
                .Include(Constant.ContentStyleRoot + "vendor/uiSelect/select.css");
            bundles.Add(contentBundle);

            //bundle css for frontend
            bundles.Add(new StyleBundle("~/css")
                .Include(Constant.ContentStyleRoot + "frontend/blue.css")
                .Include(Constant.ContentStyleRoot + "vendor/bootstrap/bootstrap.css")
                .Include(Constant.ContentStyleRoot + "vendor/other/bootstrap-timepicker.css")
                .Include(Constant.ContentStyleRoot + "vendor/uploadfile/jquery.fileupload.css")
                .Include(Constant.ContentStyleRoot + "vendor/uploadfile/jquery.fileupload-ui.css")
                .Include(Constant.ContentStyleRoot + "vendor/alertify/alertify.core.css")
                .Include(Constant.ContentStyleRoot + "vendor/alertify/alertify.default.css")
                .Include(Constant.ContentStyleRoot + "frontend/skin-black.css")
                .Include(Constant.ContentStyleRoot + "validationEngine.jquery.css")
                .Include(Constant.ContentStyleRoot + "font-awesome.css")
                .Include(Constant.ContentStyleRoot + "frontend/style.css")
                .Include(Constant.ContentStyleRoot + "frontend/custom.css")
                .Include(Constant.ContentStyleRoot + "vendor/signaturepad/jquery.signaturepad.css")
                .Include(Constant.ContentStyleRoot + "vendor/loading-bar/loading-bar.css"));

            bundles.Add(new StyleBundle("~/logincss")
                .Include(Constant.ContentStyleRoot + "frontend/blue.css")
                .Include(Constant.ContentStyleRoot + "vendor/bootstrap/bootstrap.css")
                .Include(Constant.ContentStyleRoot + "frontend/skin-black.css")
                .Include(Constant.ContentStyleRoot + "validationEngine.jquery.css")
                .Include(Constant.ContentStyleRoot + "font-awesome.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                Constant.ANGULAR_APP_ROOT + "vendor/bootstrap/bootstrap.js"));

            var frontendScript = new ScriptBundle("~/fe")
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "frontend", "*.js", true)
                .Include(Constant.ANGULAR_APP_ROOT + "app/appConfig.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/service-helper.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/SalesCustomerService.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/role-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/view-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/SalesOrderService.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/SalesOrderCompleteService.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/SalesOrderDeliveryService.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/ng-table-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "frontend/app/services/resource-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "frontend/app/services/app-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "frontend/app/services/registry-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/EntityFieldsService.js")
                 .Include(Constant.ANGULAR_APP_ROOT + "app/services/user-service.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/lodash.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/request-context.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/beans/render-context.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/controllers/profile/profileCtrl.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/services/orderAttachmentService.js")
                .Include(Constant.ANGULAR_APP_ROOT + "app/controllers/common/dialog-controller.js")
;

            var frontendScriptLogin = new ScriptBundle("~/loginjs")
                .Include(Constant.ANGULAR_APP_ROOT + "vendor/form-validator/jquery.form-validator.js")
                .Include(Constant.ANGULAR_APP_ROOT + "frontend/app.js")
                .IncludeDirectory(Constant.ANGULAR_APP_ROOT + "frontend/plugins", "*.js", true);
               
            bundles.Add(frontendScriptLogin);
            bundles.Add(frontendScript);
            //BundleTable.EnableOptimizations = true;
        }

        private static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }       
    }
}