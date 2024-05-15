(function () {
  $(function () {

	$.validator.addMethod("maxDate", function (value, element) {
	  var curDate = new Date();
	  var inputDate = new Date(value);
	  if (inputDate < curDate)
		return true;
	  return false;
	}, "Invalid Date!");
	$.validator.addMethod("date", function (value, element) {
	  var year = new Date(value).getFullYear();
	  if (year < 1900) {
		return false;
	  } else {
		return true
	  }
	}, "Invalid Date!");
	$.validator.addMethod(
	  "minDate",
	  function (value, element) {
		var day = new Date(value).getDay();
		var month = new Date(value).getMonth();
		var year = new Date(value).getFullYear();
		var age = 16;
		var setDate = new Date(year + age, month - 1, day);
		var currdate = new Date();
		if (currdate >= setDate) {
		  return true;
		} else {
		  return false;
		}

	  }

	);
	var GeoUnits = {
	  _geoUnitService: abp.services.app.geoUnit,
	  _Province: $('select#Province'),
	  _District: $('select#District'),
	  _ProvinceOfCandidate: $('input[name=ProvinceId]'),
	  _DistrictOfCandidate: $('input[name=DistrictId]'),
	  reloadProvince: function (data) {
		$.each(data, function (index, item) {
		  if (item.parentId == null) {
			GeoUnits._Province.append($('<option>',
			  {
				value: item.id,
				text: item.displayName,
			  }));
		  }
		})
		if (GeoUnits._ProvinceOfCandidate.val() != null && GeoUnits._ProvinceOfCandidate.val() != '' && GeoUnits._ProvinceOfCandidate.val() != undefined) {
		  GeoUnits._Province.val(GeoUnits._ProvinceOfCandidate.val());
		  GeoUnits.getDistrictfromServer(GeoUnits._ProvinceOfCandidate.val());

		}
		GeoUnits.changeProvince();
	  },
	  changeProvince: function () {
		GeoUnits.getDistrictfromServer(GeoUnits._Province.val())
		GeoUnits._Province.change(function () {
		  GeoUnits.getDistrictfromServer($(this).val())
		});
	  },
	  reloadDistrict: function (data) {
		GeoUnits._District.children().remove();
		$.each(data, function (index, item) {
		  GeoUnits._District.append($('<option>',
			{
			  value: item.id,
			  text: item.displayName,
			}));
		});
		if (GeoUnits._DistrictOfCandidate.val() != null && GeoUnits._DistrictOfCandidate.val() != '' && GeoUnits._DistrictOfCandidate.val() != undefined) {
		  GeoUnits._District.val(GeoUnits._DistrictOfCandidate.val());
		}
	  },
		getProvincefromServer: function () {
		   $.ajax({
                url:  "/UserJob/GetGeoUnit",
                caches: false,
                success: function (result) {
                    GeoUnits.reloadProvince(result.result)
                }
            })
		//GeoUnits._geoUnitService.getGeoUnits({}).done(function (result) {
		//  GeoUnits.reloadProvince(result.items)
		//})
	  },
	  getDistrictfromServer: function (ProvinceId) {
		if (ProvinceId == 0 || ProvinceId == null || ProvinceId == undefined) {
		  return;
		  }
		  $.ajax({
			  url: "/UserJob/GetGeoUnitChildren",
			  data: { id: ProvinceId },
			  caches: false,
			  success: function (result) {
				  GeoUnits.reloadDistrict(result.result)
			  }
		  })
		//GeoUnits._geoUnitService.getChildrenGeoUnit(ProvinceId).done(function (result) {
		//  GeoUnits.reloadDistrict(result.items)
		//})
	  },
	  init: function () {
		GeoUnits.getProvincefromServer();
	  }

	}
	GeoUnits.init();


	_$candidateInfoForm = $('form[name=CandidateInfoForm]');

	_$candidateInfoForm.validate({
	  validClass: "valid",  // default
	  errorClass: "invalid-feedback", // default is "error"
	  highlight: function (element, errorClass, validClass) {
		$(element).addClass('is-invalid').removeClass('is-valid');
	  },
	  unhighlight: function (element, errorClass, validClass) {
		$(element).addClass('is-valid').removeClass('is-invalid');
	  },
	  rules: {
		"Address": {
		  required: true,
		},
		"DateOfBirth": {
		  required: true,
		  date: true,
		  maxDate: true,
		  minDate: true,
		}

	  },
	  messages: {
		"Address": {
		  required: app.localize("Địa chỉ không được để trống"),
		},
		"DateOfBirth": {
		  required: app.localize("Ngày sinh không được để trống"),
		  date: app.localize("Ngày sinh không hợp lệ"),
		  maxDate: app.localize("Ngày sinh không hợp lệ"),
		  minDate: app.localize("Bạn chưa đủ 16 tuổi"),
		}
	  }
	});

        _$candidateInfoForm.find(".updateCandidate")
            .click(function (event) {
                //_$candidateInfoForm.addClass('was-validated');
                if (!_$candidateInfoForm.valid()) { // Not Valid
                    return false;
                } else {
                    var data = _$candidateInfoForm.serializeFormToObject();
                    data.userId = $("input[name=UserId]").val(),
                        data.id = $("input[name=CandidateId]").val(),
                        $.ajax({
                            type: "POST",
                            url: abp.appPath + "Profile/Candidate/Update",
                            data: data,
                            caches: false,
                            success: function (result) {
                                abp.notify.info(app.localize('Cập nhật thành công'));
                                //setTimeout(function () {
                                //    location.reload();
                                //}, 3000);
                                _$candidateInfoForm.find('input').removeClass('is-valid').removeClass('is-invalid');
                                _$candidateInfoForm.find('select').removeClass('is-valid').removeClass('is-invalid');
                            }
                        }).done(function () {

			});
		}


	  })

        
	//Update Account User

	  var handleUpdateUserInfo = function () {
		  _$userInformationForm = $('form[name=UserInformationsForm]');
		  var $submitButton = $('#updateUserType');

		  _$userInformationForm.validate({
			  rules: {
				  PhoneNumber: 'phoneNumberVN'
			  },
		  });

		  _$userInformationForm.keypress(function (e) {
			  if (e.which === 13) {
				  if (_$userInformationForm.valid()) {
					  console.log('submit called');

					  trySubmitForm();
					  //_$userInformationForm.submit();
				  }
				  return false;
			  }
		  });

		  $submitButton.click(function () {
			  trySubmitForm();
		  });

		  function trySubmitForm() {
			  if (!_$userInformationForm.valid()) {
				  return;
			  }

			  abp.ui.setBusy(
				  null,
				  abp
					  .ajax({
						  contentType: app.consts.contentTypes.formUrlencoded,
						  url: _$userInformationForm.attr('action'),
						  data: _$userInformationForm.serialize(),
						  abpHandleError: false,
					  })
					  .done(function (result) {
						  //console.log(result);
						  abp.message.success('Cập nhật thành công').done(function () { location.reload(); });

					  })
					  .fail(function (error) {
						  abp.ajax.showError(error);
					  })
			  );
		  }

		  var changeProfilePictureModal = new app.ModalManager({
			  viewUrl: abp.appPath + 'App/Profile/ChangePictureModal',
			  scriptUrl: abp.appPath + 'view-resources/App/Profile/_ChangePictureModal.js',
			  modalClass: 'ChangeProfilePictureModal',
		  });
	

		  _$userInformationForm.find('#changeProfilePicture')
			  .click(function () {
				  changeProfilePictureModal.open({ userId: _$userInformationForm.find('input[name=UserId]').val() });
			  });

		  _$candidateInfoForm.find('#changeProfileAvatar')
			  .click(function () {
				  changeProfilePictureModal.open({ userId: _$userInformationForm.find('input[name=UserId]').val() });
			  });
		  changeProfilePictureModal.onClose(function () {
			  _$userInformationForm.find('.user-edit-dialog-profile-image').attr('src', abp.appPath + "Profile/GetProfilePictureByUser?userId=" + _$userInformationForm.find('input[name=UserId]').val());
			  _$candidateInfoForm.find('.user-edit-dialog-profile-image').attr('src', abp.appPath + "Profile/GetProfilePictureByUser?userId=" + _$userInformationForm.find('input[name=UserId]').val());
		  });

	  };

	jQuery(document).ready(function () {
	  handleUpdateUserInfo();
	});


  });

})();
