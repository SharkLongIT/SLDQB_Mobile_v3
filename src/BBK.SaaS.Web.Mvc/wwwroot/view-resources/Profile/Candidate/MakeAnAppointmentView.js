(function () {
    var _$AppointmentTable = $('#ApplicationRequestTable');
    var _makeAnAppointmentService = abp.services.app.makeAnAppointment;
    moment.locale(abp.localization.currentLanguage.name);

    var _EditModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Candidate/InterviewConfirmationModal',
        scriptUrl: abp.appPath + 'view-resources/Profile/Candidate/InterviewConfirmationModal.js',
        modalClass: 'InterviewConfirmationModal',
        modalType: 'modal-xl'
    });
    var _DetailModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Candidate/DetailMakeAnAppiontment',
        modalClass: 'DetailModal',
        modalType: 'modal-xl'
    });
    var _RefuseInterviewModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Candidate/RefuseInterviewModal',
        scriptUrl: abp.appPath + 'view-resources/Profile/Candidate/RefuseInterviewModal.js',
        modalClass: 'RefuseInterviewModal',
        modalType: 'modal-xl'
    });

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        if ($('#StatusOfCandidate').val() != null && $('#StatusOfCandidate').val() != undefined && $('#StatusOfCandidate').val() != "") {
            dataFilter.statusOfCandidate = $('#StatusOfCandidate').val();
        }
        dataFilter.rank = $('#Ranks').val();
        dataFilter.interviewTime = $('#InterviewTime').val();
        return dataFilter;
    }

    var dataTable = _$AppointmentTable.DataTable({
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
        listAction: {
            ajaxFunction: _makeAnAppointmentService.getAllOfCandidate,
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
                data: "name",
                render: function (data, type, row, meta) {
                    return '<span> <a href=' + abp.appPath + "JobUser/Recruiters/RecruiterInfo/" + row.recruiter.id + '>' + row.recruiter.companyName + '</a> </span>'
                    //return row.recruiter.companyName
                }
            },
            {
                orderable: false,
                targets: 2,
                render: function (data, type, row, meta) {
                    return '<span> <a href=' + abp.appPath + "JobUser/ViewRecruitment/" + row.recruitment.id + '>' + row.recruitment.title + '</a> </span>'
                    return 
                }
            },
            {
                orderable: true,
                targets: 3,
                data: 'interviewTime',
                render: function (data, type, row, meta) {
                    return row.jobApplication.title
                }
            },
            {
                orderable: false,
                targets: 4,
                data: 'interviewTime',
                render: function (data, type, row, meta) {
                    return row.ranks.displayName
                }
            },
            {
                orderable: true,
                targets: 5,
                data: 'typeInterview',
                render: function (data, type, row, meta) {
                    return moment(row.interviewTime).format('DD-MM-YYYY HH:mm')
                }
            },
            {
                orderable: true,
                targets: 6,
                data: 'interviewResultLetter',
                render: function (data, type, row, meta) {
                    if (row.statusOfCandidate == 1) {
                        return `<span class="badge bg-light text-black rounded-pill p-2">Chờ phỏng vấn</span>`
                    } else if (row.statusOfCandidate == 2) {
                        return `<span class="badge bg-info text-white rounded-pill p-2">Xác nhận phỏng vấn</span>`
                    } else if (row.statusOfCandidate == 3) {
                        return `<span class="badge bg-danger text-white rounded-pill p-2">Từ chối phỏng vấn</span>`
                    } else if (row.statusOfCandidate == 4) {
                        return `<span class="badge bg-success text-white rounded-pill p-2">Đã phỏng vấn</span>`
                    }
                    // 1.Chờ pv, 2.Xác nhận pv, 3.Từ chối pv, 4.Đỗ pv 
                }
             },
             {
                orderable: true,
                targets: 7,
                data: 'interviewResultLetter',
                render: function (data, type, row, meta) {
                     if (row.interviewResultLetter == 0) {
                        return `<span class="badge bg-light text-black rounded-pill p-2">Chưa có kết quả</span>`
                     } else if (row.interviewResultLetter == 1) {
                          return `<span class="badge bg-success text-white rounded-pill p-2">Pass</span>`
                     } else if (row.interviewResultLetter == 2) {
                        return `<span class="badge bg-danger text-white rounded-pill p-2">Fail</span>`
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
                            text: app.localize('Xem công việc'),
                            action: function (data) {
                                var dataFilter = { id: data.record.recruitment.id };
                                window.location.href = abp.appPath + "Profile/Recruitments/ViewRecruitment?id=" + dataFilter.id;
                            },
                        },
                        {
                            text: app.localize("Xem chi tiết"),
                            action: function (data) {
                                dataFilter = { id: data.record.id };
                                _DetailModal.open(dataFilter);
                            },
                            visible: function (data) {
                                return data.record.statusOfCandidate > 1;
                            }
                        },
                        {
                            text: app.localize('Xác nhận phỏng vấn'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _EditModal.open(dataFilter);
                            },
                            visible: function (data) {
                                return data.record.statusOfCandidate == 1;
                            }
                        },
                        {
                            text: app.localize('Từ chối phỏng vấn'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _RefuseInterviewModal.open(dataFilter);
                            },
                            visible: function (data) {
                                return data.record.statusOfCandidate == 1;
                            }
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
    jQuery(document).ready(function () {
        $("#SearchTerm").focus();
    });
})(jQuery);
