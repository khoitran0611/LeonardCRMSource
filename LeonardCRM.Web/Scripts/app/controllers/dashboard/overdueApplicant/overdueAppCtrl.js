(function () {

	'use strict';

	angular.module("LeonardCRM").controller("OverdueAppCtrl", ctrl);

	ctrl.$inject = ["$scope", "requestContext", "toolbarService", "salesOrderService", 'ngTableParams', "_", "ngTableService"];

	function ctrl($scope, requestContext, toolbarService, salesOrderService, ngTableParams, _, ngTableService) {

		// --- Define Controller Variables. ---------------------- //

		// --- Define Scope Variables. ---------------------- //
		$scope.tableParams;
		$scope.overdueMonth = 24;
		$scope.checkboxWrapper = { isCheckAll: false };

		// Get the render context local to this controller (and relevant params).
		var renderContext = requestContext.getRenderContext('dashboard.overdue_applicants');

		// --- Define Controller Method. ---------------------- //
		function init() {
			$scope.setWindowTitle($scope.languages.OVERDUE_APPLICANTS.MANAGE_TITLE);
			toolbarService.ShowAdvanceSearch(false);
			var setting = angular.fromJson(localStorage.getItem("settings"));

			if (angular.isDefined(setting) && setting != null) {

				$scope.tableParams = new ngTableParams({
					page: 1, // show first page
					count: setting.ITEMS_PER_PAGE, // count per page
					sorting: { Id: "asc" } // initial sort order,
				},
			    {
			    	paginationMaxBlocks: setting.MAX_PAGE_NUMBERS,
			    	paginationMinBlocks: 1,

			    	getData: function ($defer, params) {
			    		var filterParams = params.filter();

			    		var sorting = params.sorting();
			    		var keySorting = Object.keys(sorting)[0];
			    		var valueSorting = sorting[keySorting];

			    		var count = params.count();
			    		if (!count)
			    			count = 10;
			    		var pageIndex = params.page();			    		

			    		var overdueMonth = !angular.isDefined($scope.overdueMonth) || $scope.overdueMonth == null || $scope.overdueMonth == '' ? 0 : $scope.overdueMonth;

			    		salesOrderService.getOverdueApp(pageIndex, count, keySorting + ' ' + valueSorting, overdueMonth, filterParams).then(function (data) {
			    			var obj = angular.fromJson(data);
			    			if (obj != null) {
			    				if (obj.ReturnCode == 200) {
			    					params.total(obj.Total);
			    					$defer.resolve(obj.Data);
			    				}
			    				else {
			    					NotifyError(obj.Message);
			    				}
			    			}
			    			else
			    			{
			    				NotifyError($scope.languages.COMMON.UNEXPECTED_ERROR_MESSAGE_USER);
			    			}
			    		});
			    	}
			    });
			}
		}

		// --- Define Scope Method. ---------------------- //
		$scope.reloadData = function (isRequireMonth, isKeepFilter) {
			var isEmptyMonth = !angular.isDefined($scope.overdueMonth) || $scope.overdueMonth == null || $scope.overdueMonth == '';

			if (isRequireMonth && isEmptyMonth) {
				NotifyError($scope.languages.OVERDUE_APPLICANTS.EMPTY_MONTH_ERROR_MSG);
				return;
			}

			var isCallReload = !$scope.tableParams.hasFilter();

			//clear all filters
			if ((!angular.isDefined(isKeepFilter) || isKeepFilter == null || isKeepFilter == true)) {
				$scope.tableParams.filter({});
			}

			//reload data			
			if (isCallReload) {
				$scope.tableParams.reload();
			}
		}

		$scope.deleteApps = function () {
			$scope.SetConfirmMsg($scope.languages.OVERDUE_APPLICANTS.DELETE_APP_CONFIRM_MSG, "deleteApps", $scope.languages.OVERDUE_APPLICANTS.DELETE_APP_CONFIRM_HEADER);
		}

		$scope.isSelectApps = function () {
			return $scope.tableParams.total() > 0;
		}

		$scope.isDisableFilter = function () {
			return $scope.overdueMonth == null || $scope.overdueMonth == '';
		}

		$scope.getColorFromPicklist = ngTableService.getColorFromPicklist;

		// --- Bind To Scope Events. ------------------------ //
		$scope.$on('refreshEvent', function (event) {
			$scope.reloadData(false);
		});

		// I handle changes to the request context.
		$scope.$on(
            "requestContextChanged",
            function () {
            	// Make sure this change is relevant to this controller.
            	if (!renderContext.isChangeRelevant()) {
            		return;
            	}

            	// Update the view that is being rendered.
            	$scope.subview = renderContext.getNextSection();
            }
        );

		$scope.$on('yesEvent', function (event, args) {
			if (args == 'deleteApps') {
				var total = $scope.tableParams.total();
				
				salesOrderService.deleteOverdueApps((!angular.isDefined($scope.overdueMonth) || $scope.overdueMonth == null) ? 0 : $scope.overdueMonth, total).then(function (data) {
					if (data.ReturnCode == 200) {
						NotifySuccess(data.Result);
						$scope.reloadData(false, true);
					}
					else {
						NotifyError(data.Result);
					}
				});
			}
		});

		init();
	}

})();