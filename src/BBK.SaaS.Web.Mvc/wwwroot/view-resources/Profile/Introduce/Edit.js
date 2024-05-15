(function ($) {
    app.modals.EditModal = function () {
        var _introduceService = abp.services.app.introduce;
        var _modalManager;
        var _$IntroduceForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$IntroduceForm = _modalManager.getModal().find('form[name=EditIntroduceForm]');
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
                _introduceService.update(data).done(function () {
                    _modalManager.close();
                    abp.event.trigger('app.reloadDocTable');
                    abp.notify.info(abp.localization.localize("Cập nhật thành công"));
                }).always(function () {
                    _modalManager.setBusy(false);
                })
            };
        }
    };
})(jQuery);