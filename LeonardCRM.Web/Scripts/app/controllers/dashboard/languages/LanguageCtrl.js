(function () {

    'use strict';

    angular.module("LeonardCRM").controller("LanguageCtrl", ctrl);

    ctrl.$inject = ["$scope", "$compile", "resourceService", "registryService", "toolbarService"];

    function ctrl($scope, $compile, resourceService, registryService, toolbarService) {

        // --- Define Controller Variables. ---------------------- //


        // --- Define Controller Method. ---------------------- //
        var getArrayFromObject = function(currentObj, defaultObj) {
            var output = [];
            for (var key in currentObj) {
                if (currentObj.hasOwnProperty(key)) {
                    var tempObj = {};
                    tempObj['Translation'] = currentObj[key];
                    tempObj['Default'] = defaultObj[key];
                    tempObj['Key'] = key;
                    output.push(tempObj);
                }
            }
            return output;
        }

        var dataProcessingForDefault = function (data) {
            $scope.currentPage = angular.copy(data.resString);
            $scope.pages = angular.copy(data.pages);

            $scope.pageSelected = angular.copy($scope.pages[0]);
            $scope.currentLanguage = $scope.defaultLang[$scope.pageSelected];
            $scope.arrayTranslation = getArrayFromObject($scope.currentPage, $scope.currentLanguage);
        };

        var getDefaultLanguageCallback = function (data) {
            $scope.datasource = angular.fromJson(data);
            $scope.defaultLang = angular.copy($scope.datasource.defaultLang);
            $scope.langfiles = angular.copy($scope.datasource.languageFiles);

            dataProcessingForDefault($scope.datasource);
        };

        var languageChangedCallback = function (data) {
            $scope.datasource = angular.fromJson(data);
            
            dataProcessingForDefault($scope.datasource);
        };

        var dataProcessingForPage = function (data) {
            $scope.currentPage = angular.copy(data.resString);
            $scope.pages = angular.copy(data.pages);

            $scope.currentLanguage = $scope.defaultLang[$scope.pageSelected];
            $scope.arrayTranslation = getArrayFromObject($scope.currentPage, $scope.currentLanguage);
        }

        var pageChangedCallback = function (data) {
            $scope.datasource = angular.fromJson(data);
            $scope.defaultLang = angular.copy($scope.datasource.defaultLang);

            dataProcessingForPage($scope.datasource);
        };

        var saveLanguageChangedCallback = function (data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result);
            } else {
                NotifyError(data.Result);
            }
        };

        var initDialog = function () {
            $('#language-dialog').dialog({
                title: $scope.languages.EDIT_LANGUAGE.CREATE_LANGUAGE_TITLE,
                autoOpen: false,
                resizable: false,
                height: 'auto',
                show: { effect: 'drop', direction: 'up' },
                modal: true,
                draggable: true,
                closeOnEscape: false,
                width: 550,
                close: function (event, ui) {
                    //event handler for Esc pressed
                },
                open: function (event, ui) {
                },
                buttons:
                [
                    {
                        text: $scope.languages.COMMON.SAVE_BUTTON,
                        click: function () {
                            $scope.CreateLanguage();
                        }
                    },
                    {
                        text: $scope.languages.COMMON.CANCEL_BUTTON,
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ],
                position: ['top', 5],
                dialogClass: 'no-close'
            });

            $('#resource-dialog').dialog({
                title: $scope.languages.EDIT_LANGUAGE.ADD_RESOURCE_TITLE,
                autoOpen: false,
                resizable: false,
                height: 'auto',
                show: { effect: 'drop', direction: 'up' },
                modal: true,
                draggable: true,
                closeOnEscape: false,
                width: 550,
                close: function (event, ui) {
                    //event handler for Esc pressed
                },
                open: function (event, ui) {
                },
                buttons:
                [
                    {
                        text: $scope.languages.COMMON.SAVE_BUTTON,
                        click: function () {
                            $scope.AddResource();
                        }
                    },
                    {
                        text: $scope.languages.COMMON.CANCEL_BUTTON,
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ],
                position: ['top', 5],
                dialogClass: 'no-close'
            });

            $('#translation-dialog').dialog({
                title: $scope.languages.EDIT_LANGUAGE.ADD_TRANSLATION_TITLE,
                autoOpen: false,
                resizable: false,
                height: 'auto',
                show: { effect: 'drop', direction: 'up' },
                modal: true,
                draggable: true,
                closeOnEscape: false,
                width: 550,
                close: function (event, ui) {
                    //event handler for Esc pressed
                },
                open: function (event, ui) {
                },
                buttons:
                [
                    {
                        text: $scope.languages.COMMON.SAVE_BUTTON,
                        click: function () {
                            $scope.AddTranslation();
                        }
                    },
                    {
                        text: $scope.languages.COMMON.CANCEL_BUTTON,
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ],
                position: ['top', 5],
                dialogClass: 'no-close'
            });

            $('.ui-dialog-buttonset button').addClass(function (index) {
                return "btn btn-sm label-primary";
            });

            //Load create language to dialog
            var divTemplete = '<div></div>';

            var divEl = angular.element(divTemplete);
            var divLanguageContent = $('#languageContent');
            divEl.load('/appviews/dashboard/languageeditor/detail.html', function () {
                var divContainer = angular.element(divLanguageContent);
                var html = $compile(divEl)($scope);
                divContainer.html(html);
                divLanguageContent.fadeIn();
            });

            var divResource = angular.element(divTemplete);
            var divResourceContent = $('#resourceContent');
            divResource.load('/appviews/dashboard/languageeditor/resourceDialog.html', function () {
                var divContainer = angular.element(divResourceContent);
                var html = $compile(divResource)($scope);
                divContainer.html(html);
                divResourceContent.fadeIn();
            });

            var divTranslation = angular.element(divTemplete);
            var divTranslationContent = $('#translationContent');
            divTranslation.load('/appviews/dashboard/languageeditor/translationDialog.html', function () {
                var divContainer = angular.element(divTranslationContent);
                var html = $compile(divTranslation)($scope);
                divContainer.html(html);
                divTranslationContent.fadeIn();
            });
        };

        var init = function () {
            $scope.setWindowTitle($scope.languages.EDIT_LANGUAGE.TITLE);
            toolbarService.NeedSaveCommand(true);
            toolbarService.NeedCanCelCommand(false);
            resourceService.getDefaultLanguages().then(getDefaultLanguageCallback);

            initDialog();
        };

        // --- Define Scope Variables. ---------------------- //
        $scope.datasource = {};
        $scope.defaultLang = {};

        $scope.langfiles = [];
        $scope.langSelected = registryService.siteSettings.DEFAULT_LANGUAGE;
        $scope.pages = [];
        $scope.pageSelected = '';
        $scope.arrayTranslation = [];
        
        $scope.currentLanguage = {};
        $scope.currentPage = {}; //for save xml


        // --- Define Scope Method. ---------------------- //

        $scope.lang_changed = function () {
            resourceService.languageChanged($scope.langSelected)
                .then(languageChangedCallback);
        };

        $scope.page_changed = function () {
            resourceService.pageChanged($scope.langSelected, $scope.pageSelected)
                .then(pageChangedCallback);
        };

        $scope.translationChanged = function (item) {
            if (angular.isDefined(item) && item !== null) {
                $scope.currentPage[item.Key] = item.Translation;
            }
        };

        $scope.orderByValue = function (value) {
            return value;
        };

        $scope.OpenCreateLanguage = function () {
            $('#language-dialog').dialog('open');
            $scope.$broadcast('dialogLanguageDetail', 0);
        };

        $scope.OpenAddResource = function () {
            $('#resource-dialog').dialog('open');
            $scope.$broadcast('dialogResourceDetail', 0);
        };

        $scope.OpenAddTranslation = function () {
            $('#translation-dialog').dialog('open');
            $scope.$broadcast('dialogTranslationDetail',
            {
                FileName: $scope.langSelected,
                ResourceName: $scope.pageSelected
            });
        };

        $scope.CreateLanguage = function () {
            $scope.$broadcast('dialogLanguageSave');
        };

        $scope.AddResource = function () {
            $scope.$broadcast('dialogResourceSave');
        };

        $scope.AddTranslation = function () {
            $scope.$broadcast('dialogTranslationSave');
        };

        $scope.refreshCache = function() {
            resourceService.refreshCache().then(saveLanguageChangedCallback);
        };
        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('reloadLanguageData', function (event, data) {
            $('#language-dialog').dialog('close');

            if (angular.isDefined(data.Names) && data.Names !== null) {
                var languageName = data.Names;

                resourceService.getListLanguages().then(function(data1) {
                    $scope.langfiles = data1;
                    $scope.langSelected = languageName;

                    resourceService.languageChanged(languageName)
                        .then(languageChangedCallback);
                });

            }
        });

        $scope.$on('reloadResourceData', function (event, data) {
            $('#resource-dialog').dialog('close');

            if (angular.isDefined(data.Names) && data.Names !== null) {
                $scope.pageSelected = data.Names;

                resourceService.pageChanged($scope.langSelected, $scope.pageSelected)
                 .then(pageChangedCallback);
            }
        });

        $scope.$on('reloadTranslationData', function (event, data) {
            $('#translation-dialog').dialog('close');

            resourceService.pageChanged($scope.langSelected, $scope.pageSelected)
                .then(pageChangedCallback);
        });

        $scope.$on('saveEvent', function (event) {
            resourceService.saveLanguage($scope.langSelected, $scope.pageSelected, $scope.currentPage)
                .then(saveLanguageChangedCallback);
            event.preventDefault();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                
            }
        );

        // --- Initialize. ---------------------------------- //
        init();
        
        //---Dispose-----------------------------------------//
        $scope.$on('$destroy', function () {
            toolbarService.NeedCanCelCommand(true);
        });
    }

})();