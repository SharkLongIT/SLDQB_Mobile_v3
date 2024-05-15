(function ($) {

    var _service = abp.services.app.report;
    var StatTime = $("#StartTime").val();
    var EndTime = $("#EndTime").val();
    var Type = $("#Type").val();
    const randomColor = (i) => {
        const colors = ["#93B5C6", "#DDEDAA", "#F0CF65", "#D7816A", "#BD4F6C", "#FF00FF", "#FFCCFF", "#FFCC33", "#00CC66", "#FF6600", "#008000",
            "#008080", "#FF99CC", "#FF9900", "#CC9966", "#9999CC", "#999900", "#669933", "#FF6600", "#666699", "#993300", "#CCCCCC", "#9999FF", "#CC6666", "#333300", "#990033"];
        return colors[i % colors.length];
    }


   // let series = [];
    //let categories = [];

    //_service.getReportCat(StatTime, EndTime, Type).done(function (value) {

    //    $.each(value.reportListCat, function (i, v) {
    //        series.push({
    //            name: v.cat,
    //            data: v.countRecruiment
    //        });
    //    });
    //});


    // column chart
    var options3 = {
        chart: {
            height: 350,
            type: "bar",
            toolbar: {
                show: false,
            },
        },
        plotOptions: {
            bar: {
                horizontal: false,
                endingShape: "rounded",
                columnWidth: "55%",
            },
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            show: true,
            width: 2,
            colors: ["transparent"],
        },
        series:[],
        xaxis: {
            categories: [],
        },
        yaxis: {
            title: {
                text: "số lượng",
            },
        },
        fill: {
            opacity: 1,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return  val ;
                },
            },
        },
        colors: ["#93B5C6", "#DDEDAA", "#F0CF65", "#D7816A", "#BD4F6C", "#FF00FF", "#FFCCFF", "#FFCC33", "#00CC66", "#FF6600", "#008000",
            "#008080", "#FF99CC", "#FF9900", "#CC9966", "#9999CC", "#999900", "#669933", "#FF6600", "#666699", "#993300", "#CCCCCC", "#9999FF", "#CC6666", "#333300", "#990033"],
    };
    _service.getReportCatApex(StatTime, EndTime, Type).done(function (value) {
        $.each(value.reportListCat, function (i, v) {
            options3.series.push({
                name: v.cat,
                data: [v.countRecruiment]
            });
            options3.xaxis.categories.push(v.date);
        });
    });

    var chart3 = new ApexCharts(document.querySelector("#column-chart"), options3);
    chart3.render();


   
})(jQuery);