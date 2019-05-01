(function () {

    "use strict";

    angular.module("LeonardCRM").controller("PagingGridCtrl", ctrl);

    ctrl.$inject = ['$scope', '$route', '$location', '$routeParams', '$http', 'registryService'];

    function ctrl($scope, $route, $location, $routeParams, $http, registryService) {

        // --- Define Controller Methods. ------------------- //

        var defineValues = function (pageIndex, pageSize, totalRow) {
            $scope.PageIndex = pageIndex;
            $scope.pageIndexInput = pageIndex;
            $scope.PageSize = pageSize;
            $scope.TotalRow = totalRow;
            var m = totalRow % pageSize;
            var i = 0;
            if (m > 0) {
                i += 1;
            }

            var totalPage = parseInt(totalRow / pageSize) + i;
            $scope.TotalPages = totalPage;
            $scope.FromIndex = ((pageIndex - 1) * pageSize) + 1;
            $scope.ToIndex = pageIndex * pageSize;
            if ($scope.PageSize == $scope.maxPageSize)
                $scope.ToIndex = $scope.TotalRow;
            if (pageIndex == totalPage) {
                $scope.ToIndex = $scope.TotalRow;
            }
            if (totalRow == 0)
                $scope.FromIndex = $scope.ToIndex = 0;

            $scope.DisplayItem = $scope.languages.GRID_VIEW.DISPLAY_CURRENT_ITEMS;
            $scope.DisplayItem = $scope.DisplayItem.replace('[0]', $scope.FromIndex)
                                                   .replace('[1]', $scope.ToIndex)
                                                   .replace('[2]', $scope.TotalRow);
        };

        var init = function () {
            //for (var i = 1; i <= 20; i++) {
            //    $scope.ListPageSizeNumber.push(i * 5);
            //}
            $scope.ListPageSizeNumber = [10, 25, 50, 100];
            $scope.PageSize = registryService.siteSettings.ITEMS_PER_PAGE;
        };

        // --- Define Scope Methods. ------------------------ //

        $scope.setPage = function (index) {
            var currentIndex = parseInt(index);
            if (currentIndex > 0) {
                if (currentIndex > $scope.TotalPages && $scope.pageInfoPaging.PageIndex == $scope.TotalPages) {
                    //do nothing
                } else {
                    if (currentIndex > $scope.TotalPages)
                        $scope.PageIndex = currentIndex = $scope.TotalPages;

                    $scope.pageInfoPaging.PageIndex = $scope.pageIndexInput = $scope.PageIndex = currentIndex;
                    $scope.pageInfoPaging.Id = $scope.id;
                    $scope.$emit('PageIndexChanged', $scope.pageInfoPaging);
                }
            }
        };

        $scope.pageChanged = function () {
            $scope.setPage($scope.PageIndex);
        };

        $scope.setPageIndex = function () {
            if ($scope.PageIndex != $scope.pageIndexInput) {
                $scope.setPage($scope.pageIndexInput);
            }
        };

        $scope.pageSize_Changed = function () {
            $scope.pageInfoPaging.PageSize = $scope.PageSize;
            var currentPageIndexTotal = $scope.PageSize * $scope.PageIndex;
            if (currentPageIndexTotal > $scope.pageInfoPaging.TotalRow && $scope.PageIndex > 1) {
                $scope.pageInfoPaging.PageIndex = $scope.PageIndex - 1;
            }
            $scope.setPage($scope.pageInfoPaging.PageIndex);
        };

        // --- Define Controller Variables. ----------------- //



        // --- Define Scope Variables. ---------------------- //

        $scope.PageIndex = 1;
        $scope.PageSize = 10;
        $scope.TotalRow = 0;
        $scope.TotalPages = 0;
        $scope.MaxPage = registryService.siteSettings.MAX_PAGE_NUMBERS;
        $scope.ListPageSizeNumber = [];
        $scope.FromIndex = 0;
        $scope.ToIndex = 0;
        $scope.DisplayItem = '';
        $scope.pageInfoPaging = null;
        $scope.id = 0;
        $scope.key = '';
        $scope.pageIndexInput = 0;

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('Grid_DataBinding', function (event, args) {
            var checkKey = 'grid' + args.ModuleId.toString() + args.ViewId.toString() + args.Id.toString();
            if ($scope.GridKey == checkKey) {
                $scope.pageIndexInput = args.PageIndex;
                $scope.pageInfoPaging = args;
                $scope.id = args.Id;
                if (args.Models != null) {
                    $scope.key = args.Models.module;
                } else {
                    $scope.key = '';
                }
                defineValues($scope.pageInfoPaging.PageIndex, $scope.pageInfoPaging.PageSize, $scope.pageInfoPaging.TotalRow);
            }
        });

        init();
    };

})();