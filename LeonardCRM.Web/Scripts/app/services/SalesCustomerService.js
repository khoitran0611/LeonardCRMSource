(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("salesCustomerService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];
    
    function ser($http, $q, serviceHelper) {
        var url = '/api/SalesCustomerApi/';

        var saveCustomer = function (customer, mode) {        	
        	return serviceHelper.post(url + 'SaveCustomer' + (angular.isDefined(mode) &&  mode != null ? ('?t=' + mode) : ""), customer);
        };
        var getSalesCustomerById = function (pageInfo) {
            return serviceHelper.post(url + 'GetCustomerById', pageInfo);
        };
        var deleteCustomers = function (customers) {
            return serviceHelper.post(url + 'DeleteCustomers', customers);
        };
        function getRecentlyAddedClients(onlyMe) {
            return serviceHelper.get(url + 'GetRecentlyAddedCustomers/?onlyMe=' + onlyMe);
        }

        function getChartData(onlyMe) {
            return serviceHelper.get(url + 'GetChartData/?onlyMe=' + onlyMe);
        }
        function getReportDataByUsers(users) {
            return serviceHelper.post(url + 'GetReportDataByUsers', users);
        }

        function getAllClients() {
            return serviceHelper.get(url + 'GetAllClients');
        }

        function getAllCustomers() {
            return serviceHelper.get(url + 'GetAllCustomers');
        }
        var getSheetNames = function (fileName) {
            return serviceHelper.get(url + 'GetSheetNames/?fileName=' + fileName);
        };
        var importData = function (parserData, moduleId) {
            return serviceHelper.post(url + 'ImportData/', parserData, moduleId);
        };

        var columnMapping = function (parserData, moduleId) {
            return serviceHelper.post(url + 'ColumnMapping/', parserData, moduleId);
        };

        var dataInjectionGetFieldDatatype = function (moduleId) {
            return serviceHelper.get(url + 'DataInjectionGetFieldDatatype/' + moduleId, moduleId);
        }
        var getAllParsers = function (moduleId) {
            return serviceHelper.get(url + 'GetAllParsers/', moduleId);
        };
        var validateExcelFile = function (parserData, moduleId) {
            return serviceHelper.post(url + 'ValidateExcelFile/', parserData, moduleId);
        };

        var getApplicantById = function (appId) {
            return serviceHelper.get(url + 'GetApplicantById?appId=' + appId);            
        }

        var getContractContent = function (appId) {
            return serviceHelper.get(url + 'GetContractContent?appId=' + appId);
        }

        return {
            SaveCustomer: saveCustomer,
            GetSalesCustomerById: getSalesCustomerById,
            DeleteCustomers: deleteCustomers,
            getRecentlyAddedClients: getRecentlyAddedClients,
            getChartData: getChartData,
            getReportDataByUsers: getReportDataByUsers,
            getAllClients: getAllClients,
            getAllCustomers: getAllCustomers,
            getSheetNames: getSheetNames,
            importData: importData,
            columnMapping: columnMapping,
            dataInjectionGetFieldDatatype: dataInjectionGetFieldDatatype,
            getAllParsers: getAllParsers,
            validateExcelFile: validateExcelFile,
            getApplicantById: getApplicantById,
            getContractContent: getContractContent
        };
    }
    
})(angular);