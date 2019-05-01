(function (ng) {

    'use strict';

    angular.module('LeonardCRM').service("viewCustomService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/ViewCustomApi/';

        var pagingData = function (viewId, pageIndex, pageSize, sortExpression, filterStr, groupName, isFilterExpiring) {
            return serviceHelper.get(url + 'PagingData/?viewId=' + viewId
                + '&pageIndex=' + pageIndex
                + '&pageSize=' + pageSize
                + '&sortExpression=' + sortExpression
                + '&filterStr=' + filterStr
                + '&groupName=' + groupName
                + '&isFilterExpiring=' + isFilterExpiring);
        };

        var createCustomView = function (viewModel) {
            return serviceHelper.post(url + 'CreateCustomView', viewModel);
        }

        var saveCustomView = function (viewModel) {
            return serviceHelper.post(url + 'SaveCustomView', viewModel);
        }

        var saveView = function (view) {
            return serviceHelper.post(url + 'SaveView', view);
        }

        //get the custom view by Id
        var getViewObjById = function (viewId) {
            return serviceHelper.get(url + 'GetViewObjById?viewId=' + viewId);
        }

        //delete the custom view by Id
        var deleteCustomView = function (viewId) {
            return serviceHelper.get(url + 'DeleteCustomView?viewId=' + viewId);
        }

        return {
            pagingData: pagingData,
            createCustomView: createCustomView,
            saveCustomView: saveCustomView,
            getViewObjById: getViewObjById,
            saveView: saveView,
            deleteCustomView: deleteCustomView
        };
    }

})(angular);