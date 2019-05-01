(function (ng) {

    'use strict';

    ng.module("LeonardCRM").directive("sliderPips", dir);

    dir.$inject = ["_"];

    function dir(_) {
        return {
            restrict: 'A',
            scope: {
                labels: '=labels'
            },
            link: function (scope, element, attrs) {
                scope.dataValue = scope.labels[0];
                scope.$emit('slidebarCurrentValue', scope.dataValue);

                //sortBy SortOrder Property
                scope.labels = _.sortBy(scope.labels, 'SortOrder');

                //get label
                var labelPips = [];
                angular.forEach(scope.labels, function (obj, index) {
                    labelPips.push(obj.Title);
                });

                var $awesome = $(element).slider({
                    range: "min",
                    max: labelPips.length - 1,
                    value: 0,
                    change: function (event, ui) {
                        scope.dataValue = angular.copy(scope.labels[ui.value]);
                        scope.$emit('slidebarCurrentValue', scope.dataValue);
                        $(element).find('.sliderpiptooltip').tooltipster('show');
                        //console.log('4');
                    },
                    slide: function (event, ui) {
                        //console.log('2');
                    },
                    stop: function (event, ui) {
                        //console.log('3');
                    },
                    start: function (event, ui) {
                        $(element).find('.sliderpiptooltip').tooltipster('show');
                        //console.log('1');
                    }
                });

                scope.$on('NeedAnswers', function () {
                    scope.$emit('slidebarCurrentValue', scope.dataValue);
                });

                $awesome.slider("pips", {
                    labels: labelPips
                }).slider("float", { labels: labelPips });
                $(element).find('.sliderpiptooltip').tooltipster({
                    animation: 'fade',
                    delay: 0,
                    touchDevices: true,
                    trigger: 'click',
                    position: 'top',
                    positionTracker: true,
                    autoClose: false,
                    speed: 0,
                    updateAnimation: false
                });
                $(element).find('.sliderpiptooltip').tooltipster('show');
            }
        }
    }
})(angular);