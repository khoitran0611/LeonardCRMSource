(function( ng, _ ) {
	
    "use strict";

    angular.module("LeonardCRM").factory("_", ser);

    ser.$inject = [];

	// I provide an augmented lodash library.
    function ser() {


        // I filter the collection down to items with the given property value.
        _.filterWithProperty = function (collection, name, value) {

            var result = _.filter(
                collection,
                function (item) {

                    return (item[name] === value);

                }
            );

            return (result);

        };

        // I filter the collection by in array of values
        _.filterWithValues = function (collection, property, values) {
            var result = _.filter(collection, function (item) {
                return _.contains(values, item[property]);
            });
            return (result);
        };

        // I filter the collection by not in array of values
        _.filterWithNotInValues = function (collection, property, values) {
            var result = _.filter(collection, function (item) {
                return !_.contains(values, item[property]);
            });
            return (result);
        };


        // I find the first collection item with the given property value.
        _.findWithProperty = function (collection, name, value) {

            var result = _.find(
                collection,
                function (item) {

                    return (item[name] === value);

                }
            );

            return (result);

        };


        // I sort the collection on the given property.
        _.sortOnProperty = function (collection, name, direction) {

            var indicator = ((direction.toLowerCase() === "asc") ? -1 : 1);

            collection.sort(
                function (a, b) {

                    if (a[name] < b[name]) {

                        return (indicator);

                    } else if (a[name] > b[name]) {

                        return (-indicator);

                    }

                    return (0);

                }
            );

            return (collection);

        };


        // ---------------------------------------------- //
        // ---------------------------------------------- //


        // Return the public API.
        return (_);


    }

})( angular, _.noConflict() );
/* Release the global reference to the lodash library. This way, we make sure that everyone goes
 through our service object in order to get to the utility library.*/