(function (ng) {

    'use strict';

    angular.module("LeonardCRM").service("serviceHelper", ser);

    ser.$inject = ["$http", "$q", "$window"];

    function ser($http, $q, $window) {
        function get(getUrl) {
            //$('#loadingWidget').show();
            var defer = $q.defer();
            $http.get(getUrl).success(function (data) {
                defer.resolve(data);
            }).error(function (data, status) {
                console.log(data, status);
                defer.reject();
                if (status == 401 && !noRedirect) {
                    $window.location = '/deny';
                } else if (status == 404 && !noRedirect) {
                    $window.location = '/notfound';
                } else if (status == 500 && !isDebug) {
                    $window.location = '/error';
                }
                else {
                    if (isDebug)
                        NotifyError('Serious problem occurred. Please view console for detail.');
                    console.log(data);
                }
            });
            return defer.promise;
        }

        function post(postUrl, param) {
            //$('#loadingWidget').show();
            var defer = $q.defer();
            $http.post(postUrl, JSON.stringify(param)).success(function (data) {
                defer.resolve(data);
            }).error(function (data, status) {
                console.log(data, status);
                defer.reject();
                if (status == 401 && !noRedirect) {
                    $window.location = '/deny';
                } else if (status == 404 && !noRedirect) {
                    $window.location = '/notfound';
                } else if (status == 500 && !isDebug) {
                    $window.location = '/error';
                }
                else {
                    if (isDebug)
                        NotifyError('Serious problem occurred. Please view console for detail.');
                    console.log(data);
                }
            });
            return defer.promise;
        }

        return {
            get: get,
            post: post
        };
    }
})(angular);
var isDebug = true;
var noRedirect = true;