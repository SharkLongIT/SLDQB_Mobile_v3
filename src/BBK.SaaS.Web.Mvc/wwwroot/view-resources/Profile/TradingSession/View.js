(function ($) {
    app.modals.ViewModal = function () {
        var _Service = abp.services.app.tradingSession;
        var _modalManager;
        var _frmtradingForm = null;
        var _geoUnitService = abp.services.app.geoUnit;
        var selectProvinceOfBirth = $('#Province');
        var selectDistrictOfBirth = $('#District');
        var selectVillageOfBirth = $('#Village');

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmtradingForm = _modalManager.getModal().find('form[name=CreateTradingForm]');

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
                //changeStatusSelect(selectDistrictOfBirth, false);
                loadAddress(selectDistrictOfBirth, $('#Province').val(), selectVillageOfBirth);
                //changeStatusSelect(selectVillageOfBirth, false);
            });
            $('#District').on('change', function () {
                loadVillage(selectDistrictOfBirth, selectVillageOfBirth);
                //changeStatusSelect(selectVillageOfBirth, false);
            });


            $("#Description").summernote({ height: 270, forcus: true });
            $('#Description').summernote('disable');
        }
    };
})(jQuery);