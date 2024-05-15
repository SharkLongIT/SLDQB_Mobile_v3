(function () {
    var _$AppointmentTable = $('#AppointmentTable');
    var _makeAnAppointmentService = abp.services.app.makeAnAppointment;
    moment.locale(abp.localization.currentLanguage.name);

    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Appointments/MakeAnAppointment',
        //scriptUrl: abp.appPath + 'view-resources/Profile/Appointment/MakeAnAppointment.js',
        modalClass: 'CreateModal',
        modalType: 'modal-xl'
    });


    var _EditModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Appointments/EditMakeAnAppointment',
        scriptUrl: abp.appPath + 'view-resources/Profile/Appointments/Edit.js',
        modalClass: 'EditModal',
        modalType: 'modal-xl'
    });

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        dataFilter.experience = $('#Experience').val();
        dataFilter.rank = $('#Rank').val();
        dataFilter.interviewResultLetter = $('#InterviewResultLetter').val();
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
        "ajax": {
            "url": "/Profile/Appointments/getAll",
            data: function (d) {
                d.search = $('#SearchTerm').val();
                d.experience = $('#Experience').val();
                d.rank = $('#Rank').val();
                d.interviewResultLetter = $('#InterviewResultLetter').val();
                d.interviewTime = $('#InterviewTime').val();
            },
            dataSrc: "result.items"
        },
        //listAction: {
        //    ajaxFunction: _makeAnAppointmentService.getAll,
        //    inputFilter: getFilter
        //},
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
                orderable: false,
                targets: 1,
                data: "name"
            },
            {
                orderable: false,
                targets: 2,
                render: function (data, type, row, meta) {
                    return row.ranks.displayName
                }
            },
            {
                orderable: true,
                targets: 3,
                data: 'interviewTime',
                render: function (creationTime) {
                    return moment(creationTime).format('L');
                }
            },
            {
                orderable: false,
                targets: 4,
                data: 'interviewTime',
                render: function (creationTime) {
                    return moment(creationTime).format('hh:mm a');
                }
            },
            {
                orderable: true,
                targets: 5,
                data: 'typeInterview',
                render: function (data, type, row, meta) {
                    if (row.typeInterview == 1) {
                        return 'Online'
                    }
                    else return 'Trực tiếp'
                }
            },
            {
                orderable: true,
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.statusOfCandidate == 1) {
                        return '<span class="badge bg-light text-black rounded-pill p-2">Chờ phỏng vấn</span>'
                    }
                    else if (row.statusOfCandidate == 2) {
                        return '<span class="badge bg-info text-white rounded-pill p-2">Xác nhận phỏng vấn</span>'
                    } else if (row.statusOfCandidate == 3) {
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Từ chối phỏng vấn</span>'
                    }
                    else if (row.statusOfCandidate == 4) {
                        return '<span class="badge bg-success text-white rounded-pill p-2">Đã phỏng vấn</span>'
                    }
                    else return ''
                }
            },
            {
                orderable: true,
                targets: 7,
                //data: 'interviewResultLetter',
                render: function (data, type, row, meta) {
                    if (row.interviewResultLetter == 1) {
                        return '<span class="badge bg-success text-white rounded-pill p-2">Đỗ</span>'
                    }
                    else if (row.interviewResultLetter == 2){
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Trượt</span >'
                    }
                    else return ''
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
                            text: app.localize('Chi tiết'),
                            action: function (data) {
                                var dataFilter = { id: data.record.id};
                                _Modal.open(dataFilter);
                            },
                        },
                        {
                            text: app.localize('Cập nhật'),
                            visible: function (data) {
                                if (data.record.statusOfCandidate == 1) {
                                    return false
                                }
                                else {
                                    return true
                                }
                            },
                            action: function (data) {
                                var dataFilter = { id: data.record.id };
                                _EditModal.open(dataFilter);
                            },
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
