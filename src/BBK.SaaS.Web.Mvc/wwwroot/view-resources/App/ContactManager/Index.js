(function () {
    var _$ContactTable = $('#ContactTable');
    var _contactService = abp.services.app.contact;
    moment.locale(abp.localization.currentLanguage.name);


    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'App/ContactManager/ContactDetail',
        scriptUrl: abp.appPath + 'view-resources/App/ContactManager/Send.js',
        modalClass: 'SendModal',
        modalType: 'modal-xl'
    });

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.status = $("#Status").val();
        dataFilter.startDay = $("#StartDay").val();
        dataFilter.endDay = $("#EndDay").val();
        return dataFilter;
    }


    var dataTable = _$ContactTable.DataTable({
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
            ajaxFunction: _contactService.getAll,
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
                data: "description"
            },
            {
                orderable: false,
                targets: 2,
                data: "fullName"
            },
            {
                orderable: false,
                targets: 3,
                data: "email"
            },
            {
                orderable: false,
                targets: 4,
                data: "phone"
            },
            {
                orderable: true,
                targets: 5,
                data: 'creationTime',
                render: function (creationTime) {
                    return moment(creationTime).format('L');
                }
            },
            {
                orderable: false,
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.status == true) {
                        return app.localize('Đã trả lời')
                    }
                    else {
                        return app.localize('Chưa trả lời')
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
                            text: app.localize('Xem'),
                            action: function (data) {
                                //window.location.href =
                                //    "/App/ContactManager/ContactDetail/" + data.record.id;
                                var dataFilter = { id: data.record.id };
                                _Modal.open(dataFilter);
                            },
                        },
                        {
                            //text: app.localize('Ẩn'),
                            //action: function (data) {
                            //    Update(data.record.id);
                            //},
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
