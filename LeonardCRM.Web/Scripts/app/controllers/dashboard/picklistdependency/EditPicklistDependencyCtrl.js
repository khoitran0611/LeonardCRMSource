(function () {

    'use strict';

    angular.module("LeonardCRM").controller("EditPicklistDependencyCtrl", ctrl);

    ctrl.$inject = ['$scope', 'filterService', 'dialogService', 'picklistService', 'picklistDependencyService', 'entityFieldService', '_'];

    function ctrl($scope, filterService, dialogService, picklistService, picklistDependencyService, entityFieldService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var showDialog = function (isShown) {
            if (isShown) {
                $scope.countDialog += 1;
                if ($scope.countDialog <= 1) {
                    $('#dialogDetail').appendTo('body').modal();
                }
                $('#dialogDetail').modal('show');
            }
            else {
                $('#dialogDetail').modal('hide');
            }
        };

        var setFormDisable = function () {
            if ($scope.picklisObj.Id > 0) {
                $scope.Controls = {
                    'ModuleControl': { 'Disable': true, 'Visible': true },
                    'SourceControl': { 'Disable': true, 'Visible': true },
                    'TargetControl': { 'Disable': true, 'Visible': true },
                    'NextButtonControl': { 'Disable': true, 'Visible': false },
                    'CancelButtonControl': { 'Disable': true, 'Visible': false },
                    'SelectSourceValueButtonControl': { 'Disable': false, 'Visible': true }
                };
            }
            else if ($scope.picklisObj.ModuleId > 0 && $scope.picklisObj.MasterFieldId > 0 && $scope.picklisObj.ChildFieldId > 0
                && $scope.picklisObj.ChildFieldId != $scope.picklisObj.MasterFieldId) {
                $scope.Controls.NextButtonControl.Disable = false;
                $scope.Controls.CancelButtonControl.Disable = true;
                $scope.Controls.SelectSourceValueButtonControl.Visible = false;
            } else {
                $scope.Controls.ModuleControl.Disable = false;
                $scope.Controls.SourceControl.Disable = false;
                $scope.Controls.TargetControl.Disable = false;
                $scope.Controls.NextButtonControl.Disable = true;
                $scope.Controls.CancelButtonControl.Disable = true;
                $scope.Controls.SelectSourceValueButtonControl.Visible = false;
            }
        };

        var getNames = function () {
            angular.forEach($scope.picklisObj.Fields, function(item, index) {
                if (item.Id == parseInt($scope.picklisObj.MasterFieldId)) {
                    $scope.FirstColumn.SourceName = item.LabelDisplay;
                }
                if (item.Id == parseInt($scope.picklisObj.ChildFieldId)) {
                    $scope.FirstColumn.TargetName = item.LabelDisplay;
                }
            });
        };

        var createPicklistDetail = function() {
            var items = _.filterWithProperty($scope.FieldValues.SourceValues, 'Selected', true);
            if (items.length == 0) {
                NotifyError($scope.languages.PICKLIST_DEPENDENCY.SELECTED_SOURCE_VALUE_MSG);
                return;
            }
            $scope.picklisObj.Eli_ListDependencyDetail = [];
            angular.forEach($scope.TableObj.Columns, function(item, index) {
                for (var i = 0; i < $scope.TableObj.Rows.length; i++) {
                    if ($scope.TableObj.Rows[i][index].Selected == true) {
                        var row = $scope.TableObj.Rows[i][index];
                        var itemObj = {
                            'Id':row.Id,
                            'ParentId': $scope.picklisObj.Id,
                            'MasterValueId': row.MasterValueId,
                            'ChildValueId': row.ChildValueId
                        };
                        $scope.picklisObj.Eli_ListDependencyDetail.push(itemObj);
                    }
                }
            });
        };

        var getAvailableFields = function (fieldId) {
            var fields = _.filter($scope.picklisObj.Fields, function (item) {
                return item.Id != fieldId;
            });
            return fields;
        };

        var updateSourceTargetValues = function () {
            var columns = _.filterWithProperty($scope.FieldValues.SourceValues, 'Selected', true);
            var indexes = [];
            // For Removing Source Values
            angular.forEach($scope.TableObj.Columns, function(column,index) {
                var sourceItem = _.find(columns, function (item) {
                    return column.Id == item.Id;
                });
                if (sourceItem == null) {
                    indexes.push(index);
                } 
            });
            indexes = _.sortBy(indexes);
            indexes = indexes.reverse();
            for (var i = 0; i < indexes.length; i++) {
                $scope.TableObj.Columns.splice(indexes[i], 1);
                angular.forEach($scope.TableObj.Rows, function (row, index) {
                    row.splice(indexes[i], 1);
                });
            }

            // For Inserting Source Values
            angular.forEach(columns, function (column, index) {
                var sourceItem = _.find($scope.TableObj.Columns, function (item) {
                    return column.Id == item.Id;
                });
                if (sourceItem == null) {
                    // Insert Columns
                    $scope.TableObj.Columns.splice(index, 0, column);

                    // Insert Cell Data
                    angular.forEach($scope.TableObj.Rows, function (row, j) {
                        var cellData = {
                            'Id': 0,
                            'MasterValueId': column.Id,
                            'ChildValueId': $scope.FieldValues.TargetValues[j].Id,
                            'ChildValueName': $scope.FieldValues.TargetValues[j].Description,
                            'Selected': true
                        };
                        $scope.TableObj.Rows[j].splice(index, 0, cellData);
                    });
                }
            });
        };

        var bindingSourceTargetValues = function () {
            $scope.TableObj = {
                'Columns': [],
                'Rows': []
            };
            var columns = [];
            var targetValues = angular.copy($scope.FieldValues.TargetValues);
            if ($scope.picklisObj.Id == 0) {

                columns = _.filterWithProperty($scope.FieldValues.SourceValues, 'Selected', true);

                $scope.TableObj.Columns = columns;
                angular.forEach(targetValues, function(item2, index2) {
                    var row = [];
                    angular.forEach($scope.TableObj.Columns, function(item1, index1) {
                        var cellData = {
                            'Id': 0,
                            'MasterValueId': item1.Id,
                            'ChildValueId': item2.Id,
                            'ChildValueName': item2.Description,
                            'Selected': true
                        };
                        row.push(cellData);
                    });
                    $scope.TableObj.Rows.push(row);
                });
            } else {
                var masterGroups = _.groupBy($scope.picklisObj.Eli_ListDependencyDetail, function(item) {
                    return item.MasterValueId;
                });
                var props = Object.keys(masterGroups);
                angular.forEach(props, function(prop,index) {
                    var column = _.find($scope.FieldValues.SourceValues, function (item) {
                        return item.Id == prop;
                    });
                    columns.push(column);
                });
                $scope.TableObj.Columns = columns;
                angular.forEach(targetValues, function (item2, index2) {
                    var row = [];
                    angular.forEach($scope.TableObj.Columns, function (item1, index1) {
                        var cellData = {};
                        var itemObj = _.find($scope.picklisObj.Eli_ListDependencyDetail, function (item) {
                            return item.MasterValueId == item1.Id && item.ChildValueId == item2.Id;
                        });
                        if (itemObj != null) {
                            cellData = {
                                'Id': itemObj.Id,
                                'MasterValueId': itemObj.MasterValueId,
                                'ChildValueId': itemObj.ChildValueId,
                                'ChildValueName': item2.Description,
                                'Selected': true
                            };
                        } else {
                            cellData = {
                                'Id': 0,
                                'MasterValueId': item1.Id,
                                'ChildValueId': item2.Id,
                                'ChildValueName': item2.Description,
                                'Selected': false
                            };
                        }
                        row.push(cellData);
                    });
                    $scope.TableObj.Rows.push(row);
                });
            }
        };

        var getListValueByFieldIdCallback = function (data) {
            $scope.FieldValues = angular.fromJson(data);
            if ($scope.picklisObj.Id > 0) {
                var masterGroups = _.groupBy($scope.picklisObj.Eli_ListDependencyDetail, function (item) {
                    return item.MasterValueId;
                });
                var props = Object.keys(masterGroups);
                angular.forEach($scope.FieldValues.SourceValues, function (sourceItem, i) {
                    var result = false;
                    angular.forEach(props, function (prop, j) {
                        if (sourceItem.Id == prop) {
                            result = true;
                        }
                    });
                    sourceItem.Selected = result;
                });
            } 
            picklistDependencyService.setSourceValues($scope.FieldValues.SourceValues);
            bindingSourceTargetValues();
            $scope.Controls.SelectSourceValueButtonControl.Visible = true;
        };

        var getFieldListTypeCallback = function (data) {
            $scope.picklisObj.Fields = angular.fromJson(data);
            $scope.pickListForm.SourceFields = angular.copy($scope.picklisObj.Fields);
            $scope.pickListForm.TargetFields = angular.copy($scope.picklisObj.Fields);

            var master = _.find($scope.pickListForm.SourceFields, function (item) {
                return item.Id === $scope.picklisObj.MasterFieldId;
            });

            $scope.picklisObj.MasterFieldId = angular.isDefined(master) ? master.Id : null;

            var child = _.find($scope.pickListForm.TargetFields, function (item) {
                return item.Id === $scope.picklisObj.ChildFieldId;
            });

            $scope.picklisObj.ChildFieldId = angular.isDefined(child) ? child.Id : null;

            getNames();
            setFormDisable();
        };

        var getObjByIdCallback = function (data) {
            $scope.picklisObj = angular.fromJson(data);
            var referenceList = filterService.getReferenceList($scope.CurrentParam.ModuleId);
            $scope.pickListForm.ModuleId = _.filterWithProperty(referenceList, 'FieldName', 'ModuleId');

            if ($scope.picklisObj.Id > 0) {
                $scope.setTitle($scope.languages.PICKLIST_DEPENDENCY.EDIT_TITLE);
                $scope.Next_Clicked();
            } else {
                $scope.setTitle($scope.languages.PICKLIST_DEPENDENCY.ADD_NEW_TITLE);
                if ($scope.pickListForm.ModuleId.length > 0) {
                    $scope.picklisObj.ModuleId = $scope.pickListForm.ModuleId[0].Id;
                }
            }
            $scope.Module_Changed();
        };

        var saveObjectCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                filterService.resetAllPicklistDependencies();
                if (data.Id == 0) {
                    $scope.$emit('CloseAndOpenWindow', data.Id);
                } else {
                    $scope.$emit('closeWindow');
                }
            } else {
                NotifyError(data.Result, 5000);
            }
        };
        
        var loadSourceFieldValues = function () {
            picklistService.getListValueByFieldIds($scope.picklisObj.MasterFieldId, $scope.picklisObj.ChildFieldId)
                .then(getListValueByFieldIdCallback);
        };

        var loadData = function () {
            picklistDependencyService.getObjById($scope.CurrentParam.Id)
                .then(getObjByIdCallback);
        };
        
        var initForm = function () {
            loadData();
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.picklisObj = {};
        $scope.pickListForm = {};
        $scope.select2Options = {
            allowClear: true
        };
        $scope.FirstColumn = {
            SourceName: '',
            TargetName:'',
        }
        $scope.FieldValues = {
            'SourceValues': [],
            'TargetValues': []
        };
        $scope.TableObj = {
            'Columns': [],
            'Rows': []
        };
        $scope.Controls = {
            'ModuleControl': { 'Disable': false, 'Visible': true },
            'SourceControl': { 'Disable': false, 'Visible': true },
            'TargetControl': { 'Disable': false, 'Visible': true },
            'NextButtonControl': { 'Disable': false, 'Visible': true },
            'CancelButtonControl': { 'Disable': true, 'Visible': true },
            'SelectSourceValueButtonControl': { 'Disable': false, 'Visible': false }
        };
        
        $scope.countDialog = 0;
        $scope.detailFormUrl = '/appviews/dashboard/picklistdependency/sourcevalues.html';
        $scope.FormTitle = $scope.languages.PICKLIST_DEPENDENCY.SELECT_SOURCE_TITLE;
        $scope.CurrentModule = $scope.CurrentParam.ModuleId;
        // --- Define Scope Method. ---------------------- //

        $scope.Module_Changed = function () {
            entityFieldService.getFieldListType($scope.picklisObj.ModuleId)
                .then(getFieldListTypeCallback);
        };

        $scope.SourceField_Changed = function () {
            var fields = getAvailableFields($scope.picklisObj.MasterFieldId);
            $scope.pickListForm.TargetFields = fields;
            setFormDisable();
            getNames();
        };
        $scope.TargetField_Changed = function () {
            var fields = getAvailableFields($scope.picklisObj.ChildFieldId);
            $scope.pickListForm.SourceFields = fields;
            setFormDisable();
            getNames();
        };
        $scope.Next_Clicked = function () {
            $scope.Controls.ModuleControl.Disable = true;
            $scope.Controls.SourceControl.Disable = true;
            $scope.Controls.TargetControl.Disable = true;
            $scope.Controls.NextButtonControl.Disable = true;
            $scope.Controls.CancelButtonControl.Disable = false;
            
            loadSourceFieldValues();
        };
        $scope.Cancel_Clicked = function () {
            setFormDisable();
            $scope.Controls.ModuleControl.Disable = false;
            $scope.Controls.SourceControl.Disable = false;
            $scope.Controls.TargetControl.Disable = false;
            $scope.Controls.SelectSourceValueButtonControl.Visible = false;
            $scope.TableObj = {
                'Columns': [],
                'Rows': []
            };
        };
        $scope.Select_SourceValues = function () {
            showDialog(true);
        };
        
        $scope.Save = function () {
            picklistDependencyService.saveSelectedSourceValue();
            showDialog(false);
        };
        $scope.CloseDialog = function () {
            picklistDependencyService.setSourceValues($scope.FieldValues.SourceValues);
        };
        $scope.TargetValue_CheckChanged = function (sourceIndex,targetIndex) {
            var result = false;
            angular.forEach($scope.TableObj.Rows, function (item, index) {
                if (item[sourceIndex].Selected == true) {
                    result = true;
                }
            });
            $scope.TableObj.Columns[sourceIndex].Selected = result;
        };
        $scope.SourceValue_CheckChanged = function(sourceIndex) {
            var result = $scope.TableObj.Columns[sourceIndex].Selected;
            angular.forEach($scope.TableObj.Rows, function(item, index) {
                item[sourceIndex].Selected = result;
            });
        };
        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('SetSourceValuesEvent', function (event, args) {
            $scope.FieldValues.SourceValues = picklistDependencyService.getSourceValues();
            updateSourceTargetValues();
        });

        $scope.$on('saveDataEvent', function (event) {
            createPicklistDetail();
            var items = _.filterWithProperty($scope.FieldValues.SourceValues, 'Selected', true);
            if (items.length == 0) {
                return;
            }
            picklistDependencyService.saveObject($scope.picklisObj, $scope.CurrentParam.ModuleId)
                .then(saveObjectCallback);
        });

        // --- Initialize. ------------------------ //

        initForm();
    }

})();