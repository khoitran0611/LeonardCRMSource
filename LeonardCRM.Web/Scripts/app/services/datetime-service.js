(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("datetimeService", ser);

    ser.$inject = [];

    function ser() {

        var isDate = function (date) {
            if (!angular.isUndefined(date)
                && date !== null
                && !angular.isUndefined(date.constructor)
                && date.constructor !== null) {
                return date.constructor && date.constructor.toString().indexOf("Date") > -1;
            }
            return false;
        };

        var getMomentObject = function (date) {

            if (!angular.isUndefined(date) && date !== null) {
                if (isDate(date))
                    return moment(date);

                if (moment(date, moment.ISO_8601).isValid() && moment(date, moment.ISO_8601).year() !== 1)
                    return moment(date, moment.ISO_8601);
            }
            return moment(new Date());
        }

        var getDatetimeObjectFromString = function (dateString) {
            return getMomentObject(dateString).toDate();
        }

        var getStringFromDatetimeObject = function (obj) {
            return moment(obj).toISOString();
        }

        return {
            getMomentObject: getMomentObject,
            getDatetimeObjectFromString: getDatetimeObjectFromString,
            getStringFromDatetimeObject: getStringFromDatetimeObject,
            isDate: isDate
        };
    }

})(angular);