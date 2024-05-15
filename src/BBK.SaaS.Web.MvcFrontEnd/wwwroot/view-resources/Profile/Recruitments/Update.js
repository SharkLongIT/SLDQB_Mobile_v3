(function ($) {
    $(function () {
        _$FormUpdate = $('form[name=FormUpdate]');
        //var _geoUnitService = abp.services.app.geoUnit;
        var selectProvinceOfBirth = $('#Province');
        var selectDistrictOfBirth = $('#District');

        _$FormUpdate.validate({
            rules: {
                PhoneNumber: 'phoneNumberVN'
            },
        });

        $.ajax({
            url: '/Profile/Recruitments/GetAllGeo',
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
                $(selectProvinceOfBirth).val($("#ProvinceId").val()).change();
                return results;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
        //_geoUnitService.getAll().done(function (items) {
        //    $.each(items, function (i, item) {
        //        if (item.parentId == null) {
        //            selectProvinceOfBirth.append($('<option>',
        //                {
        //                    value: item.id,
        //                    text: item.displayName,
        //                }));
        //        }

        //    });
        //    $(selectProvinceOfBirth).val($("#ProvinceId").val()).change();
        //});

        loadAddress(selectDistrictOfBirth, $(selectProvinceOfBirth).val());

        function loadAddress(selectDistrict, provinceId) {
            if (provinceId != null) {
                $.ajax({
                    url: '/Profile/Recruitments/getChildrenGeoUnit/' + provinceId,
                    type: "get",
                    cache: false,
                    success: function (results) {
                        for (let i = 0; i < results.result.items.length; i++) {
                            $(selectDistrict).append($('<option>',
                                {
                                    value: results.result.items[i].code,
                                    text: results.result.items[i].displayName,
                                }));
                        }
                        $(selectDistrict).val($('#DistrictCode').val());
                        return results;
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("error");
                    }
                });
                //_geoUnitService.getChildrenGeoUnit(provinceId).done(function (districts) {
                //    for (let i = 0; i < districts.items.length; i++) {
                //        $(selectDistrict).append($('<option>',
                //            {
                //                value: districts.items[i].code,
                //                text: districts.items[i].displayName,
                //            }));
                //    }
                //    $(selectDistrict).val($('#DistrictCode').val());
                //});
            };
        };



        function changeStatusSelect(idSelect, stt) {
            $(idSelect).empty();
            $(idSelect).prop('disabled', stt);
        }

        $('#Province').on("change", function () {
            changeStatusSelect(selectDistrictOfBirth, false);
            loadAddress(selectDistrictOfBirth, $('#Province').val(), 'Chọn Quận/Huyện');
        });

        var docTien = new DocTienBangChu();
        var min = docTien.doc($('#MinSalary').val());
        var max = docTien.doc($('#MaxSalary').val());

        $('#result').text(min);
        $('#resultMax').text(max);

        $('#MinSalary').on('input', (e) => {
            var amount = e.target.value;
            var result = docTien.doc(amount);
            $('#result').text(result);
        });

        $('#MaxSalary').on('input', (e) => {
            var amount = e.target.value;
            var result = docTien.doc(amount);
            $('#resultMax').text(result);
        });


        $("#JobDesc").summernote({ height: 270, forcus: true });
        $("#JobRequirementDesc").summernote({ height: 270, forcus: true });
        $("#BenefitDesc").summernote({ height: 270, forcus: true });

        function check() {
            var id1 = parseInt($('#MinAge').val());
            var id2 = parseInt($('#MaxAge').val());
            if (id1 > id2) {
                abp.message.error('', abp.localization.localize("Tuổi tối thiếu không được lớn hơn tuổi tối đa!"));
                return false;
            }
            else {
                return true;
            }
        }

        function checkWage() {
            var id1 = parseInt($('#MinSalary').val());
            var id2 = parseInt($('#MaxSalary').val());
            if (id1 > id2) {
                abp.message.error('', abp.localization.localize("Mức lương tối thiếu không được lớn hơn mức lương tối đa!"));
                return false;
            }
            else {
                return true;
            }
        }
        function checkSummernote() {
            var JobDesc = $('#JobDesc').val();
            var JobRequirementDesc = $('#JobRequirementDesc').val();
            var BenefitDesc = $('#BenefitDesc').val();
            if (JobDesc == "<p><br></p>") {
                abp.message.error('', abp.localization.localize("Mô tả công việc không được để trống!"));
                return false;
            }
            else if (JobRequirementDesc == "<p><br></p>") {
                abp.message.error('', abp.localization.localize("Yêu cầu công việc không được để trống!"));
                return false;
            }
            else if (BenefitDesc == "<p><br></p>") {
                abp.message.error('', abp.localization.localize("Quyền lợi không được để trống!"));
                return false;
            }
            else {
                return true;
            }
        }

        $("#btnUpdate").click(function () {
            _$FormUpdate.addClass('was-validated');
            if (_$FormUpdate[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            else {
                if (check() == true && checkWage() == true && checkSummernote() == true) {
                    var data = _$FormUpdate.serializeFormToObject();
                    data.districtCode = selectDistrictOfBirth.val();
                    data.workAddress = $("#WorkAddress").val();
                    data.provinceId = selectProvinceOfBirth.val();
                    if ($('#Status').is(':checked')) data.Status = true;
                    $.ajax({
                        url: '/Profile/Recruitments/Update',
                        data: data,
                        type: "post",
                        cache: false,
                        success: function (results) {
                            abp.notify.info(app.localize('Cập nhật tin thành công'));
                            window.setTimeout(function () {
                                window.location.href =
                                    "/Profile/Recruitments/Recruitment";
                            }, 2000)
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("error");
                        }
                    });
                }
            }
        });

    });

})(jQuery);