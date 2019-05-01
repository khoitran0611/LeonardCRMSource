(function () {

	"use strict";

	angular.module("LeonardCRM").controller("windowStageCtrl", ctrl);

	ctrl.$inject = ["$scope", "$compile", "$timeout", "$location", "windowService", "_", "requestContext"];

	function ctrl($scope, $compile, $timeout, $location, windowService, _, requestContext) {

		// --- Define Controller Variables. ----------------- //

		var domString = '<div id="[domid]" ng-include=" \'/appviews/Layouts/eli-window.html\' "></div>';
		var renderContext = requestContext.getRenderContext();

		// --- Define Controller Methods. ------------------- //

		var closeAllWindows = function () {
			$scope.windowsCollection = [];
			showStage();
			windowService.closeWindow($scope.windowsCollection.length);
		};

		var showStage = function () {
			var count = $scope.windowsCollection.length;
			if (count > 0) {
				$scope.stageVisible = false;
			} else {
				$scope.stageVisible = true;
			}
		};

		// --- Define Scope Methods. ------------------------ //

		// --- Define Scope Variables. ---------------------- //

		$scope.callCounter = 0;
		$scope.stageVisible = true;
		$scope.windowsCollection = [];

		// --- Bind To Scope Events. ------------------------ //

		$scope.$on('openWindow', function (event, args) {
			$scope.stageVisible = false;
			$scope.callCounter += 1;
			var obj = {
				Url: angular.copy(args.Url),
				Id: angular.copy(args.Id),
				ParentId: angular.copy(args.ParentId),
				ViewId: angular.copy(args.ViewId),
				ModuleId: angular.copy(args.ModuleId),
				Key: angular.copy(args.Key),
				Model: angular.copy(args.Model),
				Title: angular.copy(args.Title),
				doneEvent: function () {
					$scope.$broadcast('init', obj);
				}
			};
			$scope.windowsCollection.push(obj);
		});

		$scope.$on('closeWindowEvent', function (event, args) {
			windowService.refresh(args);
			var keepGoing = true;
			angular.forEach($scope.windowsCollection, function (value, index) {
				if (keepGoing) {
					if (value.Key == args) {
						$scope.windowsCollection.splice(index, 1);
						keepGoing = false;
					}
				}
			});

			showStage();
			$scope.callCounter -= 1;
			$scope.refreshViewMenu();
			windowService.closeWindow($scope.windowsCollection.length);
		});

		$scope.$on('CloseAndOpenWindowEvent', function (event, args) {
			//Close
			var keepGoing = true;
			angular.forEach($scope.windowsCollection, function (value, index) {
				if (keepGoing) {
					if (value.Key == args.Key) {
						$scope.windowsCollection.splice(index, 1);
						keepGoing = false;
					}
				}
			});

			showStage();
			$scope.callCounter -= 1;
			//windowService.closeWindow();

			//Open
			$scope.stageVisible = false;
			$scope.callCounter += 1;
			var obj = {
				Id: angular.copy(args.Id),
				Url: angular.copy(args.Url),
				ParentId: angular.copy(args.ParentId),
				ViewId: angular.copy(args.ViewId),
				ModuleId: angular.copy(args.ModuleId),
				Key: angular.copy(args.Key),
				doneEvent: function () {
					$scope.$broadcast('init', obj);
				}
			};

			$scope.windowsCollection.push(obj);
			$scope.refreshViewMenu();
		});

		$scope.$on('currentWindowClosed', function () {
			$scope.windowsCollection.splice($scope.windowsCollection.length - 1, 1);
		});

		$scope.$watch('ModuleId', function (newValue, oldValue) {
			closeAllWindows();
		});
		$scope.$watch('ViewId', function (newValue, oldValue) {
			closeAllWindows();
		});

		$scope.$on("requestContextChanged", function () {
			// Make sure this change is relevant to this controller.
			if (renderContext.isChangeRelevant()) {
				closeAllWindows();
				return;
			}
		}
		);
		//$scope.$watch(function () { return $location.path(); }, function (newLocation, oldLocation) {
		//    closeAllWindows();
		//});
		// --- Initialize. ---------------------------------- //
	}

})();