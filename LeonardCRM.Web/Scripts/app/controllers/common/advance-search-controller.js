(function () {

    "use strict";

    angular.module("LeonardCRM").controller("advanceSearchCtrl", ctrl);

    ctrl.$inject = ["$scope", "filterService", "$compile", "requestContext", "$timeout", "viewService", "toolbarService"];

    function ctrl($scope, filterService, $compile, requestContext, $timeout, viewService, toolbarService) {
        var searchText = $scope.languages.SEARCH_FORM.SEARCH_BUTTON;
        var resetSearchText = $scope.languages.SEARCH_FORM.RESET_SEARCH_BUTTON;
        //var closeText = $scope.languages.SEARCH_FORM.CLOSE_BUTTON;
        $scope.count = 0;
        $scope.FireGridButtonEvent = false; // [true: advancesearh, false: grid button] 
        var newScope;

        function resetAdvancedSearch() {
            filterService.setAndConditions([], toolbarService.getCurrentViewId());
            filterService.setOrConditions([], toolbarService.getCurrentViewId());
            $scope.$broadcast('initCondition'); //reset advanced search conditions popup
            filterService.CollectConditions(); //collect conditions from grid columns filter
            filterService.doFilter(toolbarService.getCurrentViewId());  //reload grid data without advanced search
        }

        $scope.showSearchDialog = function () {
            $("#dialog-edit").dialog({
                title: $scope.languages.SEARCH_FORM.TITLE,
                autoOpen: false,
                resizable: false,
                height: 'auto',
                show: { effect: 'drop', direction: "up" },
                modal: true,
                draggable: true,
                closeOnEscape: false,
                width: 750,
                close: function (event, ui) {
                    //event handler for Esc pressed
                },
                open: function (event, ui) {
                },
                buttons:
                    [
                        {
                            text: searchText,
                            click: function () {
                                $scope.doAdvanceSearch();
                                toolbarService.ResetQuickSearch(false);
                            }
                        },
                        {
                            text: resetSearchText,
                            click: function () {
                                resetAdvancedSearch();
                                toolbarService.ResetQuickSearch(false);
                            }
                        }
                    ],
                position: ['top', 5],
                dialogClass: 'dialog-close-icon'
            });

            $('.ui-dialog-buttonset button').addClass(function (index) {
                return "btn btn-sm label-primary";
            });

            if ($scope.subview == 'dashboard') {
                $('#saveFilterDialog').hide();
            } else {
                $('#saveFilterDialog').show();
            }

            if (newScope == null) {
                newScope = $scope.$new();
            }

            $("#dialog-edit").dialog('open');
            if ($scope.count == 0) {
                $scope.count += 1;
                var divTemple = '<div></div>';
                var divEl = angular.element(divTemple);
                var divContent = $('#searchContent');
                divEl.load('/appviews/dashboard/views/view-conditions.html', function () {
                    var divContainer = angular.element(divContent);
                    var html = $compile(divEl)(newScope);
                    $timeout(function () {
                        divContainer.html(html);
                        divContent.fadeIn();
                    });
                }, 500);
            }
            return false;
        };
        $scope.doAdvanceSearch = function () {
            filterService.doFilter(toolbarService.getCurrentViewId());
        };

        $scope.$on('receivedConditionsEvent', function (event, filterConditions) {
            // Default data thread
            filterService.doFilter(toolbarService.getCurrentViewId());
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
        });

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext();

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();

        //$scope.$on('AdvanceSearchPaging', function (event, args) {
        //    // handle event only if it was not defaultPrevented
        //    if (event.defaultPrevented) {
        //        return;
        //    }
        //    // mark event as "not handle in children scopes"
        //    event.preventDefault();

        //    $scope.doAdvanceSearch();
        //});

        $scope.$on('getViewCondion', function (event, args) {
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();

            // Is Fire Grid Button Event
            $scope.FireGridButtonEvent = true;
            $scope.$broadcast('needConditionsEvent');
        });
        $scope.$on('refreshEvent', function (event) {
            $scope.ClearConditions();
        });
        $scope.$on('resetQuickSearchEvent', function (event, isResetAdvance) {
            if (isResetAdvance == undefined || isResetAdvance == true) {
                resetAdvancedSearch();
            }
        });

        $scope.ClearConditions = function () {
            filterService.resetAllConditions();
            $scope.count = 0;
            if (newScope != null) {
                newScope.$destroy();
                newScope = null;
            }
        };

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                $scope.ClearConditions();

                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()
                    || ($scope.subview == renderContext.getNextSection() && $scope.subview != 'dashboard')) {
                    return;
                }
                filterService.setAndConditions([], toolbarService.getCurrentViewId());
                filterService.setOrConditions([], toolbarService.getCurrentViewId());
                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();
            }
        );


    }

})();
