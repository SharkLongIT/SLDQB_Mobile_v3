(function () {
    var _$NVNVTableTable = $('#NVNVTable');
    var _nVNVRecruiterService = abp.services.app.nVNVRecruiter;
    moment.locale(abp.localization.currentLanguage.name);

   


    var _EditModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/Recruiters/NVNVEditRecruiter',
        scriptUrl: abp.appPath + 'view-resources/Profile/Recruiters/NVNVUpdate.js',
        modalClass: 'EditModal',
        modalType: 'modal-xl'
    });

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        dataFilter.sphereOfActivity = $('#SphereOfActivity').val();
        dataFilter.address = $('#Address').val();
        return dataFilter;
    }


    var dataTable = _$NVNVTableTable.DataTable({
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
            ajaxFunction: _nVNVRecruiterService.getAllRecruiter,
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
                data: "companyName"
            },
            {
                orderable: false,
                targets: 2,
                data :"taxCode"
            },
            {
                orderable: true,
                targets: 3,
              
                render: function (data, type, row, meta) {
                    return row.humanResSizeCat.displayName
                }
            },
            {
                orderable: false,
                targets: 4,
                data: 'dateOfEstablishment',
                render: function (DateOfEstablishment) {
                    return moment(DateOfEstablishment).format('L');
                }
            },
            {
                orderable: true,
                targets: 5,
                render: function (data, type, row, meta) {
                    return row.sphereOfActivity.displayName
                }
            },
            {
                orderable: true,
                targets: 6,
                render: function (data, type, row, meta) {
                    return row.province.displayName
                }
            },
            {
                orderable: true,
                targets: 7,
                data: "contactPhone"
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
                                window.location.href =
                                    "/Profile/Recruiters/NVNVViewDetailRecruiter/" + data.record.userId;
                            },
                        },
                        {
                            text: app.localize('Sửa'),
                            action: function (data) {
                                var dataFilter = { id: data.record.userId };
                                _EditModal.open(dataFilter);
                            },
                        },
                        {
                            text: app.localize('Xoá'),
                            action: function (data) {
                                deleteRecrui(data.record.id);
                            },
                        },
                    ],
                },
            }
        ]
    });

    function deleteRecrui(id) {
        debugger
        abp.message.confirm(
            app.localize('Delete'),
            app.localize('AreYouSure'),
            function (isConfirmed) {
                if (isConfirmed) {
                    _nVNVRecruiterService
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
