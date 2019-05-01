(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("NoteCtrl", ctrl);

    ctrl.$inject = ["$scope", "$sce", "noteService", "registryService", "_"];

    function ctrl($scope, $sce, noteService, registryService, _) {

        // --- Define Controller Methods. ------------------- //

        $scope.$on('note_request', function () {
            $scope.$emit('notes_saved', $scope.notes);
        });

        var getNotesByRecordIdSuccessCallBack = function (data, status, headers, config) {
            $scope.notes = angular.fromJson(data);
        };

        var saveNodeSuccessCallback = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                $scope.indexEdit = 0;
                $scope.EditMode = false;
                loadNotes();
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var deleteNoteSuccessCallback = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                loadNotes();
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var saveNote = function (item) {
            var flag = true;
            if (item.Description == '') {
                NotifyLog($scope.languages.NOTES.REQUIRED_DESCRIPTION);
                flag = false;
            }

            if (item.NoteDate == null || angular.isDate(new Date(item.NoteDate)) == false) {
                NotifyLog($scope.languages.NOTES.MISSING_NOTE_DATE);
                flag = false;
            }
            if (flag) {
                if ($scope.CurrentParam.Id > 0) {
                    noteService.SaveNote(item).then(saveNodeSuccessCallback);
                } else {
                    $scope.indexEdit = 0;
                    $scope.EditMode = false;
                }
            }
        };

        var getCurrentDate = function () {
            var currentDate = new Date();
            return currentDate;
        };

        var setNote = function () {
            $scope.note.NoteDate = getCurrentDate();
            $scope.note.Description = '';
            $scope.note.ModuleId = $scope.CurrentParam.ModuleId;
            $scope.note.RecordId = $scope.CurrentParam.Id;
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

        var loadNotes = function () {
            setNote();

            if ($scope.note.RecordId > 0) {
                noteService.GetNotesByRecordId($scope.note)
                       .then(getNotesByRecordIdSuccessCallBack);
            }
        };

        var resetForm = function () {
            $scope.note = {
                NoteDate: getCurrentDate(),
                Description: '',
                ModuleId: 0,
                RecordId: 0,
                IsActive: true
            };
            $scope.notes = [];
        };

        var initNote = function () {
            if ($scope.CurrentParam.Id > 0) {
                loadNotes();
            } else {
                resetForm();
            }
        };

        // --- Define Scope Methods. ------------------------ //

        $scope.editNote = function (index, item) {
            note = angular.copy($scope.notes[index]);
            $scope.notes[index].Description = toTextArea($scope.notes[index].Description);
            $scope.indexEdit = index;
            $scope.EditMode = true;
        };
        $scope.deleteNote = function (index, item) {
            if ($scope.CurrentParam.Id > 0) {
                noteService.DeleteNote(item.Id, item.ModuleId).then(deleteNoteSuccessCallback);
            } else {
                $scope.notes.splice(index, 1);
            }
        };
        $scope.updateNote = function (index, item) {
            saveNote(item);
        };
        $scope.cancelNote = function (index, item) {
            if (item.Id == null || item.Id == 0) {
                $scope.notes.splice(0, 1);
            } else {
                $scope.notes[index] = angular.copy(note);
                $scope.notes[index].Description = toHtml($scope.notes[index].Description);
            }

            $scope.indexEdit = 0;
            $scope.EditMode = false;
        };


        $scope.saveNoteClicked = function () {
            if ($scope.EditMode != true) {
                setNote();
                var note1 = angular.copy($scope.note);
                $scope.notes.splice(0, 0, note1);
                $scope.indexEdit = 0;
                $scope.EditMode = true;
            }
        };

        $scope.bindTrustHtml = function (str) {
            str = toHtml(str);
            return $sce.trustAsHtml(str);
        };

        $scope.onlyActive_Checked = function () {
            if ($scope.CurrentParam.Id > 0) {
                loadNotes();
            }
        };

        // --- Define Controller Variables. ----------------- //

        var note = {};

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
        $scope.noteDate = getCurrentDate();
        $scope.noteDrip = '';
        $scope.indexEdit = 0;
        $scope.EditMode = false;
        $scope.isFirstLoad = true;

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('$destroy', function () {
            if ($scope.CurrentParam.Id > 0) {
                resetForm();
            }
        });

        // --- Initialize. ---------------------------------- //

        initNote();
    }

})(angular);