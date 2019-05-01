(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("noteService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/NoteApi/';

        var getNotesByRecordId = function (note) {
            return serviceHelper.post(url + 'GetNotesByRecordId', note);
        };

        var saveNote = function (note) {
            return serviceHelper.post(url + 'EditNote', note);
        };

        var deleteNote = function (id, moduleId) {
            return serviceHelper.get(url + 'DeleteNote/?id=' + id + '&mid=' + moduleId);
        };

        return {
            GetNotesByRecordId: getNotesByRecordId,
            SaveNote: saveNote,
            DeleteNote: deleteNote
        };
    }

})(angular);