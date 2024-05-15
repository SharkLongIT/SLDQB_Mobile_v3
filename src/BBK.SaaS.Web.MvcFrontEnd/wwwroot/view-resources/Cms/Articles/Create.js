(function () {
  $(function () {
	var _articlesService = abp.services.app.articles;

	var $selectedDateTime = {
	  startDate: moment()
	};

	$('#StartDate, #EndDate').daterangepicker({
	  timePicker: true,
	  singleDatePicker: true,
	  startDate: moment().startOf('minute'),
	  locale: {
		format: "L LT"
	  },
	}, (start) => $selectedDateTime.startDate = start);

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

	$('#UploadAvatarButton').click(function () {
	  
	});

	$('#CreateNewButton').click(function () {
	  //console.log($('#GeneralInfoForm').serializeFormToObject());

	  var articlesInfo = $('form[name=GeneralInfoForm]').serializeFormToObject();
	  //articlesInfo.body = $('#Body').summernote('code');

	  $.extend(
		articlesInfo,
		$('#SEOForm').serializeFormToObject(),
		$('#SharingForm').serializeFormToObject()
	  );

	  console.log(articlesInfo);

	  _articlesService
		.create(articlesInfo)
		.done(function () {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		  window.location = '/Cms/Articles/Index';
		});
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
			Name: 'contentHtml', Value: $('#Content').summernote('code')
		  })
			.done(function (results) {
			  console.log(results);

			  $('#Content').summernote('code', results.value);
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

			$('#Content').summernote('editor.saveRange');

		  _insertMediaModal.open({},
			function (selectedItems) {
			  console.log(selectedItems);
			  $('#Content').summernote('editor.restoreRange');
			  $('#Content').summernote('editor.focus');
			  $('#Content').summernote("insertImage", selectedItems);
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

	$(document).ready(function () {


	});

  });
})();
