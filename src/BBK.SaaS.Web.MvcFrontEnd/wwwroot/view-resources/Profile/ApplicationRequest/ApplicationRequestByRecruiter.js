(function () {
    var _$AppRequestTable = $('#ApplicationRequestTable');
    var _ApplicationRequestService = abp.services.app.applicationRequest;
    moment.locale(abp.localization.currentLanguage.name);

    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/JobApplication/MakeAnAppointment',
        scriptUrl: abp.appPath + 'view-resources/Profile/Candidate/MakeAnAppointment.js',
        modalClass: 'CreateModal',
        modalType: 'modal-xl'
    });

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        if ($('#Status').val() != '0') {
            dataFilter.status = $('#Status').val();
        }
        if ($('#Rank').val() != '0') {
            dataFilter.rank = $('#Rank').val();
        }
        if ($('#Experience').val() != '0') {
            dataFilter.experience = $('#Experience').val();
        }
        return dataFilter;
    }


    var dataTable = _$AppRequestTable.DataTable({
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
        //listAction: {
        //    ajaxFunction: _ApplicationRequestService.getAllByRecruiter,
        //    inputFilter: getFilter
        //},
        "ajax": {
            "url": "/Profile/ApplicationRequest/GetAllByRecruiter",
            data: function (d) {
                d.search = $('#SearchTerm').val();
                if ($('#Status').val() != '0') {
                    d.status = $('#Status').val();
                }
                if ($('#Rank').val() != '0') {
                    d.rank = $('#Rank').val();
                }
                if ($('#Experience').val() != '0') {
                    d.experience = $('#Experience').val();
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
                    return row.recruitment.title
                }
            },
            {
                orderable: true,
                targets: 2,
                render: function (data, type, row, meta) {
                    return row.recruitment.ranks.displayName
                }
            },
            {
                orderable: false,
                targets: 3,
                render: function (data, type, row, meta) {
                    return row.jobApplication.candidate.account.name
                }
            },
            {
                orderable: false,
                targets: 4,
                render: function (data, type, row, meta) {
                    return moment(row.creationTime).format('L')
                }
            },
            {
                orderable: true,
                targets: 5,
                data: "status",
                render: function (data, type, row, meta) {
                    return row.recruitment.experiences.displayName;
                }
            }, {
                orderable: true,
                targets: 6,
                data: "status",
                render: function (data, type, row, meta) {
                    return row.content;
                }
            },
            {
                orderable: true,
                targets: 7,
                data: "status",
                render: function (data, type, row, meta) {
                    if (row.status === 1) {
                        return '<span class="badge bg-light text-black rounded-pill p-2">Đang ứng tuyển</span>';
                    } else if (row.status === 2) {
                        return '<span class="badge bg-info text-white rounded-pill p-2">NTD đã xem</span>';
                    } else if (row.status === 3) {
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Hồ sơ phù hợp</span>';
                    } else {
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Trạng thái chưa được xác định</span>';
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
                    text: ' ' + app.localize('Actions') + '',
                    items: [{
                        text: app.localize('Xem CV'),
                        action: function (data) {
                            window.location.href =
                                "/UserJob/Detail?id=" + data.record.jobApplicationId;
                        }
                    },
                    {
                        text: app.localize('Đặt lịch'),
                        action: function (data) {
                            var dataFilter = { jobId: data.record.jobApplicationId, recruimentId: data.record.recruitment.id };
                            _Modal.open(dataFilter);
                        }
                    },
                    {
                        text: app.localize('Xóa'),
                        action: function (data) {
                            $.ajax({
                                url: '/Profile/ApplicationRequest/Delete/' + data.record.id,
                                type: "post",
                                cache: false,
                                success: function (results) {
                                    abp.notify.info(abp.localization.localize("Xoá thành công"));
                                    getDocs();
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert("error");
                                }
                            });
                        }
                    }
                    ]

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
    jQuery(document).ready(function () {
        $("#SearchTerm").focus();
    });
})(jQuery);
