(function (ng) {
    'use strict';
    'use strict';

    angular.module("LeonardCRM").controller("userCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "requestContext", "FileUploader", "viewService", "userService", "filterService", "toolbarService", "_", "roleService", "$timeout"];

    function ctrl($scope, $http, requestContext, FileUploader, viewService, userService, filterService, toolbarService, _, roleService, $timeout) {
        $scope.newUser = null;
        $scope.user = null;
        $scope.roles = null;
        $scope.filesUploaded = [];
        $scope.pickListForm = {
            'Store': [],
            'Status': []
        };
        $scope.photo = '';
        $scope.detailFormUrl = '/appviews/dashboard/users/details.html';
        $scope.FormTitle = $scope.languages.USERS.DETAIL_TITLE;
        $scope.pageInfo = {};
        $scope.count = 0;
        $scope.viewName = '';


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
                //error msg
                return false;
            }
        });

        $scope.select2Options = {
            allowClear: true,
            dropdownAutoWidth: true
            //minimumResultsForSearch: 10
        };

        //-----Toolbar event handlers----------
        $scope.$on('refreshEvent', function () {
            $scope.pageInfo.AdvanceSearch = false;
            $scope.pageInfo.PageIndex = 1;
            reloadUserData();
        });
        $scope.$on('deleteEvent', function () {
            $scope.SetConfirmMsg($scope.languages.USERS.CONFIRM_ON_DELETE);
        });
        $scope.$on('yesEvent', function () {
            if ($scope.EditValues.length > 0) {
                userService.deleteUser($scope.EditValues).then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result);

                        if (data.Names != null && data.Names.length > 0) {
                            var names = data.Names.substring(0, data.Names.length - 1);
                            var str = $scope.languages.USERS.DENY_DELETE_NAME;
                            str = str.replace("[name]", names);
                            NotifyError(str, 10000);
                        }

                        reloadUserData();
                    } else
                        NotifyError(data.Result);
                }, function (data) {
                    NotifyError(data.ExceptionMessage);
                });
            }
        });
        $scope.$on('addEvent', function () {
            //always set this to false when not redirecting to detail page, e.g: edit using dialog
            $scope.user = jQuery.extend({}, $scope.newUser);
            $scope.filesUploaded = [];
            $scope.uploader.queue = [];
            showDialog(true);
        });

        //-----grid events------------ 
        $scope.$on('rowClickEvent', function (event, args) {
            userService.getUserById(args.RowId).then(function (data) {
                $scope.user = angular.fromJson(data);

                if ($scope.user.Avatar != null)
                    $scope.photo = $scope.user.Avatar.length > 0 ? $scope.UploadFolderUrl + $scope.user.Avatar : '';
                showDialog(true);
            });
        });
        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter(toolbarService.getCurrentViewId());
        });
        $scope.$on('sortEvent', function (event, args) {
            reloadUserData(args);
        });
        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            $scope.LoadUsersData();
            event.preventDefault();
        });

        //------Advance Search Event Listener---------------------------------
        $scope.$on('doFilterEvent', function (event, filterConditions) {
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression)
                .then(function (data) {
                    var users = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', users);
                });
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.CurrentView);
        });

        //----dialog commands---------------------
        $scope.Save = function () {
            userService.save($scope.user, $scope.CurrentModule).then(function (data) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                    if ($scope.user.Id > 0)
                        showDialog(false);
                    $scope.user = jQuery.extend({}, $scope.newUser);
                    //reload data
                    reloadUserData();

                } else {
                    NotifyError(data.Result, 10000);
                }
            }, function (data) {
                if ($scope.isDebug)
                    NotifyError(data.Message);
            });
        };

        $scope.CloseDialog = function () {
            //do nothing now when the dialog closed
        };

        //------Define controller's methods

        function reloadUserData(args) {
            if (args != null) {
                $scope.pageInfo = args;
            }
            $scope.LoadUsersData();
        }

        function showDialog(isShown) {
            if (isShown) {
                $('#dialogDetail').appendTo("body").modal({
                    show: true,
                    keyboard: false,
                    backdrop: 'static'
                });

                $('#dialogDetail').on('hide.bs.modal',
                    function (e) {
                        $scope.isActiveEditor = false;
                    });

                $('#dialogDetail').on('shown.bs.modal',
                    function (e) {
                        $scope.isActiveEditor = true;
                    });
            }
            else {
                $('#dialogDetail').modal('hide');
            }
        }

        //------Define scope's methods
        $scope.initUserPage = function () {
            $scope.setWindowTitle($scope.languages.USERS.MANAGE_TITLE);
            $scope.NoSelectedItems = true;
            userService.getUserById(0).then(function (data) {
                $scope.newUser = $scope.user = angular.fromJson(data);
                if ($scope.listValues && $scope.listValues.length > 0)
                    $scope.user.Status = $scope.listValues[0].Id;
            });
        };

        $scope.LoadUsersData = function () {
            viewService.GetView($scope.pageInfo).then(function (data, status, headers, config) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            }, function (data) {
                if ($scope.isDebug)
                    NotifyError(data.toString());
            });

            var pickList = filterService.getPickList($scope.pageInfo.ModuleId);
            $scope.pickListForm = {
                'Status': _.where(pickList, { 'FieldName': 'Status' }),
                'Store': _.where(pickList, { 'FieldName': 'StoreId' })
            };

            //loading roles
            roleService.getRoles().then(function (d) {
                $scope.roles = angular.fromJson(d);
                //init a role for new user
                if ($scope.user.Id <= 0)
                    $scope.newUser.RoleId = $scope.user.RoleId = $scope.roles[0].Id;
            });
        };

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

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                $("#dialogDetail").remove();
                $scope.count = 0;
            }
        );

        //---Initiallize------------
        $scope.initUserPage();
    }
})(angular);