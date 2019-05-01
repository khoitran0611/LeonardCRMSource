(function (ng) {

    'use strict';

    angular.module("LeonardCRM").service("viewService", ser);

    ser.$inject = ["serviceHelper"];

    function ser(serviceHelper) {
        var url = '/api/ViewApi/';
        var viewInfo = {
            'ModuleId': 0,
            'ViewId': 0
        };
        var paginginfo = {};
        var changeColumnsState = false;

        function getView(pageInfo) {
            return serviceHelper.post(url + 'GetView', pageInfo);
        };
        function getViewObjById(id) {
            return serviceHelper.get(url + 'GetViewObjById/' + id);
        }
        function getViewByModuleId(id) {
            return serviceHelper.get(url + 'GetViewByModuleId/' + id);
        };

        function getViewsWithTotal(id) {
            return serviceHelper.get(url + 'getViewsWithTotal/' + id);
        };
        function saveView(model) {
            return serviceHelper.post(url + 'SaveView', model);
        };
        function savePersonalView(view, isCreateNewView, moduleId) {
            return serviceHelper.post(url + 'SavePersonalView/?isCreateNewView=' + isCreateNewView, view, moduleId);
        };
        function advanceSearch(columns, viewId, defaultOrder, pageSize, pageIndex, groupColumn, sortExpression) {
            return serviceHelper.post(url + 'AdvanceSearch/?id=' + viewId + '&defaultOrder=' + defaultOrder + '&pageSize=' + pageSize + '&pageIndex=' + pageIndex + '&groupColumn=' + groupColumn + '&sortExpression=' + sortExpression, columns);
        };
        function deleteViews(params) {
            return serviceHelper.post(url + 'DeleteViews', params);
        };
        function exportCsv(pageInfo) {
            return serviceHelper.post(url + 'ExportCsv', pageInfo);
        };
        function exportAllDataCsv(pageInfo) {
            return serviceHelper.post(url + 'ExportAllDataCsv', pageInfo);
        };
        function saveFilter(entity) {
            return serviceHelper.post(url + 'SaveFilter', entity);
        };
        function serverFilterData(serverConditions, viewId, moduleId, id, pageSize, pageIndex, groupColumn, sortExpression) {
            return serviceHelper.post(url + 'ServerFilterData/?viewId=' + viewId + '&moduleId=' + moduleId + '&id=' + id + '&pageSize=' + pageSize + '&pageIndex=' + pageIndex + '&groupColumn=' + groupColumn + '&sortExpression=' + sortExpression, serverConditions);
        };

        var changeColumns = function (items, viewId, moduleId) {
            return serviceHelper.post(url + 'ColumnsVisibleChanged/?viewId=' + viewId + '&moduleId=' + moduleId, items);
        };
        var checkViewInfo = function (viewId, moduleId) {
            if (viewInfo.ViewId != viewId || viewInfo.ModuleId != moduleId) {
                changeColumnsState = false;
            }
        };
        var setViewInfo = function (item) {
            viewInfo = item;
        };
        var getViewInfo = function () {
            return viewInfo;
        };
        var setChangeColumnsState = function (flag) {
            changeColumnsState = flag;
        };
        var getChangeColumnState = function () {
            return changeColumnsState;
        };
        var setPaginginfo = function (item) {
            paginginfo = item;
        };
        var getPaginginfo = function () {
            return paginginfo;
        };

        var getDefaultViewByRoleAndModule = function (mId) {
            return serviceHelper.get(url + 'GetDefaultViewByRoleAndModule?mId=' + mId);
        }

        return {
            GetView: getView,
            GetViewObjById: getViewObjById,
            GetViewByModuleId: getViewByModuleId,
            SaveView: saveView,
            deleteViews: deleteViews,
            AdvanceSearch: advanceSearch,
            ExportCsv: exportCsv,
            ExportAllDataCsv: exportAllDataCsv,
            getViewsWithTotal: getViewsWithTotal,
            saveFilter: saveFilter,
            serverFilterData: serverFilterData,
            savePersonalView: savePersonalView,
            changeColumns: changeColumns,
            setViewInfo: setViewInfo,
            getViewInfo: getViewInfo,
            setChangeColumnsState: setChangeColumnsState,
            getChangeColumnState: getChangeColumnState,
            setPaginginfo: setPaginginfo,
            getPaginginfo: getPaginginfo,
            checkViewInfo: checkViewInfo,
            getDefaultViewByRoleAndModule: getDefaultViewByRoleAndModule
        };
    }

})(angular);