$(function () {
    $(".btnPDF").click(function () {
        var IdTemplate = $(this).attr('data-id');
        $.ajax({
            url: '/Profile/Candidate/GenPDF?JobId=' + $("#JobId").val() + "&IdTemplate=" + IdTemplate,
            cache: false,
            success: function (results) {
                abp.notify.info(abp.localization.localize("Gen thanh cong"));
                window.setTimeout(function () {
                    window.location.reload();
                }, 2000)
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
    })
})