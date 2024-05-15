(function () {
    var _$RecruitmentTable = $('#RecruitmentTable');
    var _recruitmentService = abp.services.app.recruitment;
    moment.locale(abp.localization.currentLanguage.name);

    $("#CreateNewButtonxx").click(function () {
        window.location.href =
            "/Profile/Recruitments/CreateRecruitment";
    })

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.filtered = $('#SearchTerm').val();

        dataFilter.numberOfRecruits = $('#NumberOfRecruits').val();

        if ($('#FormOfWork').val() != "") {
            dataFilter.formOfWork = $('#FormOfWork').val();
        }
        if ($('#Experience').val() != "") {
            dataFilter.experience = $('#Experience').val();
        }
        if ($('#Job').val() != "") {
            dataFilter.job = $('#Job').val();
        }
        dataFilter.fromDate = $("#StartDay").val();
        dataFilter.toDate = $("#EndDay").val();
        return dataFilter;
    }


    var dataTable = _$RecruitmentTable.DataTable({
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
            ajaxFunction: _recruitmentService.getAll,
            inputFilter: getFilter
        },
        order: [[1, 'asc']],
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                className: 'dt-body-center text-center',
                render: function (data, type, row, meta) {
                    return '<input type="checkbox" id="checkDelete" data-value="' + row.id + '">';
                }
            },
            {
                targets: 1,
                orderable: true,
                className: 'dt-body-center text-center',
                render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                orderable: true,
                targets: 2,
                data: "title"
            },
            {
                orderable: false,
                targets: 3,
                data: "jobCatUnitName"
            },
            {
                orderable: false,
                targets: 4,
                render: function (data, type, row, meta) {
                    if (row.formOfWork == 1) {
                        return app.localize('Toàn thời gian cố định')
                    }
                    else {
                        return app.localize('Toàn thời gian tạm thời')
                    }
                }
            },
            {
                orderable: true,
                targets: 5,
                render: function (data, type, row, meta) {
                    return row.experiences.displayName
                }
            },
            {
                orderable: true,
                targets: 6,
                data: 'numberOfRecruits'
            },
            {
                orderable: true,
                targets: 7,
                data: 'deadlineSubmission',
                render: function (creationTime) {
                    return moment(creationTime).format('L');
                }
            },
            {
                orderable: true,
                targets: 8,
                render: function (data, type, row, meta) {
                    if (row.status == false) {
                        return '<div class="form-check-size"><div class="form-check form-switch form-check-inline"><input class="form-check-input switch-primary check-size" id="checkAn" type="checkbox" role="switch" disabled></div></div>'
                    }
                    else {
                        return '<div class="form-check-size"><div class="form-check form-switch form-check-inline"><input class="form-check-input switch-primary check-size" id="checkAn" type="checkbox" role = "switch" checked = "" disabled ></div></div>'
                    }
                }

            },
            {
                targets: 9,
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
                            text: app.localize('Edit'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/Recruitments/EditRecruitment/" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Xem'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/Recruitments/ViewRecruitment/" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Delete'),
                            action: function (data) {
                                $.ajax({
                                    url: '/Profile/Recruitments/Delete/' + data.record.id,
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
                            },
                        },
                    ],
                },
            }
        ]
    });


    //xoa all
    $('#example-select-all').on('click', function () {
        // Get all rows with search applied
        var rows = dataTable.rows({ 'search': 'applied' }).nodes();
        // Check/uncheck checkboxes for all rows in the table
        $('#checkDelete', rows).prop('checked', this.checked);
        $('#DeleteAll').removeAttr('disabled');
        var selected = new Array();
        $('#RecruitmentTable tbody input[type="checkbox"]:checked').each(function () {
            selected.push($(this));
        });
        if (selected.length > 0) {
            $('#DeleteAll').removeAttr('disabled');
        } else {
            $('#DeleteAll').prop('disabled', true);
        }
    });


    $('#RecruitmentTable tbody').on('change', 'input[type="checkbox"]', function () {
        // If checkbox is not checked
        var selected = new Array();
        $('#RecruitmentTable tbody input[type="checkbox"]:checked').each(function () {
            selected.push($(this));
        });
        if (selected.length > 0) {
            $('#DeleteAll').removeAttr('disabled');
        } else {
            $('#DeleteAll').prop('disabled', true);
        }

        if (!this.checked) {
            var el = $('#example-select-all').get(0);
            if (el && el.checked && ('indeterminate' in el)) {
                el.indeterminate = true;
            }
        }
    });

    dataTable.$('input[type="checkbox"]').each(function (index, value) {
        if (value > 0) {
            $('#DeleteAll').removeAttr('disabled');
        }
    })


    $('#DeleteAll').on('click', function (e) {
        abp.message.confirm(
            app.localize(abp.localization.localize("Delete")),
            app.localize(abp.localization.localize("AreYouSure")),
            function (isConfirmed) {
                if (isConfirmed) {
                    // Iterate over all checkboxes in the table
                    dataTable.$('input[type="checkbox"]').each(function (index, value) {
                        if ($(value).is(":checked") && $(value).attr("data-value") != 'undefined') {
                            $.ajax({
                                url: '/Profile/Recruitments/Delete/' + $(value).attr("data-value"),
                                type: "post",
                                cache: false,
                                success: function (results) {
                                    $('#example-select-all').prop('checked', false);
                                    abp.notify.success(app.localize('Xóa thành công'));
                                    getDocs();
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert("error");
                                }
                            });
                            //_recruitmentService
                            //    .deleteDoc(
                            //        $(value).attr("data-value")
                            //    ).done(function () {
                            //        $('#example-select-all').prop('checked', false);
                            //        abp.notify.success(app.localize('Xóa thành công'));
                            //        getDocs();
                            //    });
                        }
                    }
                    );

                }
            });
    })


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
