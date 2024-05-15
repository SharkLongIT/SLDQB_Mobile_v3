(function () {
    var _$RecruitmentTable = $('#RecruitmentTable');
    var _jobApplicationService = abp.services.app.jobApplication;
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

  
    
    $("#CreateNewButtonxx").click(function () {
        window.location.href =
            "/Profile/JobApplication/CreateJobOfCandidate";
    })

    /* DEFINE TABLE */
    var getFilter = function () {
        let dataFilter = {};
        dataFilter.search = $('#SearchTerm').val();
        if ($('#WorkSite').val() != 0 && $('#WorkSite').val() != "" && $('#WorkSite').val() != undefined && $('#WorkSite').val() != null) {
            dataFilter.workSite = $('#WorkSite').val();
        }
        if ($('#FormOfWorkId').val() != 0 && $('#FormOfWorkId').val() != "" && $('#FormOfWorkId').val() != undefined && $('#FormOfWorkId').val() != null) {
            dataFilter.formOfWorkId = $('#FormOfWorkId').val();
        }
        if ($('#OccupationId').val() != 0 && $('#OccupationId').val() != "" && $('#OccupationId').val() != undefined && $('#OccupationId').val() != null) {
            dataFilter.occupationId = $('#OccupationId').val();
        }
        if ($('#LiteracyId').val() != 0 && $('#LiteracyId').val() != "" && $('#LiteracyId').val() != undefined && $('#LiteracyId').val() != null) {
            dataFilter.literacyId = $('#LiteracyId').val();
        }
        if ($('#ExperiencesId').val() != 0 && $('#ExperiencesId').val() != "" && $('#ExperiencesId').val() != undefined && $('#ExperiencesId').val() != null) {
            dataFilter.experiencesId = $('#ExperiencesId').val();
        }
       
    
       
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
            ajaxFunction: _jobApplicationService.getListJobAppOfCandidate,
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
                data: "occupations", 
                render: function (data, type, row, meta) {
                    return row.occupations.displayName;
                }
            },
            {
                orderable: false,
                targets: 4,
                data: "literacy",
                render: function (data, type, row, meta) {
                    return row.literacy.displayName;
                }
            },
            {
                orderable: true,
                targets: 5,
                data: "experiences",
                   render: function (data, type, row, meta) {
                       return row.experiences.displayName;
                }
              
            },
            {
                orderable: true,
                targets: 6,
                data: 'workSite',
                render : function (data,type, row, meta){
                    return row.province.displayName;
                }
            },
            {
                orderable: true,
                targets: 7,
                data: 'formOfWork',
                render: function (data, type, row, meta) {
                    return row.formOfWork.displayName;
                }
               
            },
            {
                orderable: true,
                targets: 8,
                render: function (data, type, row, meta) {
                    return moment(row.creationTime).format('L')
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
                                    "/Profile/JobApplication/CreateJobOfCandidate?jobAppId=" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Xem'),
                            action: function (data) {
                                window.location.href =
                                    "/Profile/JobApplication/Detail?id=" + data.record.id;
                            },
                        },
                        {
                            text: app.localize('Delete'),
                            action: function (data) {
                                var url = abp.appPath + "Profile/JobApplication/DeleteJobApplication";
                                var data = data.record
                                callApi.ajax(url, data, getDocs, null, abp.notify.info(abp.localization.localize("Successfully")))
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
                                url: '/Profile/JobApplication/DeleteJobApplication/' + $(value).attr("data-value"),
                                type: "post",
                                cache: false,
                                success: function (results) {
                                    $('#example-select-all').prop('checked', false);
                                   
                                    getDocs();
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert("error");
                                }
                            });
                        }
                    }
                    );
                     abp.notify.success(app.localize('Xóa thành công'));
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
