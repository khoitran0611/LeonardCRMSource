(function () {

    "use strict";

    angular.module("LeonardCRMFrontEnd").controller("AppController", ctrl);

    ctrl.$inject = ['$scope', 'resourceService', 'feFilterService', 'requestContext', '$route', '$routeParams', '$sce', 'registryService', 'appConfig', 'cfpLoadingBar', '$timeout', 'viewService', 'appService'];

    function ctrl($scope, resourceService, feFilterService, requestContext, $route, $routeParams, $sce, registryService, appConfig, cfpLoadingBar, $timeout, viewService, appService) {

        $scope.datetime_format = registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT;
        $scope.dateFormat = registryService.siteSettings.DATE_FORMAT;
        $scope.monthFormat = registryService.siteSettings.MONTH_FORMAT;
        $scope.currencyDisplay = registryService.siteSettings.CURRENCY;
        $scope.formatDecimalPlaces = registryService.siteSettings.FORMAT_DECIMAL_PLACES;
        $scope.noUserEmail = registryService.siteSettings.NO_USER_EMAIL;
        $scope.allowImageExt = ['jpg', 'png', 'gif', 'bmp'];
        $scope.maxUploadFileSize = registryService.siteSettings.MAX_UPLOAD_FILESIZE;
        $scope.UploadFolderUrl = '/UploadFiles/';
        $scope.UploadHandlerUrl = '/Handler/UploadHandler.ashx';
        $scope.languages = resourceService.loadResources();
        feFilterService.getPickListByModules([2, 3, 29, appConfig.usersModule, appConfig.salesCompleteModule, appConfig.saleCusRefModule], false);
        $scope.monthOptions = { minMode: 'month', datepickerMode: 'month' };
        $scope.modeParam = getModeParam();
        $scope.showBackAdminBtn = $scope.modeParam != null && $scope.modeParam != '';
        $scope.defaultOrdersView = appService.getDefaultView("order");
		
        // I check to see if the given route is a valid route; or, is the route being
        // re-directed to the default route (due to failure to match pattern).
        function isRouteRedirect(route) {

            // If there is no action, then the route is redirection from an unknown 
            // route to a known route.
            return (!route.current.action);

        };

        function getModeParam() {
        	var param = window.location.search;
        	return angular.isDefined(param) ? param.replace("?mode=", "") : "";
        }


        // I update the title tag.
        $scope.setWindowTitle = function (title) {
            $scope.windowTitle = title;
        };

        //set the page header which is displayed on top of page
        $scope.setPageHeader = function (header, headerDesc) {
            $scope.pageHeader = $sce.trustAsHtml(header);
            if (headerDesc != null)
                $scope.pageHeaderDescription = headerDesc;
        };

        $scope.yesEvent = function () {
            $scope.$broadcast('yesEvent', $scope.configmkey);
            $scope.configmkey = '';
        };
        $scope.noEvent = function () {
            $scope.$broadcast('noEvent', $scope.configmkey);
            $scope.conirmMsg = '';
            $scope.configmkey = '';
        };
        $scope.SetConfirmMsg = function (msg, key, header) {
            $scope.configmkey = key;
            $scope.conirmMsg = msg;
            $scope.confimHeader = $scope.languages.COMMON.DIALOG_CONFIRM_TITLE;
            if (angular.isDefined(header) && header.length > 0) {
                $scope.confimHeader = header;
            }
            $('#dialogConfirm').appendTo("body").modal();
        };

        $scope.start = function () {
            cfpLoadingBar.start();
        };

        $scope.complete = function () {
            cfpLoadingBar.complete();
        }

        $scope.getFileName = function (filePath) {
            if (filePath != null && filePath.length > 0 && filePath.indexOf('/') > -1) {
                var lastQuoute = filePath.lastIndexOf('/');
                filePath = filePath.substr(lastQuoute + 1);
            }
            return filePath;
        }

        // fake the initial load so first time users can see it right away:
        $scope.start();
        $scope.fakeIntro = true;
        $timeout(function () {
            $scope.complete();
            $scope.fakeIntro = false;
        }, 750);
        viewService.getDefaultViewByRoleAndModule(appConfig.orderModule).then(function (data) {
            $scope.defaultOrdersView = (data != 0 ? data : $scope.defaultOrdersView);
        });

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext();

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant() || ($scope.subview == renderContext.getNextSection())) {
                    return;
                }
                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();
                $scope.pageHeader = '';
            }
        );

        // Listen for route changes so that we can trigger request-context change events.
        $scope.$on(
            "$routeChangeSuccess",
            function (event) {
                // If this is a redirect directive, then there's no taction to be taken.
                if (isRouteRedirect($route)) {

                    return;

                }

                // Update the current request action change.
                requestContext.setContext($route.current.action, $routeParams);
                
                // Announce the change in render conditions.
                $scope.$broadcast("requestContextChanged", requestContext);
            }
        );
    }

})();
