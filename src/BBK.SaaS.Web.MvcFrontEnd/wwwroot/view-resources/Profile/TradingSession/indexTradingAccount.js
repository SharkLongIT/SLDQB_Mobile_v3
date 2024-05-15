(function () {
    var _$TradingTable = $('#TradingAccountTable');
    var _TradingAccountService = abp.services.app.tradingSessionAccount;
    moment.locale(abp.localization.currentLanguage.name);

    var _ViewModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/TradingSession/ViewTrading',
        scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/View.js',
        modalClass: 'ViewModal',
        modalType: 'modal-xl'
    });
   

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();

        dataFilter.fromDate = $("#StartDay").val();
        dataFilter.toDate = $("#EndDay").val();
     

        if ($("#WorkSite").val().length > 0) {
            dataFilter.workSite = [];
            $.each($("#WorkSite").val(), function (index, value) {
                if (value != undefined) {
                    dataFilter.workSite.push(value);
                }
            });
        }
        return dataFilter;
    }

    // sắp diên ra
    var dataTable = _$TradingTable.DataTable({
        paging: true,
        serverSide: false,
        processing: true,
        searching: false,
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
        listAction: {
            ajaxFunction: _TradingAccountService.getAll,
            inputFilter: getFilter
        },
        order: [[0, 'asc']],
        columnDefs: [
            {
                targets: 0,
                orderable: true,
                className: 'dt-body-center text-center',
                render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                orderable: true,
                targets: 1,
                data: "nameTrading"
            },
            {
                orderable: false,
                targets: 2,
                render: function (data, type, row, meta) {
                    return row.province.displayName;
                }
            },
            {
                orderable: true,
                targets: 3,
                data: 'startTime',
                render: function (StartTime) {
                    return moment(StartTime).format('DD/MM/Y HH:mm');
                }
            },
            {
                orderable: true,
                targets: 4,
                data: 'endTime',
                render: function (EndTime) {
                    return moment(EndTime).format('DD/MM/Y HH:mm');
                }
            },
            {
                orderable: true,
                targets: 5,
                render: function (data, type, row, meta) {
                    var d = new Date();
                    var now = moment(d).format('DD/MM/Y HH:mm');
                    var startTime = moment(row.startTime).format('DD/MM/Y HH:mm');
                    var endTime = moment(row.endTime).format('DD/MM/Y HH:mm');
                    if (startTime <= now && endTime >= now) {
                        return `<span class="badge bg-success text-white rounded-pill p-2">Đang diễn ra</span>`
                    }
                    else if (startTime > now) {
                        return `<span class="badge bg-light text-black rounded-pill p-2">Sắp diễn ra</span>`
                    }
                    else {
                        return `<span class="badge bg-danger text-white rounded-pill p-2">Đã diễn ra</span>`
                    }
                }
            },
            {
                orderable: true,
                targets: 6,
                render: function (data, type, row, meta) {
                   
                    if (row.status == 0) {
                        return `<span class="badge bg-success text-white rounded-pill p-2">Được mời</span>`
                    }
                    else {
                        return `<span class="badge bg-danger text-white rounded-pill p-2">Tham gia</span>`
                    }
                }
            },
            {
                targets: 7,
                data: null,
                orderable: false,
                autoWidth: false,
                defaultContent: '',
                rowAction: {
                    text:
                        '<i class="fa fa-cog"></i> <span class="d-none d-md-inline-block d-lg-inline-block d-xl-inline-block">' +
                        app.localize('Actions') +
                        '</span> <span class="caret"></span>',
                    items: [
                       
                        {
                            text: app.localize('Chi tiết'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _ViewModal.open(dataFilter);
                            },
                        },
                        {
                            text: app.localize(''),
                        },
                    ],
                },
            }
        ]
    });


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
    function getTradingPre() {
        dataTablePre.ajax.reload();
    }
    function getTradingPast() {
        dataTablePast.ajax.reload();
    }
    jQuery(document).ready(function () {
        $("#SearchTerm").focus();
    });
})(jQuery);
