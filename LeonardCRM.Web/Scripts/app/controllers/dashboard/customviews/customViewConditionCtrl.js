(function () {
    "use strict";

    angular.module('LeonardCRM').controller("customViewConditionCtrl", ctrl);

    ctrl.$inject = ["$scope", "$rootScope", "$timeout", "requestContext", "appService", "_", "registryService", "filterService", "toolbarService"];

    function ctrl($scope, $rootScope, $timeout, requestContext, appService, _, registryService, filterService, toolbarService) {
        var tempAnd = [];
        var tempOr = [];
        $scope.isLoading = true;
        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.columns = [];
        $scope.andConditions = [];
        $scope.orConditions = [];
        $scope.listNames = filterService.getPickList(toolbarService.getCurrentModuleId());
        //$scope.referenceLists = filterService.getReferenceList(toolbarService.getCurrentModuleId());

        $scope.$on('initCondition', function () {
            initCondition();
        });

        $scope.addAndCondition = function () {
            var condition = { ColumnName: $scope.columns[0].ColumnName, FieldId: $scope.columns[0].FieldId, Operator: '=', FilterValue: '', FilterValue1: '', IsAND: true, ListValues: [], ViewId: toolbarService.getCurrentViewId() };
            condition.Type = getType(condition);
            $scope.andConditions.push(condition);
            filterService.setAndConditions($scope.andConditions, toolbarService.getCurrentViewId());
        };
        $scope.removeAndCondition = function (index) {
            $scope.andConditions.splice(index, 1);
            filterService.setAndConditions($scope.andConditions, toolbarService.getCurrentViewId());
        };
        $scope.addOrCondition = function () {
            var condition = { ColumnName: $scope.columns[0].ColumnName, FieldId: $scope.columns[0].FieldId, Operator: '=', FilterValue: '', FilterValue1: '', IsAND: false, ListValues: [], ViewId: toolbarService.getCurrentViewId() };
            condition.Type = getType(condition);
            $scope.orConditions.push(condition);
            filterService.setOrConditions($scope.orConditions, toolbarService.getCurrentViewId());
        };
        $scope.removeOrCondition = function (index) {
            $scope.orConditions.splice(index, 1);
            filterService.setOrConditions($scope.orConditions, toolbarService.getCurrentViewId());
        };
        $scope.changeType = function (index) {            
            var currentCondition = $scope.andConditions[index];
            var currentCol = findColumnByAliasName(currentCondition.ColumnName);
            //update field
            currentCondition.FieldId = currentCol.FieldId;
            //update type by field
            currentCondition.Type = getType(currentCondition);
            //clear the previous value 
            currentCondition.FilterValue = null;
            currentCondition.FilterValue1 = null;
            filterService.setAndConditions($scope.andConditions, toolbarService.getCurrentViewId());
        };
        $scope.changeOrType = function (index) {
            var element = $scope.orConditions[index];
            var currentCol = findColumnByAliasName(element.ColumnName);
            //update field
            element.FieldId = currentCol.FieldId;
            //update type by field
            element.Type = getType(element);
            //clear the previous value 
            element.FilterValue = null;
            element.FilterValue = null;

            if (element.Type != 'date' && element.Type != 'listbox')
                element.Operator = '=';
            filterService.setOrConditions($scope.orConditions, toolbarService.getCurrentViewId());
        };
        $scope.changeOperator = function (index) {
            var element = $scope.andConditions[index];
            if (element.Operator == 'Within')
                element.FilterValue = '-7';
            else if (element.Type == 'date')
                element.FilterValue1 = element.FilterValue = new Date();
            filterService.setAndConditions($scope.andConditions, toolbarService.getCurrentViewId());
        };
        $scope.changeOrOperator = function (index) {
            var element = $scope.orConditions[index];
            if (element.Operator == 'Within')
                element.FilterValue = '-7';
            else if (element.Type == 'date')
                element.FilterValue1 = element.FilterValue = new Date();
            filterService.setAndConditions($scope.orConditions, toolbarService.getCurrentViewId());
        };

        //-------listen to parent's event to get filter stuffs---------------------
        $scope.$on('filterColumnsEvent', function (event, columns) {
            $scope.columns = [];
            $scope.andConditions = [];
            $scope.orConditions = [];            
            if (columns.Fields !== null && columns.Fields.length > 0) {
                //var moduleId = columns.Fields[0].ModuleId;
                var viewId = columns.ViewId;

                $scope.columns = angular.copy(columns.Fields);                                
                if (columns.Conditions) {
                    filterService.setFields($scope.columns);
                    tempAnd = _.filter(columns.Conditions, function(e) { return e.IsAND === true; });
                    tempOr = _.filter(columns.Conditions, function(e) { return e.IsAND === false; });
                }

                appService.getPickListByCustomViewId(viewId).then(function (data) {
                    var picklists = angular.fromJson(data);
                    filterService.setPickList(picklists);
                    processConditions(picklists);
                });
            }

            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
        });

        function processConditions(pickList) {
            $scope.listNames = pickList.PickList;
            //$scope.referenceLists = pickList.ReferenceList;
            processConditionTypes(tempAnd);
            $scope.andConditions = tempAnd;
            filterService.setAndConditions($scope.andConditions, toolbarService.getCurrentViewId());

            processConditionTypes(tempOr);
            $scope.orConditions = tempOr;
            filterService.setOrConditions($scope.orConditions, toolbarService.getCurrentViewId());

            $scope.isLoading = false;
        }

        //---listen to parent's event, then send the conditions to parent----------------
        $scope.$on('needConditionsEvent', function (event) {
            var filterConditions = filterService.getAllConditions(toolbarService.getCurrentViewId());
            filterConditions = _.filter(filterConditions, function (value) {
                return value.FilterValue !== null && (value.FilterValue !== '');
            });
            processMultiSelectValues(filterConditions);

            $scope.$emit('receivedConditionsEvent', filterConditions);
            resetMultiSelectFilter(filterConditions);
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
        });

        $scope.$on('saveFilterEvent', function (event) {
            var filterConditions = filterService.getAllConditions(toolbarService.getCurrentViewId());
            processMultiSelectValues(filterConditions);
            $scope.$emit('receivedSaveFilterEvent',
                _.filter(filterConditions, function (value) { return value.FilterValue !== null && value.FilterValue !== ''; })
            );
            resetMultiSelectFilter(filterConditions);
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
        });

        function processMultiSelectValues(conditions) {
            _.forEach(conditions, function (cd) {
                if (cd.Type == 'listbox') {
                    cd.FilterValue = cd.FilterValue.join(',');
                }
            });
        }

        function resetMultiSelectFilter(conditions) {
            _.forEach(conditions, function (cd) {
                if (cd.Type == 'listbox') {
                    cd.FilterValue = cd.FilterValue.split(',');
                }
            });
        }

        function getType(condition) {
            if (condition.FieldId > 0) {
                var type = findColumn(condition.FieldId);
                if (type.IsList == true) {                   
                    //if (type.ListNameId != null)
                    condition.ListValues = _.filter($scope.listNames, function (e) { return e.FieldName == type.ColumnName; });
                    //TODO: not use reference list in customview
                    //else if (type.ListSql)
                    //    condition.ListValues = _.filter($scope.referenceLists, function (e) { return e.FieldName == type.ColumnName; });
                    if ((condition.FilterValue == '' || condition.FilterValue < 1)
                        && condition.ListValues != null && condition.ListValues.length > 0)
                        condition.FilterValue = condition.ListValues[0].Id;

                    if (condition.ListValues == null || condition.ListValues.length == 0) {
                        return "text";
                    }                     

                    return "list";
                }

                if (type.IsCheckBox == true) {
                    condition.FilterValue = false;
                    return "check";
                }
                if (type.IsDecimal == true || type.IsInteger == true || type.IsCurrency == true) {
                    condition.FilterValue1 = condition.FilterValue = 0;
                    return "number";
                }
                if (type.IsDate == true || type.IsDateTime == true) {                    
                    if (condition.Operator != 'Within')
                        condition.FilterValue1 = condition.FilterValue = new Date();

                    return "date";
                }
                if (type.IsMultiSelecttBox == true) {
                    if (type.ListNameId != null)
                        condition.ListValues = _.filter($scope.listNames, function (e) { return e.ListNameId == type.ListNameId; });
                    else if (type.ListSql)
                        condition.ListValues = _.filter($scope.referenceLists, function (e) { return e.FieldId == type.FieldId; });
                    if (condition.FilterValue == '' && condition.ListValues != null && condition.ListValues.length > 0)
                        condition.FilterValue = [condition.ListValues[0].Id];
                    else {
                        if (condition.FilterValue.toString().indexOf(',') > 0)
                            condition.FilterValue = condition.FilterValue.split(',');
                        else
                            condition.FilterValue = '';
                    }
                    condition.Operator = 'Like';
                    return "listbox";
                }
                //condition.FilterValue = 'keyword';
                return "text";
            }
            return "text";
        }
        
        function getTypeForExistingConditions(condition) {            
            if (condition.FieldId > 0) {
                var type = findColumn(condition.FieldId);
                if (type.IsList == true) {
                    //if (type.ListNameId != null)
                    condition.ListValues = _.filter($scope.listNames, function (e) { return e.FieldName == type.ColumnName; });
                    //TODO: not use reference list in customview
                    //else if (type.ListSql)
                    //    condition.ListValues = _.filter($scope.referenceLists, function (e) { console.log(e); return e.FieldName == type.ColumnName; });
                    if ((condition.FilterValue == '' || condition.FilterValue < 1)
                        && condition.ListValues != null && condition.ListValues.length > 0)
                        condition.FilterValue = condition.ListValues[0].Id;

                    if (condition.ListValues == null || condition.ListValues.length == 0)
                        return "text";

                    return "list";
                }
                if (type.IsCheckBox == true) {
                    return "check";
                }
                if (type.IsDecimal == true || type.IsInteger == true || type.IsCurrency == true) {
                    return "number";
                }
                if (type.IsDate == true || type.IsDateTime == true) {
                    return "date";
                }
                if (type.IsMultiSelecttBox == true) {
                    if (type.ListNameId != null)
                        condition.ListValues = _.filter($scope.listNames, function (e) { return e.ListNameId == type.ListNameId; });
                    else if (type.ListSql)
                        condition.ListValues = _.filter($scope.referenceLists, function (e) { return e.FieldId == type.FieldId; });
                    if (condition.FilterValue == '' && condition.ListValues != null && condition.ListValues.length > 0)
                        condition.FilterValue = [condition.ListValues[0].Id];
                    else {
                        if (condition.FilterValue.toString().indexOf(',') > 0)
                            condition.FilterValue = condition.FilterValue.split(',');
                        else if(condition.FilterValue)
                            condition.FilterValue = [condition.FilterValue];
                            
                    }
                    condition.Operator = 'Like';
                    return "listbox";
                }
                return "text";
            }
            return "text";
        }

        function processConditionTypes(conditions) {
            _.forEach(conditions, function (cd) {
                cd.Type = getTypeForExistingConditions(cd);
            });
        }

        function findColumn(fieldId) {
            var returnCol = {};
            for (var i = 0; i < $scope.columns.length; i++) {
                if ($scope.columns[i].FieldId == fieldId) {
                    returnCol = $scope.columns[i];
                    break;
                }
            }
            return returnCol;
        };

        function findColumnByAliasName(name) {
            var returnCol = {};
            for (var i = 0; i < $scope.columns.length; i++) {
                if ($scope.columns[i].ColumnAlias == name) {
                    returnCol = $scope.columns[i];
                    break;
                }
            }
            return returnCol;
        };

        function initCondition() {
            //TODO: when create the new custom view
            //$timeout(function () {
            //    var context = requestContext.getRenderContext('dashboard.customviews');
            //    var section = context.getNextSection();
            //    if (section == 'add' && requestContext.getParamAsInt('viewId', 0) == 0) {
            //        filterService.setFields([]);
            //        filterService.setAndConditions([], toolbarService.getCurrentViewId());
            //        filterService.setOrConditions([], toolbarService.getCurrentViewId());
            //        $scope.andConditions = $scope.orConditions = $scope.columns = [];
            //        $scope.isLoading = false;
            //    }
            //    else if (section == null || section != 'add' || requestContext.getParamAsInt('viewId', 0) == 0) {
            //        $scope.columns = filterService.getFields(toolbarService.getCurrentModuleId());
            //        $scope.columns = _.filter($scope.columns, function (e) { return e.AdvanceSearch == true; });
            //        $scope.andConditions = filterService.getAndConditions(toolbarService.getCurrentViewId());
            //        $scope.orConditions = filterService.getOrConditions(toolbarService.getCurrentViewId());
            //        $scope.isLoading = false;
            //    }
            //}, 500);
        }

        //-----datepicker stuffs-------------------
        $scope.today = function () {
            $scope.filterValue = new Date();
        };
        //$scope.today();

        $scope.showWeeks = false;
        $scope.toggleWeeks = function () {
            $scope.showWeeks = !$scope.showWeeks;
        };

        $scope.clear = function () {
            $scope.filterValue = null;
        };

        // Disable weekend selection
        $scope.disabled = function (date, mode) {
            return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
        };

        $scope.toggleMin = function () {
            $scope.minDate = ($scope.minDate) ? null : new Date();
        };
        $scope.toggleMin();

        $scope.open = function () {
            $timeout(function () {
                $scope.opened = true;
            });
        };

        $scope.open1 = function () {
            $timeout(function () {
                $scope.opened1 = true;
            });
        };

        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };

        initCondition();
    }
})();