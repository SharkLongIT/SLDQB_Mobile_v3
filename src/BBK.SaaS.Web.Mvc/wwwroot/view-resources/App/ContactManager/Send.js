(function ($) {
    app.modals.SendModal = function () {
        var _contactService = abp.services.app.contact;
         var _modalManager;
        var _frmEditContactForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmEditContactForm = _modalManager.getModal().find('form[name=EditContactForm]');
        }

      

      
        this.save = function () {
            var data = _frmEditContactForm.serializeFormToObject();
            _modalManager.setBusy(true);
            _contactService.sendMail(data).done(function () {
                _modalManager.close();
                abp.event.trigger('app.reloadDocTable');
                abp.message.info('.', abp.localization.localize("Gửi câu trả lời thành công!"));
            }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})();
