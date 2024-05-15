(function ($) {
    app.modals.CreateModal = function () {
        var _Service = abp.services.app.makeAnAppointment;
        var _recruiment = abp.services.app.recruitment;
        var _modalManager;
        var _frmMakeAnAppointmentForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmMakeAnAppointmentForm = _modalManager.getModal().find('form[name=MakeAnAppointmentForm]');
        }

        $('#ApplicationRequestId').change(function () {
            var optionSelected = $(this).find("option:selected");
            var valueSelected = optionSelected.val();
            _recruiment.getDetail(valueSelected).done(function (e) {
                $('#Rank').val(e.rank).trigger("change");
            })
        });

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
            _Service.create(data)
                .done(function () {
                    _modalManager.close();
                    abp.notify.info(abp.localization.localize("Successfully"));
                    window.setTimeout(function () {
                        window.location.reload();
                    }, 2000)
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);