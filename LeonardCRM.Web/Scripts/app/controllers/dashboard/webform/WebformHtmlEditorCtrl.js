(function (ng) {
    'use strict';

    angular.module("LeonardCRM").controller("WebformHtmlEditorCtrl", ctrl);

    ctrl.$inject = ["_", "$scope", "$sce", "$compile", "dataTypeService", "registryService", "filterService"];

    function ctrl(_, $scope, $sce, $compile, dataTypeService, registryService, filterService) {

        $scope.getLinkSubmit = function () {
            var url = window.location.href;
            var arr = url.split("/");
            var result = arr[0] + "//" + arr[2];
            return result + '/public/webform';
        };


        $scope.webform = $scope.CurrentParam.Model;
        $scope.html = '';
        $scope.urlSubmit = $scope.getLinkSubmit();

        $scope.localFilterObj = {
            Collection: {}
        };

        var init = function () {
            $scope.setTitle($scope.languages.WEBFORM.WEBFORM_HTML_TITLE);
            loadTabPanel();

            dataTypeService.getAllDataType()
                .then(function (data) {
                    $scope.dataTypeSource = angular.fromJson(data);

                    setCollection();

                    $scope.getPickList();
                });
        };

        var loadTabPanel = function () {
            $('#WebformHtmlTab a:first').tab('show');
            $('#WebformHtmlTab a').click(function (e) {
                if ($(this).parent('li').hasClass('disabled')) {
                    return false;
                };
                e.preventDefault();
                $(this).tab('show');
                loadHtmlScreen($scope.html);
            });
        };

        var setCollection = function () {
            angular.forEach($scope.webform.Eli_WebformDetail, function (field, key) {
                var dataType = _.findWithProperty($scope.dataTypeSource, 'Id', field.DataTypeId);
                if (dataType && dataType.IsList) {
                    $scope.localFilterObj.Collection['_' + field.FieldName] = '';
                }
            });
        };

        var loadHtmlContent = function () {

            $scope.html = '<form action="' + $scope.urlSubmit + '" method="post" >';
            $scope.html += '\n\t';
            $scope.html += '<input type="hidden" name="ReturnUrl" value="' + $scope.webform.ReturnUrl + '" />';
            $scope.html += '\n\t';
            $scope.html += '<input type="hidden" name="PublicId" value="' + $scope.webform.PublicId + '" />';
            $scope.html += '\n';
            angular.forEach($scope.webform.Eli_WebformDetail, function (item, key) {
                $scope.html += generateHtml(item, key);
            });
            $scope.html += '\t';
            $scope.html += '<input type="submit"></input>';
            $scope.html += '\n';
            $scope.html += '</form>';

            loadHtmlScreen($scope.html);
        };

        var loadHtmlScreen = function (htmlSource) {
            $('#webformHtml').html($compile(htmlSource)($scope));
        };


        var generateHtml = function (field, index) {
            var required = field.Mandatory ? 'required' : '';

            var dataType = _.findWithProperty($scope.dataTypeSource, 'Id', field.DataTypeId);

            var html = '\t<p>';
            html += '\n';
            html += '\t\t';
            html += '<label>' + field.LabelDisplay + '</label>';
            html += '\n';
            html += '\t\t';
            if (dataType) {
                if (dataType.IsCheckBox) {
                    html += '<input type="checkbox" name="' + field.FieldName + '"/>';
                }
                else if (dataType.IsEmail) {
                    html += '<input type="email" name="' + field.FieldName + '" ' + required + '/>';
                }
                else if (dataType.IsUrl) {
                    html += '<input type="url" name="' + field.FieldName + '" ' + required + ' pattern="https?://.+"/>';
                }
                else if (dataType.IsDate) {
                    html += '<input type="date" name="' + field.FieldName + '" ' + required + '/>';
                }
                else if (dataType.IsDateTime) {
                    html += '<input type="datetime" name="' + field.FieldName + '" ' + required + '/>';
                }
                else if (dataType.IsCurrency || dataType.IsDecimal || dataType.IsInteger || dataType.IsText) {
                    html += '<input type="text" name="' + field.FieldName + '" ' + required + '/>';
                }
                else if (dataType.IsList) {

                    var optionsHtml = '';

                    if (!field.Required) {
                        optionsHtml += '\n\t\t\t';
                        optionsHtml += ' <option value="null">&nbsp;</option>';
                    }

                    for (var i = 0; i < $scope.localFilterObj.Collection['_' + field.FieldName].length; i++) {
                        optionsHtml += '\n\t\t\t';
                        optionsHtml += ' <option value="' + $scope.localFilterObj.Collection['_' + field.FieldName][i].Id + '">' + $scope.localFilterObj.Collection['_' + field.FieldName][i].Description + '</option>';
                    }

                    html += '<select name="' + field.FieldName + '">'
                        + optionsHtml
                        + '\n\t\t</select>';
                }
                else if (dataType.IsTextArea) {
                    html += '<textarea name="' + field.FieldName + '" ' + required + '></textarea>';
                }
                else if (dataType.IsDate || dataType.IsDateTime) {
                    html += '<input type="datetime" name="' + field.FieldName + '" ' + required + '/>';
                } else if (dataType.IsTime) {
                    html += '<input type="time" name="' + field.FieldName + '" ' + required + '/>';
                }
            } else {
                console.log(field);
            }
            html += '\n';
            html += '\t';
            html += '</p>';
            html += '\n';
            return html;
        };


        $scope.pickList = {
            PickList: [],
            ReferenceList: []
        };
        $scope.loadedPickList = false;

        $scope.getPickList = function () {
            if (!$scope.loadedPickList) {
                filterService.preLoadFilterColumnsAndPickLists($scope.webform.ModuleId);
            } else {
                $scope.loadedPickList = true;
                $scope.pickList.PickList = filterService.getPickList($scope.webform.ModuleId);
                $scope.pickList.ReferenceList = filterService.getReferenceList($scope.webform.ModuleId);
                $scope.setPickList();
            }
            loadHtmlContent();

        };

        $scope.$on('filterPickListEvent', function (event, args) {
            $scope.pickList.PickList = filterService.getPickList($scope.webform.ModuleId);
            $scope.pickList.ReferenceList = filterService.getReferenceList($scope.webform.ModuleId);
            $scope.setPickList();
            loadHtmlContent();
        });

        $scope.setPickList = function () {
            var keys = Object.keys($scope.localFilterObj.Collection);
            angular.forEach(keys, function (value, index) {
                if (value.indexOf('_') == 0) {
                    var str = value.substr(1, value.length);
                    var objs = _.where($scope.pickList.PickList, { 'FieldName': str });
                    if (objs.length == 0) {
                        objs = _.where($scope.pickList.ReferenceList, { 'FieldName': str });
                    }
                    $scope.localFilterObj.Collection[value] = objs;
                }
            });
        };

        init();
    }

})(angular);