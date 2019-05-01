(function (ng) {

	'use strict';
	var app = ng.module("LeonardCRM");

	app.constant('appConfig', {
		attachment: 'Attachments',
		uploadFolderUrl: '/UploadFiles/',
		usersModule: 7,
		customerModule: 2,
		orderModule:3,
		salesCompleteModule: 30,
		salesDeliveryModule: 29,
		saleCusRefModule: 28,
		entityFieldModule: 18,
		residenceTypes: {
			Own: 237,
			Rent: 238
		},
		landTypes: {
			Own: 239,
			Rent: 240
		},
		deliveryTypes: {
			standardDeliveryType: 258,
			moveJobType: 259,
			tileDown: 383,
			other: 388
		},
		completedOrderView: 50,
		defaultOrderView: 2,
		orderStatus: {
		    pendingStatus: 4,
		    preApprovalStatus: 5,		    
		    pendingCusAcceptStatus: 6,		    
		    inProgressStatus : 8,		    
		    pendingDeliveryStatus: 244,		    
		    deliveredNotSigned: 245,
		    completedStatus: 246,
		    cancelledStatus: 256
		}
	});

})(angular);