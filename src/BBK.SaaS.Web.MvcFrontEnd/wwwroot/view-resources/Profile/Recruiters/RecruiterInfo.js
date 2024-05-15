(function () {
    $(function () {
        var _geoUnitService = abp.services.app.geoUnit;
        var selectProvinceOfBirth = $('#Province');
        var selectDistrictOfBirth = $('#District');
        var selectVillageOfBirth = $('#Village');
        let userAvatar = null;
        let userCover = null;

        _$recruiterInfoForm = $('form[name=RecruiterInfoForm]');
        //_$recruiterInfoForm.validate();

        _$recruiterInfoForm.validate({
            rules: {
                ContactPhone: 'phoneNumberVN'
            },
        });
        var changeProfilePictureModal = new app.ModalManager({
            viewUrl: abp.appPath + 'App/Profile/ChangePictureModal',
            scriptUrl: abp.appPath + 'view-resources/App/Profile/_ChangePictureModal.js',
            modalClass: 'ChangeProfilePictureModal',
        });


        $.ajax({
            url: '/Profile/Recruiters/GetAllGeo',
            type: "get",
            cache: false,
            success: function (results) {
                $.each(results.result, function (i, item) {
                    if (item.parentId == null) {
                        selectProvinceOfBirth.append($('<option>',
                            {
                                value: item.id,
                                text: item.displayName,
                            }));
                    }
                });
                $(selectProvinceOfBirth).val($('#ProvinceId').val()).change();
                return results;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });

        //_geoUnitService.getAll().done(function (items) {
        //  $.each(items, function (i, item) {
        //	if (item.parentId == null) {
        //	  selectProvinceOfBirth.append($('<option>',
        //		{
        //		  value: item.id,
        //		  text: item.displayName,
        //		}));
        //	}

        //  });
        //  $(selectProvinceOfBirth).val($('#ProvinceId').val()).change();
        //});
        loadAddress(selectDistrictOfBirth, $(selectProvinceOfBirth).val(), selectVillageOfBirth);
        function loadAddress(selectDistrict, provinceId, selectVillage) {
            if (provinceId != null) {
                $.ajax({
                    url: '/Profile/Recruiters/getChildrenGeoUnit/' + provinceId,
                    type: "get",
                    cache: false,
                    success: function (results) {
                        for (let i = 0; i < results.result.items.length; i++) {
                            $(selectDistrict).append($('<option>',
                                {
                                    value: results.result.items[i].id,
                                    text: results.result.items[i].displayName,
                                }));
                            //$(selectDistrict).append(`<option value="${districts.items[i].code}">${districts.items[i].displayName}</option>`)
                        }
                        $(selectDistrict).val($('#DistrictId').val());
                        if ($(selectDistrict).val() != null) {
                            loadVillage(selectDistrict, selectVillage);
                        }
                        return results;
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("error");
                    }
                });
                //_geoUnitService.getChildrenGeoUnit(provinceId).done(function (districts) {
                //  for (let i = 0; i < districts.items.length; i++) {
                //	$(selectDistrict).append($('<option>',
                //	  {
                //		value: districts.items[i].id,
                //		text: districts.items[i].displayName,
                //	  }));
                //  }

                //});
            };
        };

        function loadVillage(selectDistrict, selectVillage) {
            $.ajax({
                url: '/Profile/Recruiters/getChildrenGeoUnit/' + $(selectDistrict).val(),
                type: "get",
                cache: false,
                success: function (results) {
                    for (let i = 0; i < results.result.items.length; i++) {
                        $(selectVillage).append(`<option value="${results.result.items[i].id}">${results.result.items[i].displayName}</option>`);
                        //$(selectDistrict).append(`<option value="${districts.items[i].code}">${districts.items[i].displayName}</option>`)
                    }
                    $(selectVillageOfBirth).val($('#VillageId').val());
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });
            //  _geoUnitService.getChildrenGeoUnit($(selectDistrict).val()).done(function (villages) {
            //	for (let i = 0; i < villages.items.length; i++) {
            //	  $(selectVillage).append(`<option value="${villages.items[i].id}">${villages.items[i].displayName}</option>`);
            //	}
            //	$(selectVillageOfBirth).val($('#VillageId').val());
            //  });
        }

        function changeStatusSelect(idSelect, stt) {
            $(idSelect).empty();
            $(idSelect).prop('disabled', stt);
        }

        $('#Province').on("change", function () {
            changeStatusSelect(selectDistrictOfBirth, false);
            loadAddress(selectDistrictOfBirth, $('#Province').val(), selectVillageOfBirth);
            changeStatusSelect(selectVillageOfBirth, false);
        });
        $('#District').on('change', function () {
            loadVillage(selectDistrictOfBirth, selectVillageOfBirth);
            changeStatusSelect(selectVillageOfBirth, false);
        });





        $("#fileCover").bind("change", function () {
            let fileData = $(this).prop("files")[0];
            let math = ["image/png", "image/jpg", "image/jpeg"];
            if (!fileData) {
                $("#outputcover").attr("src", null);
                return false;
            }
            if ($.inArray(fileData.type, math) === -1) {
                alert("Kiểu file không hợp lệ, chỉ chấp nhận jpg & png");
                $(this).val(null);
                return false;
            }
            if (typeof (FileReader) != "undefined") {
                let imagePreview = $("#image-cover");
                imagePreview.empty();
                let fileReader = new FileReader();
                fileReader.onload = function (element) {
                    $("<img>", {
                        "src": element.target.result,
                        "id": "outputcover",
                        "alt": "cover image",
                        "style": "width:400px; height: 200px"
                    }).appendTo(imagePreview);
                }
                imagePreview.show();
                fileReader.readAsDataURL(fileData);
                coverImage = fileData;

                let formData = new FormData();
                formData.append("file", fileData);
                formData.append("id", $('#UserId').val());
                userCover = formData;
                $.ajax({
                    'url': '/Profile/Recruiters/UploadFile',
                    'type': 'POST',
                    'data': formData,
                    'contentType': false,
                    'processData': false
                }).done(function (results) {
                    $("#outputcover").val(results.result.files[0].fileUrl);
                    abp.ui.clearBusy($('body'));
                })
                    .catch(e => console.log(e));

            } else {
                alert("Trình duyệt không hỗ trợ đọc file.");
            }
        });
        $(document).on('click', '#upload_image_cover', function () {

            $(".file_img_cover").click();
        });

        //$("#input-11").filestyle({
        //    btnClass: "btn-outline-primary",
        //    placeholder: "No file",
        //    input: false,
        //    text: 'Tải lên',
        //    htmlIcon: '<i class="fa fa-upload"></i>&nbsp;',
        //    'onChange': async function (files) {
        //        console.log(files);
        //        abp.ui.setBusy($('body'));

        //        let form_data = new FormData();
        //        jQuery.each(jQuery('#input-11')[0].files, function (i, file) {
        //            form_data.append('file', file);
        //            form_data.append('id', _$recruiterInfoForm.find('input[name=Id]').val());
        //        });

        //        await $.ajax({
        //            'url': '/Profile/Recruiters/UploadFile',
        //            'type': 'POST',
        //            'data': form_data,
        //            'contentType': false,
        //            'processData': false
        //        }).done(function (results) {
        //            $(".recruiterBLUpload").html('');
        //            $(".recruiterBLUpload").append(`<a href="` + results.result.files[0].fileUrl + `" target="_blank"><span><i class="f-22 fa fa-file-pdf-o m-r-10"></i>` + results.result.files[0].fileName + `</span></a>								
        //<div class="flex-grow-1">
        //	<span class="badge badge-light-warning m-r-15">On The Way</span>
        //	<a class="pull-right delRecruiterBL">
        //		<svg width="24" height="24">
        //			<use href="/themes/mofi/assets/svg/icon-sprite.svg#recycle"></use>
        //		</svg>
        //	</a>
        //</div>`);

        //            abp.ui.clearBusy($('body'));
        //        })
        //            .catch(e => console.log(e));
        //    }
        //});

        $("#btnCreate").click(function () {
            _$recruiterInfoForm.addClass('was-validated');
            if (_$recruiterInfoForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            else {
                var data = _$recruiterInfoForm.serializeFormToObject();
                $.ajax({
                    url: '/Profile/Recruiters/Create',
                    data: data,
                    type: "post",
                    cache: false,
                    success: function (results) {
                        abp.notify.info(abp.localization.localize("Tạo mới thành công"));
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("error");
                    }
                });
            }
        });

        $("#btnUpdate").click(function () {
            _$recruiterInfoForm.addClass('was-validated');
            if (_$recruiterInfoForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            else {
                var data = _$recruiterInfoForm.serializeFormToObject();

                if ($("#output").val() != "") {
                    data.avatarUrl = $("#output").val();
                }
                else {
                    data.avatarUrl = $("#AvatarUrl").val();
                }

                if ($("#outputcover").val() != "") {
                    data.imageCoverUrl = $("#outputcover").val();
                }
                else {
                    data.imageCoverUrl = $("#ImageCoverUrl").val();
                }
                $.ajax({
                    url: '/Profile/Recruiters/Update',
                    data: data,
                    type: "post",
                    cache: false,
                    success: function (results) {
                        abp.notify.info(abp.localization.localize("Cập nhật thành công"));
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("error");
                    }
                });
            }
        });

        var handleUpdateUserInfo = function () {
            _$userInformationForm = $('form[name=UserInformationsForm]');
            var $submitButton = $('#btnCapNhatAjax');

            _$userInformationForm.validate({
                rules: {
                    PhoneNumber: 'phoneNumberVN'
                },
                //submitHandler: function (form) {
                //  console.log('submit called');
                //  form.submit();
                //  //trySubmitForm();
                //},
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

            //console.log('handle done!');

            _$userInformationForm.find('#changeProfilePicture')
                .click(function () {
                    changeProfilePictureModal.open({ userId: _$userInformationForm.find('input[name=UserId]').val() });
                });

            changeProfilePictureModal.onClose(function () {
                _$userInformationForm.find('.user-edit-dialog-profile-image').attr('src', abp.appPath + "Profile/GetProfilePictureByUser?userId=" + _$userInformationForm.find('input[name=UserId]').val());
                _$recruiterInfoForm.find('.user-edit-dialog-profile-image').attr('src', abp.appPath + "Profile/GetProfilePictureByUser?userId=" + _$userInformationForm.find('input[name=UserId]').val());
            });

            $("#file").bind("change", function () {
                let fileData = $(this).prop("files")[0];
                let math = ["image/png", "image/jpg", "image/jpeg"];
                if (!fileData) {
                    $("#output").attr("src", null);
                    return false;
                }
                if ($.inArray(fileData.type, math) === -1) {
                    alert("Kiểu file không hợp lệ, chỉ chấp nhận jpg & png");
                    $(this).val(null);
                    return false;
                }
                if (typeof (FileReader) != "undefined") {
                    let imagePreview = $("#image-cover-thumbnail");
                    imagePreview.empty();
                    let fileReader = new FileReader();
                    fileReader.onload = function (element) {
                        $("<img>", {
                            "src": element.target.result,
                            "id": "output",
                            "alt": "cover image",
                            "style": "width:264.463px; height: 150px"
                        }).appendTo(imagePreview);
                    }
                    imagePreview.show();
                    fileReader.readAsDataURL(fileData);
                    coverImage = fileData;

                    let formData = new FormData();
                    formData.append("file", fileData);
                    formData.append("id", $('#UserId').val());
                    userAvatar = formData;
                    $.ajax({
                        'url': '/Profile/Recruiters/UploadFile',
                        'type': 'POST',
                        'data': formData,
                        'contentType': false,
                        'processData': false
                    }).done(function (results) {
                        $("#output").val(results.result.files[0].fileUrl);

                        abp.ui.clearBusy($('body'));
                    })
                        .catch(e => console.log(e));

                } else {
                    alert("Trình duyệt không hỗ trợ đọc file.");
                }
            });
            $(document).on('click', '#upload_image', function () {

                $(".file_img_avatar").click();
            });


        };

        jQuery(document).ready(function () {
            handleUpdateUserInfo();
        });

    });
})();
