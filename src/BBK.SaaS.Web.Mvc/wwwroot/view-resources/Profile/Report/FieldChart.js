(function ($) {
    var _service = abp.services.app.report;
    google.charts.load("current", { packages: ["corechart", "bar"] });
    google.charts.setOnLoadCallback(drawBasic);
    var StatTime = $("#StartTime").val();
    var EndTime = $("#EndTime").val();
    var Type = $("#Type").val();
    const randomColor = (i) => {
        const colors = ["#93B5C6", "#DDEDAA", "#F0CF65", "#D7816A", "#BD4F6C", "#FF00FF", "#FFCCFF", "#FFCC33", "#00CC66", "#FF6600", "#008000",
            "#008080", "#FF99CC", "#FF9900", "#CC9966", "#9999CC", "#999900", "#669933", "#FF6600", "#666699", "#993300", "#CCCCCC", "#9999FF", "#CC6666", "#333300", "#990033"];
        return colors[i % colors.length];
    }
    function GetDataByTime(Stat, End, type) {
        _service.getReportCat(Stat, End, type).done(function (value) {

            var dataArray = [[]];
            if (type == 1) {
                dataArray = [["Lĩnh vực", "Số lượng bài tuyển dụng", {
                    role: "style",
                }]];
            }
            else {
                dataArray = [["Lĩnh vực", "Số lượng hồ sơ", {
                    role: "style",
                }]];
            }

            $.each(value.reportListCat, function (i, v) {
                const color = randomColor(i);
                if (type == 1) {
                    dataArray.push([v.cat, v.countRecruiment, color]);
                }
                else {
                    dataArray.push([v.cat, v.countJob, color]);
                }
                var a = google.visualization.arrayToDataTable(dataArray),
                    d = new google.visualization.DataView(a);

                d.setColumns([
                    0,
                    1,
                    {
                        calc: "stringify",
                        sourceColumn: 1,
                        type: "string",
                        role: "annotation",
                    },
                    2,
                ]);
                var b = {
                    title: "Thống kê số lượng tin tuyển dụng hoặc hồ sơ người lao động theo lĩnh vực trong 30 ngày gần nhất",
                    width: "100%",
                    height: 2000,
                    bar: {
                        groupWidth: "100%",
                    },
                    legend: {
                        position: "none",
                    },
                },
                    c = new google.visualization.BarChart(
                        document.getElementById("column-chart")
                    );
                c.draw(d, b);
            });
        });
    }

    function drawBasic() {
        if ($("#column-chart").length > 0) {

            GetDataByTime(StatTime, EndTime, Type);
            $("#btnSearchArticle").click(function () {
                var selecteted = $("#Type").find(":selected").val();
                var Stat = $("#StartTime").val();
                var End = $("#EndTime").val();
                if (StatTime != "" && EndTime != "") {
                    GetDataByTime(Stat, End, selecteted);
                }
                else {
                    abp.message.error('', 'Vui lòng chọn ngày bắt đầu và ngày kết thúc!');
                }
            });
            $('#Type').change(function () {
                var selected = this.value;
                GetDataByTime(StatTime, EndTime, selected);
            });
        }
    }


    $('#btnExcelCat').on('click', function (e) {
        var selecteted = $("#Type").find(":selected").val();
        if (StatTime != "" && EndTime != "") {
            _service.exportReportCat(StatTime, EndTime, selecteted)
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
