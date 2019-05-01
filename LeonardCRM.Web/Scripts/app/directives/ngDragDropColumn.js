(function(ng) {
    'use strict';

    ng.module("LeonardCRM").directive("ngDragDrop", dir);

    dir.$inject = ['$parse', '$timeout', '$interval'];

    function dir($parse, $timeout, $interval) {
        return {
            restrict: 'A',
            scope: {
                headerViewColumns: '='
            },
            link: function ($scope, $element, attrs) {
                //scope.$apply(function () {
                //    scope.currentwidth.width = elem.outerWidth() + 'px';
                //});
                var options = {
                    revert: false, // smooth revert
                    dragHandle: '.table-handle', // handle for moving cols, if not exists the whole 'th' is the handle
                    maxMovingRows: 40, // 1 -> only header. 40 row should be enough, the rest is usually not in the viewport
                    excludeFooter: false, // excludes the footer row(s) while moving other columns. Make sense if there is a footer with a colspan. */
                    onlyHeaderThreshold: 100, // TODO:  not implemented yet, switch automatically between entire col moving / only header moving
                    dragaccept: '.borderright', //null, // draggable cols -> default all
                    persistState: null, // url or function -> plug in your custom persistState function right here. function call is persistState(originalTable)
                    restoreState: null, // JSON-Object or function:  some kind of experimental aka Quick-Hack TODO: do it better
                    exact: true, // removes pixels, so that the overlay table width fits exactly the original table width
                    clickDelay: 0, // ms to wait before rendering sortable list and delegating click event
                    containment: null, // @see http://api.jqueryui.com/sortable/#option-containment, use it if you want to move in 2 dimesnions (together with axis: null)
                    cursor: 'move', // @see http://api.jqueryui.com/sortable/#option-cursor
                    cursorAt: false, // @see http://api.jqueryui.com/sortable/#option-cursorAt
                    distance: 0, // @see http://api.jqueryui.com/sortable/#option-distance, for immediate feedback use "0"
                    tolerance: 'pointer', // @see http://api.jqueryui.com/sortable/#option-tolerance
                    axis: 'x', // @see http://api.jqueryui.com/sortable/#option-axis, Only vertical moving is allowed. Use 'x' or null. Use this in conjunction with the 'containment' setting
                    beforeStart: $.noop, // returning FALSE will stop the execution chain.
                    beforeMoving: $.noop,
                    beforeReorganize: $.noop,
                    beforeStop: $.noop,
                    isDragDrop: attrs.ngDragDrop,
                    headerBefore: []
                };
                var originalTable = {
                    el: null,
                    selectedHandle: null,
                    sortOrder: null,
                    startIndex: 0,
                    endIndex: 0
                };
                var sortableTable = {
                    el: $(),
                    selectedHandle: $(),
                    movingRow: $()
                };
                var persistState = function () {
                    originalTable.el.find('div.headerrow>div.divcol').each(function (i) {
                        if (this.id !== '') {
                            originalTable.sortOrder[this.id] = i;
                        }
                    });
                    $.ajax({
                        url: options.persistState,
                        data: originalTable.sortOrder
                    });
                };



                var rearrangeTable = function () {
                    return function () {
                        // remove handler-class -> handler is now finished
                        originalTable.selectedHandle.removeClass('dragtable-handle-selected');
                        // add disabled class -> reorgorganisation starts soon
                        sortableTable.el.sortable("disable");
                        sortableTable.el.addClass('dragtable-disabled');
                        //options.beforeReorganize(originalTable, sortableTable);
                        // // do reorganisation asynchronous
                        // // for chrome a little bit more than 1 ms because we want to force a rerender
                        originalTable.endIndex = sortableTable.movingRow.prevAll().size() + 1;
                        var from = originalTable.startIndex;
                        var to = originalTable.endIndex;
                        if (from !== to) {
                            $(".filterRow").css("display", "none");
                            $(".contentDD").css("display", "block");
                        }
                        // $timeout(rearrangeTableBackroundProcessing(from,to), 5);
                        //options.beforeStop(originalTable);
                        sortableTable.el.remove();
                        restoreTextSelection();
                        $('ul.dragtable-sortable').remove();
                        successEvent(from, to, options.headerBefore);

                    };
                };
                /*
            * Disrupts the table. The original table stays the same.
            * But on a layer above the original table we are constructing a list (ul > li)
            * each li with a separate table representig a single col of the original table.
            */
                var generateSortable = function (e) {

                    !e.cancelBubble && (e.cancelBubble = true);
                    if ($("ul.dragtable-sortable").length > 0) {
                        $('ul.dragtable-sortable').remove();
                    }
                    // table attributes
                    var attrs = originalTable.el[0].attributes;
                    var attrsString = '';
                    var i;
                    if (/chrom(e|ium)/.test(navigator.userAgent.toLowerCase())) {
                        for (i = 0; i < attrs.length; i++) {
                            if (attrs[i].value && attrs[i].nodeName != 'id' && attrs[i].nodeName != 'width') {
                                attrsString += attrs[i].nodeName + '="' + attrs[i].value + '" ';
                            }
                        }
                    } else {
                        for (i = 0; i < attrs.length; i++) {
                            if (attrs[i].nodeValue && attrs[i].nodeName != 'id' && attrs[i].nodeName != 'width') {
                                attrsString += attrs[i].nodeName + '="' + attrs[i].nodeValue + '" ';
                            }
                        }
                    }


                    // row attributes
                    var rowAttrsArr = [];
                    //compute height, special handling for ie needed :-(
                    var heightArr = [];
                    // compute width, no special handling for ie needed :-)
                    var widthArr = [];
                    // compute total width, needed for not wrapping around after the screen ends (floating)
                    var totalWidth = 0;
                    //get all row move
                    var orTable = originalTable.el.find("div.headerrow>div.divcol"); //find('div').slice(0, options.maxMovingRows);
                    orTable.each(function (i, v) {
                        // row attributes
                        var attrs = this.attributes;
                        var attrsString = "";
                        var j;
                        if (/chrom(e|ium)/.test(navigator.userAgent.toLowerCase())) {
                            for (j = 0; j < attrs.length; j++) {
                                if (attrs[j].value && attrs[j].nodeName != 'id') {
                                    attrsString += " " + attrs[j].nodeName + '="' + attrs[j].value + '"';
                                }
                            }
                        } else {
                            for (j = 0; j < attrs.length; j++) {
                                if (attrs[j].nodeValue && attrs[j].nodeName != 'id') {
                                    attrsString += " " + attrs[j].nodeName + '="' + attrs[j].nodeValue + '"';
                                }
                            }
                        }
                        rowAttrsArr.push(attrsString);
                        heightArr.push($(this).height());

                        var w = $(this).outerWidth();
                        widthArr.push(w);
                        //console.log(w);
                        totalWidth += w;

                    });


                    /* Find children thead and tbody.
                     * Only to process the immediate tr-children. Bugfix for inner tables
                     */
                    //get all cell
                    //var orTable = orTable;//thtb.find('div.headerrow > div.divcol');
                    //orTable.each(function(i, v) {
                    //    //var w = $(this).outerWidth();
                    //    //widthArr.push(w);
                    //    ////console.log(w);
                    //    //totalWidth += w;
                    //});
                    // totalWidth = $(".headerrow").outerWidth();
                    if (options.exact) {
                        var difference = totalWidth - originalTable.el.outerWidth();
                        widthArr[0] -= difference;
                    }
                    // one extra px on right and left side
                    totalWidth += 2;

                    var sortableHtml = '<ul class="dragtable-sortable" style="position:absolute;margin-top:0px;z-index:9; width:' + totalWidth + 'px;>';
                    orTable.each(function (j) {
                        sortableHtml += '<li>';
                        sortableHtml += '<div ' + attrsString + '>';
                        // TODO: May cause duplicate style-Attribute
                        var rowContent = $(this).clone().wrap('<div></div>').parent().html();

                        sortableHtml += rowContent;

                        sortableHtml += '</div>';
                        sortableHtml += '</li>';
                    });
                    sortableHtml += '</ul>';
                    sortableTable.el = originalTable.el.before(sortableHtml).prev();
                    sortableTable.selectedHandle = sortableTable.el.find('li.dragtable-handle-selected');
                    var items = !options.dragaccept ? 'li' : 'li:has(' + options.dragaccept + ')';
                    sortableTable.el.sortable({
                        items: items,
                        stop: rearrangeTable(),
                        // pass thru options for sortable widget
                        revert: options.revert,
                        tolerance: options.tolerance,
                        containment: options.containment,
                        cursor: options.cursor,
                        cursorAt: options.cursorAt,
                        distance: options.distance,
                        axis: options.axis
                    });
                    // assign start index
                    originalTable.startIndex = $(e.target).closest('.borderright').prevAll().size() + 1; //angular.element(e.target).closest('div.divcol').prevAll().size() + 1;

                    options.beforeMoving(originalTable, sortableTable);
                    // Start moving by delegating the original event to the new sortable table
                    sortableTable.movingRow = $('ul.dragtable-sortable').children('li:nth-child(' + originalTable.startIndex + ')');

                    // prevent the user from drag selecting "highlighting" surrounding page elements
                    disableTextSelection();
                    // clone the initial event and trigger the sort with it
                    var pX = e.pageX;
                    var pY = e.pageY;
                    sortableTable.movingRow.trigger($.extend($.Event(e.type), {
                        which: 1,
                        clientX: e.clientX,
                        clientY: e.clientY,
                        pageX: pX,
                        pageY: pY,
                        screenX: e.screenX,
                        screenY: e.screenY
                    }));

                    //// Some inner divs to deliver the posibillity to style the placeholder more sophisticated
                    var placeholder = $('ul.dragtable-sortable').find('.ui-sortable-placeholder');
                    if (!placeholder.height() <= 0) {
                        placeholder.css('height', $('ul.dragtable-sortable').find('.ui-sortable-helper').height());
                    }

                    placeholder.html('<div class="outer" style="height:100%;"><div class="inner" style="height:100%;"></div></div>');
                };

                var bindTo = {};

                var create = function () {
                    originalTable = {
                        el: $element,
                        selectedHandle: $(),
                        sortOrder: {},
                        startIndex: 0,
                        endIndex: 0
                    };
                    bindTo = {};
                    // bind draggable to 'th' by default
                    bindTo = originalTable.el.find('div.headerrow > div.borderright');
                    // filter only the cols that are accepted
                    if (options.dragaccept) {
                        bindTo = bindTo.filter(options.dragaccept);
                    }
                    // bind draggable to handle if exists
                    if (bindTo.find(options.dragHandle).size() > 0) {
                        bindTo = bindTo.find(options.dragHandle);
                    }
                    // restore state if necessary
                    //if (options.restoreState !== null) {
                    //    $.isFunction(options.restoreState) ? options.restoreState(originalTable)
                    //        : restoreState(options.restoreState);
                    //}
                    // destroy();
                    bindTo.bind("mousedown", function (evt) {
                        // listen only to left mouse click
                        //console.log(evt.target.className);
                        if (evt.which !== 1 || evt.target.className == '' || evt.target.localName == "b" || evt.target.className.indexOf('selectcolumn') > -1 || evt.target.className.indexOf('cbAll') > -1 || evt.target.className.indexOf('ui-resizable-e') > -1) return;
                        if (options.beforeStart(originalTable) === false) {
                            return;
                        }

                        $timeout.cancel(downTimer(evt));

                    });
                    bindTo.bind("mouseup", function (evt) {

                        // $timeout.cancel(downTimer(evt));
                    });
                };
                var downTimer = (function (evt) {
                    //$timeout(function() {
                    originalTable.selectedHandle = $(this);
                    originalTable.selectedHandle.addClass('dragtable-handle-selected');
                    generateSortable(evt);
                    //}, options.clickDelay);
                });
                var successEvent = function (from, to, header) {
                    var option = {
                        from: from,
                        to: to,
                        headerBefore: header
                    };

                    if (from != to)
                        $scope.$emit("dragdropcompleted", option);
                };
                $scope.$on('initDragDrop', function (event, args) {
                    init();
                });
                //Hide loading when completed swap
                $scope.$on('hideLoadingDragDrop', function (event, args) {
                    $timeout(function () {
                        if ($(".filterRow").css("display") == 'block') {
                            restoreTextSelection();
                        } else {
                            $(".filterRow").css("display", "block");
                            $(".contentDD").css("display", "none");
                            disableTextSelection();
                        }
                    }, 500);
                });

                var init = function () {
                    if (options.isDragDrop == 'true') {
                        create();
                    }
                };
                var redraw = function () {
                    // destroy();
                    create();
                };
                var destroy = function () {
                    if (bindTo != undefined && bindTo.length > 0) {
                        bindTo.unbind('mousedown');
                        bindTo.unbind("mouseup");
                        originalTable = {
                            el: null,
                            selectedHandle: null,
                            sortOrder: null,
                            startIndex: 0,
                            endIndex: 0
                        };
                        sortableTable = {
                            el: $(),
                            selectedHandle: $(),
                            movingRow: $()
                        };
                        create();
                    }

                    // desInit();
                    // $.Widget.prototype.destroy.apply(this, arguments); // default destroy
                    // now do other stuff particular to this widget
                };
                //var desInit= $scope.$on("$destroy", function () {
                //    initDragDrop();
                //});
                /** closure-scoped "private" functions **/
                var bodyOnselectstartSave = $(document.body).attr('onselectstart'),
                    bodyUnselectableSave = $(document.body).attr('unselectable');

                // remove the <style> tag, and restore the original <body> onselectstart attribute
                function disableTextSelection() {
                    // jQuery doesn't support the element.text attribute in MSIE 8
                    // http://stackoverflow.com/questions/2692770/style-style-textcss-appendtohead-does-not-work-in-ie
                    var $style = $('<style id="__dragtable_disable_text_selection__" type="text/css">body { -ms-user-select:none;-moz-user-select:-moz-none;-khtml-user-select:none;-webkit-user-select:none;user-select:none; }</style>');
                    $(document.head).append($style);
                    $(document.body).attr('onselectstart', 'return false;').attr('unselectable', 'on');
                    if (window.getSelection) {
                        window.getSelection().removeAllRanges();
                    } else {
                        document.selection.empty(); // MSIE http://msdn.microsoft.com/en-us/library/ms535869%28v=VS.85%29.aspx
                    }
                }

                function restoreTextSelection() {
                    $('#__dragtable_disable_text_selection__').remove();
                    if (bodyOnselectstartSave) {
                        $(document.body).attr('onselectstart', bodyOnselectstartSave);
                    } else {
                        $(document.body).removeAttr('onselectstart');
                    }
                    if (bodyUnselectableSave) {
                        $(document.body).attr('unselectable', bodyUnselectableSave);
                    } else {
                        $(document.body).removeAttr('unselectable');
                    }
                }



            }
        };
    }

})(angular);