(function () {
    var _$CandidateTable = $('#CandidateTable');
    var _Service = abp.services.app.tradingSessionAccount;
    moment.locale(abp.localization.currentLanguage.name);


    var _frmForm = null;
    var _modalManager;
    this.init = function (modalManager) {
        _modalManager = modalManager;
        _frmForm = _modalManager.getModal().find('form[name=FormCandidate]');
    }

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.id = $("#TradingId").val();
        return dataFilter;
    }


    var dataTable = _$CandidateTable.DataTable({
        paging: true,
        serverSide: false,
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
            ajaxFunction: _Service.getAllCandidateNot,
            inputFilter: getFilter
        },
        order: [[1, 'asc']],
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                className: 'dt-body-center text-center',
                render: function (data, type, row, meta) {
                    return '<input type="checkbox" id="checkDelete" data-value="' + row.candidate.id + '"  data-userid="' + row.candidate.account.id + '" data-jobid="' + row.id + '" >';
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
                render: function (data, type, row, meta) {
                    return row.candidate.account.name;
                }
            },
            {
                orderable: false,
                targets: 3,
                render: function (data, type, row, meta) {
                    if (row.candidate.gender == 1) {
                        return 'Nam';
                    }
                    else if (row.candidate.gender == 2) {
                        return 'Nữ';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                orderable: false,
                targets: 4,
                render: function (data, type, row, meta) {
                    return moment(row.candidate.dateOfBirth).format('L');
                }
            },
            {
                orderable: true,
                targets: 5,
                render: function (data, type, row, meta) {
                    if (row.candidate.province != null) {
                        return row.candidate.province.displayName;
                    }
                    else {
                        return ' ';
                    }
                }
            },
            {
                orderable: true,
                targets: 6,
                render: function (data, type, row, meta) {
                    return row.occupations.displayName;
                }
            },
            {
                orderable: true,
                targets: 7,
                render: function (data, type, row, meta) {
                    return row.literacy.displayName;
                }
            },
            {
                orderable: true,
                targets: 8,
                render: function (data, type, row, meta) {
                    return row.experiences.displayName;
                }
            },
        ]
    });


    //xoa all
    $('#example-select-all').on('click', function () {
        // Get all rows with search applied
        var rows = dataTable.rows({ 'search': 'applied' }).nodes();
        // Check/uncheck checkboxes for all rows in the table
        $('#checkDelete', rows).prop('checked', this.checked);
        $('#btnCreate').removeAttr('disabled');
        var selected = new Array();
        $('#CandidateTable tbody input[type="checkbox"]:checked').each(function () {
            selected.push($(this));
        });
        if (selected.length > 0) {
            $('#btnCreate').removeAttr('disabled');
        } else {
            $('#btnCreate').prop('disabled', true);
        }
    });


    $('#CandidateTable tbody').on('change', 'input[type="checkbox"]', function () {
        // If checkbox is not checked
        var selected = new Array();
        $('#CandidateTable tbody input[type="checkbox"]:checked').each(function () {
            selected.push($(this));
        });
        if (selected.length > 0) {
            $('#btnCreate').removeAttr('disabled');
        } else {
            $('#btnCreate').prop('disabled', true);
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
            $('#btnCreate').removeAttr('disabled');
        }
    })


    $('#btnCreate').on('click', function (e) {
        abp.message.confirm(
            app.localize(abp.localization.localize("Mời người lao động")),
            app.localize(abp.localization.localize("AreYouSure")),
            function (isConfirmed) {
                if (isConfirmed) {
                    // Iterate over all checkboxes in the table
                    dataTable.$('input[type="checkbox"]').each(function (index, value) {
                        if ($(value).is(":checked") && $(value).attr("data-value") != 'undefined') {
                            var data = {};
                            data.tradingSessionId = $("#TradingId").val();
                            data.status = 0;
                            data.candidateId = $(value).attr("data-value");
                            data.usertId = $(value).attr("data-userid")
                            data.jobApplicationId = $(value).attr("data-jobid")
                            _Service.create(data)
                                .done(function () {
                                    $("#FormCandidate .close-button").click()
                                    abp.notify.info(abp.localization.localize("Mời thành công"));
                                    window.setTimeout(function () {
                                        window.location.reload();
                                    }, 2000)
                                }).always(function () {
                                    abp.ui.clearBusy('#FormCandidate');
                                });
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
    //jQuery(document).ready(function () {
    //    $("#SearchTerm").focus();
    //});
})(jQuery);
