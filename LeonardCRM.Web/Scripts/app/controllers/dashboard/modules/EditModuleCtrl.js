(function () {

    'use strict';

    angular.module("LeonardCRM").controller("EditModuleCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "FileUploader", "appService", "toolbarService"];

     function ctrl($scope, $http, FileUploader, appService, toolbarService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var initModuleForm = function () {
            $scope.setTitle($scope.languages.MODULES.EDIT_HEADING);
            toolbarService.NeedSaveCommand(false);
            toolbarService.NeedCanCelCommand(false);
            appService.getModuleByModuleId($scope.CurrentParam.Id).then(function (data) {
                $scope.currentModuleObj = angular.fromJson(data);
                if ($scope.currentModuleObj.MenuIcon != null && $scope.currentModuleObj.MenuIcon.length > 0) {
                    $scope.fileName = angular.copy($scope.currentModuleObj.MenuIcon);
                }
            });
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.listFilesResponse = [];
        $scope.fileName = '';

        $scope.currentModuleObj = {};


        // --- Define Scope Method. ---------------------- //


        $scope.DeletePhoto = function (imageName) {
            $http.post($scope.UploadHandlerUrl + '?f=' + imageName)
                .success(function (data, status, headers, config) {
                    $scope.listFilesResponse = [];
                    $scope.fileName = '';
                    $scope.currentModuleObj.MenuIcon = '';
                })
                .error(function (data, status, headers, config) {
                    alert(data);
                });
        };

        //Upload File
        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            url: $scope.UploadHandlerUrl + '?folder=icons',
            autoUpload : true
        });
        
        uploader.filters.push({
            name: 'filterName',
            fn: function (item) {
                    var extension = item.name.split('.').pop();
                    if ($.inArray(extension, $scope.allowImageExt) > -1) {
                        $scope.listFilesResponse = [];
                        $scope.fileName = '';
                        return true;
                    }
                    //error msg
                    return false;
                }
        });

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('saveDataEvent', function (event) {
            appService.updateModule($scope.currentModuleObj, $scope.CurrentParam.ModuleId).then(function (data) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result, 5000);
                    $scope.$emit('closeWindow');
                } else {
                    NotifyError(data.Result, 5000);
                }
            });
        });


        uploader.onSuccessItem = function (item, response, status, headers) {            
            $scope.listFilesResponse = angular.copy(response);
            $scope.fileName = $scope.listFilesResponse[0].name;

            $scope.currentModuleObj.MenuIcon = angular.copy($scope.fileName);
            $scope.listFilesResponse = [];
            $scope.uploader.clearQueue();
        };

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

            }
        );

        // --- Initialize. ---------------------------------- //
        initModuleForm();
    }

})();