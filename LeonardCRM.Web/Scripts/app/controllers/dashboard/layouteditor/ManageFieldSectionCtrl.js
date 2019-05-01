(function () {

    'use strict';

    angular.module("LeonardCRM").controller("ManageFieldSectionCtrl", ctrl);

    ctrl.$inject = ["$scope", "_"];

    function ctrl($scope, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        $scope.sectionObjs = [];
        $scope.FieldDetailUrl = '';
        $scope.BooleanObj = [
            { 'Id': true, 'Text': 'Show' },
            { 'Id': false, 'Text': 'Hidden' }
        ];
        $scope.CustomFields = [];


        // --- Define Scope Method. ---------------------- //

        $scope.InitSection = function (items) {
            $scope.sectionObjs = items;
            $scope.CustomFields = angular.copy($scope.FieldSource);
            $scope.FieldDetailUrl = '/appviews/dashboard/layouteditor/section.html';
        };

        $scope.SectionMoveUp = function (index) {
            var item = angular.copy($scope.sectionObjs[index]);
            $scope.sectionObjs[index] = angular.copy($scope.sectionObjs[index - 1]);
            $scope.sectionObjs[index].SortOrder = item.SortOrder;
            item.SortOrder = $scope.sectionObjs[index - 1].SortOrder;
            $scope.sectionObjs[index - 1] = item;
        };
        $scope.SectionMoveDown = function (index) {
            var item = angular.copy($scope.sectionObjs[index]);
            $scope.sectionObjs[index] = angular.copy($scope.sectionObjs[index + 1]);
            $scope.sectionObjs[index].SortOrder = item.SortOrder;
            item.SortOrder = $scope.sectionObjs[index + 1].SortOrder;
            $scope.sectionObjs[index + 1] = item;
        };
        $scope.SectionDelete = function (index) {
            var section = $scope.sectionObjs[index];
            var fieldIds = _.pluck(section.Eli_FieldsSectionDetail, 'FieldId');
            var fields = _.filterWithValues($scope.FieldSource, 'Id', fieldIds);
            $scope.CustomFields = $scope.CustomFields.concat(fields);
            $scope.sectionObjs.splice(index, 1);
            angular.forEach($scope.sectionObjs, function (item, i) {
                item.SortOrder = i + 1;
            });
        };
        $scope.UpdateSection = function (items, index) {
            $scope.sectionObjs[index].Eli_FieldsSectionDetail = items;
        };
        $scope.UpdateCustomField = function (fieldIds) {
            if (fieldIds.length > 0) {
                $scope.CustomFields = _.filterWithNotInValues($scope.CustomFields, 'Id', fieldIds);
            }
            if (fieldIds.length == 1) {
                $scope.$broadcast('UpdateCustomFieldsEvent', fieldIds[0]);
            }
            if (fieldIds.length == 0) {
                $scope.$broadcast('ResetCustomFieldsEvent');
            }
        };
        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ------------------------ //

    }

})();