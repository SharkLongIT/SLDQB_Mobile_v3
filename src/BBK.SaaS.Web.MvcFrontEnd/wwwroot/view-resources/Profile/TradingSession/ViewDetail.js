(function () {

    $(".btnMake").click(function () {
        var data = {};
        data.tradingSessionId = $(this).attr("data-objid");
        data.status = 1;
        $.ajax({
            url: '/TradingSession/Check?TradingSessionId=' + data.tradingSessionId,
            cache: false,
            success: function (results) {
                if (results.result == 2) {
                    abp.message.warn('Bạn đã tham gia!', '');
                }
                else if (results.result == 1) {
                    abp.message.confirm(
                        app.localize('Tham gia phiên giao dịch'),
                        app.localize('AreYouSure'),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                $.ajax({
                                    url: '/TradingSession/UpdateStatus',
                                    data: data,
                                    type: "post",
                                    cache: false,
                                    success: function (results) {
                                        abp.notify.info(app.localize('Tham gia thành công'));
                                        window.setTimeout(function () {
                                            window.location.reload();
                                        }, 2000)
                                        return results;
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert("error");
                                    }
                                });
                            }
                        }
                    );
                }
                else {
                    abp.message.confirm(
                        app.localize('Tham gia phiên giao dịch'),
                        app.localize('AreYouSure'),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                $.ajax({
                                    url: '/TradingSession/Create',
                                    data: data,
                                    type: "post",
                                    cache: false,
                                    success: function (results) {
                                        abp.notify.info(app.localize('Tham gia thành công'));
                                        window.setTimeout(function () {
                                            window.location.reload();
                                        }, 2000)
                                        return results;
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert("error");
                                    }
                                });
                            }
                        }
                    );
                }

                return results;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
            }
        });
    });
})(jQuery);
