(function (ng) {

    "use strict";

    angular.module("LeonardCRM").controller("GridCtrl", ctrl);

    ctrl.$inject = ["$scope", "$filter", "$timeout", "$compile", "$sce", "appService", "registryService", "requestContext", "toolbarService", "filterService", "viewService", "_"];

    function ctrl($scope, $filter, $timeout, $compile, $sce, appService, registryService, requestContext, toolbarService, filterService, viewService, _) {
        var divContent;
        var divContainer;

        // --- Define Controller Methods. ------------------- //

        var toHtml = function (str) {
            var output = '';
            if (str != null) {
                var pattern = '\n';
                var reg = new RegExp(pattern, 'g');
                output = str.replace(reg, '<br/>');
            }
            return output;
        };

        var convertViewColumn = function () {
            $scope.personalViewColumns = [];
            var viewCol = {};
            if ($scope.hasIdColumn) {
                angular.forEach($scope.viewColumnName, function (colName, index) {
                    viewCol = {
                        Width: colName.Width,
                        Id: colName.Id,
                        FieldId: colName.FieldId,
                        SortOrder: index + 1
                    };
                    $scope.personalViewColumns.push(viewCol);
                });
            } else {
                angular.forEach($scope.viewColumnName, function (colName, index) {
                    viewCol = {
                        Width: colName.Width,
                        Id: colName.Id,
                        FieldId: colName.FieldId, SortOrder: index + 1
                    };
                    $scope.personalViewColumns.push(viewCol);
                });
            }
        };
        var savePersonalViewEvent = function (isCreateNewView) {
            convertViewColumn();// process column: convert viewcolumnDataType to ViewColumn

            // GroupBy State
            var colnameGroup = _.filterWithProperty($scope.columnObjs, 'ColumnName', $scope.ColumnNameGroup);
            if (colnameGroup.length > 0) {
                colnameGroup = { ViewId: 0, ColumnName: $scope.ColumnNameGroup, FieldId: colnameGroup[0].FieldId };
            }
            // OrderBy State
            var nameArray = $scope.pageInfoGrid.SortExpression.split(/\s+/);// white space
            var colname = nameArray[0];
            var direction = nameArray[1];
            var colNameOrderBy = { ViewId: 0, ColumnName: colname, FieldId: 0, OrderDirection: direction };
            var orderColumn = _.filterWithProperty($scope.columnObjs, 'ColumnName', colname);
            if (orderColumn.length > 0) {
                colNameOrderBy.FieldId = orderColumn[0].FieldId;
            }
            var persionView = {};
            if (isCreateNewView) {
                $('#dialog-viewname').dialog({
                    title: $scope.languages.VIEW.VIEW_TITLE_DIALOG,
                    autoOpen: false,
                    resizable: false,
                    height: 'auto',
                    show: { effect: 'drop', direction: "up" },
                    modal: true,
                    draggable: true,
                    closeOnEscape: false,
                    width: 400,
                    close: function (evt, ui) {
                        //event handler for Esc pressed
                    },
                    open: function (evt, ui) {
                    },
                    buttons:
                    [
                        {
                            text: $scope.languages.COMMON.SAVE_BUTTON,
                            click: function () {
                                if ($scope.viewName != null && $scope.viewName.length > 0) {
                                    persionView = {
                                        Id: $scope.pageInfoGrid.ViewId,
                                        ViewName: $scope.viewName,
                                        ModuleId: $scope.ModuleId,
                                        Eli_ViewConditions: filterService.getAllConditions($scope.pageInfoGrid.ViewId),
                                        Eli_ViewColumns: $scope.personalViewColumns,
                                        Eli_ViewGroupBy: [],
                                        Eli_ViewOrderBy: [],
                                        PageSize: $scope.pageInfoGrid.PageSize
                                    };
                                    persionView.Eli_ViewGroupBy.push(colnameGroup);
                                    persionView.Eli_ViewOrderBy.push(colNameOrderBy);

                                    viewService.savePersonalView(persionView, isCreateNewView, $scope.ModuleId).then(function (data) {
                                        if (data.ReturnCode == 200) {
                                            NotifySuccess(data.Result);
                                            $scope.viewName = '';
                                            $scope.refreshViewMenu();
                                            $('#dialog-viewname').dialog("close");
                                        } else
                                            NotifyError(data.Result);
                                    });
                                } else {
                                    NotifyError($scope.languages.VIEW.NAME_REQUIRED);
                                }
                            }
                        },
                        {
                            text: $scope.languages.COMMON.CANCEL_BUTTON,
                            click: function () {
                                $scope.$apply(function () {
                                    $scope.viewName = '';
                                });

                                $(this).dialog('close');
                            }
                        }
                    ],
                    position: ['top', 5],
                    dialogClass: 'no-close'
                });
                $('.ui-dialog-buttonset button').addClass(function (index) {
                    return "btn btn-sm label-primary";
                });
                $('#dialog-viewname').dialog('open');
                if ($scope.countViewName == 0) {
                    $scope.countViewName += 1;
                    var divTemple = '<div></div>';
                    var divEl = angular.element(divTemple);
                    divContent = $('#viewNameContent');
                    divEl.load('/appviews/Layouts/viewname-dialog.html', function () {
                        divContainer = angular.element(divContent);
                        var html = $compile(divEl)($scope);
                        $timeout(function () {
                            divContainer.html(html);
                            divContent.fadeIn();
                        }, 500);
                    });
                }
            } else {
                persionView = {
                    Id: $scope.pageInfoGrid.ViewId,
                    ViewName: $scope.viewName,
                    ModuleId: $scope.ModuleId,
                    Eli_ViewConditions: filterService.getAllConditions($scope.pageInfoGrid.ViewId),
                    Eli_ViewColumns: $scope.personalViewColumns,
                    Eli_ViewGroupBy: [],
                    Eli_ViewOrderBy: [],
                    PageSize: $scope.pageInfoGrid.PageSize
                };
                // Check null
                if (colnameGroup.length > 0) {
                    colnameGroup.ViewId = $scope.pageInfoGrid.ViewId;
                }
                // group by
                persionView.Eli_ViewGroupBy.push(colnameGroup);
                // order by
                colNameOrderBy.ViewId = $scope.pageInfoGrid.ViewId;
                persionView.Eli_ViewOrderBy.push(colNameOrderBy);

                viewService.savePersonalView(persionView, isCreateNewView, $scope.ModuleId).then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result);
                        $scope.viewName = '';
                        $scope.refreshViewMenu();
                    } else
                        NotifyError(data.Result);
                });
            }
            return false;
        };
        // --- Define Scope Methods. ------------------------ //

        $scope.GetListNameById = function () {
            var fields = filterService.getFields($scope.pageInfoGrid.ModuleId);
            if ($scope.pageInfoGrid.Id > 0 && fields.length == 0) {
                filterService.preLoadFilterColumnsAndPickLists($scope.pageInfoGrid.ModuleId);
            } else {
                $scope.pickList.PickList = filterService.getPickList($scope.pageInfoGrid.ModuleId);
                $scope.pickList.ReferenceList = filterService.getReferenceList($scope.pageInfoGrid.ModuleId);
                $scope.setPickList();
            }
        };

        $scope.getColor = function (text) {
            if (text)
                return text.split('@@')[1];
            return 'none';
        };

        $scope.getColumnStype = function (index) {
            return {
                'width': $scope.viewColumnName[index].Width
            };
        };
        $scope.getListColumnStype = function (index, text) {
            return {
                'background-color': $scope.getColor(text),
                'width': $scope.viewColumnName[index].Width
            };
        };

        $scope.getListDescription = function (text) {
            if (text)
                return text.split('@@')[0];
            return '';
        };

        $scope.RowClick = function (moduleId, viewId, rowId, moduleName, columnName, isCommand) {
            $scope.args = {
                ModuleName: moduleName,
                ModuleId: moduleId,
                ViewId: viewId,
                RowId: rowId,
                Key: $scope.key,
                ParentId: $scope.pageInfoGrid.Id,
                ColumnName: columnName,
                IsCommand: isCommand
            };
            $scope.$emit('rowClickEvent', $scope.args);
        };

        $scope.RowHover = function (moduleId, viewId, rowId, moduleName, columnName, isCommand) {
            $scope.args = {
                ModuleName: moduleName,
                ModuleId: moduleId,
                ViewId: viewId,
                RowId: rowId,
                ColumnName: columnName,
                IsCommand: isCommand
            };
            $scope.$emit('rowHoverEvent', $scope.args);
        };

        $scope.GroupBy_Changed = function () {
            $scope.pageInfoGrid.GroupColumn = $scope.ColumnNameGroup;
            if ($scope.ColumnNameGroup == null) {
                $scope.pageInfoGrid.GroupColumn = '';
            }
            $scope.setViewInfo();
            $scope.$emit('Grid_OnNeedDataSource', $scope.pageInfoGrid);
        };

        $scope.removeStatusString = function (str) {
            var index = str.indexOf('@@');
            if (index > 0) {
                str = str.substr(0, index);
            }
            if (str != null && str.length > 0 && str != 'null') {
                if ($scope.columnObj[0].IsDate) {
                    index = Date.parse(str);
                    if (index > 0) {
                        str = $filter('date')(str, $scope.dateFormat);
                    }
                    return str;
                }
                if ($scope.columnObj[0].IsDateTime) {
                    index = Date.parse(str);
                    if (index > 0) {
                        str = $filter('date')(str, $scope.dateTimeFormat);
                    }
                    return str;
                }
                return str;
            }
            else {
                if ($scope.columnObj[0].IsCheckBox) {
                    return "false";
                }
                return $scope.languages.GRID_VIEW.EMPTY;
            }
        };

        $scope.RegisterGroupBy = function () {
            $scope.columnObjs = filterService.getFields($scope.pageInfoGrid.ModuleId);

            if ($scope.ColumnNameGroup != null && $scope.ColumnNameGroup.length > 0 && $scope.pageInfoGrid.Id == 0) {
                $scope.uiDatasource = [];
                $scope.columnObj = _.where($scope.columnObjs, { 'ColumnName': $scope.ColumnNameGroup });
                var isDatetime = false;
                if ($scope.columnObj.length > 0 && ($scope.columnObj[0].IsDateTime || $scope.columnObj[0].IsDate)) {
                    isDatetime = true;
                    $scope.dataGroupSource = _.groupBy($scope.datasource, function (item) {
                        return $filter('date')(new Date(item[$scope.ColumnNameGroup]), $scope.dateFormat);
                    });
                } else {
                    $scope.dataGroupSource = _.groupBy($scope.datasource, $scope.ColumnNameGroup);
                }
                var flag = false;
                var objKeys = Object.keys($scope.dataGroupSource);

                for (var i = 0; i < objKeys.length; i++) {
                    if (objKeys[i] == '') {
                        objKeys[i] = 'null';
                        flag = true;
                        break;
                    }
                }

                if (flag) {
                    $scope.dataGroupSource['null'] = $scope.dataGroupSource[''];
                    delete $scope.dataGroupSource[''];
                }
                var groupResult = JSON.parse($scope.pageInfoGrid.GroupResult.toString());
                $scope.titleGroups = [];
                angular.forEach(groupResult, function (value, index) {
                    if (isDatetime) {
                        value.GroupName = $filter('date')(new Date(value.GroupName), $scope.dateFormat).toString();
                    }
                });
                for (var j = 0; j < objKeys.length; j++) {
                    var res = _.findWithProperty(groupResult, 'GroupName', objKeys[j]);
                    if (res != null) {
                        $scope.titleGroups.push({ GroupName: res.GroupName, IsCollapsed: false, ServerTotal: res.Total });
                    } else {
                        $scope.titleGroups.push({ GroupName: objKeys[j], IsCollapsed: false, ServerTotal: $scope.dataGroupSource[objKeys[j]].length });
                    }
                }
                $scope.ShowPanelGroup = true;
            } else {
                $scope.ShowPanelGroup = false;
                $scope.uiDatasource = angular.copy($scope.datasource);
            }
        };

        $scope.sortGrid = function (columnName, moduleName, moduleId, viewId) {
            $scope.reverse = !$scope.reverse;
            $scope.pageInfoGrid.ModuleName = moduleName;
            $scope.pageInfoGrid.ModuleId = moduleId;
            $scope.pageInfoGrid.ViewId = viewId;

            if ($scope.reverse) {
                $scope.pageInfoGrid.SortExpression = columnName + ' Asc';
            } else {
                $scope.pageInfoGrid.SortExpression = columnName + ' Desc';
            }
            $scope.$broadcast('clearKeyEVent', $scope.key);
            $scope.pageInfoGrid.Models = {};
            $scope.pageInfoGrid.DefaultOrderBy = false;

            $scope.setViewInfo();

            $scope.$emit('sortEvent', $scope.pageInfoGrid);
        };

        $scope.Grid_Init = function (moduleId, viewId, moduleName, id, viewColumnName, columnNameGroup, sortExpression, pageSize, isShareView) {
            $scope.pageInfoGrid.ModuleId = moduleId;
            $scope.pageInfoGrid.ViewId = viewId;
            $scope.pageInfoGrid.ModuleName = moduleName;
            $scope.pageInfoGrid.Id = id;
            $scope.viewColumnName = angular.fromJson(viewColumnName);
            $scope.pageInfoGrid.SortExpression = sortExpression;
            $scope.pageInfoGrid.PageSize = pageSize;

            $scope.GridKey = 'grid' + moduleId.toString() + viewId.toString() + id.toString();

            angular.forEach($scope.viewColumnName, function (obj, index) {
                if (obj.ColumnName == 'Id') {
                    $scope.hasIdColumn = true;
                }
            });
            if ($scope.hasIdColumn == false) {
                $scope.GridWidthColumns.push('0px');
            }

            if (isShareView == 'True') {
                $scope.IsPublicView = true;

            } else {
                $scope.pageInfoGrid.PageSize = pageSize;
            }

            $scope.pageInfoGrid.ViewColumns = angular.copy($scope.viewColumnName);

            var fields = filterService.getFields(moduleId);
            $scope.Fields = _.filter(fields, { 'AllowGroup': true });;
            var field = _.findWithProperty($scope.Fields, 'ColumnName', columnNameGroup);
            columnNameGroup = field != undefined ? columnNameGroup : '';
            $scope.pageInfoGrid.GroupColumn = columnNameGroup;
            $scope.ColumnNameGroup = columnNameGroup;
            if (id == 0) {
                viewService.checkViewInfo($scope.pageInfoGrid.ViewId, $scope.pageInfoGrid.ModuleId);
                filterService.setCurrentGridColumns($scope.viewColumnName);
                if (viewService.getChangeColumnState() == true) {
                    var viewInfo = viewService.getViewInfo();
                    $scope.pageInfoGrid.PageIndex = viewInfo.PageIndex;
                    $scope.pageInfoGrid.PageSize = viewInfo.PageSize;
                    $scope.pageInfoGrid.SortExpression = viewInfo.SortExpression;
                    $scope.pageInfoGrid.GroupColumn = viewInfo.GroupColumn;
                    $scope.ColumnNameGroup = viewInfo.GroupColumn;

                    // Get Filter Conditions & Set Gridview
                    var filterConditions = filterService.getFilterConditions($scope.pageInfoGrid.ViewId);
                    angular.forEach(filterConditions, function (item, index) {
                        $scope.localFilterObj[item.ColumnName] = item.FilterValue;
                    });

                } else {
                    $scope.setViewInfo();
                }
            }
            $scope.$emit('Grid_OnNeedDataSource', $scope.pageInfoGrid);
        };

        $scope.addNew = function (moduleName) {
            var obj = {
                ModuleName: moduleName,
                ParentId: $scope.pageInfoGrid.Id,
                ViewId: $scope.pageInfoGrid.ViewId,
                ModuleId: $scope.pageInfoGrid.ModuleId,
                Key: $scope.key
            };
            $scope.$emit('Grid_AddNewClicked', obj);
        };

        //nameCss : item name in datalist. Ex: Order50 in datalist
        //id : item.Id
        $scope.LoadChildView = function (nameCss, id, viewId, moduleId, moduleName) {
            //Declare values
            var buttonEl = '.' + nameCss + id + 'bt > i';
            var keyStr = nameCss + id;
            var domCss = '.' + nameCss + id;
            var divTemple = '<div id="' + keyStr + '" class="childgridview"></div>';
            var divEl = angular.element(divTemple);
            var indexKey = 0;

            //Check key is existed
            angular.forEach($scope.keys, function (value, index) {
                if (keyStr == value.Key) {
                    $scope.isExist = true;
                    indexKey = index;
                }
            });

            if ($scope.isExist) {
                if ($scope.keys[indexKey].IsExpanded) {
                    //Is Shown
                    $(domCss).stop().slideUp('fast', function () {
                        $(buttonEl).removeClass('glyphicon-chevron-down');
                        $(buttonEl).addClass('glyphicon-chevron-right');
                    });
                } else {
                    $(domCss).stop().slideDown('fast', function () {
                        $(buttonEl).removeClass('glyphicon-chevron-right');
                        $(buttonEl).addClass('glyphicon-chevron-down');
                    });
                }
                $scope.keys[indexKey].IsExpanded = !$scope.keys[indexKey].IsExpanded;
            } else {

                //Load Grid Template
                divEl.load('/Home/SubView/' + viewId + '/' + moduleId + '/' + id, function () {
                    divContainer = angular.element($(domCss));
                    var html = $compile(divEl)($scope);
                    divContainer.append(html);

                    //firing an event after grid template loaded.
                    var pageInfoChildGrid = {
                        ModuleId: moduleId,
                        ViewId: viewId,
                        Id: id,
                        ModuleName: moduleName,
                        SortExpression: 'Id Asc'
                    };
                    $scope.$emit('LoadChildGrid', pageInfoChildGrid);
                });

                //Expanding Grid
                $(domCss).stop().slideDown('fast', function () {
                    $(buttonEl).removeClass('glyphicon-chevron-right');
                    $(buttonEl).addClass('glyphicon-chevron-down');
                });

                //Add Key
                var objKey = {
                    Key: keyStr,
                    IsExpanded: true
                };
                $scope.keys.push(objKey);
            }


            //Reset Values
            $scope.isExpanded = false;
            $scope.isExist = false;
        };

        $scope.checkChanges = function (index) { };

        $scope.bindTrustHtml = function (str) {
            str = toHtml(str);
            return $sce.trustAsHtml(str);
        };
        $scope.serverConditions = [];

        $scope.createServerConditions = function () {
            var keyObjs = Object.keys($scope.localFilterObj);
            $scope.serverConditions = [];
            angular.forEach(keyObjs, function (key, index) {
                if ($scope.localFilterObj[key] != null && $scope.localFilterObj[key] != '' && key != 'Collection') {

                    var objValue = $scope.localFilterObj[key];

                    var obj = {
                        ViewId: $scope.pageInfoGrid.ViewId,
                        ColumnName: key,
                        IsAND: true,
                        FilterValue: objValue
                    };

                    //if (angular.isArray(objValue)) {
                    //    obj.FilterValue = objValue.join(',');
                    //}

                    $scope.serverConditions.push(obj);
                }
            });
            filterService.setFilterCondition($scope.serverConditions, $scope.pageInfoGrid.ViewId);
        };

        $scope.serverFilter = function (isSubview) {
            $scope.createServerConditions();
            $scope.pageInfoGrid.PageIndex = 1;
            if (!isSubview) {
                $scope.$emit('ServerFilter', $scope.pageInfoGrid);
            }
            else {
                var objArgs = {
                    conditions: $scope.serverConditions,
                    Id: $scope.pageInfoGrid.Id,
                    ViewId: $scope.pageInfoGrid.ViewId,
                    ModuleId: $scope.pageInfoGrid.ModuleId
                };
                $scope.$emit('ServerFilter', objArgs);
            }
        };

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
            $scope.loadedPickList = true;
        };
        $scope.resetServerFilter = function () {
            var keyObjs = Object.keys($scope.localFilterObj);
            angular.forEach(keyObjs, function (key, index) {
                if ($scope.localFilterObj[key] != null && $scope.localFilterObj[key] != '' && key != 'Collection') {
                    $scope.localFilterObj[key] = '';
                }
            });
            $scope.pageInfoGrid.PageIndex = 1;
        };
        $scope.saveView = function () {
            savePersonalViewEvent(false);
        };

        $scope.createView = function () {
            savePersonalViewEvent(true);
        };
        $scope.addColumnsArray = function (l) {
            $scope.GridWidthColumns.push(l);
        };

        $scope.init = function () {

        };
        $scope.initDragDrop = function () {
            $scope.$broadcast('initDragDrop', true);
        };
        $scope.setViewInfo = function () {
            if ($scope.pageInfoGrid.Id == 0) {
                viewService.setViewInfo({
                    'ModuleId': $scope.pageInfoGrid.ModuleId,
                    'ViewId': $scope.pageInfoGrid.ViewId,
                    'PageIndex': $scope.pageInfoGrid.PageIndex,
                    'PageSize': $scope.pageInfoGrid.PageSize,
                    'SortExpression': $scope.pageInfoGrid.SortExpression,
                    'GroupColumn': $scope.pageInfoGrid.GroupColumn
                });
            }
        };



        // --- Define Scope Variables. ---------------------- //

        $scope.needToResetPageIndex = true;
        $scope.Fields = [];
        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };
        $scope.dateFormat = registryService.siteSettings.DATE_FORMAT;
        $scope.currencyFormat = registryService.siteSettings.CURRENCY;
        $scope.dateTimeFormat = registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT;
        $scope.AllSelectedItems = false;
        $scope.NoSelectedItems = true;
        $scope.key = '';
        $scope.keys = [];
        $scope.predicate = '';
        $scope.reverse = true;
        $scope.datasource = [];
        $scope.uiDatasource = [];
        $scope.isExpanded = false;
        $scope.isExist = false;
        $scope.viewColumnName = [];
        $scope.ShowPanelGroup = false;
        $scope.ColumnNameGroup = '';
        $scope.showBodyTable = false;
        $scope.dataGroupSource = [];
        $scope.titleGroups = [];
        $scope.columnObjs = [];
        $scope.columnObj = {};
        $scope.hasIdColumn = false; // have the display Id column or no
        $scope.viewName = '';
        $scope.localFilterObj = {
            Collection: {}
        };
        $scope.GridKey = '';
        $scope.GridWidthColumns = [];// 1 string array save width of the columns in gridview
        $scope.width = 0; // Variables used to calculate the width of the grid
        $scope.widthDefault = 0; // Save the width of the initial value
        $scope.tableStyle = {
            'width': '100%',
            'background-color': '#fff',
            'overflow': 'visible',
            'margin-right': '10px'
        };
        $scope.isResing = false;
        $scope.IsPublicView = false;
        $scope.viewSource = [];
        $scope.countViewName = 0;
        $scope.personalViewColumns = [];

        $scope.serverConditions = [];

        $scope.pageInfoGrid = {
            ViewId: 1,
            ModuleId: 1,
            ModuleName: '',
            SortExpression: 'Id Asc',
            PageIndex: 1,
            PageSize: registryService.siteSettings.ITEMS_PER_PAGE,
            Id: 0,
            RoleId: 0,
            AdvanceSearch: false,
            totalRow: 0,
            Models: {},
            DefaultOrderBy: true,
            ViewColumns: []
        };
        $scope.pickList = {
            PickList: [],
            ReferenceList: []
        };
        $scope.loadedPickList = false;
        $scope.resoption = {
            disabled: false
        };
        $scope.isResing = false;
        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('exportCsvEvent', function (event) {
            if ($scope.pageInfoGrid.Models == null || $scope.pageInfoGrid.Models[$scope.pageInfoGrid.Models.module].length < 1) {
                NotifyError($scope.languages.GRID_VIEW.NO_DATA_TO_EXPORT);
            } else {
                $scope.pageInfoGrid.ViewColumns = angular.copy($scope.viewColumnName);
                $scope.$emit('exportCsvInfoEvent', $scope.pageInfoGrid);
            }
            event.preventDefault();
        });

        $scope.$on('checkAllEvent', function (event, args) {
            $scope.AllSelectedItems = args.AllSelectedItems;
            $scope.NoSelectedItems = args.NoSelectedItems;
            if ($scope.ShowPanelGroup) {
                $scope.AddEditValue($scope.datasource);
            } else {
                $scope.AddEditValue($scope.uiDatasource);
            }

            toolbarService.ItemSelected(!$scope.NoSelectedItems);
            event.preventDefault();
        });

        $scope.$on('Grid_DataBinding', function (event, args) {
            var checkKey = 'grid' + args.ModuleId.toString() + args.ViewId.toString() + args.Id.toString();
            if ($scope.GridKey == checkKey) {
                $scope.pageInfoGrid = args;
                $scope.key = $scope.pageInfoGrid.Models.module;
                $scope.datasource = $scope.pageInfoGrid.Models[$scope.key];
                $scope.RegisterGroupBy();
                $scope.keys = [];
                $scope.initDragDrop();

                //checking Create View function allowed or not
                var module = appService.getModuleById($scope.pageInfoGrid.ModuleId);
                $scope.AllowCreateView = module.AllowCreateView;

                if ($scope.loadedPickList == false) {
                    $scope.GetListNameById();
                }
            }

        });

        $scope.$on('PageIndexChanged', function (event, args) {
            if ($scope.key == args.Models.module) {
                args.Models[args.Models.module] = [];
                $scope.pageInfoGrid.PageIndex = args.PageIndex;
                $scope.pageInfoGrid.PageSize = args.PageSize;
                $scope.setViewInfo();
                if ($scope.pageInfoGrid.AdvanceSearch == true) {
                    $scope.$emit('GridPageIndexChanged', $scope.pageInfoGrid);
                } else {
                    $scope.$emit('Grid_OnNeedDataSource', $scope.pageInfoGrid);
                }
            }
        });

        $scope.$on('clearKeyEVent', function (event, args) {
            if ($scope.key != args) {
                $scope.key = '';
            }
            $scope.keys = [];
        });

        $scope.$on('refeshDataByKey', function (event, args) {

            if ($scope.key == args) {
                // handle event only if it was not defaultPrevented
                if (event.defaultPrevented) {
                    return;
                }
                // mark event as "not handle in children scopes"
                event.preventDefault();
                //$scope.pageInfoGrid.PageIndex = 1;
                //$scope.pageInfoGrid.AdvanceSearch = false;
                $scope.needToResetPageIndex = false;
                $scope.$emit('Grid_OnNeedDataSource', $scope.pageInfoGrid);

            }
        });

        $scope.$on('filterPickListEvent', function (event, args) {
            if ($scope.loadedPickList == false && $scope.pageInfoGrid.ModuleId == args) {
                $scope.pickList.PickList = filterService.getPickList($scope.pageInfoGrid.ModuleId);
                $scope.pickList.ReferenceList = filterService.getReferenceList($scope.pageInfoGrid.ModuleId);
                $scope.setPickList();
            }
        });

        $scope.$on('exportAllDataCsvEvent', function (event) {
            $scope.pageInfoGrid.ViewColumns = angular.copy($scope.viewColumnName);
            $scope.$emit('exportAllDataCsvInfoEvent', $scope.pageInfoGrid);
        });

        $scope.$on('refreshEvent', function (event) {
            $scope.resetServerFilter();
        });

        $scope.$on('ColumnReize', function (event, args) {
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();

            $scope.tableStyle.width = '9999px'; // preventing cell new line when column resize
            $scope.tableStyle.overflow = 'hidden';
            $scope.resizeEnabled = true;
        });


        $scope.$on('ColumnReizeStop', function (event, args) {
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
            $scope.isResing = false;
            $scope.resizeEnabled = false;
            var sum = 30;
            angular.forEach($scope.viewColumnName, function (column, index) {
                var columnW = column.Width.substr(0, column.Width.length - 2);
                sum += parseInt(columnW);
            });
            sum += 20;
            $scope.tableStyle.width = sum.toString() + 'px';
            $scope.tableStyle.overflow = 'visible';
            filterService.setCurrentGridColumns($scope.viewColumnName);
        });
        $scope.$on('ColumnReizeStart', function (event, args) {
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
            $scope.isResing = true;
        });

        $scope.$on('DivWidth', function (event, args) {

            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
            $scope.widthDefault = angular.copy(args);

            $scope.tableStyle.width = args.toString() + 'px';
            var sum = 30; // 30px is checkbox column
            $scope.width = args - sum;
            var columnwidth = Math.round($scope.width / $scope.viewColumnName.length);
            // check total width
            var total = (columnwidth * $scope.viewColumnName.length) + 30;
            if ($scope.widthDefault < total) {
                columnwidth -= 1; // minus 1px
            }

            var changeColumnState = viewService.getChangeColumnState();
            // update each column in gridview
            if ($scope.IsPublicView == true && changeColumnState == false) {

                // If there are more than 12 columns, set 100px as default for all columns.
                if ($scope.viewColumnName.length >= 12) {
                    columnwidth = '100';
                    sum += ($scope.viewColumnName.length * 100);
                    sum += 20;
                    $scope.tableStyle.width = sum.toString() + 'px';
                }
                angular.forEach($scope.viewColumnName, function (value, index) {
                    value.Width = columnwidth.toString() + 'px';
                });
            } else {
                angular.forEach($scope.viewColumnName, function (column, index) {
                    var wStr = column.Width || '100px';
                    var w = wStr.substr(0, wStr.length - 2);
                    sum += parseInt(w);
                });
                sum += 10;
                $scope.tableStyle.width = sum.toString() + 'px';
            }
            filterService.setCurrentGridColumns($scope.viewColumnName);
        });

        $scope.$on('collectConditionsEvent', function (event) {
            $scope.createServerConditions();
            if ($scope.needToResetPageIndex) {
                $scope.pageInfoGrid.PageIndex = 1;
            } else {
                $scope.needToResetPageIndex = true;
            }
        });

        $scope.$on('requestContextChanged', function () {
            viewService.setChangeColumnsState(false);
            if ($scope.countViewName > 0) {
                $('#dialog-viewname').dialog('destroy');
            }
        });

        // --- Initialize. ---------------------------------- //
        $scope.init();
        $scope.isChangeTemplate = false;
        $scope.$on("dragdropcompleted", function (event, args) {
            var from = args.from;
            var to = args.to;
            var headerBefore = angular.copy($scope.viewColumnName);
            if (from !== to) {
                $scope.isChangeTemplate = true;
                $(".dynamicClass").removeAttr("ng-include");
                $scope.reRenderTemplate(from, to, headerBefore);
            } else {
                $scope.isChangeTemplate = false;
            }

        });
        $scope.gridTemplate = '';
        $scope.reRenderTemplate = function (from, to, headerBefore) {
            if ($scope.gridTemplate != '') {
                swapAllNodes($scope.gridTemplate, from, to, headerBefore);
                updateHeader(from, to);
                updateTemplate();
                $scope.$broadcast("hideLoadingDragDrop");
            } else {
                if ($scope.isChangeTemplate) {
                    $.ajax({
                        type: "GET",
                        async: "false",
                        url: '/home/dynamicgrid/' + $scope.pageInfoGrid.ViewId + "/" + $scope.pageInfoGrid.ModuleId + "/?t=" + Math.random(),
                        data: "",
                        success: function (html) {
                            var htmlContent = $(html).find(".contentDragDrop"); // a response via AJAX containing HTML
                            $scope.gridTemplate = htmlContent;
                            swapAllNodes($scope.gridTemplate, from, to, headerBefore);
                            updateHeader(from, to);
                            updateTemplate();
                            $scope.$broadcast("hideLoadingDragDrop");
                        }
                    });
                }
            }
        };
        function swapAllNodes(htmlContent, from, to, headerBefore) {
            var fIndex = 0;
            var toIndex = 0;
            var fromCell;
            var toCell;
            var j;
            var i;
            var wFrom;
            var wTo;
            if (from < to) {
                for (i = from; i < to; i++) {
                    fIndex = i;
                    toIndex = i + 1;
                    fromCell = $(htmlContent).find('div.gridlocalfilter>div.divcol:nth-child(' + fIndex + ')');
                    toCell = $(htmlContent).find('div.gridlocalfilter>div.divcol:nth-child(' + toIndex + ')');
                    wFrom = headerBefore[fIndex - 2].Width;
                    wTo = headerBefore[fIndex - 1].Width;
                    for (j = 0; j < fromCell.length; j++) {
                        swapNodes1(fromCell[j], toCell[j], wFrom, wTo, true);
                    }
                    //For group
                    fromCell = $(htmlContent).find('div.dataRowGroup>a.rowlink:nth-child(' + fIndex + ')');
                    toCell = $(htmlContent).find('div.dataRowGroup>a.rowlink:nth-child(' + toIndex + ')');
                    for (j = 0; j < fromCell.length; j++) {
                        swapNodes1(fromCell[j], toCell[j], wFrom, wTo, true);
                    }

                    ////For not group
                    fromCell = $(htmlContent).find('div.dataRowNoGroup>a.rowlink:nth-child(' + fIndex + ')');
                    toCell = $(htmlContent).find('div.dataRowNoGroup>a.rowlink:nth-child(' + toIndex + ')');
                    for (j = 0; j < fromCell.length; j++) {
                        swapNodes1(fromCell[j], toCell[j], wFrom, wTo, true);
                    }
                }
            } else {
                for (i = from; i > to; i--) {
                    fIndex = i;
                    toIndex = i - 1;
                    fromCell = $(htmlContent).find('div.gridlocalfilter>div.divcol:nth-child(' + fIndex + ')');
                    toCell = $(htmlContent).find('div.gridlocalfilter>div.divcol:nth-child(' + toIndex + ')');
                    wFrom = headerBefore[fIndex - 2].Width;
                    wTo = headerBefore[toIndex - 1].Width;
                    for (j = 0; j < fromCell.length; j++) {
                        swapNodes1(fromCell[j], toCell[j], wFrom, wTo, false);
                    }
                    ////For group
                    fromCell = $(htmlContent).find('div.dataRowGroup>a.rowlink:nth-child(' + fIndex + ')');
                    toCell = $(htmlContent).find('div.dataRowGroup>a.rowlink:nth-child(' + toIndex + ')');
                    for (j = 0; j < fromCell.length; j++) {
                        swapNodes1(fromCell[j], toCell[j], wFrom, wTo, false);
                    }

                    //For not group
                    fromCell = $(htmlContent).find('div.dataRowNoGroup>a.rowlink:nth-child(' + fIndex + ')');
                    toCell = $(htmlContent).find('div.dataRowNoGroup>a.rowlink:nth-child(' + toIndex + ')');
                    for (j = 0; j < fromCell.length; j++) {
                        swapNodes1(fromCell[j], toCell[j], wFrom, wTo, false);
                    }
                }
            }


        }
        function swapNodes1(a, b, wfrom, wto, flag) {
            var atr1 = $(a).attr("ng-style");
            var atr2 = $(b).attr("ng-style");
            var color1 = $(a).css("background-color");
            var color2 = $(b).css("background-color");

            $(a).removeAttr("style");
            $(b).removeAttr("style");


            var fW = "width:" + wfrom + ";background-color:" + color1;
            var tW = "width:" + wto + ";background-color:" + color2;
            $(a).attr("style", fW);
            $(b).attr("style", tW);


            if (flag) //if flag=true; from<to
            {
                if (atr1.indexOf('getListColumnStype') > -1 && atr2.indexOf('viewColumnName') > -1) {
                    atr1 = adjustGetListColumnStype(atr1, true);
                    atr2 = adjustViewColumnWidth(atr2, false);
                }
                if (atr1.indexOf('viewColumnName') > -1 && atr2.indexOf('getListColumnStype') > -1) {
                    atr1 = adjustViewColumnWidth(atr1, true);
                    atr2 = adjustGetListColumnStype(atr2, false);

                }
                if (atr1.indexOf('getListColumnStype') > -1 && atr2.indexOf('getListColumnStype') > -1) {
                    atr1 = adjustGetListColumnStype(atr1, true);
                    atr2 = adjustGetListColumnStype(atr2, false);
                }

                if (atr1.indexOf('getListColumnStype') > -1 && atr2.indexOf('getColumnStype') > -1) {
                    atr1 = adjustGetListColumnStype(atr1, true);
                    atr2 = adjustGetColumnStype(atr2, false);
                }
                if (atr1.indexOf('getColumnStype') > -1 && atr2.indexOf('getListColumnStype') > -1) {
                    atr1 = adjustGetColumnStype(atr1, true);
                    atr2 = adjustGetListColumnStype(atr2, false);
                }

                if (atr1.indexOf('getColumnStype') > -1 && atr2.indexOf('viewColumnName') > -1) {
                    atr1 = adjustGetColumnStype(atr1, true);
                    atr2 = adjustViewColumnWidth(atr2, false);
                }
                if (atr1.indexOf('viewColumnName') > -1 && atr2.indexOf('getColumnStype') > -1) {
                    atr1 = adjustViewColumnWidth(atr1, true);
                    atr2 = adjustGetColumnStype(atr2, false);
                }
                if (atr1.indexOf('getColumnStype') > -1 && atr2.indexOf('getColumnStype') > -1) {
                    atr1 = adjustGetColumnStype(atr1, true);
                    atr2 = adjustGetColumnStype(atr2, false);
                }
                if (atr1.indexOf('viewColumnName') > -1 && atr2.indexOf('viewColumnName') > -1) {
                    atr1 = adjustViewColumnWidth(atr1, true);
                    atr2 = adjustViewColumnWidth(atr2, false);
                }
            } else {
                if (atr1.indexOf('getListColumnStype') > -1 && atr2.indexOf('viewColumnName') > -1) {
                    atr1 = adjustGetListColumnStype(atr1, false);
                    atr2 = adjustViewColumnWidth(atr2, true);
                }
                if (atr1.indexOf('viewColumnName') > -1 && atr2.indexOf('getListColumnStype') > -1) {
                    atr1 = adjustViewColumnWidth(atr1, false);
                    atr2 = adjustGetListColumnStype(atr2, true);
                }
                if (atr1.indexOf('getListColumnStype') > -1 && atr2.indexOf('getListColumnStype') > -1) {
                    atr1 = adjustGetListColumnStype(atr1, false);
                    atr2 = adjustGetListColumnStype(atr2, true);
                }
                if (atr1.indexOf('getListColumnStype') > -1 && atr2.indexOf('getColumnStype') > -1) {
                    atr1 = adjustGetListColumnStype(atr1, false);
                    atr2 = adjustGetColumnStype(atr2, true);
                }
                if (atr1.indexOf('getColumnStype') > -1 && atr2.indexOf('getListColumnStype') > -1) {
                    atr1 = adjustGetColumnStype(atr1, false);
                    atr2 = adjustGetListColumnStype(atr2, true);
                }

                if (atr1.indexOf('getColumnStype') > -1 && atr2.indexOf('viewColumnName') > -1) {
                    atr1 = adjustGetColumnStype(atr1, false);
                    atr2 = adjustViewColumnWidth(atr2, true);
                }
                if (atr1.indexOf('viewColumnName') > -1 && atr2.indexOf('getColumnStype') > -1) {
                    atr1 = adjustViewColumnWidth(atr1, false);
                    atr2 = adjustGetColumnStype(atr2, true);

                }
                if (atr1.indexOf('getColumnStype') > -1 && atr2.indexOf('getColumnStype') > -1) {
                    atr1 = adjustGetColumnStype(atr1, false);
                    atr2 = adjustGetColumnStype(atr2, true);
                }
                if (atr1.indexOf('viewColumnName') > -1 && atr2.indexOf('viewColumnName') > -1) {
                    atr1 = adjustViewColumnWidth(atr1, false);
                    atr2 = adjustViewColumnWidth(atr2, true);
                }
            }
            $(a).attr("ng-style", atr1);
            $(b).attr("ng-style", atr2);

            var aparent = a.parentNode;
            var asibling = a.nextSibling === b ? a : a.nextSibling;
            b.parentNode.insertBefore(a, b);
            aparent.insertBefore(b, asibling);
        }

        function adjustGetListColumnStype(style, flag) {
            var rel = style.replace('getListColumnStype', '').replace('(', '').replace(')', '');
            var arr = rel.split(',');
            arr[0] = flag ? parseInt(arr[0]) + 1 : parseInt(arr[0]) - 1;
            return 'getListColumnStype(' + arr[0] + ',' + arr[1] + ')';
        }
        function adjustGetColumnStype(style, flag) {
            var index = style.replace('getColumnStype', '').replace('(', '').replace(')', '');
            index = flag ? parseInt(index) + 1 : parseInt(index) - 1;
            return 'getColumnStype(' + index + ')';
        }

        function adjustViewColumnWidth(style, flag) {
            var index = style.replace('{width: viewColumnName[', '').replace('].Width }', '');
            index = flag ? parseInt(index) + 1 : parseInt(index) - 1;
            return '{width: viewColumnName[' + index + '].Width }';
        }


        function updateTemplate() {
            var row = $($scope.gridTemplate);
            //Hide filter
            var section1 = row.find(".filterRow");
            section1.css("display", "none");
            var mss = row.find(".contentDD");
            mss.css("display", "block");
            //Update template
            var htmlTemplate = angular.element(row.clone().wrap('<div></div>').html());
            var dyContent = $(".contentDragDrop");
            var dynamicCompile = $compile(htmlTemplate)($scope);
            var divDynamic = angular.element(dyContent);
            divDynamic.html(dynamicCompile);
            $scope.setPickList();
            filterService.setCurrentGridColumns($scope.viewColumnName);
        }
        function updateHeader(from, to) {
            var fromIndex = from - 2;
            var toIndex = to - 2;
            var gf;
            var gf1;
            var j;
            var sortOrder1;
            var sortOrder2;
            if (from < to) {
                for (j = fromIndex; j < toIndex; j++) {
                    gf = $scope.viewColumnName[j];
                    gf1 = $scope.viewColumnName[j + 1];
                    sortOrder1 = angular.copy(gf.SortOrder);
                    sortOrder2 = angular.copy(gf1.SortOrder);
                    $scope.viewColumnName[j] = gf1;
                    $scope.viewColumnName[j].SortOrder = sortOrder1;
                    $scope.viewColumnName[j + 1] = gf;
                    $scope.viewColumnName[j + 1].SortOrder = sortOrder2;
                }
            } else {
                for (j = fromIndex; j > toIndex; j--) {
                    gf = $scope.viewColumnName[j];
                    gf1 = $scope.viewColumnName[j - 1];
                    sortOrder1 = angular.copy(gf.SortOrder);
                    sortOrder2 = angular.copy(gf1.SortOrder);
                    $scope.viewColumnName[j] = gf1;
                    $scope.viewColumnName[j].SortOrder = sortOrder1;
                    $scope.viewColumnName[j - 1] = gf;
                    $scope.viewColumnName[j - 1].SortOrder = sortOrder2;
                }
            }
        }
    }

})(angular);