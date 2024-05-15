(function () {
  $(function () {
	var _articlesService = abp.services.app.articles;
	var $generalForm = $('form[name=GeneralInfoForm]');
	var $fileToken = $generalForm.find('#FileToken').val();

	$('#StartDate, #EndDate').daterangepicker({
	  timePicker: true,
	  singleDatePicker: true,
	  startDate: moment().startOf('minute'),
	  locale: {
		format: "L LT"
	  },
	}, (start) => $selectedDateTime.startDate = start);

	$generalForm.validate();

	var cropPrimaryImageModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/MediasMgr/CropPrimaryImageModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/Articles/_CropPrimaryImageModal.js',
	  modalClass: 'CropPrimaryImageModal',
	});

	//$('#UserProfileChangePictureLink').click(function (e) {
	//  e.preventDefault();
	//  changeProfilePictureModal.open();
	//});

	$("#uploadPrimaryImg").filestyle({
	  btnClass: "btn-outline-primary",
	  placeholder: "No file",
	  input: false,
	  text: 'Tải lên',
	  htmlIcon: '<i class="fa fa-upload"></i>&nbsp;',
	  'onChange': async function (files) {
		//console.log(files);
		abp.ui.setBusy($('body'));

		let form_data = new FormData();
		jQuery.each(jQuery('#uploadPrimaryImg')[0].files, function (i, file) {
		  form_data.append('file', file);
		  //form_data.append('recruiterId', _$recruiterInfoForm.find('input[name=RecruiterId]').val());
		});

		await $.ajax({
		  'url': '/Cms/Articles/UploadFile',
		  'type': 'POST',
		  'data': form_data,
		  'contentType': false,
		  'processData': false
		}).done(function (results) {
		  console.log(results);

		  $("#PrimaryImage").html('<em class="txt-secondary fw-bold text-underline">' + results.result.files[0].fileName + '</em>');
		  $("#PrimaryImage").attr('href', results.result.files[0].fileUrl);
		  $("#PrimaryImageUrl").val(results.result.files[0].filePath);

		  abp.ui.clearBusy($('body'));
		})
		  .catch(e => console.log(e));
	  }
	});

	var _insertMediaModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/MediasMgr/InsertMediaModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_InsertMediaModal.js',
	  modalClass: 'InsertMediaModal',
	  cssClass: 'scrollable-modal',
	  modalSize: 'modal-xl'
	});

	$('#ChoseAvatarButton').click(function () {
	  _insertMediaModal.open({},
		function (selectedItems) {
		  //console.log(selectedItems);
		  $("#PrimaryImage").html('<em class="txt-secondary fw-bold text-underline">' + selectedItems.filename + '</em>');
		  $("#PrimaryImage").attr('href', '/file/get?c=' + selectedItems.publicUrl + '&ver=' + selectedItems.modified);
		  $("#PrimaryImageUrl").val(selectedItems.publicUrl);

		  cropPrimaryImageModal.open({ token: $fileToken, c: selectedItems.publicUrl });
		});
	});

	$('#UpdateButton').click(function () {
	  var articlesInfo = $('form[name=GeneralInfoForm]').serializeFormToObject();
	  //articlesInfo.body = $('#Body').summernote('code');
	  articlesInfo.PrimaryImageData = $('.preview-primaryimg').attr('src');

	  $.extend(
		articlesInfo,
		$('#SEOForm').serializeFormToObject(),
		$('#SharingForm').serializeFormToObject()
	  );

	  _articlesService
		.update(articlesInfo)
		.done(function () {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		});
	});

	$('#BackToList').click(function () {
	  window.location = '/Cms/Articles/Index';
	});

	$(document).ready(function () {
	  var cleantHtmlButton = function (itemId) {
		var ui = $.summernote.ui;
		var button = ui.button({
		  className: 'note-btn-blockquote',
		  contents: '<i class="fa fa-child"> Clean HTML</i>',
		  //tooltip: 'Blockquote',
		  click: function () {
			//$('#' + itemId).summernote('editor.formatBlock', 'blockquote');
			_articlesService.cleanHtml({
			  Name: 'contentHtml', Value: $('#Content').summernote('code')
			})
			  .done(function (results) {
				console.log(results);

				$('#Content').summernote('code', results.value);
			  });
		  }
		});

		return button.render();
	  }

	  var insertPictureButton = function () {
		var ui = $.summernote.ui;
		var button = ui.button({
		  className: 'note-btn',
		  contents: '<i class="note-icon-picture"> Thêm ảnh từ thư viện</i>',
		  //tooltip: 'Blockquote',
		  click: function () {

			$('#Content').summernote('editor.saveRange');

			_insertMediaModal.open({},
			  function (selectedItems) {
				console.log(selectedItems);
				$('#Content').summernote('editor.restoreRange');
				$('#Content').summernote('editor.focus');
				$('#Content').summernote("insertImage", '/file/get?c=' + selectedItems.publicUrl + '&ver=' + selectedItems.modified);
			  });

		  }
		});

		return button.render();
	  }

	  $('#Content').summernote({
		toolbar: [
		  ['app', ['cleanHtml', 'insertPicture']],
		  ['style', ['bold', 'italic', 'underline', 'clear']],
		  ['font', ['strikethrough', 'superscript', 'subscript']],
		  ['para', ['ul', 'ol', 'paragraph']],
		  ['insert', ['link', 'video']],
		  ['view', ['codeview']]
		],
		buttons: {
		  cleanHtml: cleantHtmlButton,
		  insertPicture: insertPictureButton,

		}
	  });
	});

  });
})();
