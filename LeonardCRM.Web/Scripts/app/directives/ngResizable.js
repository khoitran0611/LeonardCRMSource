(function (ng) {

    'use strict';

    ng.module("LeonardCRM").directive("resizable", dir);

    dir.$inject = [];

    function dir() {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ---------------------------------- //
        return {
            restrict: 'A',
            scope: {
                columnsource: '=',
                colindex: '=colindex',
                resoption: '=resoption'
            },
            link: function (scope, elem, attrs) {
                elem.resizable({
                    handles: 'e',
                    disabled: scope.resoption.disabled,
                    resize: function (evt, ui) {
                        scope.$apply(function () {
                            //scope.columnsource[scope.colindex].Width = elem.outerWidth() + 'px';
                            scope.$emit('ColumnReize');
                        });
                    },
                    start: function (event, ui) {
                        scope.$emit('ColumnReizeStart');
                    },
                    stop: function (evt, ui) {
                        scope.$apply(function () {
                            scope.columnsource[scope.colindex].Width = elem.outerWidth() + 'px';
                            scope.$emit('ColumnReizeStop');
                        });
                    }
                });
            }
        };
    }

})(angular);