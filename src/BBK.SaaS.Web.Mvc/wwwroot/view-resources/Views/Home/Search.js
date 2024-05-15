$(function () {
    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + "Profile/ApplicationRequest/CreateApplicationRequestModal",
        scriptUrl: abp.appPath + 'view-resources/Profile/ApplicationRequest/CreateApplicationRequest.js',
        modalClass: "ApplicationRequest"
    });

   
    $('.js-example-basic-multiple').select2({
        //multiple: true,
        placeholder: "Chọn địa điểm",
    });


    $(".btnMake").click(function () {
        if ($("#Check").val() == "value") {
            var dataFilter = { RecruitmentId: $(this).attr("data-objid") };
            _Modal.open(dataFilter);
        }
        else {
            abp.message.confirm(
                app.localize(''),
                app.localize('Bạn cần đăng nhập !Nhấn Ok để tiếp tục'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        window.location.href = abp.appPath + "Account/Register";
                    }
                })
        }
    });


})
