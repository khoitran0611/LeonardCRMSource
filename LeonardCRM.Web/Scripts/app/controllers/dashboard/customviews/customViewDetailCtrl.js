(function () {
    'use strict';

    angular.module('LeonardCRM').controller("customViewDetailCtrl", ctrl);

    ctrl.$inject = ["$scope", "$rootScope", "$http", "$location", "$routeParams", "requestContext", "viewService", "viewCustomService", "entityFieldService", "appService", "toolbarService", "_", "$timeout"];

    function ctrl($scope, $rootScope, $http, $location, $routeParams, requestContext, viewService, viewCustomService, entityFieldService, appService, toolbarService, _, $timeout) {
        $scope.newView = {};
        $scope.currentView = {};
        $scope.viewColumns = [];
        $scope.fieldsSortable = [];
        $scope.fieldsGroup = [];
        $scope.viewlist = [];

        $scope.sortDirection = '';
        $scope.sortColumnId = 0;
        $scope.orderByColumns = {};
        $scope.groupingColumns = {};
        $scope.viewId = $scope.CurrentParam.Id;

        $scope.sortableOptions1 = {
            placeholder: 'field',
            connectWith: '.fields-container',
            stop: function (event, ui) {

            }
        };

        var saveViewSuccessCallBack = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                $rootScope.$broadcast("reloadCurrentCustomView", $scope.CurrentParam.Model);
                $scope.$emit('closeWindow');
            } else {
                NotifyError(data.Result, 5000);
            }
        };


        //------Define scope's methods
        function initViewPage() {
            $.validate({
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    var cols = _.where($scope.currentView.Columns, { 'Visible': true });
                    if (cols.length > 0) {
                        if (status) {
                            viewCustomService.saveView($scope.currentView)
                                .then(saveViewSuccessCallBack);
                        }
                    } else {
                        NotifyError($scope.languages.CUSTOM_VIEW.MUST_SELECT_COLUMN);
                    }
                    return false;
                },
                onError: function () {
                    return false;
                },
                onValidate: function () {
                    return "";
                }
            });

            viewCustomService.getViewObjById($scope.viewId)
                .then(function (data) {
                    $scope.currentView = angular.fromJson(data);
                    $scope.viewColumns = _.where($scope.currentView.Columns, { 'Searchable': true });;
                    $scope.orderByColumns = _.where($scope.currentView.Columns, { 'Sortable': true });
                    $scope.groupingColumns = _.where($scope.currentView.Columns, { 'AllowGroup': true });
                    if ($scope.viewId > 0 && $scope.currentView != null) {
                        var obj = {
                            Fields: $scope.viewColumns,
                            Conditions: $scope.currentView.Eli_ViewCustomConditions,
                            ViewId: $scope.viewId
                        };
                        $timeout(function () {
                            $scope.$broadcast('filterColumnsEvent', obj);
                        }, 1500);
                    }
                });

            $scope.setTitle($scope.languages.CUSTOM_VIEW.EDIT_VIEW);
        };

        $scope.$on('saveDataEvent', function (event) {
            if ($scope.currentView != null) {
                $scope.$broadcast('needConditionsEvent');
                event.preventDefault();
            }
        });

        $scope.$on('receivedConditionsEvent', function (event, args) {
            $scope.currentView.Eli_ViewCustomConditions = args;
            var form = $('#userForm');
            form.submit();
            event.preventDefault();
        });

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext();

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                // Make sure this change is relevant to this controller.
                if (angular.isDefined(renderContext)) {
                    if (!renderContext.isChangeRelevant()) {
                        return;
                    }
                }
            }
        );

        //---Initalize------
        initViewPage();

    }
}());