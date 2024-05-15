(function () {
    var _$IntroduceTable = $('#IntroduceTable');
    var _introduceService = abp.services.app.introduce;
    moment.locale(abp.localization.currentLanguage.name);

    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Introduce/ViewIntroduce',
        scriptUrl: abp.appPath + 'view-resources/Profile/Introduce/Edit.js',
        modalClass: 'EditModal',
        modalType: 'modal-xl'
    });

    var _ViewModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Introduce/Detail',
        // scriptUrl: abp.appPath + 'view-resources/Profile/Introduce/Edit.js',
        modalClass: 'Modal',
        modalType: 'modal-xl'
    });


    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        if ($('#Status').val() != '0') {
            dataFilter.status = $('#Status').val();
        }
        return dataFilter;
    }


    var dataTable = _$IntroduceTable.DataTable({
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
            ajaxFunction: _introduceService.getAll,
            inputFilter: getFilter
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
                    return row.article.title
                }
            },
            {
                orderable: true,
                targets: 2,
                data: "fullName",
            },
            {
                orderable: false,
                targets: 3,
                data: "phone",
            },
            {
                orderable: false,
                targets: 4,
                data: "email",
            },
            //{
            //    orderable: false,
            //    targets: 5,
            //    data: "name",
            //},
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
                        return app.localize("Chờ xử lý");
                    }
                    else if (row.status === 2) {
                        return app.localize("Đã xử lý");
                    }
                    else {
                        return app.localize("Không liên lạc được");
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
                        text: app.localize('Xem chi tiết'),
                        action: function (data) {
                            var dataFilter = { id: data.record.id };
                            _ViewModal.open(dataFilter);
                        }
                    },
                    {
                        text: app.localize('Xoá'),
                        action: function (data) {
                            deleteIntroduce(data.record.id);
                        }
                    },
                    {
                        text: app.localize('Cập nhật trạng thái'),
                        action: function (data) {
                            var dataFilter = { id: data.record.id };
                            _Modal.open(dataFilter);
                        }
                    }
                    ]

                },
            }
        ]
    });

  
    function deleteIntroduce(id) {
        abp.message.confirm(
            app.localize('Delete'),
            app.localize('AreYouSure'),
            function (isConfirmed) {
                if (isConfirmed) {
                    _introduceService
                        .delete(id)
                        .done(function () {
                            getDocs();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

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
