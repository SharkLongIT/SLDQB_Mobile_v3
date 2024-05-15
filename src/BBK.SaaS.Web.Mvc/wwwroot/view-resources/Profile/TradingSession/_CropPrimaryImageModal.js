(function ($) {
  app.modals.CropPrimaryImageModal = function () {
	var _modalManager;
	var $cropperJsApi = null;

	this.init = function (modalManager) {
	  _modalManager = modalManager;

	  var $profilePictureResize = $('#PrimaryImageResize');
	  $cropperJsApi = $profilePictureResize.cropper({
		aspectRatio: 16 / 9,
		viewMode: 1,
		autoCropArea: 1
	  });

	};

	this.save = function () {
	  $('.preview-primaryimg').attr('src', $cropperJsApi.cropper('getCroppedCanvas').toDataURL("image/png"));
	  _modalManager.close();
	};	
  };
})(jQuery);
