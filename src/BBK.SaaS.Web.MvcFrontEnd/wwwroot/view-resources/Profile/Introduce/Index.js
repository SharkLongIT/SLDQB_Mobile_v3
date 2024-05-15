(function () {
    var _introduceService = abp.services.app.introduce;
    _$IntroduceForm = $('form[name=CreateIntroduceForm]');
    var _CreateModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Article/CreateIntroduce',
        scriptUrl: abp.appPath + 'view-resources/Profile/Introduce/Create.js',
        modalClass: 'CreateModal',
        modalType: 'modal-xl'
    });

    _$IntroduceForm.validate({
        rules: {
            Phone: 'phoneNumberVN'
        },
    });

    $.ajax({
        url: '/Article/GetCountByUserId',
        type: "get",
        cache: false,
        success: function (value) {
            $("#btnIntroduce").click(function () {
                if (value.result >= 10 && abp.session.userId != null) {
                    abp.message.warn("", abp.localization.localize("Bạn đã giới thiệu 10 lần trong ngày hôm nay"));
                }
                else {
                    _$IntroduceForm.addClass('was-validated');
                    if (_$IntroduceForm[0].checkValidity() === false) {
                        event.preventDefault();
                        event.stopPropagation();
                        return;
                    }
                    else {
                        var data = _$IntroduceForm.serializeFormToObject();
                        $.ajax({
                            url: '/Article/Create',
                            data: data,
                            type: "post",
                            cache: false,
                            success: function (results) {
                                abp.notify.info(abp.localization.localize("Giới thiệu thành công"));
                                window.setTimeout(function () {
                                    window.location.reload();
                                }, 1000)
                                return results;
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert("error");
                            }
                        });
                    }
                }
            });
            return value;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("error");
        }
    });


    //_introduceService.getCountByUserId().done(function (value) {
    //    $("#btnIntroduce").click(function () {
    //        if (value >= 10 && abp.session.userId != null) {
    //            abp.message.warn("", abp.localization.localize("Bạn đã giới thiệu 10 lần trong ngày hôm nay"));
    //        }
    //        else {
    //            _$IntroduceForm.addClass('was-validated');
    //            if (_$IntroduceForm[0].checkValidity() === false) {
    //                event.preventDefault();
    //                event.stopPropagation();
    //                return;
    //            }
    //            else {
    //                var data = _$IntroduceForm.serializeFormToObject();
    //                $.ajax({
    //                    url: '/Article/Create',
    //                    data: data,
    //                    type: "post",
    //                    cache: false,
    //                    success: function (results) {
    //                        abp.notify.info(abp.localization.localize("Giới thiệu thành công"));
    //                        window.setTimeout(function () {
    //                            window.location.reload();
    //                        }, 1000)
    //                        return results;
    //                    },
    //                    error: function (xhr, ajaxOptions, thrownError) {
    //                        alert("error");
    //                    }
    //                });
    //            }
    //        }
    //    });
    //});


})(jQuery);