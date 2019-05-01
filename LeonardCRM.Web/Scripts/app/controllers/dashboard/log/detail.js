(function() {

    'use strict';

    angular.module("LeonardCRM").controller("logDetail", ctrl);

    ctrl.$inject = ['$scope', '$http', 'logService'];

    function ctrl($scope, $http, logService) {

        $scope.log = {};
        $scope.logId = $scope.CurrentParam.Id;

        $scope.Init = function () {
            logService.GetLogById($scope.logId).then(function (data) {
                $scope.log = angular.fromJson(data);
                $scope.setTitle($scope.languages.LOG.DETAIL_TITLE);
                $scope.setSaveButtonVisible(false);
            });
        };
        $scope.Init();

    }    

})();