(function () {
    var _$TradingTable = $('#TradingTable');
    var _$TradingTablePresent = $('#TradingTablePresent');
    var _$TradingTablePast = $('#TradingTablePast');
    var _TradingService = abp.services.app.tradingSession;
    moment.locale(abp.localization.currentLanguage.name);

    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/TradingSession/CreateTrading',
        scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/Create.js',
        modalClass: 'CreateModal',
        modalType: 'modal-xl'
    });


    var _EditModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/TradingSession/EditTrading',
        scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/Edit.js',
        modalClass: 'EditModal',
        modalType: 'modal-xl'
    });

    var _ViewModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/TradingSession/ViewTrading',
        scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/View.js',
        modalClass: 'ViewModal',
        modalType: 'modal-xl'
    });

    //var _ViewRecuiterModal = new app.ModalManager({
    //    viewUrl: abp.appPath + 'Profile/TradingSession/Recruiter',
    //    scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/Recruiter.js',
    //    modalType: 'modal-xl'
    //});

    $('#WorkSite').select2({
        placeholder: 'Tất cả địa điểm',
    });

    $("#CreateNewButtonxx").click(function () {
        _Modal.open();
    })

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        dataFilter.fromDate = $("#StartDay").val();
        dataFilter.toDate = $("#EndDay").val();
        if ($('select[name=location]').val() != '') {
            dataFilter.workSite = [];
            $.each($('select[name=location]').val(), function (index, value) {
                if (value != '') {
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
            ajaxFunction: _TradingService.getAllFuture,
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
                data: 'creationTime',
                render: function (CreationTime) {
                    return moment(CreationTime).format('L');
                }
            },
            {
                targets: 6,
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
                            text: app.localize('Người tham gia phiên'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/TradingSession/TradingAccount?Id=" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Edit'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _EditModal.open(dataFilter);
                            },
                        },
                        {
                            text: app.localize('Xem'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _ViewModal.open(dataFilter);
                            },
                        },
                        {
                            text: app.localize('Delete'),
                            action: function (data) {
                                deleteTrading(data.record.id);
                            },
                        },
                        //{
                        //    text: app.localize('Mời NTD'),
                        //    action: function (data) {
                        //        var dataFilter = { id: data.record.id };
                        //        _ViewRecuiterModal.open(dataFilter);
                        //    },
                        //},
                    ],
                },
            }
        ]
    });



    // đang diễn ra

    var dataTablePre = _$TradingTablePresent.DataTable({
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
            ajaxFunction: _TradingService.getAllPresent,
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
                data: 'creationTime',
                render: function (CreationTime) {
                    return moment(CreationTime).format('L');
                }
            },
            {
                targets: 6,
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
                            text: app.localize('Người tham gia phiên'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/TradingSession/TradingAccount?Id=" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Xem'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _ViewModal.open(dataFilter);
                            },
                        },

                    ],
                },
            }
        ]
    });




    // đã diễn ra

    var dataTablePast = _$TradingTablePast.DataTable({
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
            ajaxFunction: _TradingService.getAllPast,
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
                data: 'creationTime',
                render: function (CreationTime) {
                    return moment(CreationTime).format('L');
                }
            },
            {
                targets: 6,
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
                            text: app.localize('Người tham gia phiên'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/TradingSession/TradingAccount?Id=" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Xem'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _ViewModal.open(dataFilter);
                            },
                        },


                    ],
                },
            }
        ]
    });





    function deleteTrading(id) {
        abp.message.confirm(
            app.localize('Delete'),
            app.localize('AreYouSure'),
            function (isConfirmed) {
                if (isConfirmed) {
                    _TradingService
                        .delete(id)
                        .done(function () {
                            getDocs();
                            abp.notify.success(app.localize('Xoá phiên giao dịch thành công'));
                        });
                }
            }
        );
    }


    abp.event.on('app.reloadDocTable', function () {
        getDocs();
        getTradingPre();
        getTradingPast();
    });

    $('#Search').click(function (e) {
        e.preventDefault();
        getDocs();
        getTradingPre();
        getTradingPast();
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
