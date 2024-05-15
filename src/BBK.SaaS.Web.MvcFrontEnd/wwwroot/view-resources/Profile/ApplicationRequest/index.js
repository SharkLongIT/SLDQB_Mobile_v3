(function () {
    var _$AppRequestTable = $('#ApplicationRequestTable');
    var _ApplicationRequestService = abp.services.app.applicationRequest;
    moment.locale(abp.localization.currentLanguage.name);

    var callApi = {
        ajax: function (url, data, callBack, beforeSend, handleData) {
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                caches: false,
                beforeSend: beforeSend,
                success: function (result) {
                    if (callBack != undefined) {

                        callBack(result)
                    }
                    if (handleData != undefined) {
                        handleData
                    }
                }
            })
        }
    };



    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        if ($('#Status').val() != '0') {
            dataFilter.status = $('#Status').val();
        }
        dataFilter.startTime = $('#StartTime').val();
        dataFilter.endTime = $('#EndTime').val();
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
        //    ajaxFunction: _ApplicationRequestService.getAll,
        //    inputFilter: getFilter
        //},
        "ajax": {
            "url": "/Profile/ApplicationRequest/GetAll",
            data: function (d) {
                d.search = $('#SearchTerm').val();
                if ($('#Status').val() != '0') {
                    d.status = $('#Status').val();
                }
                d.startTime = $('#StartTime').val();
                d.endTime = $('#EndTime').val();
            },
            dataSrc: "result.items"
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
                     return '<span> <a href=' + abp.appPath + "JobUser/NVNVViewDetailRecruiter/" + row.recruitment.recruiter.userId + '>' + row.recruitment.recruiter.companyName + '</a> </span>'
                }
            },
            {
                orderable: true,
                targets: 2,
                render: function (data, type, row, meta) {
                    return '<span> <a href=' + abp.appPath + "JobUser/ViewRecruitment/" + row.recruitment.id + '>' + row.recruitment.title + '</a> </span>'
                }
            },
            {
                orderable: true,
                targets: 3,
                render: function (data, type, row, meta) {
                    return row.jobApplication.title
                }
            },
            {
                orderable: false,
                targets: 4,
                render: function (data, type, row, meta) {
                    return row.recruitment.ranks.displayName
                }
            },
            {
                orderable: false,
                targets: 5,
                render: function (data, type, row, meta) {
                    return moment(row.creationTime).format('L')
                }
            },
            {
                orderable: true,
                targets: 6,
                data: "status",
                render: function (data, type, row, meta) {
                    if (row.status === 1) {
                        return `<span class="badge bg-light text-black rounded-pill p-2">Đang ứng tuyển</span>`
                    } else if (row.status === 2) {
                        return `<span class="badge bg-info text-white rounded-pill p-2">NTD đã xem</span>`
                    } else if (row.status === 3) {
                        return `<span class="badge bg-success text-white rounded-pill p-2">Hồ sơ phù hợp</span>`
                    } else {
                        return `<span class="badge bg-light text-black rounded-pill p-2">Đang ứng tuyển</span>`
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
                    cssClass: 'btn btn-brand dropdown-toggle',
                    text: ' ' + app.localize('Actions') + '',
                    items: [{
                        text: app.localize('Xem hồ sơ'),
                        iconStyle: 'far fa-eye m-2',
                        action: function (data) {
                            window.location.href =
                                "/Profile/JobApplication/PreViewJobOfCandidate?jobAppId=" + data.record.jobApplicationId;
                        },
                        visible: function () {
                            return false;
                        },
                    }
                        , {
                            text: app.localize('Xem công việc'),
                            iconStyle: 'far fa-eye m-2',
                            action: function (data) {
                                window.location.href =
                                    "/Profile/Recruitments/ViewRecruitment?id=" + data.record.recruitmentId;
                            },
                            visible: function () {
                                return false;
                            },
                        },
                        {
                            text: app.localize('Xem Chi tiết'),
                            iconStyle: 'far fa-eye m-2',
                            action: function (data) {
                                new app.ModalManager({
                                    viewUrl: abp.appPath + "Profile/ApplicationRequest/ApplicationRequestModal",
                                }).open({ Id: data.record.id })
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
