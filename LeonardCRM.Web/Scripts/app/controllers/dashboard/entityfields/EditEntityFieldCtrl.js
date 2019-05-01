(function () {

    'use strict';

    angular.module("LeonardCRM").controller("EditEntityFieldCtrl", ctrl);

    ctrl.$inject = ["$scope", "roleService", "entityFieldService", "requestContext", "dataTypeService", "viewService", "toolbarService", "picklistService", "filterService", "_", "appConfig", "$timeout"];

    function ctrl($scope, roleService, entityFieldService, requestContext, dataTypeService, viewService, toolbarService, picklistService, filterService, _, appConfig, $timeout) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var setVisible = function () {
            var obj = _.filterWithProperty($scope.dataTypeSource, "Id", $scope.currentField.DataTypeId);
            if (obj.length > 0) {
                $scope.currentDataType = obj[0];
                $scope.showDataLength = $scope.currentDataType.IsDate ||
                                        $scope.currentDataType.IsDateTime || $scope.currentDataType.IsDecimal ||
                                        $scope.currentDataType.IsCurrency || $scope.currentDataType.IsTime ||
                                        $scope.currentDataType.IsList || $scope.currentDataType.IsMultiSelecttBox ||
                                        $scope.currentDataType.IsCheckBox;
            }
            if ($scope.currentDataType.IsMultiSelecttBox == true || $scope.currentDataType.IsList == true) {
                // Id = 0 : create entity
                // Id != 0 & not foreignkey & LisName is null : binding picklist default
                if ($scope.currentField.Id == 0 ||
                    ($scope.currentField.Id > 0 && $scope.currentField.ListNameId == undefined &&
                    (($scope.currentField.ForeignKey == undefined || $scope.currentField.ForeignKey == false)))) {
                    if ($scope.listName.length > 0) {
                        $scope.currentField.ListNameId = angular.copy($scope.listName[0].Id);
                    }
                }
            } else {
                $scope.currentField.ListNameId = null;
            }

            if ($scope.showDataLength) {
                $scope.currentField.DataLength = null;
            }
        };

        var saveEntityFieldCallback = function (data) {
        	if (data.ReturnCode == 200) {
        		filterService.resetAllFields();
        		filterService.preLoadFilterColumnsAndPickLists(appConfig.entityFieldModule);
                NotifySuccess(data.Result, 5000);
                if ($scope.currentField.Id == 0) {
                    var temp = $scope.currentField.DataTypeId;
                    $scope.currentField = angular.copy($scope.shadowField);
                    $scope.currentField.DataTypeId = temp;
                    $timeout(function () {
                    	$scope.$emit('CloseAndOpenWindow', data.Id);
                    }, 400);                    
                } else {
                    $scope.$emit('closeWindow');
                }
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var getListNameByModule = function () {
            picklistService.GetListNameByModuleId($scope.CurrentParam.ModuleId)
                .then(function (data) {
                    $scope.listName = angular.fromJson(data);
                });
        };

        var getDataType = function () {
            dataTypeService.getAllDataType()
                .then(function (data) {
                    $scope.dataTypeSource = angular.fromJson(data);

                    entityFieldService.getEntityFieldById($scope.CurrentParam.Id).then(function (data1) {
                        $scope.currentField = angular.fromJson(data1);
                        $scope.shadowField = angular.copy($scope.currentField);
                        if ($scope.currentField.Id < 1) {
                            $scope.currentField.DataTypeId = $scope.dataTypeSource[0].Id;

                        }
                        
                        var properties = Object.keys($scope.currentField);
                        var fields = filterService.getFields(appConfig.entityFieldModule)
                        angular.forEach(properties, function (prop, index) {
                        	var field = _.findWithProperty(fields, 'ColumnName', prop);
                        	if (field != null) {
                        		$scope.currentField[prop + 'Control'] = {
                        			'Visible': field.Visible,
                        			'Locked': field.Locked,
                        			'Mandatory': field.Mandatory
                        		};
                        	} else {
                        		$scope.currentField[prop + 'Control'] = {
                        			'Visible': false,
                        			'Locked': true,
                        			'Mandatory': false
                        		};
                        	}
                        });
                        
                        setVisible();
                    });
                });
        };

        var initEditForm = function () {
            if ($scope.CurrentParam.Id == 0) {
                $scope.setTitle($scope.languages.ENTITY_FIELD.ADD_NEW_HEADING);
            } else {
                $scope.setTitle($scope.languages.ENTITY_FIELD.EDIT_HEADING);

            }
            getDataType();
            getListNameByModule();
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.currentField = {};
        $scope.shadowField = {};
        $scope.dataTypeSource = [];
        $scope.currentDataType = {};
        $scope.showDataLength = true;
        $scope.listName = [];

        // --- Define Scope Method. ---------------------- //

        $scope.dataTypeChanged = function () {
            setVisible();
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('saveDataEvent', function (event) {
            if (($scope.currentField.Deletable && isNaN($scope.currentField.DataLength)) || parseInt($scope.currentField.DataLength) <= 0) {
                NotifyError($scope.languages.ENTITY_FIELD.DATA_LENGTH_INVALID_MSG, 5000);
                return;
            }
            if (isNaN($scope.currentField.Point)) {
                NotifyError($scope.languages.ENTITY_FIELD.POINT_INVALID_MSG, 5000);
                return;
            }

            $scope.currentField.ModuleId = $scope.CurrentParam.ModuleId;
            entityFieldService.saveEntityField($scope.currentField).then(saveEntityFieldCallback);
        });

        // --- Initialize. ---------------------------------- //
        initEditForm();
    }

})();