(function () {

    'use strict';

    angular.module("LeonardCRM").controller("DetailFieldSectionCtrl", ctrl);

    ctrl.$inject = ["$scope", "_"];

    function ctrl($scope, _) {

        // --- Define Controller Variables. ---------------------- //

        var fields = [];

        // --- Define Controller Method. ---------------------- //

        var resetFieldModel = function (fieldId) {
            
            if ($scope.CustomFields.length > 0) {
                var firstField = $scope.CustomFields[0];
                if ($scope.CustomFields.length == 1) {
                    $scope.sectionLeftFieldObj.FieldId = firstField.Id;
                    $scope.sectionLeftFieldObj.LabelDisplay = firstField.LabelDisplay;
                    $scope.sectionRightFieldObj.FieldId = firstField.Id;
                    $scope.sectionRightFieldObj.LabelDisplay = firstField.LabelDisplay;
                } else {
                    if (fieldId == $scope.sectionLeftFieldObj.FieldId) {
                        $scope.sectionLeftFieldObj.FieldId = firstField.Id;
                        $scope.sectionLeftFieldObj.LabelDisplay = firstField.LabelDisplay;
                    }
                    if (fieldId == $scope.sectionRightFieldObj.FieldId) {
                        $scope.sectionRightFieldObj.FieldId = firstField.Id;
                        $scope.sectionRightFieldObj.LabelDisplay = firstField.LabelDisplay;
                    }
                }
            }
        };

        var removeField = function (fieldId) {
            $scope.UpdateCustomField([fieldId]);
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.ListFieldDetail = [];
        $scope.sectionLeftFieldObj = {
            'Id': 0,
            'SectionId': 0,
            'FieldId': 0,
            'SortOrder': 0,
            'LeftSide': true
        };
        $scope.sectionRightFieldObj = {
            'Id': 0,
            'SectionId': 0,
            'FieldId': 0,
            'SortOrder': 0,
            'LeftSide': false
        };
        $scope.sectionLeftFieldObjs = [];
        $scope.sectionRightFieldObjs = [];
        $scope.FieldObjs = [];
        $scope.CurrentIndex = 0;

        // --- Define Scope Method. ---------------------- //

        $scope.InitFieldDetail = function (item,index) {
            item.Eli_FieldsSectionDetail = _.sortBy(item.Eli_FieldsSectionDetail, 'SortOrder');

            fields = angular.copy($scope.FieldSource);

            angular.forEach(item.Eli_FieldsSectionDetail, function (col) {
                var field = _.findWithProperty(fields, 'Id', col.FieldId);
                col.LabelDisplay = field.LabelDisplay;
            });

            $scope.ListFieldDetail = item.Eli_FieldsSectionDetail;

            $scope.CurrentIndex = index;

            $scope.sectionLeftFieldObjs = _.filterWithProperty(item.Eli_FieldsSectionDetail, 'LeftSide', true);
            $scope.sectionRightFieldObjs = _.filterWithProperty(item.Eli_FieldsSectionDetail, 'LeftSide', false);

            var fieldIds = _.pluck(item.Eli_FieldsSectionDetail, 'FieldId');

            $scope.UpdateCustomField(fieldIds);

            if ($scope.CustomFields.length > 0) {
                var firstField = $scope.CustomFields[0];
                $scope.sectionLeftFieldObj = {
                    'Id': 0,
                    'SectionId': $scope.sectionObj.Id,
                    'FieldId': firstField.Id,
                    'LabelDisplay': firstField.LabelDisplay,
                    'SortOrder': 0,
                    'LeftSide': true
                };
                $scope.sectionRightFieldObj = {
                    'Id': 0,
                    'SectionId': $scope.sectionObj.Id,
                    'FieldId': firstField.Id,
                    'LabelDisplay': firstField.LabelDisplay,
                    'SortOrder': 0,
                    'LeftSide': false
                };
            }
        };

        $scope.MergeFields = function () {
            angular.forEach($scope.sectionLeftFieldObjs, function(item,index) {
                item.SortOrder = index + 1;
            });
            angular.forEach($scope.sectionRightFieldObjs, function (item, index) {
                item.SortOrder = index + 1;
            });
            $scope.ListFieldDetail = $scope.sectionLeftFieldObjs.concat($scope.sectionRightFieldObjs);
            $scope.UpdateSection($scope.ListFieldDetail,$scope.CurrentIndex);
        };

        $scope.AddLeftField = function () {
            var item = angular.copy($scope.sectionLeftFieldObj);
            item.SortOrder = $scope.sectionLeftFieldObjs.length + 1;
            $scope.sectionLeftFieldObjs.push(item);
            removeField(item.FieldId);
            $scope.MergeFields();
        };
        $scope.AddRightField = function () {
            var item = angular.copy($scope.sectionRightFieldObj);
            item.SortOrder = $scope.sectionRightFieldObjs.length + 1;
            $scope.sectionRightFieldObjs.push(item);
            removeField(item.FieldId);
            $scope.MergeFields();
        };
        $scope.LeftFieldChanged = function() {
            var item = _.findWithProperty(fields, 'Id', $scope.sectionLeftFieldObj.FieldId);
            if (item != undefined) {
                $scope.sectionLeftFieldObj.LabelDisplay = item.LabelDisplay;
            } else {
                $scope.sectionLeftFieldObj.LabelDisplay = '';
            }
        };
        $scope.RightFieldChanged = function () {
            var item = _.findWithProperty(fields, 'Id', $scope.sectionRightFieldObj.FieldId);
            if (item != undefined) {
                $scope.sectionRightFieldObj.LabelDisplay = item.LabelDisplay;
            } else {
                $scope.sectionRightFieldObj.LabelDisplay = '';
            }
        };
        $scope.MoveRight = function(item) {
            item.LeftSide = false;
            item.SortOrder = $scope.sectionRightFieldObjs.length + 1;
            $scope.sectionRightFieldObjs.push(item);
            $scope.sectionLeftFieldObjs = _.reject($scope.sectionLeftFieldObjs, { 'FieldId': item.FieldId });
            $scope.MergeFields();
        };
        $scope.MoveLeft = function (item) {
            item.LeftSide = true;
            item.SortOrder = $scope.sectionLeftFieldObjs.length + 1;
            $scope.sectionLeftFieldObjs.push(item);
            $scope.sectionRightFieldObjs = _.reject($scope.sectionRightFieldObjs, { 'FieldId': item.FieldId });
            $scope.MergeFields();
        };
        $scope.AsideLefMoveUp = function (index) {
            var item = angular.copy($scope.sectionLeftFieldObjs[index]);
            $scope.sectionLeftFieldObjs[index] = angular.copy($scope.sectionLeftFieldObjs[index - 1]);
            $scope.sectionLeftFieldObjs[index].SortOrder = item.SortOrder;
            item.SortOrder = $scope.sectionLeftFieldObjs[index - 1].SortOrder;
            $scope.sectionLeftFieldObjs[index - 1] = item;
            $scope.MergeFields();
        };
        $scope.AsideLeftMoveDown = function (index) {
            var item = angular.copy($scope.sectionLeftFieldObjs[index]);
            $scope.sectionLeftFieldObjs[index] = angular.copy($scope.sectionLeftFieldObjs[index + 1]);
            $scope.sectionLeftFieldObjs[index].SortOrder = item.SortOrder;
            item.SortOrder = $scope.sectionLeftFieldObjs[index + 1].SortOrder;
            $scope.sectionLeftFieldObjs[index + 1] = item;
            $scope.MergeFields();
        };
        $scope.AsideRightMoveUp = function (index) {
            var item = angular.copy($scope.sectionRightFieldObjs[index]);
            $scope.sectionRightFieldObjs[index] = angular.copy($scope.sectionRightFieldObjs[index - 1]);
            $scope.sectionRightFieldObjs[index].SortOrder = item.SortOrder;
            item.SortOrder = $scope.sectionRightFieldObjs[index - 1].SortOrder;
            $scope.sectionRightFieldObjs[index - 1] = item;
            $scope.MergeFields();
        };
        $scope.AsideRightMoveDown = function (index) {
            var item = angular.copy($scope.sectionRightFieldObjs[index]);
            $scope.sectionRightFieldObjs[index] = angular.copy($scope.sectionRightFieldObjs[index + 1]);
            $scope.sectionRightFieldObjs[index].SortOrder = item.SortOrder;
            item.SortOrder = $scope.sectionRightFieldObjs[index + 1].SortOrder;
            $scope.sectionRightFieldObjs[index + 1] = item;
            $scope.MergeFields();
        };
        $scope.RemoveField = function (item) {
            $scope.sectionLeftFieldObjs = _.reject($scope.sectionLeftFieldObjs, { 'FieldId': item.FieldId });
            $scope.sectionRightFieldObjs = _.reject($scope.sectionRightFieldObjs, { 'FieldId': item.FieldId });
            var field = _.findWithProperty(fields, 'Id', item.FieldId);
            $scope.CustomFields.push(field);
            $scope.UpdateCustomField([]);
            $scope.MergeFields();
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('UpdateCustomFieldsEvent', function (event, args) {
            resetFieldModel(args);
        });
        $scope.$on('ResetCustomFieldsEvent', function (event) {
            if ($scope.CustomFields.length == 1) {
                var firstField = $scope.CustomFields[0];
                $scope.sectionLeftFieldObj.FieldId = firstField.Id;
                $scope.sectionLeftFieldObj.LabelDisplay = firstField.LabelDisplay;
                $scope.sectionRightFieldObj.FieldId = firstField.Id;
                $scope.sectionRightFieldObj.LabelDisplay = firstField.LabelDisplay;
            }
        });
        // --- Initialize. ------------------------ //

    }

})();