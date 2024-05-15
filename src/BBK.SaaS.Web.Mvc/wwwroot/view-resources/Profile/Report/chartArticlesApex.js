(function ($) {
    var _service = abp.services.app.report;
    google.charts.load("current", { packages: ["corechart", "bar"] });
    google.charts.setOnLoadCallback(drawBasic);

    function GetData(start,end) {
        _service.getReportArticleApex(start, end).done(function (value) {
            var dataArray = [["Ngày", "Tổng số tin"]];
            $.each(value.reportListArticle, function (i, v) {
                dataArray.push([v.cat, v.countArticle]);
                var a = google.visualization.arrayToDataTable(dataArray),
                    b = {
                        chart: {
                            title: "Tổng số tin tức",
                        },
                        bars: "vertical",
                        vAxis: {
                            format: "decimal",
                        },
                        height: 400,
                        width: "100%",
                        colors: [
                            MofiAdminConfig.primary,
                            MofiAdminConfig.secondary,
                            "#51bb25",
                        ],
                    },
                    c = new google.charts.Bar(document.getElementById("column-chart-Apex"));
                c.draw(a, google.charts.Bar.convertOptions(b));
            });
        });
    }

    function drawBasic() {
        if ($("#column-chart-Apex").length > 0) {
            var StatTime = $("#StartTime").val();
            var EndTime = $("#EndTime").val();
            GetData(StatTime, EndTime);


            $("#btnSearchArticleApex").click(function () {
                var Stat = $("#StartTime").val();
                var End = $("#EndTime").val();
                if (Stat != null && End != null) {
                    GetData(Stat, End);
                }
                else {
                    abp.message.error('', 'Vui lòng chọn ngày bắt đầu và ngày kết thúc!');
                }
            })

        }



    }
    $('#btnExcelApex').on('click', function (e) {
        var StatTime = $("#StartTime").val();
        var EndTime = $("#EndTime").val();

        if (StatTime != "" && EndTime != "") {
            _service.exportReportArticleApex(StatTime, EndTime)
                .done(function (fileResult) {
                    abp.notify.info(abp.localization.localize("Xuất file thành công"));
                    location.href = abp.appPath + 'File/DownloadTempFile?fileType=' + fileResult.fileType + '&fileToken=' + fileResult.fileToken + '&fileName=' + fileResult.fileName;
                });
        }
        else {
            abp.message.error('', 'Vui lòng chọn ngày bắt đầu và ngày kết thúc!');
        }
    });



})(jQuery);
