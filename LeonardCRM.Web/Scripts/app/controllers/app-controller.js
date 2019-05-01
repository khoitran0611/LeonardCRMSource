(function () {

    "use strict";

    angular.module("LeonardCRM").controller("AppController", ctrl);

    ctrl.$inject = ['$scope', '$route', '$routeParams', '$location', '$cookies', 'requestContext', 'appService', 'registryService', 'resourceService', 'filterService', 'toolbarService', 'cfpLoadingBar', '$timeout', 'roleService', 'appConfig'];

    function ctrl($scope, $route, $routeParams, $location, $cookies, requestContext, appService, registryService, resourceService, filterService, toolbarService, cfpLoadingBar, $timeout, roleService, appConfig) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        $scope.dashboard = {
            IsRequest: true,
            IsOffer: false,
            IsBoth: false
        };

        $scope.allowImageExt = ['jpg', 'png', 'gif', 'bmp'];
        $scope.UploadFolderUrl = '/UploadFiles/';
        $scope.UploadHandlerUrl = '/Handler/UploadHandler.ashx';
        $scope.previousUrl = '';
        $scope.isDebug = true;
        $scope.configmkey = '';
        // Set up the default window title.
        $scope.windowTitle = registryService.siteSettings.DEFAULT_TITLE;
        $scope.languages = resourceService.loadResources();
        $scope.maxPageSize = 999999;
        $scope.jqueryScrollbarOptions = {
            'type': 'simpble',
            'onScroll': function (y, x) {

            }
        };    
        $scope.datetime_format = registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT;
        $scope.currency = registryService.siteSettings.CURRENCY;
        $scope.currencyDisplay = registryService.siteSettings.CURRENCY;
        $scope.formatDecimalPlaces = registryService.siteSettings.FORMAT_DECIMAL_PLACES;
        $scope.date_format = $scope.dateFormat = registryService.siteSettings.DATE_FORMAT;
        $scope.monthFormat = registryService.siteSettings.MONTH_FORMAT;
        $scope.noUserEmail = registryService.siteSettings.NO_USER_EMAIL;
        $scope.monthOptions = { minMode: 'month', datepickerMode: 'month' };
        $scope.hashParam = "";

        //load localization resources
        $scope.$on('loadingReourcesCompleted', function (event) {
            $scope.languages = resourceService.loadResources();
            event.preventDefault();
        });

        $scope.$on('refreshEvent', function (event) {
            $scope.refreshViewMenu();
            event.preventDefault();
        });

        // --- Define Controller Methods. ------------------- //
        function reloadData() {
            $scope.toolbarUrl = "";
            $scope.ModuleId = appService.getModuleId($scope.subview);

            //Do not expand left menu when current module doesn't use Dynamic grid
            $scope.moduleObj = appService.getModuleById($scope.ModuleId);
            if ($scope.moduleObj && $scope.moduleObj.UseCustomView)
                $scope.isExpanded = true;

            toolbarService.setCurrentModuleId($scope.ModuleId);
            if ($scope.subview == 'dashboard' && requestContext.getRenderContext('dashboard').getNextSection() == null)
                $scope.toolbarUrl = "";
            else {
                if ($scope.subview != 'dashboard' && $scope.ModuleId > 0) {
                    $scope.toolbarUrl = '/Home/_ToolBar/?p=' + $scope.ModuleId;
                    filterService.preLoadFilterColumnsAndPickLists($scope.ModuleId);

                } else {
                    var tempModuleId = appService.getModuleId(requestContext.getRenderContext('dashboard').getNextSection());
                    $scope.toolbarUrl = '/Home/_ToolBar/?p=' + tempModuleId;
                }
            }
        };

        //only load grid after picklists loaded to avoid grid filter function error
        $scope.$on('filterPickListEvent', function (event, moduleId) {
            if ($scope.ModuleId > 0 && moduleId == $scope.ModuleId && $scope.subview != 'dashboard') {
                $scope.gridUrl = '';
                $scope.gridUrl = '/home/dynamicgrid/' + $scope.ViewId + "/" + $scope.ModuleId + "/?t=" + Math.random();
            }
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
        });

        // I check to see if the given route is a valid route; or, is the route being
        // re-directed to the default route (due to failure to match pattern).
        function isRouteRedirect(route) {

            // If there is no action, then the route is redirection from an unknown 
            // route to a known route.
            return (!route.current.action);

        };

        // --- Define events for toolbar buttons ------------------------ //

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

        $scope.ReloadToolBar = function (url) {
            $scope.toolbarUrl = url;
        };
        $scope.AddEditValue = function (value) {
            $scope.EditValues = [];

            angular.forEach(value, function (val, key) {
                if (val.Selected) {
                    var obj = {
                        Id: val.Id
                    };
                    $scope.EditValues.push(obj);
                }
            });
        };

        $scope.SetCurrentView = function (viewId) {
            toolbarService.setCurrentViewId(viewId);
            $scope.ViewId = viewId;
            if ($scope.ModuleId != null)
                $scope.gridUrl = '/home/dynamicgrid/' + $scope.ViewId + "/" + $scope.ModuleId + "/?t=" + Math.random();
        };

        $scope.showimage = function (imageurl) {
            var url = '';
            if (imageurl != '' && imageurl != null) {
                url = $scope.UploadFolderUrl + imageurl;
            }
            return url;
        };

        $scope.refreshViewMenu = function () {
            $scope.$broadcast('reloadViewMenuEvent');
        };

        // I get the current time for use when display the time a controller was rendered.
        // This way, we can see the difference between when a controller was instantiated
        // and when it was re-populated with data.
        $scope.getInstanceTime = function () {

            var now = new Date();
            var timeString = now.toTimeString();
            var instanceTime = timeString.match(/\d+:\d+:\d+/i);

            return (instanceTime[0]);

        };

        // I update the title tag.
        $scope.setWindowTitle = function (title) {
            $scope.windowTitle = title;
        };

        $scope.init = function () {
            $scope.EditValues = [];
            $scope.ViewId = requestContext.getParamAsInt("viewId", 0);
            initScroll();

            appService.getFrontEndKey().then(function (data) {
            	$scope.hashParam = data;
            });
        };

        $scope.start = function () {
            cfpLoadingBar.start();
        };

        $scope.complete = function () {
            cfpLoadingBar.complete();
        }

        // fake the initial load so first time users can see it right away:
        $scope.start();
        $scope.fakeIntro = true;
        $timeout(function () {
            $scope.complete();
            $scope.fakeIntro = false;
        }, 750)

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext();

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();
        
        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

                if (requestContext.haveParamsChanged(["viewId"])) {
                    $scope.SetCurrentView(requestContext.getParamAsInt("viewId", 0));
                }

                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()
                    || ($scope.subview == renderContext.getNextSection() && $scope.subview != 'dashboard')) {
                    return;
                }
                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();
                $scope.isDashboard = $scope.subview == 'dashboard';
                if ($scope.subview != 'home' && $scope.subview != 'followUp') {
                    $scope.isExpanded = false;
                } else {
                    $scope.isExpanded = true;
                }
                $scope.gridUrl = '';

                reloadData();

                $scope.ViewId = requestContext.getParamAsInt("viewId", 0);
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

        $scope.$on('ColumnsChanged', function (event, args) {
            $scope.gridUrl = '';
            $scope.gridUrl = '/home/dynamicgrid/' + $scope.ViewId + "/" + $scope.ModuleId + "/?t=" + Math.random();
        });

        $scope.$watch(function () { return $location.path(); }, function (newLocation, oldLocation) {
            if (newLocation != oldLocation) {
                $scope.previousUrl = oldLocation;
            }
        });

        //process expand/collapse the left menu
        $scope.isExpanded = false;
        $scope.count = 0;
        $scope.$watch('isExpanded', function () {
            if ($scope.count > 0) {
                if ($scope.isExpanded) {
                    $('.col2 .boxmodulebody').fadeOut(100, function () {
                        $('.col2').animate({ width: 40 }, 100, function () {
                            $('.collapseexpandbutton > i').removeClass('glyphicon-chevron-left');
                            $('.collapseexpandbutton > i').addClass('glyphicon-chevron-right');
                            $('.col2').addClass('min-col2');
                        });
                        $('.col3').animate({ left: '-=210' }, 100, function () {
                        });
                        $('.col3 .boxmodulebody').animate({ width: '+=210' }, 100, function () {
                        });
                    });
                } else {
                    $('.col2').animate({ width: 250 }, 100, function () {
                        $('.col2 .boxmodulebody').fadeIn(100, function () {
                            $('.collapseexpandbutton > i').removeClass('glyphicon-chevron-right');
                            $('.collapseexpandbutton > i').addClass('glyphicon-chevron-left');
                            $('.col2').removeClass('min-col2');
                        });
                    });
                    $('.col3').animate({ left: '+=210' }, 100, function () {
                    });
                    $('.col3 .boxmodulebody').animate({ width: '-=210' }, 100, function () {
                    });
                }
            } else {
                $scope.count += 1;
            }
        });

        $scope.collapseExpandMenu = function () {
            $scope.isExpanded = !$scope.isExpanded;
        };

        $scope.collapseLeftMenu = function (isCollapsed) {
            $scope.isExpanded = isCollapsed;
        };

        $scope.ShowRequestChanged = function() {
            $scope.dashboard.IsOffer = false;
            $scope.dashboard.IsBoth = false;
            $scope.$broadcast("dashboardChanged", $scope.dashboard);
        };

        $scope.ShowOfferChanged = function () {
            $scope.dashboard.IsRequest = false;
            $scope.dashboard.IsBoth = false;
            $scope.$broadcast("dashboardChanged",$scope.dashboard);
        };

        $scope.ShowBothChanged = function () {
            $scope.dashboard.IsRequest = false;
            $scope.dashboard.IsOffer = false;
            $scope.$broadcast("dashboardChanged",$scope.dashboard);
        };

        $scope.$on('dashboardSetting', function (event, args) {
            $scope.dashboard = args;
        });

        $(window).resize(function () {
            setTimeout(function () {
                scrollResize();
            }, 500);
        });
        var toolbarHeight;
        var statusbarHeight;
        var relativeHeight, relativeWidth;
        var col3Width;
        var containerScrollHeight;

        function initScroll() {
            getSetting();
            $('.col2 .boxmodulebody').height(containerScrollHeight);
            if ($('.statusbar').length > 0) {
                containerScrollHeight -= $('.statusbar').outerHeight();
                $('.window-stage').css({ bottom: $('.statusbar').height() });
            }
            $('.col3 .boxmodulebody').height(containerScrollHeight - 5);
            $('.col3 .boxmodulebody').width(col3Width);
            setTimeout(function () {
                scrollResize();
            }, 500);
        }

        function getSetting() {
            toolbarHeight = $('.toolbar').height();
            statusbarHeight = $('.statusbar').height();

            relativeHeight = $('.relative').height();
            relativeWidth = $('.relative').width();

            col3Width = $('.col3').width();
            containerScrollHeight = relativeHeight - 45;
            $('.window-stage').css({ bottom: '0' });
        }

        function scrollResize() {
            getSetting();
            $('.col2 .boxmodulebody').animate({ height: containerScrollHeight }, 100, function () {

            });
            if ($('.statusbar').length > 0) {
                containerScrollHeight -= $('.statusbar').outerHeight();
                //window stage
            }
            $('.col3 .boxmodulebody').height(containerScrollHeight - 8);
            $('.col3 .boxmodulebody').width(col3Width);
        }

        // --- Initialize. ---------------------------------- //
        $scope.init();
    }

})();
