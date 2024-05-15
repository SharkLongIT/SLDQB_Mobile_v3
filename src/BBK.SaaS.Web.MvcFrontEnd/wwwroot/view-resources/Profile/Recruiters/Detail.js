(function () {
  $(function () {
	_$userInformationForm = $('form[name=UserInformationsForm]');
	_$userInformationForm.validate();

	_$recruiterInfoForm = $('form[name=RecruiterInfoForm]');
	_$recruiterInfoForm.validate();



	var changeProfilePictureModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'App/Profile/ChangePictureModal',
	  scriptUrl: abp.appPath + 'view-resources/App/Profile/_ChangePictureModal.js',
	  modalClass: 'ChangeProfilePictureModal',
	});

	_$userInformationForm.find('#changeProfilePicture')
	  .click(function () {
		changeProfilePictureModal.open({ userId: _$userInformationForm.find('input[name=UserId]').val() });
	  });

	changeProfilePictureModal.onClose(function () {
	  _$userInformationForm.find('.user-edit-dialog-profile-image').attr('src', abp.appPath + "Profile/GetProfilePictureByUser?userId=" + _$userInformationForm.find('input[name=UserId]').val())
	});

	_$recruiterInfoForm.on('click', '.delRecruiterBL', function (e) {
	  e.preventDefault();
		abp.message.confirm(
		  //app.localize('OrganizationUnitDeleteWarningMessage', $(selectedItem[0]).attr('title')),
		  app.localize('Bạn có chắc chắn muốn xóa \"' + $(this[0]).attr('title') + '\" không?', $(this[0]).attr('title')),
		  app.localize('AreYouSure'),
		  function (isConfirmed) {
			if (isConfirmed) {
			 // _categoryUnitService.deleteCatUnit({
				//id: selectedItem[0].dataset.id,
			 // })
				//.done(function () {
				//  abp.notify.success(app.localize('SuccessfullyDeleted'));
				//  //instance.delete_node(node);
				//  //organizationTree.refreshUnitCount();
				//  rootEle.reload();
				//})
				//.fail(function (err) {
				//  setTimeout(function () {
				//	abp.message.error(err.message);
				//  }, 500);
				//});
			}
		  }
		);
	});

	$("#input-11").filestyle({
	  btnClass: "btn-outline-primary",
	  placeholder: "No file",
	  input: false,
	  text: 'Tải lên',
	  htmlIcon: '<i class="fa fa-upload"></i>&nbsp;',
	  'onChange': async function (files) {
		//console.log(files);
		abp.ui.setBusy($('body'));

		let form_data = new FormData();
		jQuery.each(jQuery('#input-11')[0].files, function (i, file) {
		  form_data.append('file', file);
		  form_data.append('recruiterId', _$recruiterInfoForm.find('input[name=RecruiterId]').val());
		});

		await $.ajax({
		  'url': '/Profile/Recruiters/UploadFile',
		  'type': 'POST',
		  'data': form_data,
		  'contentType': false,
		  'processData': false
		}).done(function (results) {
		  console.log(results);

		  $(".recruiterBLUpload").html('');
		  $(".recruiterBLUpload").append(`<a href="`+results.result.files[0].fileUrl+`" target="_blank"><span><i class="f-22 fa fa-file-pdf-o m-r-10"></i>` + results.result.files[0].fileName + `</span></a>								
								<div class="flex-grow-1">
									<span class="badge badge-light-warning m-r-15">On The Way</span>
									<a class="pull-right delRecruiterBL">
										<svg width="24" height="24">
											<use href="/themes/mofi/assets/svg/icon-sprite.svg#recycle"></use>
										</svg>
									</a>
								</div>`);

		  abp.ui.clearBusy($('body'));
		})
		  .catch(e => console.log(e));
	  }
	});

  });
})();
