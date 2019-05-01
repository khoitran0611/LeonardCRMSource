(function() {

    "use strict";

    angular.module("LeonardCRM").controller("LoadResourceCtrl", ctrl);

    ctrl.$inject = ['$scope', '$timeout', 'resourceService'];

    function ctrl($scope, $timeout, resourceService) {
        $scope.$on('loadingReourcesCompleted', function () {
    		$timeout(function () {
    			window.location = '/#/my-applications';
    		}, 1000);
    	});
    }
    

})();