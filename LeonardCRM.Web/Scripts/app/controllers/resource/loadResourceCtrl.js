(function() {

    "use strict";

    angular.module("LeonardCRM").controller("LoadResourceCtrl", ctrl);

    ctrl.$inject = ['$scope', '$timeout', 'resourceService', 'viewService', 'appConfig'];

    function ctrl($scope, $timeout, resourceService, viewService, appConfig) {

        $scope.$on('loadingReourcesCompleted', function () {
        	$timeout(function () {
        	    viewService.getDefaultViewByRoleAndModule(appConfig.orderModule).then(function (data) {
        	        window.location = '/admin/#/order/view/' + (data != 0 ? data : appConfig.defaultOrderView);
        		});                
            }, 1000);
        });
    }
    

})();