(function () {
    var _contactService = abp.services.app.contact;
    _$recruiterInfoForm = $('form[name=FormContact]');
    _$recruiterInfoForm.validate({
        rules: {
            Phone: 'phoneNumberVN'
        },
    });
    $("#btnCreate").click(function () {
       
        _$recruiterInfoForm.addClass('was-validated');
        if (_$recruiterInfoForm[0].checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
            return;
        }
        else {
            var data = _$recruiterInfoForm.serializeFormToObject();
            $.ajax({
                url: '/Contact/Create',
                data: data,
                type: "post",
                cache: false,
                success: function (results) {
                    $("#btnCreate").attr("disabled", true);
                    abp.message.info('Chúng tôi sẽ liên hệ sớm với bạn.', abp.localization.localize("Gửi ý kiến thành công!"));
                    window.setTimeout(function () {
                        window.location.reload();
                    }, 2000)
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });
            //_contactService.create(data).done(function () {
            //    abp.message.info('Chúng tôi sẽ liên hệ sớm với bạn.', abp.localization.localize("Gửi ý kiến thành công!"));
            //    window.setTimeout(function () {
            //        window.location.reload();
            //    }, 2000)
            //})
        }
    });
})();
