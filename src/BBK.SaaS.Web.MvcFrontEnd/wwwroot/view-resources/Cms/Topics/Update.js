(function () {
  $(function () {
	var _topicService = abp.services.app.topic;
	
	$('#UpdateButton').click(function () {
	  //console.log($('#GeneralInfoForm').serializeFormToObject());

	  var topicInfo = $('form[name=GeneralInfoForm]').serializeFormToObject();
	  //topicInfo.body = $('#Body').summernote('code');
	  console.log(topicInfo);

	_topicService
		.updateTopic(topicInfo)
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
