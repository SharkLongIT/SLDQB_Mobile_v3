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
        dataFilter.rank = $('#Ranks').val();
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
        listAction: {
            ajaxFunction: _makeAnAppointmentService.getAll,
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
                data: 'typeInterview'
            },
            {
                orderable: true,
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.statusOfCandidate == 1) {
                        return 'Chờ phỏng vấn'
                    }
                    else if (row.statusOfCandidate == 2) {
                        return 'Xác nhận phỏng vấn'
                    } else if (row.statusOfCandidate == 3) {
                        return 'Từ chối phỏng vấn'
                    }
                    else if (row.statusOfCandidate == 4) {
                        return 'Đã phỏng vấn'
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
                        return 'Pass'
                    }
                    else if (row.interviewResultLetter == 2){
                        return 'Fail'
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
