(function ($) {
    app.modals.CreateModal = function () {
        var _introduceService = abp.services.app.introduce;
        var _modalManager;
        var _$IntroduceForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$IntroduceForm = _modalManager.getModal().find('form[name=CreateIntroduceForm]');

              document.getElementById("PhoneNumber").addEventListener("input", function () {
                var valueChange = funcChanePhoneNumber();
                  _$IntroduceForm.find('input[name=Phone]').val(valueChange);
            });
            function funcChanePhoneNumber() {
                var valueChange = null;
                var valueInputPhone = _$IntroduceForm.find('input[name=Phone]').val();
                if (valueInputPhone.substring(0, 1) == 0) {
                    valueChange = _$IntroduceForm.find('input[name=Phone]').val().replace('0', '');
                } else {
                    valueChange = _$IntroduceForm.find('input[name=Phone]').val().replace(/[^0-9]/g, '');
                }
                return valueChange;
            }
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
            _$IntroduceForm.addClass('was-validated');
            if (_$IntroduceForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            else {
                var data = _$IntroduceForm.serializeFormToObject();
                _modalManager.setBusy(true);
                _introduceService.create(data).done(function () {
                    _modalManager.close();
                    abp.notify.info(abp.localization.localize("Giới thiệu thành công"));
                    window.setTimeout(function () {
                        window.location.reload();
                    }, 1000)
                })
            };
        }
    };
})(jQuery);