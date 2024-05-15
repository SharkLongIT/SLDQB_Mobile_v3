(function ($) {
    var _service = abp.services.app.report;
    google.charts.load("current", { packages: ["line"] });
    google.charts.setOnLoadCallback(drawBasic);

    function GetDataChart(Year, Month) {
        _service.getReportWebsite(Year, Month).done(function (value) {
            var data = new google.visualization.DataTable();
            data.addColumn("string", "Thời gian");
            data.addColumn("number", "Nhà tuyển dụng");
            data.addColumn("number", "Người lao động");
            $.each(value.listReport, function (i, v) {
                data.addRows([
                    [v.date, v.countRecruiment, v.countCandidate]
                ]);
                var options = {
                    chart: {
                        title: "Biểu đồ thống kê số doanh nghiệp và số người lao động đăng ký hệ thống tháng hiện tại.",
                    },
                    colors: ["#c6164f", "#372363"],
                    height: 500,
                    width: "100%",
                };
                var chart = new google.charts.Line(document.getElementById("line-chart"));
                chart.draw(data, google.charts.Line.convertOptions(options));
            })
        })
    }


    function GetDataChartByYear(ToYear, FromYear) {
        _service.getReportWebsiteByYear(ToYear, FromYear).done(function (value) {
            var data = new google.visualization.DataTable();
            data.addColumn("string", "Thời gian");
            data.addColumn("number", "Nhà tuyển dụng");
            data.addColumn("number", "Người lao động");
            $.each(value.listReport, function (i, v) {
                data.addRows([
                    [v.date, v.countRecruiment, v.countCandidate]
                ]);
                var options = {
                    chart: {
                        title: "Biểu đồ thống kê số doanh nghiệp và số người lao động đăng ký hệ thống.",
                    },
                    colors: ["#c6164f", "#372363"],
                    height: 500,
                    width: "100%",
                };
                var chart = new google.charts.Line(document.getElementById("line-chart"));
                chart.draw(data, google.charts.Line.convertOptions(options));
            })
        })
    }



    function ShowChartByMonth() {
        $("#ReportYear").hide();
        $("#ReportMonth").show();
        $("#Month").val($("#MonthNow").val()).change();
        var yearNow = $("#YearNow").val();
        var monthNow = $("#MonthNow").val();
        GetDataChart(yearNow, monthNow);

        $("#btnSearch").click(function () {
            var year = $("#Year").val();
            var month = $("#Month").val();
            if (year != null && month != null) {
                GetDataChart(year, month);
            }
            else {
                abp.message.error('', 'Vui lòng chọn tháng và năm!');
            }
        })
    }

    function ShowChartByYear() {
        $("#ReportYear").show();
        $("#ReportMonth").hide();
        GetDataChartByYear($("#ToYear").val(),$("#FromYear").val());

        $("#btnSearchYear").click(function () {
            var toYear = $("#ToYear").val();
            var fromYear = $("#FromYear").val();
            if (toYear != null || fromYear != null) {
                GetDataChartByYear(toYear, fromYear);
            }
            else {
                abp.message.error('', 'Vui lòng chọn năm!');
            }
        })
    }


    function drawBasic() {
        if ($("#line-chart").length > 0) {
            if ($("#Type").val() == 0) {
                ShowChartByMonth();
            }
            else
            {
                ShowChartByYear();
            }

            $('#Type').change(function () {
                var selected = this.value;
                if (selected == 0) {
                    ShowChartByMonth();
                }
                else {
                    ShowChartByYear();
                }
            });
        }
    }


    $('#btnExcel').on('click', function (e) {
        var year = $("#Year").val();
        var month = $("#Month").val();

        if (year != null && month != null) {
            _service.exportReportWebsite(year, month)
                .done(function (fileResult) {
                    abp.notify.info(abp.localization.localize("Xuất file thành công"));
                    location.href = abp.appPath + 'File/DownloadTempFile?fileType=' + fileResult.fileType + '&fileToken=' + fileResult.fileToken + '&fileName=' + fileResult.fileName;
                });
        }
        else {
            abp.message.error('', 'Vui lòng chọn tháng và năm!');
        }


    });

    $('#btnExcelYear').on('click', function (e) {
        var toYear = $("#ToYear").val();
        var fromYear = $("#FromYear").val();

        if (toYear != null && fromYear != null) {
            _service.exportReportWebsiteByYear(toYear,fromYear)
                .done(function (fileResult) {
                    abp.notify.info(abp.localization.localize("Xuất file thành công"));
                    location.href = abp.appPath + 'File/DownloadTempFile?fileType=' + fileResult.fileType + '&fileToken=' + fileResult.fileToken + '&fileName=' + fileResult.fileName;
                });
        }
        else {
            abp.message.error('', 'Vui lòng chọn tháng và năm!');
        }


    });

})(jQuery);
