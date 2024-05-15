(function () {
  $(function () {
	var _topicService = abp.services.app.topic;	

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

	  var topicInfo = $('form[name=GeneralInfoForm]').serializeFormToObject();
	  //topicInfo.body = $('#Body').summernote('code');
	  console.log(topicInfo);

	  _topicService
		.create(topicInfo)
		.done(function () {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		});
	});


	$('#Body').summernote({ height: 200, });
	//       $('#NewsContent').summernote({
	//   placeholder: 'Hello stand alone ui',
	//   tabsize: 2,
	//   height: 120,
	//   toolbar: [
	//     ['style', ['style']],
	//     ['font', ['bold', 'underline', 'clear']],
	//     ['color', ['color']],
	//     ['para', ['ul', 'ol', 'paragraph']],
	//     ['table', ['table']],
	//     ['insert', ['link', 'picture', 'video']],
	//     ['view', ['fullscreen', 'codeview', 'help']]
	//   ]
	// });

  });
})();
