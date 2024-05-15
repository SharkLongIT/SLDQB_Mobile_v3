(function () {
    var _$QuestionsOfMeTable = $('#QuestionsOfMeTable');
    moment.locale(abp.localization.currentLanguage.name);

    var _ViewModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Recruiters/QuestionsOfMeDetail',
        modalClass: 'Modal',
        modalType: 'modal-xl'
    });


    /* DEFINE TABLE */

    var dataTable = _$QuestionsOfMeTable.DataTable({
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
            "url": "/Profile/Recruiters/GetAllQuestionsOfCandidate",
            data: function (d) {
                d.search = $('#SearchTerm').val();
                if ($('#Status').val() != '0') {
                    if ($('#Status').val() == 1) {
                        d.status = true;
                    } else {
                        d.status = false;
                    }
                }
            },
            dataSrc: "result.items"
        },
        //listAction: {
        //    ajaxFunction: _introduceService.getAllByUserType,
        //    inputFilter: getFilter
        //},
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
                    if (row.description != null) {
                        return '<span>' + row.description + ' </span>'
                    }
                }
            },
            {
                orderable: true,
                targets: 2,
                render: function (data, type, row, meta) {
                    return moment(row.creationTime).format('L')
                }
            },
            {
                orderable: false,
                targets: 3,
                render: function (data, type, row, meta) {
                    if (row.lastModificationTime != null &&  row.status === true ) {
                        return moment(row.lastModificationTime).format('L')
                    } else {
                        return '<span><span>'
                    }
                }
            },
            {
                orderable: false,
                targets: 4,
                render: function (data, type, row, meta) {
                    if (row.status === true) {
                        return '<span class="badge bg-success text-white rounded-pill p-2">Đã trả lời</span>';
                    }
                    else {
                        return '<span class="badge bg-danger text-white rounded-pill p-2">Chưa trả lời</span>';
                    }
                }
            },
            {
                targets: 5,
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
                            var dataFilter = { id: data.record.id };
                            _ViewModal.open(dataFilter);
                        }
                    },
                    {
                        text: app.localize('Xoá'),
                        visible: function () {
                            return false
                        },
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
