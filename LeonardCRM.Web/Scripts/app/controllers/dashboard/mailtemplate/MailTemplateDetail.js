(function() {

    'use strict';

    angular.module("LeonardCRM").controller("MailTemplateDetailCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'mailTemplateService', 'entityFieldService', 'toolbarService', 'appService', '$routeParams', 'filterService', 'requestContext', '$timeout', '_'];

   function ctrl($scope, $http, $location, mailTemplateService, entityFieldService, toolbarService, appService, $routeParams, filterService, requestContext, $timeout, _) {

    appService.hasPermission($scope.ModuleId).then(function (data) {
        $scope.hasPermission = data;
    });
    $scope.current = 0;
    $scope.mailtemplates = [];
    $scope.mailtemplate = {};

    function myInit() {
        toolbarService.NeedCanCelCommand(false);
        mailTemplateService.LoadData().then(function (data, status, headers, config) {
            var templates = angular.fromJson(data);
            $scope.current = templates[0].Id;
            $scope.mailtemplate = templates[0];
            $scope.mailtemplates = templates;
            tinyMCE.execCommand('mceSetContent', false, $scope.mailtemplate.TemplateContent);
        });
    }

    //--------------start Get mail template by Id------------------
    $scope.GetMailTemplateById = function () {
        $scope.mailtemplate = _.findWithProperty($scope.mailtemplates, 'Id', parseInt($scope.current));
        tinyMCE.execCommand('mceSetContent', false, $scope.mailtemplate.TemplateContent);
    };
    
    //---------------end get mail template by id-------------------

    $scope.Save = function () {
        mailTemplateService.Save($scope.mailtemplate, $scope.CurrentModule).then(function (data, status, header, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
            } else {
                NotifyError(data.Result, 5000);
            }
            toolbarService.NeedSaveCommand(true);
        });
    };

    //----------------start saving mail template-------------------


    //----------------START HANDLE EVENT---------------------

    $scope.$on('saveEvent', function (e) {
        $scope.Save();
    });


    $scope.$on('cancelEvent', function (event) {
        toolbarService.NeedSaveCommand(false);
        if ($scope.previousUrl == '') {
            $location.path('/dashboard/');
        } else {
            $location.path($scope.previousUrl);
        }
        event.preventDefault();
    });

    //---------------END HANDLE EVENT-----------------------
    //---Dispose-----------------------------------------//
    $scope.$on('$destroy', function () {
        toolbarService.NeedCanCelCommand(true);
    });
    toolbarService.NeedSaveCommand(true);
    $scope.setWindowTitle($scope.languages.MAILTEMPLATE.DETAIL_TITLE);
    myInit();
        }

})();