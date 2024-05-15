(function () {
    var _contactService = abp.services.app.contact;
    _$recruiterInfoForm = $('form[name=FormContact]');
    _$recruiterInfoForm.validate();

    $("#btnCreate").click(function () {
        _$recruiterInfoForm.addClass('was-validated');
        if (_$recruiterInfoForm[0].checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
            return;
        }
        else {
            var data = _$recruiterInfoForm.serializeFormToObject();
            _contactService.create(data).done(function () {
                abp.message.info('Chúng tôi sẽ liên hệ sớm với bạn.', abp.localization.localize("Gửi ý kiến thành công!"));
                window.setTimeout(function () {
                    window.location.reload();
                }, 2000)
            })
        }
    });
})();
