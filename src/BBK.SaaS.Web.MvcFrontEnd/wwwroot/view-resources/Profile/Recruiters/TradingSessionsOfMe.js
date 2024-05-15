(function () {
    var _$TradingSessionsOfMeTable = $('#TradingSessionsOfMeTable');
    moment.locale(abp.localization.currentLanguage.name);

    var GeoUnits = {
        _changeselect2: false,
        _multipleselect2: false,
        _location: $('select#WorkSite'),
        reloadProvince: function (location, data, selected) {
            location.children().remove();
            if (selected == undefined) {
                $.each(data, function (index, item) {

                    if (item.parentId == null) {
                        $(location).append($('<option>',
                            {
                                value: item.id,
                                text: item.displayName,
                            }));
                    }


                })
            } else {
                $.each(data, function (index, item) {
                    if (item.id == selected) {
                        if (item.parentId == null) {
                            $(location).append($('<option>',
                                {
                                    selected: true,
                                    value: item.id,
                                    text: item.displayName,
                                }));
                        }
                    } else {
                        if (item.parentId == null) {
                            $(location).append($('<option>',
                                {
                                    value: item.id,
                                    text: item.displayName,
                                }));
                        }
                    }
                })
            }
        },
        getProvincefromServer: function (location, select) {

            $.ajax({
                url: "/UserJob/GetGeoUnit",
                caches: false,
                success: function (result) {
                    GeoUnits.reloadProvince(location, result.result, select)
                }
            })

        },
        changeselect2: function (location) {
            $(location).select2({
                placeholder: app.localize('Địa điểm'),
                multiple: GeoUnits._multipleselect2,
            });
        },
        init: function (select) {
            GeoUnits.getProvincefromServer(GeoUnits._location, select);
            if (GeoUnits._changeselect2 == true) {
                GeoUnits.changeselect2(GeoUnits._location);
            }
        }
    }
    GeoUnits._multipleselect2 = true;
    GeoUnits._changeselect2 = true;
    GeoUnits.init()


    /* DEFINE TABLE */

    var dataTable = _$TradingSessionsOfMeTable.DataTable({
        paging: true,
        serverSide: false,
        processing: true,
        "searching": false,
        "language": {
            "emptyTable": "Không tìm thấy dữ liệu",
            "lengthMenu": "Hiển thị _MENU_ bản ghi",
        },
        "bInfo": false,
        "bLengthChange": true,
        lengthMenu: [
            [5, 10, 25, 50, -1],
            [5, 10, 25, 50, 'Tất cả'],
        ],

        pageLength: 10,
        "ajax": {
            "url": "/Profile/Recruiters/GetAllTradingSessionsOfMe",
            type: 'POST',
            caches: false,
            data: function (d) {
                d.search = $('#SearchTerm').val().trim();
                d.workSite = [];
                if ($('#Status').val() != '') {
                    d.status = $('#Status').val();
                }
                if ($('#StatusOfTradingSession').val() != '0') {
                    d.statusOfTradingSession = $('#StatusOfTradingSession').val();
                }
                if ($('#WorkSite').val() != '0') {
                    $.each($('#WorkSite').val(), function (index, value) {
                        if (value != '') {
                            d.workSite.push(value);
                        }
                    });
                }
                if ($('#StartTime').val() != null || $('#StartTime').val() != undefined || $('#StartTime').val() != "") {
                    d.fromDate = $('#StartTime').val();
                }
                if ($('#EndTime').val() != null || $('#EndTime').val() != undefined || $('#EndTime').val() != "") {
                    d.toDate = $('#EndTime').val();
                }
            },
            dataSrc: "result.items"
        },
        order: [[0, 'asc']],
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                className: 'dt-body-center text-center',
                render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                orderable: true,
                targets: 1,
                render: function (data, type, row, meta) {
                    return '<span> <a href="/TradingSession/ViewDetailTradingSessionAccount/' + row.tradingSession.id + '">' + row.tradingSession.nameTrading + '</a> </span>'
                }
            },
            {
                orderable: true,
                targets: 2,
                render: function (data, type, row, meta) {
                    return '<span>' + row.tradingSession.province.displayName + ' </span>'
                }
            },
            {
                orderable: true,
                targets: 3,
                render: function (data, type, row, meta) {
                    return moment(row.tradingSession.startTime).format('DD-MM-YYYY HH:mm:ss')
                }
            },
            {
                orderable: true,
                targets: 4,
                render: function (data, type, row, meta) {
                    return moment(row.tradingSession.endTime).format('DD-MM-YYYY HH:mm:ss')
                }
            },
            {
                orderable: true,
                targets: 5,
                render: function (data, type, row, meta) {
                    return moment(row.joiningDate).format('L')
                }
            },
            {
                orderable: false,
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.statusOfTradingSession === 1) {
                        return '<span class="badge bg-info text-white rounded-pill p-2">Sắp diễn ra</span>';
                    }
                    else if (row.statusOfTradingSession === 2) {
                        return '<span class="badge bg-success text-white rounded-pill p-2">Đang diễn ra</span>';
                    }
                    else if (row.statusOfTradingSession === 3) {
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Đã diễn ra</span>';
                    }
                }
            },
            {
                orderable: false,
                targets: 7,
                render: function (data, type, row, meta) {
                    if (row.status === 0) {
                        return '<span class="badge bg-info text-white rounded-pill p-2">Được mời</span>';
                    }
                    else if (row.status === 1) {
                        return '<span class="badge bg-success text-white rounded-pill p-2">Đã tham gia</span>';
                    } else if (row.status === 2) {
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Từ chối</span>';
                    }

                }
            },
            {
                targets: 8,
                data: null,
                orderable: false,
                autoWidth: false,
                defaultContent: '',
                rowAction: {
                    cssClass: 'btn btn-brand dropdown-toggle',
                    text: ' ' + app.localize('Hành động') + '',
                    items: [{
                        text: app.localize('Xem chi tiết'),
                        action: function (data) {
                            var dataFilter = { id: data.record.tradingSession.id };
                            /*_ViewModal.open(dataFilter);*/
                            location.href = "/TradingSession/ViewDetailTradingSessionAccount/" + dataFilter.id;
                        }
                    },
                    {
                        text: app.localize('Xác nhận'),
                        action: function (data) {
                            Confirmation(data);
                        },
                        visible: function (data) {
                            if (data.record.status == 0) {
                                return true;
                            } else {
                                return false;
                            }
                        },
                    },
                    {
                        text: app.localize('Từ chối'),
                        action: function (data) {
                            Reject(data)
                        },
                        visible: function (data) {
                            if (data.record.status == 0) {
                                return true;
                            } else {
                                return false;
                            }
                        },

                    },
                    ]

                },
            }
        ]
    });

    function Confirmation(data) {
        var data = data;
        abp.message.confirm(
            app.localize(abp.localization.localize("")),
            app.localize(abp.localization.localize("Bạn có chắc chắn tham gia phiên giao dịch không?")),
            function (isConfirmed) {
                if (isConfirmed) {
                    var url = abp.appPath + "Profile/Recruiters/UpdateStatusTradingSession";
                    data.id = data.record.id;
                    data.status = 1;

                    $.ajax({
                        url: url,
                        caches: false,
                        data: data,
                        type: "POST",
                        success: function (result) {
                            abp.notify.info(app.localize('Xác nhận tham gia phiên giao dịch thành công'));
                            getDocs();
                        }
                    });

                }
            }
        )
    }

    function Reject(data) {
        var data = data;
        abp.message.confirm(
            app.localize(abp.localization.localize("")),
            app.localize(abp.localization.localize("Bạn có chắc chắn từ chối tham gia phiên giao dịch không?")),
            function (isConfirmed) {
                if (isConfirmed) {
                    var url = abp.appPath + "Profile/Recruiters/UpdateStatusTradingSession";
                    data.id = data.record.id;
                    data.status = 2;

                    $.ajax({
                        url: url,
                        caches: false,
                        data: data,
                        type: "POST",
                        success: function (result) {
                            abp.notify.info(app.localize('Từ chối tham gia phiên giao dịch thành công'));
                            getDocs();
                        }
                    });

                }
            }
        )
    }

    abp.event.on('app.reloadDocTable', function () {
        getDocs();
    });

    $('#Search').click(function (e) {
        e.preventDefault();
        getDocs();
    });

    function getDocs() {
        dataTable.ajax.reload();
    }
    jQuery(document).ready(function () {
        $("#SearchTerm").focus();
    });
})(jQuery);
