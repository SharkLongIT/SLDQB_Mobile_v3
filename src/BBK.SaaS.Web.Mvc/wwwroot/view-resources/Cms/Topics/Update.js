(function () {
  $(function () {
	var _topicService = abp.services.app.topic;
	var $generalForm = $('form[name=GeneralInfoForm]')

	$generalForm.validate();

	$('#UpdateButton').click(function () {
	  //console.log($('#GeneralInfoForm').serializeFormToObject());

	  var topicInfo = $('form[name=GeneralInfoForm]').serializeFormToObject();
	  //topicInfo.body = $('#Body').summernote('code');
	  //console.log(topicInfo);

	  _topicService
		.updateTopic(topicInfo)
		.done(function () {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		});
	});

	$('#BackToList').click(function () {
	  window.location = '/CMS/Topics/Index';
	});

	var _insertMediaModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/MediasMgr/InsertMediaModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_InsertMediaModal.js',
	  modalClass: 'InsertMediaModal',
	  cssClass: 'scrollable-modal',
	  modalSize: 'modal-xl'
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
	  }

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
		  ['insert', ['link', 'video']],
		  ['view', ['codeview']]
		],
		buttons: {
		  cleanHtml: cleantHtmlButton,
		  insertPicture: insertPictureButton,

		}
	  });

  });
})();
