(function (ng) {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("NoteCtrl", ctrl);

    ctrl.$inject = ["$scope", "$sce", "noteService", "registryService", "appService", "appConfig", "_"];

    function ctrl($scope, $sce, noteService, registryService, appService, appConfig, _) {

        // --- Define Controller Methods. ------------------- //
    	var getNotesByRecordIdSuccessCallBack = function (data, status, headers, config) {
    		$scope.notes = angular.fromJson(data);
    	};

    	var toHtml = function (str) {
            var pattern = '\n';
            var reg = new RegExp(pattern, 'g');
            var output = str.replace(reg, '<br/>');
            return output;
        };

        var toTextArea = function (str) {
            var pattern = '<br/>';
            var reg = new RegExp(pattern, 'g');
            var output = str.replace(reg, '\n');
            return output;
        };

        var getCurrentDate = function () {
        	var currentDate = new Date();
        	return currentDate;
        };

        var setNote = function () {
        	$scope.note.NoteDate = getCurrentDate();
        	$scope.note.Description = '';
        	$scope.note.ModuleId = appConfig.orderModule;
        	$scope.note.RecordId = appId;
        };

        var loadNotes = function () {
            setNote();

            if ($scope.note.RecordId > 0) {
                noteService.GetNotesByRecordId($scope.note)
                       .then(getNotesByRecordIdSuccessCallBack);
            }
        };

        var initNote = function () {
        	loadNotes();
        };

        // --- Define Scope Methods. ------------------------ //

        $scope.bindTrustHtml = function (str) {
            str = toHtml(str);
            return $sce.trustAsHtml(str);
        };

        $scope.onlyActive_Checked = function () {
        	loadNotes();
        };

        // --- Define Controller Variables. ----------------- //
        var note = {};
        var appId = 0;

        // --- Define Scope Variables. ---------------------- //

        $scope.notes = [];
        $scope.note = {
        	NoteDate: getCurrentDate(),
        	Description: '',
        	ModuleId: 0,
        	RecordId: 0,
        	IsActive: true
        };
        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };
        $scope.isFirstLoad = true;        

        // --- Bind To Scope Events. ------------------------ //
        $scope.$on('ApplicantLoaded', function (e, app) {
            if (app && !app.IsCustomer && app.SalesOrders && app.SalesOrders.length > 0) {
                appId = app.SalesOrders[0].Id;
                //initalize
                initNote();
            }
        });
    }

})(angular);