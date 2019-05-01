(function () {

    'use strict';

    angular.module("LeonardCRM").controller("EditRoleCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "appService", "roleService", "toolbarService", "requestContext", "filterService", "_"];

    function ctrl($scope, $http, $route, $location, $routeParams, appService, roleService, toolbarService, requestContext, filterService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Methods. ------------------- //

        var setPermit = function (n, item) {
            var roleParents = _.filterWithProperty($scope.role.Eli_RolesPermissions, 'ModuleId', parseInt(item.ModuleParent));
            if (roleParents != null && roleParents.length > 0) {
                switch (n) {
                    case 1:
                        if (item.AllowRead)
                            roleParents[0].AllowRead = item.AllowRead;
                        break;
                    case 2:
                        if (item.AllowEdit)
                            roleParents[0].AllowEdit = item.AllowEdit;
                        break;
                    case 3:
                        if (item.AllowDelete)
                            roleParents[0].AllowDelete = item.AllowDelete;
                        break;
                    case 4:
                        if (item.AllowCreate)
                            roleParents[0].AllowCreate = item.AllowCreate;
                        break;
                    case 5:
                        roleParents[0].AllowImport = item.AllowImport;
                        break;
                    case 6:
                        roleParents[0].AllowExport = item.AllowExport;
                        break;
                    case 7:
                        roleParents[0].AllowCreateView = item.AllowCreateView;
                        break;
                    case 0:
                        if (item.FullControl)
                            roleParents[0].FullControl = roleParents[0].AllowRead = roleParents[0].AllowCreate = roleParents[0].AllowEdit = roleParents[0].AllowDelete = roleParents[0].AllowImport = roleParents[0].AllowExport = roleParents[0].AllowCreateView = item.FullControl;
                        break;
                }
                if (item.AllowDelete || item.AllowCreateView || item.AllowImport || item.AllowExport) {
                    roleParents[0].AllowRead = true;
                }
                if (!roleParents[0].AllowCreate || !roleParents[0].AllowRead || !roleParents[0].AllowEdit || !roleParents[0].AllowDelete || !roleParents[0].AllowImport || !roleParents[0].AllowExport || !roleParents[0].AllowCreateView) {
                    roleParents[0].FullControl = false;
                }
                if (roleParents[0].AllowCreate && roleParents[0].AllowRead && roleParents[0].AllowEdit && roleParents[0].AllowDelete
                     && roleParents[0].AllowImport && roleParents[0].AllowExport && roleParents[0].AllowCreateView) {
                    roleParents[0].FullControl = true;
                }
            }

            var roleChilds = _.filterWithProperty($scope.role.Eli_RolesPermissions, 'ModuleParent', parseInt(item.ModuleId));
            var i = 0;
            if (roleChilds != null && roleChilds.length > 0) {
                switch (n) {
                    case 1:
                        if (item.AllowRead == false) {
                            for (i = 0; i < roleChilds.length; i++) {
                                roleChilds[i].AllowRead = roleChilds[i].FullControl = false;
                            }
                        }
                        break;
                    case 2:
                        if (item.AllowEdit == false) {
                            for (i = 0; i < roleChilds.length; i++) {
                                roleChilds[i].AllowEdit = roleChilds[i].FullControl = false;
                            }
                        }
                        break;
                    case 3:
                        if (item.AllowDelete == false) {
                            for (i = 0; i < roleChilds.length; i++) {
                                roleChilds[i].AllowDelete = roleChilds[i].FullControl = false;
                            }
                        }
                        break;
                    case 4:
                        if (item.AllowCreate == false) {
                            for (i = 0; i < roleChilds.length; i++) {
                                roleChilds[i].AllowCreate = roleChilds[i].FullControl = false;
                            }
                        }
                        break;
                    case 0:
                        if (item.FullControl == false) {
                            for (i = 0; i < roleChilds.length; i++) {

                                roleChilds[i].FullControl = roleChilds[i].AllowCreate = roleChilds[i].AllowDelete = roleChilds[i].AllowEdit = roleChilds[i].AllowRead = roleChilds[i].AllowImport = roleChilds[i].AllowExport = roleChilds[i].AllowCreateView = false;
                            }
                        }
                        break;
                }

            }
        };

        var getRoleCallback = function (data) {
            $scope.role = angular.fromJson(data);
            $scope.roleShadow = angular.copy($scope.role);
            $scope.roleName = angular.copy($scope.role.Name);

        };

        var saveRoleCallback = function (data) {
            if (data.ReturnCode == 200) {
                filterService.resetAllFields();
                NotifySuccess(data.Result, 5000);
                if ($scope.previousUrl == '') {
                    $location.path('/dashboard/roles');
                } else {
                    $location.path($scope.previousUrl);
                }
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var getRolebyId = function () {
            roleService.getRoleById($scope.roleId, $scope.CurrentModule)
                .then(getRoleCallback);
        };

        var saveRole = function () {
            $scope.role.ModuleId = $scope.CurrentModule;
            roleService.SaveRole($scope.role, $scope.CurrentModule)
                .then(saveRoleCallback);
        };

        var init = function () {

            $.validate({
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    $scope.formStatus = status;
                    if ($scope.formStatus) {
                        if ($scope.role.Name.toLowerCase() == 'administrator' &&
                            $scope.roleId > 0 && $scope.roleShadow.Name.toLowerCase() != 'administrator') {
                            NotifyError($scope.languages.ROLES.SPECIAL_NAME);
                        } else {
                            saveRole();
                        }
                    }
                    return false;
                },
                onError: function () {
                    return false;
                },
                onValidate: function () {
                    return "";
                }
            });

            appService.hasPermission($scope.CurrentModule).then(function (data) {
                $scope.hasPermission = data;
            });
            $scope.setWindowTitle($scope.languages.ROLES.EDIT_ROLE_TITLE);
            toolbarService.NeedSaveCommand(true);
            getRolebyId();
        };

        // --- Define Scope Methods. ------------------- //



        $scope.fullControlChanged = function (item) {
            item.AllowCreate = item.AllowRead = item.AllowEdit = item.AllowDelete = item.AllowImport = item.AllowExport = item.AllowCreateView = item.FullControl;
            //if (!item.ModuleAllowCreate)
            //    item.AllowCreate = false;
            //if (!item.ModuleAllowEdit)
            //    item.AllowEdit = false;
            //if (!item.ModuleAllowDelete)
            //    item.AllowDelete = false;
            setPermit(0, item);
        };


        $scope.permitChanged = function (n, item) {
            if (item.AllowDelete || item.AllowCreateView || item.AllowImport || item.AllowExport) {
                item.AllowRead = true;
            }

            if (item.AllowCreate == false || item.AllowRead == false || item.AllowEdit == false || item.AllowDelete == false
                || item.AllowImport == false || item.AllowExport == false || item.AllowCreateView == false) {
                item.FullControl = false;
            } else {
                item.FullControl = true;
            }
            setPermit(n, item);
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.roleId = requestContext.getParamAsInt('roleId', 0);
        $scope.role = {};
        $scope.roleShadow = {};
        $scope.hasPermission = false;
        $scope.roleName = '';
        $scope.formStatus = false;

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('cancelEvent', function (event) {
            if ($scope.previousUrl == '') {
                $location.path('/dashboard/roles');
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        $scope.$on('saveEvent', function (event) {
            var form = $('#roleForm');
            form.submit();
            event.preventDefault();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                $scope.roleId = requestContext.getParamAsInt('roleId', 0);
                if (requestContext.haveParamsChanged(['roleId']) && requestContext.getParamAsInt('roleId', 0) > 0) {
                    getRolebyId();
                }
            }
        );

        // --- Initialize. ---------------------------------- //
        init();
    }

})();