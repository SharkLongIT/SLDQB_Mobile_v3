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
        this.save = function () {
            _frmAplicationRequestForm.addClass('was-validated');
            if (_frmAplicationRequestForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            var data = _frmAplicationRequestForm.serializeFormToObject();
            _modalManager.setBusy(true);
            $.ajax({
                url: '/Profile/ApplicationRequest/CreateUT',
                data: data,
                type: "post",
                cache: false,
                success: function (results) {
                    _modalManager.close();
                    abp.notify.info(abp.localization.localize("Ứng tuyển thành công"));
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
                //});
                //_Service.create(data)
                //    .done(function () {
                //        _modalManager.close();
                //        abp.notify.info(abp.localization.localize("Ứng tuyển thành công"));
                //    }).always(function () {
                //        _modalManager.setBusy(false);
                //    });
            });
        };
    };
})(jQuery);