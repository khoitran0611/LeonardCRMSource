(function (ng) {

    'use strict';

    angular.module("LeonardCRM").service("userService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    // a repository for the user.
    function ser($http, $q, serviceHelper) {
        var url = '/api/UserApi/';
        function deleteUser(users) {
            return serviceHelper.post(url + 'Delete', users);
        }
        function save(user, moduleId) {
            return serviceHelper.post(url + 'Save/?moduleId=' + moduleId, user);
        }
        function getUserById(id) {
            return serviceHelper.get(url + 'GetUserById/' + id);
        }

        function getAllUserGroup() {
            return serviceHelper.get(url + 'GetAllUserGroup');
        }

        function getAllUser() {
            return serviceHelper.get(url + 'GetAllUser');
        }

        function checkIfExistEmail(email) {
        	return serviceHelper.get(url + 'CheckIfExistEmail?email=' + email);
        }

        // Return the public API.
        return ({
            deleteUser: deleteUser,
            save: save,
            getUserById: getUserById,
            getAllUserGroup: getAllUserGroup,
            getAllUser: getAllUser,
            checkIfExistEmail: checkIfExistEmail
        });
    }

})(angular);