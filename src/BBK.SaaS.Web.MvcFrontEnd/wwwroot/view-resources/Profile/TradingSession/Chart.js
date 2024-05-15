(function ($) {
    var _service = abp.services.app.tradingSessionAccount;
    google.charts.load("current", { packages: ["corechart", "bar"] });
    google.charts.setOnLoadCallback(drawBasic);
    function drawBasic() {
        if ($("#column-chart1").length > 0) {


            $.ajax({
                url: '/TradingSession/GetByChart/' + $("#Id").val() ,
                type: "get",
                cache: false,
                success: function (results) {
                    var dataArray = [["Ngành", "Nhà tuyển dụng", "Người lao động"]];
                    $.each(results.result.listReport, function (i, v) {
                        dataArray.push([v.nameRecruitment, v.countRecruiment, v.countJob]);
                        var a = google.visualization.arrayToDataTable(dataArray),
                            b = {
                                chart: {
                                    title: "Top ngành tuyển dụng",
                                },
                                bars: "vertical",
                                vAxis: {
                                    format: "decimal",
                                },
                                height: 400,
                                width: "100%",
                                colors: [
                                    "#c6164f",
                                    "#372363",
                                    "#51bb25",
                                ],
                            },
                            c = new google.charts.Bar(document.getElementById("column-chart1"));
                        c.draw(a, google.charts.Bar.convertOptions(b));
                    });
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
            });

            //_service.getByChart($("#Id").val()).done(function (value) {
            //    var dataArray = [["Ngành", "Nhà tuyển dụng", "Người lao động"]];
            //    $.each(value.listReport, function (i, v) {
            //        dataArray.push([v.nameRecruitment, v.countRecruiment, v.countJob]);
            //        var a = google.visualization.arrayToDataTable(dataArray),
            //            b = {
            //                chart: {
            //                    title: "Top ngành tuyển dụng",
            //                },
            //                bars: "vertical",
            //                vAxis: {
            //                    format: "decimal",
            //                },
            //                height: 400,
            //                width: "100%",
            //                colors: [
            //                    "#c6164f", 
            //                    "#372363",
            //                    "#51bb25",
            //                ],
            //            },
            //            c = new google.charts.Bar(document.getElementById("column-chart1"));
            //        c.draw(a, google.charts.Bar.convertOptions(b));
            //    });
            //});
        }

    }


})(jQuery);
