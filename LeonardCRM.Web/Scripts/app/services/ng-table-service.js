(function(ng) {

    "use strict";

    angular.module("LeonardCRM").service("ngTableService", ser);

    ser.$inject = ['_'];

    // a repository for the app.
    function ser(_) {
       
        var getColorFromPicklist = function (picklist) {
            if (_.isString(picklist) && picklist.indexOf("@@")) {
                var color = picklist.split("@@")[1];
                return angular.isDefined(color) && color.trim() !== "none" ? color.trim() : "";
            }
            return "";
        };

        return {
            getColorFromPicklist: getColorFromPicklist
        };
    }

})(angular);