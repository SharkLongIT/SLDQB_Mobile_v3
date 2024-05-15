(function () {
  $(function () {
	var _articlesService = abp.services.app.articles;

	$('#StartDate, #EndDate').daterangepicker({
	  timePicker: true,
	  singleDatePicker: true,
	  startDate: moment().startOf('minute'),
	  locale: {
		format: "L LT"
	  },
	}, (start) => $selectedDateTime.startDate = start);

	$('#UpdateButton').click(function () {
	  var articlesInfo = $('form[name=GeneralInfoForm]').serializeFormToObject();
	  //articlesInfo.body = $('#Body').summernote('code');

	  $.extend(
		articlesInfo,
		$('#SEOForm').serializeFormToObject(),
		$('#SharingForm').serializeFormToObject()
	  );
	  console.log(articlesInfo);

	  _articlesService
		.update(articlesInfo)
		.done(function () {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		});
	});

	$('#BackToList').click(function () {
	  window.location = '/Cms/Articles/Index';
	});

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


	  $('#Content').summernote({
		toolbar: [
		  ['style', ['highlight', 'bold', 'italic', 'underline', 'clear']],
		  ['font', ['strikethrough', 'superscript', 'subscript']],
		  ['para', ['ul', 'ol', 'paragraph']],
		  ['view', ['codeview']]
		],
		buttons: {
		  highlight: cleantHtmlButton
		}
	  });
	});

  });
})();
