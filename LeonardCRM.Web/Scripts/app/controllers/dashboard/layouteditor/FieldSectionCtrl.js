(function () {

    'use strict';

    angular.module("LeonardCRM").controller("FieldSectionCtrl", ctrl);

    ctrl.$inject = ["$scope", "toolbarService", "filterService", "entityFieldService", "fieldSectionService", "_"];

    function ctrl($scope, toolbarService, filterService, entityFieldService, fieldSectionService, _) {

        // --- Define Controller Variables. ---------------------- //

        var referenceList = [];

        // --- Define Controller Method. ---------------------- //

        var getFieldSectionByModuleIdCallback = function (data) {
            $scope.layoutEditor.FieldSection = angular.fromJson(data);
            $scope.layoutEditor.FieldSection = _.sortBy($scope.layoutEditor.FieldSection, 'SortOrder');
            $scope.ManageSectionUrl = '/appviews/dashboard/layouteditor/manage.html';
            $scope.layoutEditor.SectionLoaded = true;
        };

        var loadData = function () {
            fieldSectionService.getFieldSectionByModuleId($scope.layoutEditor.ModuleId)
                .then(getFieldSectionByModuleIdCallback);
        };

        var getAllCustFieldByModuleCallback = function (data) {
            $scope.FieldSource = angular.fromJson(data);
            loadData();
        };

        var loadCustomFields = function () {
            entityFieldService.getAllCustFieldByModule($scope.layoutEditor.ModuleId)
            .then(getAllCustFieldByModuleCallback);
        };

        var saveObjsCallback = function (data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                loadData();
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var initForm = function () {
            toolbarService.NeedSaveCommand(true);
            toolbarService.NeedCanCelCommand(false);

            $.validate({
                form: '#sectionform',
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    fieldSectionService.saveObjs($scope.layoutEditor.FieldSection, $scope.CurrentModule, $scope.layoutEditor.ModuleId)
                        .then(saveObjsCallback);
                    return false;
                },
                onError: function () {
                    return false;
                },
                onValidate: function () {
                    return "";
                }
            });
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.select2Options = {
            allowClear: true
        };

        $scope.layoutEditor = {
            'ModuleId': 0,
            'SectionName': '',
            'FieldSection': [],
            'SectionLoaded': false
    };
        $scope.pickListForm = {
            'ModuleId': []
        };
        $scope.ManageSectionUrl = '';
        $scope.FieldSource = [];

        // --- Define Scope Method. ---------------------- //

        $scope.Module_Changed = function () {
            $scope.layoutEditor.SectionLoaded = false;
            $scope.ManageSectionUrl = '';
            loadCustomFields();
        };

        $scope.AddSection = function () {
            var section = {
                'SectionName': angular.copy($scope.layoutEditor.SectionName),
                'ModuleId': $scope.layoutEditor.ModuleId,
                'SortOrder': $scope.layoutEditor.FieldSection.length + 1,
                'Eli_FieldsSectionDetail': [],
                'IsActive': true
            };
            $scope.layoutEditor.FieldSection.push(section);
            $scope.layoutEditor.SectionName = '';
        };

        $scope.LoadedManageSection = function () {
            
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('filterPickListEvent', function (event, args) {
            if ($scope.CurrentModule == args) {
                referenceList = filterService.getReferenceList($scope.CurrentModule);
                $scope.pickListForm.ModuleId = _.filterWithProperty(referenceList, 'FieldName', 'ModuleId');
                if ($scope.pickListForm.ModuleId.length > 0) {
                    $scope.layoutEditor.ModuleId = $scope.pickListForm.ModuleId[0].Id;

                    loadCustomFields();
                }
            }
        });

        $scope.$on('saveEvent', function (event) {
            if ($scope.layoutEditor.SectionLoaded == true) {
                var form = $('#sectionform');
                form.submit();
            }
        });

        // --- Initialize. ------------------------ //

        initForm();

    }

})();