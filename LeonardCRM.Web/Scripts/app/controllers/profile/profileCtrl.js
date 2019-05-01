(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ProfileCtrl", ctrl);

    ctrl.$inject = ["$scope", "userService", "appService", "FileUploader", '$http'];

    function ctrl($scope, userService, appService, FileUploader, $http) {
        $scope.filesUploaded = [];
        $scope.photo = '';

        $scope.LoadData = function () {
            userService.getUserById(appService.getCurrentUser())
                .then(function (data) {
                    $scope.user = angular.fromJson(data);
                    if ($scope.user.Avatar != null && $scope.user.Avatar.length > 0) {
                        $scope.photo = $scope.UploadFolderUrl + $scope.user.Avatar;
                    }
                });
        };

        $scope.Save = function () {
            $scope.userModuleId = appService.getModuleId('users');
            userService.save($scope.user, $scope.userModuleId).then(function (data) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                } else {
                    NotifyError(data.Result);
                }
            });
        };

        //Upload File
        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            url: $scope.UploadHandlerUrl + "?folder=user"
        });

        uploader.filters.push({
        	name: 'filterName',
        	fn: function (item) {
        		var extension = item.name.split('.').pop();
        		if ($.inArray(extension, $scope.allowImageExt) > -1) {
        			return true;
        		}
        		return false;
        	}
        });

        uploader.onSuccessItem = function (item, response, status, headers) {
        	$scope.filesUploaded = response;
        	var filename = $scope.filesUploaded[0].name;
        	$scope.photo = $scope.UploadFolderUrl + filename;
        	$scope.user.Avatar = filename;
        	$scope.filesUploaded = [];
        	$scope.uploader.clearQueue();
        };

        $scope.DeletePhoto = function (imageName) {
            $http.post($scope.UploadHandlerUrl + '?f=' + imageName)
                .success(function (data, status, headers, config) {
                    $scope.filesUploaded = [];
                    $scope.photo = $scope.user.Avatar = '';
                }).error(function (data, status, headers, config) {
                    alert(data);
                });
        };

        //--------------START LOADING JQUERY VALIDATION-----------------
        $().ready(function () {

            $.validate({
                validateOnBlur: true,
                errorMessagePosition: 'top',
                scrollToTopOnError: false,
                showHelpOnFocus: false,
                submitHandler: function (form) {
                    return false;
                },
                onSuccess: function (status) {
                },
                onError: function () {
                },
                onValidate: function () {
                }

            });
        });

        $scope.LoadData();
        $scope.setWindowTitle($scope.languages.USERS.PROFILE_TITLE);
        try {
            $scope.setPageHeader('ACCOUNT INFO', '');
        } catch(ex)  {
        }
    }

})(angular);