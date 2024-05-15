$(function () {

    let searchParams = new URLSearchParams(window.location.search)

    let param = searchParams.getAll('WorkSite')


    $('.js-example-basic-multiple-search').select2({
        placeholder: 'Tất cả địa điểm',
    });
    $('.js-example-basic-multiple-search').val(param).change();


    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + "Profile/ApplicationRequest/CreateApplicationRequestModal",
        scriptUrl: abp.appPath + 'view-resources/Profile/ApplicationRequest/CreateApplicationRequest.js',
        modalClass: "ApplicationRequest"
    });

   
    $('.SalaryMin').on('input', function (e) {
        $(this).val(formatCurrency(this.value.replace(/[,$]/g, '')));
    }).on('keypress', function (e) {
        if (!$.isNumeric(String.fromCharCode(e.which))) e.preventDefault();
    }).on('paste', function (e) {
        var cb = e.originalEvent.clipboardData || window.clipboardData;
        if (!$.isNumeric(cb.getData('text'))) e.preventDefault();
    });

    $('.SalaryMax').on('input', function (e) {
        $(this).val(formatCurrency(this.value.replace(/[,$]/g, '')));
    }).on('keypress', function (e) {
        if (!$.isNumeric(String.fromCharCode(e.which))) e.preventDefault();
    }).on('paste', function (e) {
        var cb = e.originalEvent.clipboardData || window.clipboardData;
        if (!$.isNumeric(cb.getData('text'))) e.preventDefault();
    });

    function formatCurrency(number) {
        var n = number.split('').reverse().join("");
        var n2 = n.replace(/\d\d\d(?!$)/g, "$&,");
        return n2.split('').reverse().join('');
    }


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

})
