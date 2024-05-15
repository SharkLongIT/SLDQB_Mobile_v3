(function ($) {
    app.modals.CreateModal = function () {
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
            let currentDate = new Date().toISOString().slice(0, -8); //yyyy-MM-ddThh:mm
            document.querySelector("#StartTime").min = currentDate;
            document.querySelector("#EndTime").min = currentDate;

            //$("#EndTime").rules('add', { greaterThan: "#StartTime" });
        

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
                //$(selectProvinceOfBirth).val($('#ProvinceId').val()).change();
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
                        //$(selectDistrict).val($('#DistrictId').val());
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
                    //$(selectVillageOfBirth).val($('#VillageId').val());
                });
            }

            function changeStatusSelect(idSelect, stt) {
                $(idSelect).empty();
                $(idSelect).prop('disabled', stt);
            }

            $('#Province').on("change", function () {
                changeStatusSelect(selectDistrictOfBirth, false);
                loadAddress(selectDistrictOfBirth, $('#Province').val(), selectVillageOfBirth);
                //changeStatusSelect(selectVillageOfBirth, false);
            });
            $('#District').on('change', function () {
                loadVillage(selectDistrictOfBirth, selectVillageOfBirth);
                changeStatusSelect(selectVillageOfBirth, false);
            });


            $("#Description").summernote({ height: 270, forcus: true });

            $('#StartTime').on('change', function () {
                var startDate = new Date().toISOString().slice(0, -8);
                var endDate = new Date($('#StartTime').val()).toISOString().slice(0, -8);
                if (endDate < startDate) {
                    abp.message.warn('Giờ không được nhỏ hơn giờ hiện tại.', '');
                    $('#StartTime').val('');
                }
            });

            $('#EndTime').on('change', function () {
                var startDate = $('#StartTime').val();
                var endDate = $('#EndTime').val();
                if (endDate < startDate) {
                    abp.message.warn('Ngày kết thúc không được nhỏ hơn ngày bắt đầu.', '');
                   // alert('Ngày kết thúc không được nhỏ hơn ngày bắt đầu.');
                    $('#EndTime').val('');
                }
            });

            var _insertMediaModal = new app.ModalManager({
                viewUrl: abp.appPath + 'Cms/MediasMgr/InsertMediaModal',
                scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_InsertMediaModal.js',
                modalClass: 'InsertMediaModal',
                cssClass: 'scrollable-modal',
                modalSize: 'modal-xl'
            });

            var cropPrimaryImageModal = new app.ModalManager({
                viewUrl: abp.appPath + 'Cms/MediasMgr/CropPrimaryImageModal',
                scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/_CropPrimaryImageModal.js',
                modalClass: 'CropPrimaryImageModal',
            });

            $('#ChoseAvatarButton').click(function () {
                _insertMediaModal.open({},
                    function (selectedItems) {
                        //console.log(selectedItems);
                        $("#PrimaryImage").html('<em class="txt-secondary fw-bold text-underline">' + selectedItems.filename + '</em>');
                        $("#PrimaryImage").attr('href', '/file/get?c=' + selectedItems.publicUrl + '&ver=' + selectedItems.modified);
                        $("#PrimaryImageUrl").val(selectedItems.publicUrl);

                        cropPrimaryImageModal.open({ c: selectedItems.publicUrl });

                    });
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
            _frmtradingForm.addClass('was-validated');
            if (_frmtradingForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            var data = _frmtradingForm.serializeFormToObject();
            data.imgUrl = $('.preview-primaryimg').attr('src');
            _modalManager.setBusy(true);
            _Service.create(data)
                .done(function () {
                    _modalManager.close();
                    abp.event.trigger('app.reloadDocTable');
                    abp.notify.info(abp.localization.localize("Thêm mới thành công"));
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);