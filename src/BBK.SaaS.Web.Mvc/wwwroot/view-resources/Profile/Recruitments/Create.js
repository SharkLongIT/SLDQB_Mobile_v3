(function ($) {
    $(function () {
        _$createForm = $('form[name=FormCreate]');
        var _geoUnitService = abp.services.app.geoUnit;
        var selectProvinceOfBirth = $('#Province');
        var selectDistrictOfBirth = $('#District');

        _geoUnitService.getAll().done(function (items) {
            $.each(items, function (i, item) {
                if (item.parentId == null) {
                    selectProvinceOfBirth.append($('<option>',
                        {
                            value: item.id,
                            text: item.displayName,
                        }));
                }
            });
        });


        function loadAddress(selectDistrict, provinceId, txt) {
            if (provinceId != null) {
                _geoUnitService.getChildrenGeoUnit(provinceId).done(function (districts) {
                    for (let i = 0; i < districts.items.length; i++) {

                        $(selectDistrict).append($('<option>',
                            {
                                value: districts.items[i].code,
                                text: districts.items[i].displayName,
                            }));
                        //$(selectDistrict).append(`<option value="${districts.items[i].code}">${districts.items[i].displayName}</option>`)
                    }
                });
            };
        };

        function changeStatusSelect(idSelect, stt) {
            $(idSelect).empty();
            $(idSelect).prop('disabled', stt);
        }

        $('#Province').on("change", function () 
        {
            changeStatusSelect(selectDistrictOfBirth, false);
            loadAddress(selectDistrictOfBirth, $('#Province').val(),'Chọn Quận/Huyện');
        });



        $("#JobDesc").summernote({ height: 270, forcus: true });
        $("#JobRequirementDesc").summernote({ height: 270, forcus: true });
        $("#BenefitDesc").summernote({ height: 270, forcus: true });

        function check() {
            var id1 = parseInt($('#MinAge').val());
            var id2 = parseInt($('#MaxAge').val());
            if (id1 > id2) {
                alert('Tuổi tối thiếu lớn hơn tuổi tối đa');
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
                alert('Mức lương tối thiểu lớn hơn Mức lương tối đa');
                return false;
            }
            else {
                return true;
            }
        }

        //$('#btnAdd').click(function () {
        //    $('.AdressForm:first').clone().insertAfter('.AdressForm:last');
        //});


        $("#btnCreate").click(function () {
            _$createForm.addClass('was-validated');
            if (_$createForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            else {
                if (check() == true && checkWage() == true) {
                    var data = _$createForm.serializeFormToObject();
                    data.districtCode = selectDistrictOfBirth.val();
                    data.workAddress = $("#WorkAddress").val();
                    data.provinceId = selectProvinceOfBirth.val();
                    $.ajax({
                        url: '/Profile/Recruitments/Create',
                        data: data,
                        type: "post",
                        cache: false,
                        success: function (results) {
                            abp.ui.setBusy(_$createForm);
                            abp.notify.info(app.localize('Thêm mới thành công'));
                            window.setTimeout(function () {
                                window.location.href =
                                    "/Profile/Recruitments/NVNVRecruiment"
                            }, 2000)
                            return results;
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