(function () {

    "use strict";

    angular.module("LeonardCRM").controller("ColumnMenuCtrl", ctrl);

    ctrl.$inject = ["$scope", "filterService", "viewService", "_"];

    function ctrl($scope, filterService, viewService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var changeColumnsCallback = function (data) {
            if (data > 0) {
                $scope.$emit('ColumnsChanged');
            }
        };

        var setValues = function () {
            angular.forEach($scope.CurrentDisplayColumns, function (item, index) {
                var col = _.findWithProperty($scope.MenuFields, 'ColumnName', item.ColumnName);
                if (col) {
                    col.Selected = true;
                    col.SortOrder = item.SortOrder;
                    col.Width = item.Width;
                }
            });
            var cols = _.filterWithProperty($scope.MenuFields, 'Selected', true);
            $scope.countColVisible = cols.length;
        };

        var initFields = function () {
            $scope.MenuFields = [];
            $scope.CurrentDisplayColumns = filterService.getCurrentGridColumns();
            var viewInfo = viewService.getViewInfo();
            $scope.MenuFields = angular.copy(filterService.getFields(viewInfo.ModuleId));
            setValues();
            $scope.MenuFieldsShadow = angular.copy($scope.MenuFields);
            $scope.IsChanged = false;
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.MenuFields = [];
        $scope.MenuFieldsShadow = [];
        $scope.CurrentDisplayColumns = [];
        $scope.countColVisible = 0;
        $scope.IsChanged = false;

        // --- Define Scope Method. ---------------------- //

        $scope.ApplyToGridView = function () {
            viewService.setChangeColumnsState(true);
            var cols = _.filterWithProperty($scope.MenuFields, 'Selected', true);
            var count = cols.length;
            if (cols.length == 0) {
                $scope.MenuFields = angular.copy($scope.MenuFieldsShadow);
                NotifyError($scope.languages.VIEW.MUST_SELECT_COLUMN);
            }
            for (var i = 0; i < $scope.MenuFields.length; i++) {
                if ($scope.MenuFields[i].Selected != $scope.MenuFieldsShadow[i].Selected) {
                    $scope.IsChanged = true;
                    break;
                }
            }
            if (count == 0 || $scope.IsChanged == false) {
                return;
            }
            var viewInfo = viewService.getViewInfo();
            viewService.changeColumns(cols, viewInfo.ViewId, viewInfo.ModuleId)
                .then(changeColumnsCallback);
        };

        $scope.Cancel = function () {
            $scope.CurrentDisplayColumns = filterService.getCurrentGridColumns();
            setValues();
        };
        $scope.Selected = function (item) {
            var cols = _.filterWithProperty($scope.MenuFields, 'Selected', true);
            cols = _.sortBy(cols, 'SortOrder');
            if (item.Selected == true) {
                var col = _.findWithProperty($scope.MenuFieldsShadow, 'Selected', true);
                if (col == null) {
                    item.SortOrder = cols[cols.length - 2].SortOrder + 2;
                }
            } 
        };
        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('setCurrentGridColumnsEvent', function () {
            $scope.CurrentDisplayColumns = filterService.getCurrentGridColumns();
            setValues();
        });
        
        // --- Initialize. ---------------------------------- //

        initFields();
    }

})();
