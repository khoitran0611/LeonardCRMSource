(function () {
    'use strict';
    // Create an application module for our app.
    var webapp = angular.module('LeonardCRMFrontEnd');
   
    webapp.filter('picklist', function () {
        return function (picklist) {
            return angular.isDefined(picklist)
                && picklist != null
                && picklist !== ''
                && picklist.indexOf("@@") > -1
                ? picklist.split("@@")[0]
                : picklist;
        };
    });

    webapp.directive('loadingWidget', ['$http', '_START_REQUEST_', '_END_REQUEST_', function ($http, _START_REQUEST_, _END_REQUEST_) {
        return {
            restrict: "A",
            link: function (scope, element) {
                // hide the element initially
                element.hide();

                scope.$on(_START_REQUEST_, function (event) {
                    // got the request start notification, show the element
                    element.show();
                    event.preventDefault();
                });

                scope.$on(_END_REQUEST_, function (event) {
                    // got the request end notification, hide the element
                    element.fadeOut();
                    event.preventDefault();
                });
            }
        };
    }]);

    webapp.directive('checkboxAll', function () {
        return {
            replace: true,
            restrict: 'E',
            scope: {
                checkboxes: '=',
                allselected: '=allSelected',
                allclear: '=allClear'
            },
            template: '<input type="checkbox" ng-model="master" ng-change="masterChange()">',
            controller: ["$scope", "$element", function ($scope, $element) {
                $scope.masterChange = function () {
                    if ($scope.master) {
                        angular.forEach($scope.checkboxes, function (cb, index) {
                            cb.Selected = true;
                        });
                    } else {
                        angular.forEach($scope.checkboxes, function (cb, index) {
                            cb.Selected = false;
                        });
                    }

                };
                $scope.$watch('checkboxes', function () {
                    var allSet = true,
                        allClear = true;
                    angular.forEach($scope.checkboxes, function (cb, index) {
                        if (cb.Selected) {
                            allClear = false;
                        } else {
                            allSet = false;
                        }
                    });

                    if ($scope.allselected !== undefined) {
                        $scope.allselected = allSet;
                    }
                    if ($scope.allclear !== undefined) {
                        $scope.allclear = allClear;
                    }

                    $element.prop('indeterminate', false);
                    if (allSet) {
                        $scope.master = true;
                    } else if (allClear) {
                        $scope.master = false;
                    } else {
                        $scope.master = false;
                        $element.prop('indeterminate', true);
                    }
                    if ($scope.checkboxes == null || $scope.checkboxes.length == 0) {
                        $scope.master = false;
                    }
                    $scope.checkAll = {
                        AllSelectedItems: $scope.master,
                        NoSelectedItems: $scope.allclear
                    };
                    $scope.$emit('checkAllEvent', $scope.checkAll);
                }, true);
            }]
        };
    });

    webapp.directive('draggable', ["$document", function ($document) {
        return function (scope, element, attr) {
            var startX = 0, startY = 0, x = 0, y = 0;
            element.css({
                position: 'relative',
                border: '1px solid red',
                backgroundColor: 'lightgrey',
                cursor: 'pointer'
            });
            element.on('mousedown', function (event) {
                // Prevent default dragging of selected content
                event.preventDefault();
                startX = event.screenX - x;
                startY = event.screenY - y;
                $document.on('mousemove', mousemove);
                $document.on('mouseup', mouseup);
            });

            function mousemove(event) {
                y = event.screenY - startY;
                x = event.screenX - startX;
                element.css({
                    top: y + 'px',
                    left: x + 'px'
                });
            }

            function mouseup() {
                $document.unbind('mousemove', mousemove);
                $document.unbind('mouseup', mouseup);
            }
        };
    }]);

    webapp.directive('imageonload', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.bind('error', function () {
                    element.attr('src', '/Content/images/blank.png');
                    element.attr('style', 'display:none');
                });
            }
        };
    });

    webapp.directive('timeago', ["$timeout", "$filter", "$rootScope", "registryService", "_", function ($timeout, $filter, $rootScope, registryService, _) {
        return {
            restrict: 'A',
            terminal: true,
            transclude: true,
            link: function (scope, element) {
                element.hide();
                $timeout(function () {
                    var currentDate = new Date();
                    var date = getTimeAgo(new Date(element.html()), currentDate);
                    element.html(date);
                    element.show();
                }, 0);

                function inMins(d1, d2) {
                    var t2 = d2.getTime();
                    var t1 = d1.getTime();

                    return parseInt((t2 - t1) / (1000 * 60)) % 60;
                }

                function inHours(d1, d2) {
                    var t2 = d2.getTime();
                    var t1 = d1.getTime();

                    return parseInt((t2 - t1) / (3600 * 1000)) % 24;
                }

                function inDays(d1, d2) {
                    var t2 = d2.getTime();
                    var t1 = d1.getTime();
                    return parseInt((t2 - t1) / (24 * 3600 * 1000));
                }

                function inWeeks(d1, d2) {
                    var t2 = d2.getTime();
                    var t1 = d1.getTime();

                    return parseInt((t2 - t1) / (24 * 3600 * 1000 * 7)) % 4;
                }

                function inMonths(d1, d2) {
                    var d1Y = d1.getFullYear();
                    var d2Y = d2.getFullYear();
                    var d1M = d1.getMonth();
                    var d2M = d2.getMonth();

                    return ((d2M + 12 * d2Y) - (d1M + 12 * d1Y)) % 12;
                }

                function inYears(d1, d2) {
                    return d2.getFullYear() - d1.getFullYear();
                }

                function getTimeAgo(d1, d2) {
                    if (d1 == 'Invalid Date')
                        return '--';
                    var min = inMins(d1, d2);
                    var hrs = inHours(d1, d2);
                    var day = inDays(d1, d2);

                    if (day > 7 || d1 > d2 || (day == 7 && hrs > 1)) {
                        if (registryService.siteSettings.DATE_FORMAT)
                            return $filter('date')(d1, registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT);
                        return $filter('date')(d1);
                    }
                    if (day == 7 || (day == 6 && hrs > 0))
                        return 'a week ago';
                    if (day > 1 && hrs < 1) {
                        return day + ' day(s) ago';
                    }
                    if (day > 1) {
                        return (day + 1) + ' day(s) ago';
                    }
                    if (day == 1 && hrs > 1)
                        return (day + 1) + ' day(s) ago';
                    var hours24;
                    var hours;
                    var formatted;
                    if (day == 1 || (d2.getDay() - d1.getDay() == 1)) {
                        hours24 = d1.getHours();
                        hours = ((hours24 + 11) % 12) + 1;
                        formatted = hours24 > 11 ? " PM" : " AM";
                        return 'Yesterday ' + addZero(hours) + ':' + addZero(d1.getMinutes()) + formatted;
                    }
                    if (hrs == 1)
                        return 'an hour ago';
                    if (hrs > 1) {
                        hours24 = d1.getHours();
                        hours = ((hours24 + 11) % 12) + 1;
                        formatted = hours24 > 11 ? " PM" : " AM";
                        return 'Today ' + addZero(hours) + ':' + addZero(d1.getMinutes()) + formatted;
                    }
                    if (min > 0)
                        return min + ' min(s) ago';

                    return 'a minute ago';

                }

                function addZero(num) {
                    return (num >= 0 && num < 10) ? "0" + num : num + "";
                }
            }
        };
    }]);

    webapp.directive('formatteddate', ["$filter", function ($filter) {
        return {
            link: function (scope, element, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue) {
                    var val = element.val();
                    if (!val) {
                        return viewValue;
                    }
                    var dateStr = $filter('date')(val, element.attr('datepicker-popup'));
                    if (dateStr == undefined)
                        return viewValue;
                    var parsed = viewValue;
                    try {
                        //var dateParts;
                        //if (dateStr.indexOf('/') > 0) {
                        //    dateParts = dateStr.split('/');
                        //    parsed = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
                        //} else if (dateStr.indexOf('.') > 0) {
                        //    dateParts = dateStr.split('.');
                        //    parsed = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
                        //} else if (dateStr.indexOf('-') > 0) {
                        var reggie = /(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})/;
                        var dateArray = reggie.exec(dateStr);
                        parsed = new Date(
                            (+dateArray[1]),
                            (+dateArray[2]) - 1, // Careful, month starts at 0!
                            (+dateArray[3]),
                            (+dateArray[4]),
                            (+dateArray[5]),
                            (+dateArray[6])
                        );
                        //dateParts = dateStr.split('-');
                        //parsed = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
                        //}
                    } catch (e) {
                    }
                    return parsed;
                });
            },
            restrict: 'A',
            require: 'ngModel'
        };
    }]);

    webapp.directive('buttonsRadio', function () {
        return {
            restrict: 'E',
            scope: { model: '=', options: '=' },
            controller: ['$scope', function ($scope) {
                $scope.activate = function (option) {
                    $scope.model = option;
                };
            }],
            template: "<label type='button' class='btn btn-default' " +
                     "ng-class='{active: option.Value === model}'" +
                     "ng-repeat='option in options' " +
                     "ng-click='activate(option.Value)'><input type='radio' name='options'>{{option.Text}} " +
                   "</label>"
        };
    });
    webapp.directive('csReadonly', function () {
        return {
            restrict: "A",
            link: function (scope, iElement, iAttrs, controller) {
                scope.$watch(iAttrs.csReadonly, function (readonly) {
                    iElement.select2(readonly ? 'disable' : 'enable');
                });
            }
        };
    });

    webapp.directive('getwidth', function () {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                var width = elem.outerWidth();
                scope.$emit('DivWidth', width);
            }
        };
    });
    webapp.directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });
    webapp.directive('checkList', function () {
        return {
            scope: {
                list: '=checkList',
                value: '@'
            },
            link: function (scope, elem, attrs) {
                var handler = function (setup) {
                    var checked = elem.prop('checked');
                    var index = scope.list.indexOf(scope.value);

                    if (checked && index == -1) {
                        if (setup) elem.prop('checked', false);
                        else scope.list.push(scope.value);
                    } else if (!checked && index != -1) {
                        if (setup) elem.prop('checked', true);
                        else scope.list.splice(index, 1);
                    }
                };

                var setupHandler = handler.bind(null, true);
                var changeHandler = handler.bind(null, false);

                elem.on('change', function () {
                    scope.$apply(changeHandler);
                });
                scope.$watch('list', setupHandler, true);
            }
        };
    });
    webapp.directive('numbersOnly', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl) {
                modelCtrl.$parsers.push(function (inputValue) {
                    // this next if is necessary for when using ng-required on your input. 
                    // In such cases, when a letter is typed first, this parser will be called
                    // again, and the 2nd time, the value will be undefined
                    if (inputValue == undefined) return '';
                    var transformedInput = inputValue.replace(/[^0-9+.]/g, '');
                    if (transformedInput != inputValue) {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }

                    return transformedInput;
                });
            }
        };
    });
    webapp.directive('myMaxlength', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModelCtrl) {
                var maxlength = Number(attrs.myMaxlength);
                function fromUser(text) {
                    if (text.length > maxlength) {
                        var transformedInput = text.substring(0, maxlength);
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                        return transformedInput;
                    }
                    return text;
                }
                ngModelCtrl.$parsers.push(fromUser);
            }
        };
    });

    webapp.directive('ngBlur', ['$parse', function ($parse) {
        return function (scope, element, attr) {
            var defaultValue = attr.ngBlur;
            var val = $(element).val();
            if (val == '')
                $(element).val(defaultValue);
            element.on('blur', function (event) {
                var val = $(element).val();
                if (val == '')
                    $(element).val(defaultValue);
            });
        };
    }]);

    webapp.directive('numbersOnly', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, modelCtrl) {
                modelCtrl.$parsers.push(function (inputValue) {
                    // this next if is necessary for when using ng-required on your input. 
                    // In such cases, when a letter is typed first, this parser will be called
                    // again, and the 2nd time, the value will be undefined
                    if (inputValue == undefined) return '';
                    var transformedInput = inputValue.replace(/[^0-9+.]/g, '');
                    if (transformedInput != inputValue) {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }

                    return transformedInput;
                });
            }
        };
    });
})();