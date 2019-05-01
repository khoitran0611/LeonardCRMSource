(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("SystemAuditCtrl", ctrl);

    ctrl.$inject = ["$scope", "systemAuditService", "registryService", "userService"];

    function ctrl($scope, systemAuditService, registryService, userService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var getUsers = function () {
            userService.getAllUser().then(function (data) {
                $scope.users = angular.fromJson(data);
                var user = {
                    Id: 0,
                    Name: '',
                    RoleName: ''
                };
                $scope.users.splice(0, 0, user);
            });
        };

        var getSystemAudit = function () {
            systemAuditService.getAuditByRecordId($scope.pageInfoAudit)
                .then(function (data) {
                    $scope.pageInfoAudit = angular.fromJson(data);

                    if ($scope.pageInfoAudit.Models != null) {
                        if ($scope.isFilter == true) {
                            $scope.isFilter = false;
                            $scope.systemAudits = angular.copy($scope.pageInfoAudit.Models.sysAudit);
                        } else {
                            if ($scope.pageInfoAudit.Models.sysAudit.length > 0) {
                                angular.forEach($scope.pageInfoAudit.Models.sysAudit, function (value, index) {
                                    $scope.systemAudits.push(value);
                                });
                            }
                        }
                    }

                    $scope.numberPages = parseInt($scope.pageInfoAudit.TotalRow / $scope.pageInfoAudit.PageSize);
                    var m = $scope.pageInfoAudit.TotalRow % $scope.pageInfoAudit.PageSize;
                    if (m > 0) {
                        $scope.numberPages = $scope.numberPages + 1;
                    }
                    if ($scope.pageInfoAudit.PageIndex >= $scope.numberPages) {
                        $scope.isShowMore = true;
                    } else {
                        $scope.isShowMore = false;
                    }
                });
        };

        var initSystemAutdit = function () {
            getUsers();
            $scope.pageInfoAudit = {
                ModuleId: $scope.CurrentParam.ModuleId,
                Id: $scope.CurrentParam.Id,
                PageIndex: 1,
                PageSize: registryService.siteSettings.ITEMS_PER_PAGE,
                Models: {}
            };
            getSystemAudit();
        };



        // --- Define Scope Variables. ---------------------- //

        $scope.systemAudits = [];
        $scope.datetime_format = registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT;
        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.pageInfoAudit = {};
        $scope.numberPages = 0;
        $scope.isShowMore = true;
        $scope.users = [];
        $scope.filterObj = {
            DateModified: null,
            Operation: '',
            ColumnName: '',
            CreatedBy: 0
        };
        $scope.isFilter = false;

        // --- Define Scope Method. ---------------------- //

        $scope.showOperation = function (operation) {
            var str = '';
            if (operation == 'U') {
                str = $scope.languages.SYSTEM_AUDIT.UPDATE;
            }
            if (operation == 'I') {
                str = $scope.languages.SYSTEM_AUDIT.INSERT;
            }
            if (operation == 'D') {
                str = $scope.languages.SYSTEM_AUDIT.DELETE;
            }
            return str;
        };

        $scope.showMore = function () {
            if ($scope.pageInfoAudit.PageIndex < $scope.numberPages) {
                $scope.pageInfoAudit.PageIndex += 1;
                $scope.pageInfoAudit.Models = {};

                if ($scope.isFilter == false) {
                    getSystemAudit();
                } else {
                    systemAuditService.serverFilter($scope.filterObj, $scope.pageInfoAudit.ModuleId, $scope.pageInfoAudit.Id, $scope.pageInfoAudit.PageIndex)
                    .then(function (data) {
                        $scope.pageInfoAudit = angular.fromJson(data);
                        if ($scope.isFilter == false) {
                            $scope.isFilter = true;
                        }
                        if ($scope.pageInfoAudit.Models.sysAudit.length > 0) {
                            angular.forEach($scope.pageInfoAudit.Models.sysAudit, function (value, index) {
                                $scope.systemAudits.push(value);
                            });
                        }

                        $scope.numberPages = parseInt($scope.pageInfoAudit.TotalRow / $scope.pageInfoAudit.PageSize);
                        var m = $scope.pageInfoAudit.TotalRow % $scope.pageInfoAudit.PageSize;
                        if (m > 0) {
                            $scope.numberPages = $scope.numberPages + 1;
                        }
                        if ($scope.pageInfoAudit.PageIndex >= $scope.numberPages) {
                            $scope.isShowMore = true;
                        } else {
                            $scope.isShowMore = false;
                        }
                    });
                }
            }
        };

        $scope.serverFilter = function () {
            if ($scope.filterObj.DateModified != null || $scope.filterObj.Operation != ''
                || $scope.filterObj.ColumnName != '' || $scope.filterObj.CreatedBy > 0) {
                systemAuditService.serverFilter($scope.filterObj, $scope.pageInfoAudit.ModuleId, $scope.pageInfoAudit.Id, 1)
                    .then(function (data) {

                        $scope.pageInfoAudit = angular.fromJson(data);
                        $scope.systemAudits = angular.copy($scope.pageInfoAudit.Models.sysAudit);

                        if ($scope.isFilter == false) {
                            $scope.isFilter = true;
                        }

                        $scope.numberPages = parseInt($scope.pageInfoAudit.TotalRow / $scope.pageInfoAudit.PageSize);
                        var m = $scope.pageInfoAudit.TotalRow % $scope.pageInfoAudit.PageSize;
                        if (m > 0) {
                            $scope.numberPages = $scope.numberPages + 1;
                        }
                        if ($scope.pageInfoAudit.PageIndex >= $scope.numberPages) {
                            $scope.isShowMore = true;
                        } else {
                            $scope.isShowMore = false;
                        }
                    });
            } else {
                $scope.isFilter = true;
                $scope.pageInfoAudit = {
                    ModuleId: $scope.CurrentParam.ModuleId,
                    Id: $scope.CurrentParam.Id,
                    PageIndex: 1,
                    PageSize: registryService.siteSettings.ITEMS_PER_PAGE,
                    Models: {}
                };
                getSystemAudit();
            }
        };

        // --- Bind To Scope Events. ------------------------ //

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

            }
        );

        // --- Initialize. ---------------------------------- //
        initSystemAutdit();

    }

})(angular);