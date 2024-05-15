(function ($) {
    var _service = abp.services.app.report;
    google.charts.load("current", { packages: ["corechart", "bar"] });
    google.charts.setOnLoadCallback(drawBasic);
    const randomColor = (i) => {
        const colors = ["#93B5C6", "#DDEDAA", "#F0CF65", "#D7816A", "#BD4F6C", "#FF00FF", "#FFCCFF", "#FFCC33", "#00CC66","#FF6600"];
        return colors[i % colors.length];
    }
    function GetDataByTime(StatTime, EndTime) {
        _service.getReportArticle(StatTime, EndTime).done(function (value) {
            var dataArray = [["Chuyên mục", "Số lượng bài viết", {
                role: "style",
            }]];
            $.each(value.reportListArticle, function (i, v) {
                const color = randomColor(i);
                dataArray.push([v.cat, v.countArticle, color]);
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
                    title: "Thống Kê số lượng tin tức trong 30 ngày gần nhất",
                    width: "100%",
                    height: 400,
                    bar: {
                        groupWidth: "95%",
                    },
                    legend: {
                        position: "none",
                    },
                },
                    c = new google.visualization.BarChart(
                        document.getElementById("bar-chart2")
                    );
                c.draw(d, b);
            });
        });
    }

   

    function drawBasic() {
        if ($("#bar-chart2").length > 0) {
            var StatTime = $("#StartTime").val();
            var EndTime = $("#EndTime").val();
            GetDataByTime(StatTime, EndTime);
            $("#btnSearchArticle").click(function () {
                var Stat = $("#StartTime").val();
                var End = $("#EndTime").val();
                if (StatTime != "" && EndTime != "") {
                    GetDataByTime(Stat, End);
                }
                else {
                    abp.message.error('', 'Vui lòng chọn ngày bắt đầu và ngày kết thúc!');
                }
            });
        }
    }


    $('#WlExport').on('click', function (e) {
        var StatTime = $("#StartTime").val();
        var EndTime = $("#EndTime").val();

        if (StatTime != "" && EndTime != "") {
            _service.exportReportArticle(StatTime, EndTime)
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
