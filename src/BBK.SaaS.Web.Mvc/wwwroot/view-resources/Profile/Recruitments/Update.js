(function ($) {
    app.modals.ActiveModal = function () {
        var _$FormUpdate = null;
        //_$FormUpdate = $('form[name=FormUpdate]');
        var _modalManager;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$FormUpdate = _modalManager.getModal().find('form[name=FormUpdate]');
        }
        $("#btnUpdate").click(function () {
            var data = _$FormUpdate.serializeFormToObject();
            _modalManager.setBusy(true);
            if ($('#Status').is(':checked')) data.Status = true;
            $.ajax({
                url: '/Profile/Recruitments/Active',
                data: data,
                type: "post",
                cache: false,
                success: function (results) {
                    _modalManager.close();
                    abp.event.trigger('app.reloadDocTable');
                    abp.notify.info(app.localize('Cập nhật thành công'));
                    //window.setTimeout(function () {
                    //    window.location.href =
                    //        "/Profile/Recruitments/NVNVRecruiment";
                    //}, 2000)
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });

        });
    };

})(jQuery);