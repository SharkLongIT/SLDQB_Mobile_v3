(function () {
    var _$TradingTable = $('#TradingTable');
    var _$TradingTablePresent = $('#TradingTablePresent');
    var _TradingAccountService = abp.services.app.tradingSessionAccount;
    moment.locale(abp.localization.currentLanguage.name);



    var _ViewRecuiterModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/TradingSession/Recruiter',
        scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/Recruiter.js',
        modalType: 'modal-xl'
    });

    var _ViewCandidateModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/TradingSession/Candidate',
        scriptUrl: abp.appPath + 'view-resources/Profile/TradingSession/Candidate.js',
        modalType: 'modal-xl'
    });


    $("#btnRecruiter").click(function () {
        var dataFilter = { id: $("#TradingSessionId").val() };
        _ViewRecuiterModal.open(dataFilter);
    })

    $("#btnCandidate").click(function () {
        var dataFilter = { id: $("#TradingSessionId").val() };
        _ViewCandidateModal.open(dataFilter);
    })

    $('#WorkSite').select2({
        placeholder: 'Tất cả địa điểm',
    });

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        if ($("#WorkSite").val().length > 0) {
            dataFilter.workSite = [];
            $.each($("#WorkSite").val(), function (index, value) {
                if (value != undefined) {
                    dataFilter.workSite.push(value);
                }
            });
        }

        dataFilter.id = $("#TradingSessionId").val();
        return dataFilter;
    }

    // nha tuyen dung
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
            ajaxFunction: _TradingAccountService.getAllRecuiter,
            inputFilter: getFilter
        },
        order: [[1, 'asc']],
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
                render: function (data, type, row, meta) {
                    return row.recruiter.companyName;
                }
            },
            {
                orderable: false,
                targets: 2,
                render: function (data, type, row, meta) {
                    return row.recruiter.province.displayName;
                }
            },
            {
                orderable: true,
                targets: 3,

                render: function (data, type, row, meta) {
                    return row.recruiter.sphereOfActivity.displayName;
                }
            },
            {
                orderable: true,
                targets: 4,
                render: function (data, type, row, meta) {
                    return row.recruiter.humanResSizeCat.displayName;
                }
            },
            {
                orderable: true,
                targets: 5,
                render: function (data, type, row, meta) {
                    return moment(row.recruiter.dateOfEstablishment).format('L');
                }
            },
            {
                orderable: true,
                targets: 6,
                render: function (data, type, row, meta) {
                    
                    return row.recruiter.contactPhone;
                }
            },
            {
                orderable: true,
                targets: 7,
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
                targets: 8,
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
                            text: app.localize('Xem'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/Recruiters/RecruiterInfo/" + data.record.recruiter.userId;
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



    // nguoi lao dong

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
            ajaxFunction: _TradingAccountService.getAllCandidate,
            inputFilter: getFilter
        },
        order: [[1, 'asc']],
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
                render: function (data, type, row, meta) {
                    return row.candidate.account.name;
                }
            },
            {
                orderable: false,
                targets: 2,
                render: function (data, type, row, meta) {
                    return row.positions;
                }
            },
            {
                orderable: true,
                targets: 3,

                render: function (data, type, row, meta) {
                    return row.workSite;
                }
            },
            {
                orderable: true,
                targets: 4,
                render: function (data, type, row, meta) {
                    return row.occupations;
                }
            },
            {
                orderable: true,
                targets: 5,
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
                            text: app.localize('Xem'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/Candidate/Detail/" + data.record.candidate.account.id;
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



    //function deleteTrading(id) {
    //    abp.message.confirm(
    //        app.localize('Delete'),
    //        app.localize('AreYouSure'),
    //        function (isConfirmed) {
    //            if (isConfirmed) {
    //                _TradingService
    //                    .delete(id)
    //                    .done(function () {
    //                        getDocs();
    //                        abp.notify.success(app.localize('SuccessfullyDeleted'));
    //                    });
    //            }
    //        }
    //    );
    //}


    abp.event.on('app.reloadDocTable', function () {
        getDocs();
        getTradingPre();
    });

    $('#Search').click(function (e) {
        e.preventDefault();
        getDocs();
        getTradingPre();
    });

    function getDocs() {
        dataTable.ajax.reload();
    }
    function getTradingPre() {
        dataTablePre.ajax.reload();
    }
    jQuery(document).ready(function () {
        $("#SearchTerm").focus();
    });
})(jQuery);
