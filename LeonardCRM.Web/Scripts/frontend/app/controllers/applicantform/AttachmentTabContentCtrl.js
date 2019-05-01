(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("AttachmentTabContentCtrl", ctrl);

    ctrl.$inject = ["$scope", "FileUploader", "orderAttachmentService", "_", "registryService", "$timeout"];

    function ctrl($scope, FileUploader, orderAttachmentService, _, registryService, $timeout) {
        //----------private variables--------------------       


        //----------scope variables--------------------      
        $scope.folderPath = "documents";
        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            url: $scope.UploadHandlerUrl + "?folder=" + encodeURI($scope.folderPath)
        });

        uploader.filters.push({
        	name: 'filterName',
        	fn: function (item) {	            
        	    var extension = item.name.split('.').pop();

        	    if ($scope.modeParam != null && $scope.modeParam != '' && $scope.allowImageExt && $scope.allowImageExt.indexOf('pdf') == -1) {
        	        $scope.allowImageExt.push('pdf');
        	    }

        	    if ($.inArray(extension.toLowerCase(), $scope.allowImageExt) < 0) {
        	        if ($scope.modeParam != null && $scope.modeParam != '') {
        	            NotifyError($scope.languages.COMMON.INVALID_UPLOAD_FILE_FORMAT);
        	        }
        	        else {
        	            NotifyError($scope.languages.COMMON.INVALID_IMAGE_FORMAT);
        	        }		            
        		    return false;
        		}
        		if (item.size > $scope.maxUploadFileSize * 1048576) {
        		    NotifyError($scope.languages.COMMON.MAX_UPLOAD_FILESIZE_ERROR.replace('{0}', $scope.maxUploadFileSize));
        		    return false;
		        }
        		return true;
        	}
        });
        $scope.driverLicenseAttachments = [];
        $scope.isRequireUpdateDocument = false;
        $scope.isShowCam = false;        

        //----------scope methods----------------------     
        $scope.uploadAll = function () {
            $scope.uploader.uploadAll();
        };       

        $scope.deleteAttachment = function (item) {            
            var len = $scope.driverLicenseAttachments.length;
            for (var i = 0; i < len; i++) {
                var file = $scope.driverLicenseAttachments[i];
                if (file.Id == item.Id && file.FileName == item.FileName) {
                    $scope.driverLicenseAttachments.splice(i, 1);
                    break;
                }
            }
            saveAttachment();
        };

        $scope.openCamera = function () {
        	Webcam.set({
        		width: 600,
        		height: 400,
        		dest_width: 1024,
        		dest_height: 768,
        		image_format: 'png'
        	});

        	Webcam.set('constraints', {
        		width: 600,
        		height: 400,
        		dest_width: 1024,
        		dest_height: 768,
        	});

        	Webcam.attach('#camera-section');
        	$scope.isShowCam = true;
        }

        $scope.takeSnapshot = function () {
        	Webcam.snap(function (data_uri) {
        		$scope.driverLicenseAttachments.push(setAttachment(data_uri.replace("data:image/png;base64,", ""), 0));
        		saveAttachment();        		
        	});
        }

        $scope.checkIfShow = function (attachment) {
        	return attachment.FileName.indexOf("/") > -1;
        }

        $scope.closeCamera = function () {
        	Webcam.reset();
        	$("#camera-section").removeAttr("style");
        	$scope.isShowCam = false;
        }

    	//---------event hanlder method-----------------------
        uploader.onCompleteAll = function () {        	
        	$scope.uploader.queue = [];
        	saveAttachment();
        }

        uploader.onCompleteItem = function (item, response, status, headers) {
        	$scope.driverLicenseAttachments.push(setAttachment(response[0].name, 0));
        }

        uploader.onAfterAddingFile = function (item) {
            var msg = validateFile(item.file.name);
        	if (msg != "")//invalid file 
        	{
        		//alert the error message
        		NotifyError(msg);

        		//remove the selected file
        		$scope.uploader.queue.splice($scope.uploader.queue.length - 1, 1);
        	}
        }

        $scope.$on('$destroy', function () {
        	$scope.closeCamera();
        	Webcam.off("error");
        });

        //---------internal method-----------------------
        function init() {
            $scope.uploader.queue = [];
            loadAttachments();

            Webcam.on('error', function (err) {
            	$scope.$apply(function () {
            		$timeout(function () { $scope.closeCamera(); }, 100);
            	});
            });
        }        
               
        function setAttachment(fileName, attachmentId) {
            var attachment = {};
            attachment.OrderId = $scope.applicantId;
            attachment.FileName = fileName;
            attachment.Id = attachmentId;
            attachment.Folder = $scope.folderPath;
            return attachment;
        };
        
        function saveAttachment(extSuccessCallback) {
            orderAttachmentService.SaveAttachment($scope.applicantId, $scope.driverLicenseAttachments)
                                  .then(function (data, status, headers, config) {
                                  	saveAttachmentSuccessCallback(data, extSuccessCallback)
                                  });
        }
        
        function saveAttachmentSuccessCallback(data, extSuccessCallback) {
            if (data.ReturnCode == 200)
            {
                NotifySuccess(data.Result);
                $scope.isRequireUpdateDocument = true;

                if (angular.isFunction(extSuccessCallback)) {
                	extSuccessCallback();
                }
            }
            else
            {
                NotifyError(data.Result);

                //clear the selected file
                $scope.uploader.queue = [];               
            }

        	//reload attachment 
            loadAttachments();
        };

        function loadAttachments() {
            orderAttachmentService.GetAttachmentsByItemId($scope.applicantId)
                .then(getAttachmentsByItemIdSuccessCallBack);
        };

        function getAttachmentsByItemIdSuccessCallBack(data) {
            $scope.driverLicenseAttachments = angular.fromJson(data);
            if ($scope.isRequireUpdateDocument && angular.isDefined($scope.applicant))
            {
                $scope.applicant.SalesOrders[0].SalesDocuments = angular.fromJson(data);
            }
        };

        function validateFile(fileName) {
            var msg = "";

            var dupFile = _.findWithProperty($scope.driverLicenseAttachments, "FileName", fileName);
            var dupQueueFile = _.filter($scope.uploader.queue, function (item) {
                return item.file.name == fileName;
            });

            if (angular.isDefined(dupFile) || dupQueueFile.length > 1) {
                msg += $scope.languages.APPLICANT_FORM.UPLOAD_DUPLICATE_ATTACHMENT_ERROR_MSG;
            }
            return msg;
        }

        init();
    }

})();