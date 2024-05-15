(function () {
   // var _TradingService = abp.services.app.tradingSession;
    //var _TradingAccountService = abp.services.app.tradingSessionAccount;

    let searchParams = new URLSearchParams(window.location.search)

    let param = searchParams.getAll('WorkSite')


    $('.js-example-basic-multiple-search').select2({
        placeholder: 'Tất cả địa điểm',
    });
    $('.js-example-basic-multiple-search').val(param).change();


    //$(".btnMake").click(function () {
    //    var data = {};
    //    data.tradingSessionId = $(this).attr("data-objid");
    //    data.status = 1;

    //    $.ajax({
    //        url: '/TradingSession/Check?TradingSessionId=' + data.tradingSessionId,
    //        cache: false,
    //        success: function (results) {
    //            if (results.result == false) {
    //                abp.message.warn('Bạn đã tham gia!', '');
    //            }
    //            else {
    //                abp.message.confirm(
    //                    app.localize('Tham gia phiên giao dịch'),
    //                    app.localize('AreYouSure'),
    //                    function (isConfirmed) {
    //                        if (isConfirmed) {
    //                            $.ajax({
    //                                url: '/TradingSession/Create',
    //                                data: data,
    //                                type: "post",
    //                                cache: false,
    //                                success: function (results) {
    //                                    abp.notify.info(app.localize('Tham gia thành công'));
    //                                    window.setTimeout(function () {
    //                                        window.location.reload();
    //                                    }, 2000)
    //                                    return results;
    //                                },
    //                                error: function (xhr, ajaxOptions, thrownError) {
    //                                    alert("error");
    //                                }
    //                            });
    //                        }
    //                    }
    //                );
    //            }


    //            return results;
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert("error");
    //        }
    //    });



    //});
})(jQuery);
