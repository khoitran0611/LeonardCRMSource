(function (ng) {


    'use strict';

    angular.module("LeonardCRM").controller("CustomFieldCtrl", ctrl);

    ctrl.$inject = ["$scope", "entityFieldService", "registryService", "filterService", "picklistDependencyService", "_"];

    function ctrl($scope, entityFieldService, registryService, filterService, picklistDependencyService, _) {

        // --- Define Controller Variables. ---------------------- //

        var pickList = [];
        var pickListDependencies = [];

        // --- Define Controller Method. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.parentModuleId = -1;
        $scope.customfields = [];
        $scope.curretDate = new Date();
        $scope.masterId = 0;
        $scope.recordId = $scope.CurrentParam.Id;
        $scope.moduleId = $scope.CurrentParam.ModuleId;
        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };
        $scope.hour = [];
        $scope.minute = [];
        $scope.Time = {};
        $scope.nullvalue = { Id: "-000-", Description: "" };
        $scope.sections = [];

        // --- Define Scope Method. ---------------------- //

        $scope.Combobox_Changed = function (field) {
            var result = _.filterWithProperty(pickListDependencies, 'MasterFieldId', field.FieldId);
            if (result.length > 0) {
                var listTargetFieldIds = _.uniq(_.pluck(result, 'ChildFieldId'));
                angular.forEach(listTargetFieldIds, function (id) {
                    angular.forEach($scope.sections, function(section) {
                        angular.forEach(section.LeftSide, function (childField) {
                            if (childField.FieldId == id) {
                                var listSourceValues = _.filter(pickListDependencies, { 'MasterFieldId': field.FieldId, 'ChildFieldId': childField.FieldId, 'MasterValueId': parseInt(field.FieldData) });
                                var listTargetValueIds = _.pluck(listSourceValues, 'ChildValueId');
                                childField.ListValues = _.filterWithProperty(pickList, 'FieldName', childField.FieldName);
                                childField.ListValues = _.filterWithValues(childField.ListValues, 'Id', listTargetValueIds);
                                if (childField.Mandatory == true) {
                                    if (childField.ListValues.length > 0) {
                                        childField.FieldData = childField.ListValues[0].Id;
                                    } else {
                                        childField.FieldData = $scope.nullvalue.Id;
                                    }
                                } else {
                                    childField.FieldData = $scope.nullvalue.Id;
                                }
                                $scope.Combobox_Changed(childField);
                            }
                        });
                        angular.forEach(section.RightSide, function (childField) {
                            if (childField.FieldId == id) {
                                var listSourceValues = _.filter(pickListDependencies, { 'MasterFieldId': field.FieldId, 'ChildFieldId': childField.FieldId, 'MasterValueId': parseInt(field.FieldData) });
                                var listTargetValueIds = _.pluck(listSourceValues, 'ChildValueId');
                                childField.ListValues = _.filterWithProperty(pickList, 'FieldName', childField.FieldName);
                                childField.ListValues = _.filterWithValues(childField.ListValues, 'Id', listTargetValueIds);
                                if (childField.Mandatory == true) {
                                    if (childField.ListValues.length > 0) {
                                        childField.FieldData = childField.ListValues[0].Id;
                                    } else {
                                        childField.FieldData = $scope.nullvalue.Id;
                                    }
                                } else {
                                    childField.FieldData = $scope.nullvalue.Id;
                                }
                                $scope.Combobox_Changed(childField);
                            }
                        });
                    });
                });
            }
        };

        $scope.CollapsedClick = function (item) {
            item.Collapsed = !item.Collapsed;
        };

        $scope.LoadData = function () {
            for (var l = 0; l < 60; l += 5) {
                var tempt = l;
                var zero = "0";
                if (tempt < 10) {
                    tempt = zero + tempt;
                    $scope.minute.push(tempt);
                } else {
                    $scope.minute.push(l);
                }
            }
            for (var j = 1; j < 24; j++) {
                var m = j;
                var oh = "0";
                if (m < 10) {
                    m = oh + m;
                    $scope.hour.push(m);
                } else {
                    $scope.hour.push(j);
                }
            }
            pickList = filterService.getPickList($scope.moduleId);
            pickListDependencies = filterService.getPicklistDependency($scope.moduleId);
            var customeFields = entityFieldService.getCustomData();
            angular.forEach(customeFields, function (item, index) {
                // MultiSelectBox Type
                if (item.TypeName == 'MultiSelectBox') {
                    if (item.FieldData != null || item.FieldData != undefined && item.FieldData.length > 0) {
                        var t = item.FieldData.substr(0, item.FieldData.length - 1);
                        item.FieldData = t.split(',').map(function (p) {
                            return parseInt(p, 0);
                        });
                    }
                    item.ListValues = _.filterWithProperty(pickList, 'FieldName', item.FieldName);
                }
                // Date, Time, DateTime
                if ((item.TypeName == 'Date' || item.TypeName == 'Time' || item.TypeName == 'DateTime') && item.MasterRecordId == 0) {
                    item.FieldData = angular.copy($scope.curretDate);
                }
                // Edit Case: Time
                if (item.TypeName == 'Time' && item.MasterRecordId > 0 && item.FieldData != null &&
                    item.FieldData.length > 0 && item.FieldData.indexOf(':') > 0 && item.FieldData.indexOf('undefined') == -1) {
                    var res = item.FieldData.split(':');
                    item.selectedhour = res[0];
                    item.selectedminute = res[1];
                }

                // List Type
                if (item.TypeName == 'List') {
                    var result = _.filterWithProperty(pickListDependencies, 'MasterFieldId', item.FieldId);
                    var listValues = [];
                    var subResult = _.filterWithProperty(pickListDependencies, 'ChildFieldId', item.FieldId);
                    // Check is MasterField
                    if (result.length > 0 && subResult.length == 0) {
                        listValues = _.filterWithProperty(pickList, 'FieldName', item.FieldName);
                        item.ListValues = listValues;

                    } else {
                        subResult = _.filterWithProperty(pickListDependencies, 'ChildFieldId', item.FieldId);
                        // Check is not ChildField
                        if (subResult.length == 0) {
                            listValues = _.filterWithProperty(pickList, 'FieldName', item.FieldName);
                            item.ListValues = listValues;
                        }
                    }
                    var listTargetFieldIds = [];
                    if (item.Mandatory == true) {
                        if (subResult.length == 0 || result.length > 0) {
                            if (($scope.recordId > 0 && (item.FieldData == null || item.FieldData == undefined || item.FieldData == "-000-")) || $scope.recordId == 0) {
                                item.FieldData = item.ListValues[0].Id;
                            }
                        }
                        if (result.length > 0) {
                            listTargetFieldIds = _.uniq(_.pluck(result, 'ChildFieldId'));
                            angular.forEach(listTargetFieldIds, function (id) {
                                angular.forEach(customeFields, function (childField) {
                                    if (childField.FieldId == id) {
                                        var listSourceValues = _.filter(pickListDependencies, { 'MasterFieldId': item.FieldId, 'ChildFieldId': childField.FieldId, 'MasterValueId': parseInt(item.FieldData) });
                                        var listTargetValueIds = _.pluck(listSourceValues, 'ChildValueId');
                                        childField.ListValues = _.filterWithProperty(pickList, 'FieldName', childField.FieldName);
                                        childField.ListValues = _.filterWithValues(childField.ListValues, 'Id', listTargetValueIds);
                                        if (childField.Mandatory == true) {
                                            if (($scope.recordId > 0 && (childField.FieldData == null || childField.FieldData == undefined || childField.FieldData == "-000-")) || $scope.recordId == 0) {
                                                childField.FieldData = childField.ListValues[0].Id;
                                            }
                                        } else {
                                            childField.FieldData = $scope.nullvalue.Id;
                                        }
                                    }
                                });
                            });
                        }
                    } else {
                        if (result.length > 0 && (item.FieldData != null || item.FieldData != undefined) && item.FieldData.length > 0 && item.FieldData != "-000-") {
                            listTargetFieldIds = _.uniq(_.pluck(result, 'ChildFieldId'));
                            angular.forEach(listTargetFieldIds, function (id) {
                                angular.forEach(customeFields, function (childField) {
                                    if (childField.FieldId == id) {
                                        var listSourceValues = _.filter(pickListDependencies, { 'MasterFieldId': item.FieldId, 'ChildFieldId': childField.FieldId, 'MasterValueId': parseInt(item.FieldData) });
                                        var listTargetValueIds = _.pluck(listSourceValues, 'ChildValueId');
                                        childField.ListValues = _.filterWithProperty(pickList, 'FieldName', childField.FieldName);
                                        childField.ListValues = _.filterWithValues(childField.ListValues, 'Id', listTargetValueIds);
                                        if (childField.Mandatory == true) {
                                            if (($scope.recordId > 0 && (childField.FieldData == null || childField.FieldData == undefined || childField.FieldData == "-000-")) || $scope.recordId == 0) {
                                                childField.FieldData = childField.ListValues[0].Id;
                                            }
                                        } else {
                                            childField.FieldData = childField.FieldData || $scope.nullvalue.Id;
                                        }
                                    }
                                });
                            });
                        }
                    }
                }

            });

            var sections = _.groupBy(customeFields, function (item) {
                return item.SectionId;
            });

            $scope.sections = [];
            
            angular.forEach(sections, function (items, index) {
                var item = items[0];
                var section = {
                    'SectionId': item.SectionId,
                    'SectionName': item.SectionName,
                    'LeftSide': _.sortBy(_.filterWithProperty(items, 'LeftSide',true), 'FieldOrder'),
                    'RightSide': _.sortBy(_.filterWithProperty(items, 'LeftSide', false), 'FieldOrder'),
                    'Collapsed':true
                };

                $scope.sections.push(section);
            });
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('parentModulePassEvent', function (e) {
            $scope.LoadData();
        });

        $scope.$on('parentSaveEvent', function () {
            $scope.customfields = [];
            angular.forEach($scope.sections, function(section) {
                $scope.customfields = $scope.customfields.concat(section.LeftSide).concat(section.RightSide);
            });
            for (var i = 0; i < $scope.customfields.length; i++) {
                if ($scope.customfields[i].TypeName == "Time") {
                    $scope.customfields[i].FieldData = $scope.customfields[i].selectedhour + ':' + $scope.customfields[i].selectedminute;
                    if ($scope.customfields[i].FieldData.indexOf('undefined') >= 0) {
                        $scope.customfields[i].FieldData = null;
                    }
                    continue;
                }
                if ($scope.customfields[i].TypeName == "MultiSelectBox") {
                    if ($scope.customfields[i].FieldData != null) {
                        var temp = $scope.customfields[i];
                        var str = '';

                        if (temp.FieldData instanceof Array) {
                            if (temp.FieldData != null && temp.FieldData.length > 0) {
                                temp.FieldData = _.without(temp.FieldData, '');
                                str += temp.FieldData.join(',') + ',';
                                $scope.customfields[i].FieldData = str;
                            } else {
                                $scope.customfields[i].FieldData = "-000-";
                            }
                        }
                    }
                    continue;
                }
                if ($scope.customfields[i].TypeName == 'List') {
                    if ($scope.customfields[i].FieldData != null && $scope.customfields[i].FieldData == '-000-') {
                        $scope.customfields[i].FieldData = null;
                    }
                    continue;
                }
            }
            $scope.$emit('sendCutomFieldEvent', $scope.customfields);
        });
    }

})(angular);