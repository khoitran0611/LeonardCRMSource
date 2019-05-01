(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("roleService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    // a repository for the app.
    function ser($http, $q, serviceHelper) {
        var url = '/api/RoleApi/';
        function getAllRoles() {
            return serviceHelper.get(url + 'GetAllRoles/');
        }
        function getRoles() {
            return serviceHelper.get(url + 'GetRoles/');
        }
        function getRoleById(roleId) {
            return serviceHelper.get(url + 'GetRoleById/' + roleId);
        }

        function getRoleUserHierachy() {
            return serviceHelper.get(url + 'GetRoleUserHierachy');
        }

        function deteleRoles(roles) {
            return serviceHelper.post(url + 'DeleteRoles', roles);
        }

        function saveRole(role) {
            return serviceHelper.post(url + 'SaveRoles', role);
        }

        // Return the public API.
        return ({
            getAllRoles: getAllRoles,
            getRoles: getRoles,
            getRoleById: getRoleById,
            DeleteRoles: deteleRoles,
            SaveRole: saveRole,
            getRoleUserHierachy: getRoleUserHierachy
        });
    }

})(angular);