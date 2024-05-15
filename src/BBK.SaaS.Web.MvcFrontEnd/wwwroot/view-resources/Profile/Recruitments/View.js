(function ($) {
    //#region nav chuyển từ lịch hẹn sang chi tiết 
    $(document).ready(function () {
        var referrer = document.referrer;
        if (referrer.includes("Profile/Candidate/MakeAnAppointment") == false) {
            $('nav.nav-makeanappiontment').hide()
        }
    });
    //#endregion
    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + "Profile/ApplicationRequest/CreateApplicationRequestModal",
        scriptUrl: abp.appPath + 'view-resources/Profile/ApplicationRequest/CreateApplicationRequest.js',
        modalClass: "ApplicationRequest"
    });

    $('#btnUngTuyen').click(function () {
        if ($("#Check").val() == "value") {
            var dataFilter = { RecruitmentId: $("#Id").val() };
            _Modal.open(dataFilter);
        }
        else {
            abp.message.confirm(
                app.localize(''),
                app.localize('Bạn cần đăng nhập !Nhấn Ok để tiếp tục'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        window.location.href = abp.appPath + "Account/Login";
                    }
                })
        }
    })

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
                        window.location.href = abp.appPath + "Account/Login";
                    }
                })
        }
    });
  
})(jQuery);