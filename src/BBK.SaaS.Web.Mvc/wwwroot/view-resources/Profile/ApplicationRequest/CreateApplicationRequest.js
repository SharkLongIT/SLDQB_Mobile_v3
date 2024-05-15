(function ($) {
    app.modals.ApplicationRequest = function () {
        var _Service = abp.services.app.applicationRequest;
        var _modalManager;
        var _frmAplicationRequestForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmAplicationRequestForm = _modalManager.getModal().find('form[name=ApplicationRequest]');
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
            _frmAplicationRequestForm.addClass('was-validated');
            if (_frmAplicationRequestForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            var data = _frmAplicationRequestForm.serializeFormToObject();
            _modalManager.setBusy(true);
            _Service.create(data)
                .done(function () {
                    _modalManager.close();
                    abp.notify.info(abp.localization.localize("Successfully"));
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);