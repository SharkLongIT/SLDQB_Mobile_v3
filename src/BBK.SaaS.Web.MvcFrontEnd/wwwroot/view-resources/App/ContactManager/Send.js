(function () {
    var _contactService = abp.services.app.contact;
    _$recruiterInfoForm = $('form[name=FormContact]');
    _$recruiterInfoForm.validate();

    $("#btnSend").click(function () {
        var data = {};
        data.email = $("#Email").val();
        data.answer = $("#Reply").val();
        data.id = $("#Id").val();
        _contactService.sendMail(data).done(function () {
            abp.message.info('.', abp.localization.localize("Gửi ý kiến thành công!"));
            window.location.href =
                "/App/ContactManager";
        })

    });
})();
