(function () {
    'use strict';
    angular.module('LeonardCRM', []);
    // Create an application module for our app.
    var webapp = angular.module('LeonardCRMFrontEnd', ['LeonardCRM', 'ngRoute', 'ngCookies', 'ui.bootstrap', 'ui.utils', 'angularFileUpload', 'ngAnimate', 'ngTable', 'chieffancypants.loadingBar', 'angularPromiseButtons']);
    // Configure the routing. The $routeProvider will be automatically injected into 
    // the configurator.
    webapp.config(["$routeProvider", "$controllerProvider",
        function ($routeProvider, $controllerProvider) {
            webapp.Register = $controllerProvider.register;
            // Typically, when defining routes, you will map the route to a Template to be 
            // rendered; however, this only makes sense for simple web sites. When you are 
            // building more complex applications, with nested navigation, you probably need 
            // something more complex. In this case, we are mapping routes to render "Actions" 
            // rather than a template.
            $routeProvider
                .when(
                    "/applicantform",
                        {
                            action: "applicantform"
                        }
                )
				.when(
                    "/applicantform/:mode",
                        {
                        	action: "applicantform"
                        }
                )
                .when(
                    "/my-applications/:appId",
                        {
                            action: "edit-applicantform"
                        }
                )
                .when(
                    "/my-applications",
                        {
                            action: "my-applications"
                        }
                )
                .when(
                    "/account-setting",
                        {
                            action: "account-setting"
                        }
                )
                .when(
                    "/help",
                        {
                            action: "help"
                        }
                )
                .when(
                    "/sold",
                        {
                            action: "sold"
                        }
                )
                .otherwise(
                    {
                        action: "my-applications"
                    }
                );
        }
    ]).config(['angularPromiseButtonsProvider', function (angularPromiseButtonsProvider) {
        angularPromiseButtonsProvider.extendConfig({
            spinnerTpl: '<span class="btn-spinner"></span>',
            disableBtn: true,
            btnLoadingClass: 'is-loading',
            addClassToCurrentBtnOnly: false,
            disableCurrentBtnOnly: false
        });
    }]);

    //webapp.constant('appConfig', {
    //    attachment: 'Attachments',
    //    uploadFolderUrl: '/UploadFiles/',
    //    usersModule: 7,
	//	customerModule: 2,
    //    salesCompleteModule: 30,
    //    salesDeliveryModule: 29,
    //    saleCusRefModule: 28,
    //    residenceTypes: {
    //    	Own: 237,
    //    	Rent: 238
    //    },
    //    landTypes: {
    //    	Own: 239,
    //    	Rent: 240
    //    },
    //    completedOrderView: 50
    //});
    
    webapp.filter('trustAsHTML', ['$sce', function ($sce) {
        return function (text) {
            return $sce.trustAsHtml(text);
        };
    }]);
})();