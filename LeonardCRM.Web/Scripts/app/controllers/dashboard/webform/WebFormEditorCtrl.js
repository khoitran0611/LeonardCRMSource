(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("WebformEditorCtrl", ctrl);

    ctrl.$inject = ["_", "$scope", "$compile", "requestContext", "webformService", "appService", "filterService", "entityFieldService", "windowService", "dataTypeService"];

     function ctrl(_, $scope, $compile, requestContext, webformService, appService, filterService, entityFieldService, windowService, dataTypeService) {
        // --- Define Controller Variables. ---------------------- //
        var isFirstLoad = true;
        $scope.isNew = true;
        $scope.webformId = $scope.CurrentParam.Id;
        $scope.htmlPopupKey = 'webFormhtmlPopupKey';

        var init = function () {
            dataTypeService.getAllDataType()
            .then(function (data) {
                $scope.dataTypeSource = angular.fromJson(data);
                loadData();;
            });
            
        };

        var loadData = function () {
            appService.getModuleForEntityFields().then(function (data) {
                var modules = angular.fromJson(data);
                $scope.modules = _.where(modules, { 'IsWebform': true });
                loadWebform();
            });

            var referenceList = filterService.getReferenceList($scope.CurrentParam.ModuleId);
            var usersAssigns = _.where(referenceList, { 'FieldName': 'AssignedTo' });
            $scope.users = _.sortBy(usersAssigns, 'Description');;
        };

        function loadWebform() {
            
            webformService.getObjectById($scope.CurrentParam.Id).then(function (data) {
                $scope.webform = angular.fromJson(data);
                if ($scope.webform.Id > 0) {
                    $scope.setTitle($scope.webform.FormName);
                    $scope.isNew = false;
                } else {
                    $scope.setTitle($scope.languages.WEBFORM.NEW_TITLE);
                }

                loadControl();
                loadFieldData();
            });
        };

        var loadControl = function() {
            var properties = Object.keys($scope.webform);
            var fields = filterService.getFields($scope.CurrentParam.ModuleId);
            angular.forEach(properties, function (prop, index) {
                var field = _.findWithProperty(fields, 'ColumnName', prop);
                if (field != null) {
                    $scope.webform[prop + 'Control'] = {
                        'Visible': field.Visible,
                        'Locked': field.Locked
                    };
                } else {
                    $scope.webform[prop + 'Control'] = {
                        'Visible': false,
                        'Locked': true
                    };
                }
            });
        }

        $scope.changeModule = function() {
            loadFieldData();
        };

        var loadFieldData = function () {
            if ($scope.webform.Id == 0 && isFirstLoad) {
                $scope.webform.ModuleId = $scope.modules[0].Id;
                $scope.webform.AssignedTo = $scope.users[0].Id;
                isFirstLoad = false;
            }

            entityFieldService.getManageFieldsByModuleId($scope.webform.ModuleId)
                .then(function (data) {
                    $scope.EntityFields = angular.fromJson(data);
                    mapDataToLoad();
                    //TODO: load html
                    setCollection();

                    $scope.getPickList();
            });
        };

        //html
        $scope.localFilterObj = {
            Collection: {}
        };

        $scope.pickList = {
            PickList: [],
            ReferenceList: []
        };
        $scope.loadedPickList = false;

        var setCollection = function () {
            angular.forEach($scope.EntityFields, function (field, key) {
                var dataType = _.findWithProperty($scope.dataTypeSource, 'Id', field.DataTypeId);
                if (dataType && dataType.IsList) {
                    $scope.localFilterObj.Collection['_' + field.FieldName] = '';
                }
            });
        };

        var generateHtml = function (field, ngModel) {
            var model = 'ng-model="' + ngModel + '"';

            var dataType = _.findWithProperty($scope.dataTypeSource, 'Id', field.DataTypeId);

            var html = '';

            if (dataType) {
                if (dataType.IsCheckBox) {
                    html += '<input type="checkbox" name="' + field.FieldName + ' ' + model + '"/>';
                }
                else if (dataType.IsEmail) {
                    html += '<input type="email" name="' + field.FieldName + '" ' + model + '/>';
                }
                else if (dataType.IsUrl) {
                    html += '<input type="url" name="' + field.FieldName + '" ' + model + '/>';
                }
                else if (dataType.IsDate) {
                    html += '<input type="date" name="' + field.FieldName + '" ' + model + '/>';
                }
                else if (dataType.IsDateTime) {
                    html += '<input type="datetime" name="' + field.FieldName + '" ' + model + '/>';
                }
                else if (dataType.IsCurrency || dataType.IsDecimal  || dataType.IsInteger || dataType.IsText || dataType.IsTextArea) {
                    html += '<input type="text" name="' + field.FieldName + '" ' + model + '/>';
                }
                else if (dataType.IsList) {
                    var optionsHtml = '';

                    if (!field.Mandatory) {
                        optionsHtml += ' <option value="null">&nbsp;</option>';
                    }
                    if ($scope.localFilterObj.Collection['_' + field.FieldName]) {
                        for (var i = 0; i < $scope.localFilterObj.Collection['_' + field.FieldName].length; i++) {
                            optionsHtml += ' <option value="' + $scope.localFilterObj.Collection['_' + field.FieldName][i].Id + '">' + $scope.localFilterObj.Collection['_' + field.FieldName][i].Description + '</option>';
                        }
                    }

                    html += '<select ng-init=" ' + ngModel + ' = ' + ngModel + ' == \'\' ? null : ' + ngModel + ' " name="' + field.FieldName + '" ' + model + '>'
                        + optionsHtml
                        + '</select>';
                }
                else if (dataType.IsDate || dataType.IsDateTime) {
                    html += '<input type="datetime" name="' + field.FieldName + '" ' + model + '/>';
                } else if (dataType.IsTime) {
                    html += '<input type="time" name="' + field.FieldName + '" ' + model + '/>';
                }
            } else {
                console.log(field);
            }

            return html;
        };


        var loadHtml = function () {

            var strHtml = '';
            for (var i = 0; i < $scope.EntityFields.length; i++) {
                var field = $scope.EntityFields[i];
                if (!(!field.Display || !field.IsActive)) {
                    strHtml += '<tr>'
                        + ' <td class="col-xs-1 text-center"><input type="checkbox" ng-model="EntityFields[' + i + '].Selected" ng-disabled="' + (field.Mandatory?'{{true}}' : '{{false}}') + '" /></td>'
                        + ' <td class="col-xs-2"><span ng-show="' + (field.Mandatory ? '{{true}}' : '{{false}}') + '"><span class="mandatorysign">*</span></span>' + field.LabelDisplay + '</td>'
                        + ' <td class="col-xs-4">';

                    strHtml += '<span ng-show="(EntityFields[' + i + '].Selected || EntityFields[' + i + '].Mandatory)">';
                    strHtml += generateHtml(field, 'EntityFields[' + i + '].OverrideValue');
                    strHtml += '</span>';

                    strHtml += '</td>'
                        + ' <td class="col-xs-1 text-center"><input type="checkbox" ng-disabled="' + (field.Mandatory ? '{{true}}' : '{{false}}') + '" ng-model="EntityFields[' + i + '].Required" ng-show="' + (field.Selected ? '{{true}}' : '{{false}}') + '" /></td>'
                        + ' <td class="col-xs-4">' + field.FieldName + '</td>'
                        + ' </tr>';
                }
            }
            loadHtmlScreen(strHtml);
        };

        var loadHtmlScreen = function (htmlSource) {
            $('#webformFields').html($compile(htmlSource)($scope));
        };

        $scope.getPickList = function () {
            if (!$scope.loadedPickList) {
                filterService.preLoadFilterColumnsAndPickLists($scope.webform.ModuleId);
            } else {
                $scope.loadedPickList = true;
                $scope.pickList.PickList = filterService.getPickList($scope.webform.ModuleId);
                $scope.pickList.ReferenceList = filterService.getReferenceList($scope.webform.ModuleId);
                $scope.setPickList();
            }
            loadHtml();

        };

        $scope.$on('filterPickListEvent', function (event, args) {
            $scope.pickList.PickList = filterService.getPickList($scope.webform.ModuleId);
            $scope.pickList.ReferenceList = filterService.getReferenceList($scope.webform.ModuleId);
            $scope.setPickList();
            loadHtml();
        });

        $scope.setPickList = function () {
            var keys = Object.keys($scope.localFilterObj.Collection);
            angular.forEach(keys, function (value, index) {
                if (value.indexOf('_') == 0) {
                    var str = value.substr(1, value.length);
                    var objs = _.where($scope.pickList.PickList, { 'FieldName': str });
                    if (objs.length == 0) {
                        objs = _.where($scope.pickList.ReferenceList, { 'FieldName': str });
                    }
                    $scope.localFilterObj.Collection[value] = objs;
                }
            });
        };
        //Endhtml

        var mapDataToLoad = function () {

            for (var i = 0; i < $scope.EntityFields.length; i++) {
                var webformDetail = _.findWithProperty($scope.webform.Eli_WebformDetail, 'FieldId', $scope.EntityFields[i].Id);
                
                //hide audit field
                if ($scope.checkRemoveField($scope.EntityFields[i])) {
                    $scope.EntityFields[i].Display = false;
                }

                if (($scope.EntityFields[i].Display && $scope.EntityFields[i].IsActive) && (webformDetail || $scope.EntityFields[i].Mandatory == true)) {
                    if ($scope.isNew) {
                        $scope.EntityFields[i].OverrideValue = null;
                        $scope.EntityFields[i].Required = $scope.EntityFields[i].Mandatory;
                    } else {
                        $scope.EntityFields[i].OverrideValue = webformDetail ? webformDetail.OverrideValue : null;
                        $scope.EntityFields[i].Required = webformDetail ? webformDetail.Required : true;
                    }
                    
                    $scope.EntityFields[i].Selected = true;
                    $scope.EntityFields[i].WebformDetailId = webformDetail ? webformDetail.Id : 0;
                } else {
                    $scope.EntityFields[i].OverrideValue = '';
                    $scope.EntityFields[i].Required = false;
                    $scope.EntityFields[i].Selected = false;
                }
            }
        };



        var mapDataToSave = function() {
            $scope.webform.Eli_WebformDetail = [];
            for (var i = 0; i < $scope.EntityFields.length; i++) {
                if ($scope.EntityFields[i].Selected == true) {
                    var objDetail = {
                        Id: $scope.EntityFields[i].WebformDetailId,
                        FieldId: $scope.EntityFields[i].Id,
                        OverrideValue: $scope.EntityFields[i].OverrideValue,
                        Required: $scope.EntityFields[i].Required

                        //for html
                        , DataTypeId: $scope.EntityFields[i].DataTypeId
                        , FieldName: $scope.EntityFields[i].FieldName
                        , Value: null
                        , LabelDisplay: $scope.EntityFields[i].LabelDisplay
                        , Mandatory: $scope.EntityFields[i].Mandatory
                    };
                    $scope.webform.Eli_WebformDetail.push(objDetail);
                }
            }

            $scope.webform.currentModuleId = $scope.CurrentParam.ModuleId;
        };

        $scope.checkRemoveField = function (field) {

            if (field.FieldName == 'CreatedBy' || field.FieldName == 'ModifiedBy' || field.FieldName == 'CreatedDate' || field.FieldName == 'ModifiedDate')
                return true;
            if (field.IsWebform != true)
                return true;
            var dataType = _.findWithProperty($scope.dataTypeSource, 'Id', field.DataTypeId);
            if (!dataType || (dataType && (dataType.IsMultiSelecttBox)))
                return true;
            return false;
        };

        $scope.$on('saveDataEvent', function (event) {
            mapDataToSave();
            webformService.saveObject($scope.webform, $scope.CurrentParam.ModuleId).then(function(data) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                    $scope.CurrentParam.Id = data.Id;
                    loadWebform();
                    if ($scope.isNew) {
                        $scope.$emit('CloseAndOpenWindow', $scope.CurrentParam.Id);
                    } else {
                        $scope.$emit('closeWindow');
                    }
                } else {
                    NotifyError(data.Result);
                }
            });
        });

        $scope.showForm = function() {
            mapDataToSave();
            var param = {
                Url: '/appviews/dashboard/webform/webform-html-editor.html',
                Id: 0,
                ParentId: $scope.webformId,
                ViewId: $scope.CurrentParam.ViewId,
                ModuleId: $scope.CurrentParam.ModuleId,
                Key: $scope.htmlPopupKey,
                Model: $scope.webform
            };
            windowService.openWindow(param);
        };

        init();

    }

})(angular);