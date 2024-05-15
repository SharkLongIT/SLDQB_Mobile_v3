(function ($) {
    app.modals.EditModal = function () {
        var _nVNVRecruiterService = abp.services.app.nVNVRecruiter;
        var _geoUnitService = abp.services.app.geoUnit;
        var selectProvinceOfBirth = $('#Province');
        var selectDistrictOfBirth = $('#District');
        var selectVillageOfBirth = $('#Village');
        var _modalManager;
        var _frmMakeAnAppointmentForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmMakeAnAppointmentForm = _modalManager.getModal().find('form[name=RecruiterInfoForm]');

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
                $(selectProvinceOfBirth).val($('#ProvinceId').val()).change();
            });
            loadAddress(selectDistrictOfBirth, $(selectProvinceOfBirth).val(), selectVillageOfBirth);
            function loadAddress(selectDistrict, provinceId, selectVillage) {
                if (provinceId != null) {
                    _geoUnitService.getChildrenGeoUnit(provinceId).done(function (districts) {
                        for (let i = 0; i < districts.items.length; i++) {
                            $(selectDistrict).append($('<option>',
                                {
                                    value: districts.items[i].id,
                                    text: districts.items[i].displayName,
                                }));
                        }
                        $(selectDistrict).val($('#DistrictId').val());
                        if ($(selectDistrict).val() != null) {
                            loadVillage(selectDistrict, selectVillage);
                        }
                    });
                };
            };

            function loadVillage(selectDistrict, selectVillage) {
                _geoUnitService.getChildrenGeoUnit($(selectDistrict).val()).done(function (villages) {
                    for (let i = 0; i < villages.items.length; i++) {
                        $(selectVillage).append(`<option value="${villages.items[i].id}">${villages.items[i].displayName}</option>`);
                    }
                    $(selectVillageOfBirth).val($('#VillageId').val());
                });
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
        }

       

        //sự kiện khi đóng modal
        $('.cancel-work-button').click(function () {
            abp.libs.sweetAlert.config = {
                confirm: {
                    icon: 'warning',
                    buttons: ['Không', 'Có']
                }
            };

            abp.message.confirm(
                app.localize(abp.localization.localize("Close")),
                app.localize(abp.localization.localize("AreYouSure")),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _modalManager.close();
                        return true;

                    }
                }
            );

        });
        this.save = function () {
            _frmMakeAnAppointmentForm.addClass('was-validated');
            if (_frmMakeAnAppointmentForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            var data = _frmMakeAnAppointmentForm.serializeFormToObject();
            _modalManager.setBusy(true);
            _nVNVRecruiterService.update(data)
                .done(function () {
                    _modalManager.close();
                    abp.event.trigger('app.reloadDocTable');
                    abp.notify.info(abp.localization.localize("Successfully"));
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);