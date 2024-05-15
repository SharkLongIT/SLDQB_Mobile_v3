(function () {
  $(function () {
	var _topicService = abp.services.app.topic;	
	var $generalForm = $('form[name=GeneralInfoForm]')

	$generalForm.validate();

	var _insertMediaModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/MediasMgr/InsertMediaModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_InsertMediaModal.js',
	  modalClass: 'InsertMediaModal',
	  cssClass: 'scrollable-modal',
	  modalSize: 'modal-xl'
	});

	//$('#FilterByPermissionsButton').click(function () {
	//  _$permissionFilterModal.open({ grantedPermissionNames: _selectedPermissionNames });
	//});

	//$("#input-11").filestyle({
	//  btnClass: "btn-outline-primary",
	//  placeholder: "No file",
	//  input: false,
	//  text: 'Tải lên',
	//  htmlIcon: '<i class="fa fa-upload"></i>&nbsp;',
	//  'onChange': async function (files) {
	//	//console.log(files);
	//	abp.ui.setBusy($('body'));

	//	let form_data = new FormData();
	//	jQuery.each(jQuery('#input-11')[0].files, function (i, file) {
	//	  form_data.append('file', file);
	//	  form_data.append('recruiterId', _$recruiterInfoForm.find('input[name=RecruiterId]').val());
	//	});

	//	await $.ajax({
	//	  'url': '/Topics/Recruiters/UploadFile',
	//	  'type': 'POST',
	//	  'data': form_data,
	//	  'contentType': false,
	//	  'processData': false
	//	}).done(function (results) {
	//	  console.log(results);

	//	  $(".recruiterBLUpload").html('');
	//	  $(".recruiterBLUpload").append(`<a href="`+results.result.files[0].fileUrl+`" target="_blank"><span><i class="f-22 fa fa-file-pdf-o m-r-10"></i>` + results.result.files[0].fileName + `</span></a>
	//							<div class="flex-grow-1">
	//								<span class="badge badge-light-warning m-r-15">On The Way</span>
	//								<a class="pull-right delRecruiterBL">
	//									<svg width="24" height="24">
	//										<use href="/themes/mofi/assets/svg/icon-sprite.svg#recycle"></use>
	//									</svg>
	//								</a>
	//							</div>`);

	//	  abp.ui.clearBusy($('body'));
	//	})
	//	  .catch(e => console.log(e));
	//  }
	//});

	$('#SaveAllSettingsButton').click(function () {
	  //console.log($('#GeneralInfoForm').serializeFormToObject());

	  if (!$generalForm.valid()) {
		return;
	  }

	  var topicInfo = $generalForm.serializeFormToObject();
	  //topicInfo.body = $('#Body').summernote('code');
	  console.log(topicInfo);

	  _topicService
		.create(topicInfo)
		.done(function () {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		  window.location = '/CMS/Topics';
		});
	});

	$('#CreateAndContinueEdit').click(function () {
	  //console.log($('#GeneralInfoForm').serializeFormToObject());

	  if (!$generalForm.valid()) {
		return;
	  }

	  var topicInfo = $generalForm.serializeFormToObject();
	  //topicInfo.body = $('#Body').summernote('code');
	  console.log(topicInfo);

	  _topicService
		.create(topicInfo)
		.done(function (result) {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		  window.location = '/CMS/Topics';
		});
	});

	$('#BackToList').click(function () {
	  window.location = '/CMS/Topics/Index';
	});

	var cleantHtmlButton = function (itemId) {
	  var ui = $.summernote.ui;
	  var button = ui.button({
		className: 'note-btn-blockquote',
		contents: '<i class="fa fa-child"> Clean HTML</i>',
		//tooltip: 'Blockquote',
		click: function () {
		  //$('#' + itemId).summernote('editor.formatBlock', 'blockquote');
		  _articlesService.cleanHtml({
			Name: 'contentHtml', Value: $('#Body').summernote('code')
		  })
			.done(function (results) {
			  console.log(results);

			  $('#Body').summernote('code', results.value);
			});
		}
	  });

	  return button.render();
	};

	var insertPictureButton = function () {
	  var ui = $.summernote.ui;
	  var button = ui.button({
		className: 'note-btn',
		contents: '<i class="note-icon-picture"> Thêm ảnh từ thư viện</i>',
		//tooltip: 'Blockquote',
		click: function () {

		  $('#Body').summernote('editor.saveRange');

		  _insertMediaModal.open({},
			function (selectedItems) {
			  console.log(selectedItems);
			  $('#Body').summernote('editor.restoreRange');
			  $('#Body').summernote('editor.focus');
			  $('#Body').summernote("insertImage", '/file/get?c=' + selectedItems.publicUrl + '&ver=' + selectedItems.modified);
			});


		}
	  });

	  return button.render();
	}

	$('#Body').summernote({
	  toolbar: [
		['app', ['cleanHtml', 'insertPicture']],
		['style', ['bold', 'italic', 'underline', 'clear']],
		['font', ['strikethrough', 'superscript', 'subscript']],
		['para', ['ul', 'ol', 'paragraph']],
		//['insert', ['link', 'video']],
		['view', ['codeview']]
	  ],
	  buttons: {
		cleanHtml: cleantHtmlButton,
		insertPicture: insertPictureButton,
	  }
	});

  });
})();
