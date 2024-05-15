$(function () {
    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/JobApplication/MakeAnAppointment',
        scriptUrl: abp.appPath + 'view-resources/Profile/Candidate/MakeAnAppointment.js',
        modalClass: 'CreateModal',
        modalType: 'modal-xl'
    });

    var callApi = {
        ajax: function (url, data, callBack) {
            $.ajax({
                url: url,
                data: data,
                caches: false,
                type: 'POST',
                success: function (result) {
                    if (result.result.isCandidate == true) {
                        UserCurrent.IsCandidate = true;
                        //UserCurrent.IsRecurters = true;
                    } if (result.result.isRecruiters == true) {
                        // UserCurrent.IsCandidate = true;
                        UserCurrent.IsRecurters = true;
                    }
                    listJobApp.reload(result);
                    pagination.loadpagination(Number(data.pageSize), Math.ceil(result.result.count / Number(data.pageSize)), Number(data.page))
                }
            })
        }
    }


    var GeoUnits = {
        _changeselect2: false,
        _multipleselect2: false,

        _geoUnitService: abp.services.app.geoUnit,
        reloadProvince: function (location, data, selected) {
            location.children().remove();
            //$(location).append('<option value="" disable>Tất cả địa điểm</option>')
            if (selected == undefined) {
                $.each(data, function (index, item) {

                    if (item.parentId == null) {
                        $(location).append($('<option>',
                            {
                                value: item.id,
                                text: item.displayName,
                            }));
                    }


                })
            } else {
                $.each(data, function (index, item) {
                    if (item.id == selected) {
                        if (item.parentId == null) {
                            $(location).append($('<option>',
                                {
                                    selected: true,
                                    value: item.id,
                                    text: item.displayName,
                                }));
                        }
                    } else {
                        if (item.parentId == null) {
                            $(location).append($('<option>',
                                {
                                    value: item.id,
                                    text: item.displayName,
                                }));
                        }
                    }
                })
            }
        },
        getProvincefromServer: function (location, select) {

            $.ajax({
                url: "/UserJob/GetGeoUnit",
                caches: false,
                success: function (result) {
                    GeoUnits.reloadProvince(location, result.result, select)
                }
            })

        },
        changeselect2: function (location) {
            $(location).select2({
                placeholder: app.localize('Địa điểm mong muốn làm việc'),
                multiple: GeoUnits._multipleselect2,
            });

            $('.select2.select2-container.select2-container--default').css({
                // height: $('select.form-select').css('height'),


            });
            $('.select2-selection.select2-selection--multiple').css({
                border: $('select.form-select').css('border'),
                'background-image': $('select.form-select').css('background-image'),
                'background-repeat': "no-repeat",
                'background-position': "right 0.75rem center",
                'background-size': "16px 12px",
                'border-radius': "var(--bs-border-radius);",
                'padding': "0.375rem 2.25rem 0.375rem 0.75rem;"
            })

            //$('.select2-selection.select2-selection--multiple').css({
            //    "--bs-form-select-bg-img": `url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='%23343a40' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m2 5 6 6 6-6'/%3e%3c/svg%3e");`,
            //    "display": "block",
            //    "width": "100 %",
            //    "padding": "0.375rem 2.25rem 0.375rem 0.75rem",
            //    "font-size": "1rem",
            //    "font-weight": "400",
            //    "line-height": "1.5",
            //    "color": "var(--bs-body-color)",
            //    "background-color": "var(--bs-body-bg)",
            //    "background-image": "var(--bs-form-select-bg-img), var(--bs-form-select-bg-icon, none)",
            //    'background-repeat': "no-repeat",
            //    'background-position': "right 0.75rem center",
            //    'background-size': "16px 12px",
            //    "border": "var(--bs - border - width) solid var(--bs - border - color)",
            //    "border-radius": " var(--bs - border - radius)",
            //    "transition": "border - color 0.15s ease -in -out, box - shadow 0.15s ease -in -out",
            //    "-webkit-appearance": "none",
            //    "-moz-appearance": "none",
            //    "appearance": "none",
            //})
        },
        init: function (location, select) {
            GeoUnits.getProvincefromServer(location, select);
            if (GeoUnits._changeselect2 == true) {
                GeoUnits.changeselect2(location);
            }
        }
    }

    var filter = {
        _formsearch: $('div[name=filterAdvanced]'),
        _inputsearch: $('input[type=search]'),
        changeInput: function () {
            var url = abp.appPath + "UserJob/GetUserJob";

            var data = {
                pageSize: /*listJobApp._selectpagesize.val()*/ 10,
                page: 1,
                //search: filter._inputsearch.val().trim(),
            }
            callApi.ajax(url, data)
        },
        init: function () {
            filter._inputsearch.on('keypress', function (e) {
                if (e.which == 13) {

                    filter.changeInput();
                }
            });
            //filter._formsearch.find('button').click(function () {
            //    filter.changeInput();
            //})
        }
    };

    var filteradvanced = {
        _areasfilter: $("div[name=filterAdvanced]"),
        _location: $('select[name=location]'),
        loadGeoUnit: function () {
            GeoUnits.init(filteradvanced._location)
        },
        filter: function () {
            var WorkSite = {
                _location: $('select[name=location]').val(),
            }
            listJobApp.loadData('WorkSite', WorkSite);


        },
        init: function () {
            filteradvanced._areasfilter.find('button').click(function () {
                filteradvanced.filter();
            });

            filteradvanced.loadGeoUnit();
        },
    }

    var listJobApp = {
        _arearJobApp: $(".job-apps"),
        _selectpagesize: $(".pagesize"),
        _divdatarespone: $('.data-respone'),
        reload: function (respone) {
            this._arearJobApp.find('div').remove();
            if (respone.result != null && respone.result != undefined) {
                $.each(respone.result.candidate, function (index, value) {
                    var src = "Common/Images/default-profile-picture.png";
                    var herf = abp.appPath + "UserJob/Detail?id=" + value.jobApplication.id;
                    if (value.profilePictureId != null) {
                        src = '/Profile/GetProfilePictureByUser?userId=' + value.user.id + '&&profilePictureId=' + value.profilePictureId;
                    }
                    var divAvatar = $('<a>')
                        .attr({
                            href: herf,
                        })
                        .append($('<img>').addClass('align-self-center img-fluid img-90 mt-3')
                            .attr({
                                'src': src,
                                'width': 90,
                                'height': 90,
                            }));
                  


                    var spanWorkExp = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fa fa-briefcase font-awesome"></i> ' + value.jobApplication.experiences.displayName+ '</small > ');
                    var liSalary = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fa-solid fa-money-bill-1"></i> ' + ' Lương thỏa thuận </small > ');
                    if (value.jobApplication.desiredSalary != null) {
                        var desiredSalary = Math.floor(value.jobApplication.desiredSalary / 1000000);
                        if (desiredSalary > 1) {
                            liSalary = $('<div>').addClass('col-md-6')
                                .html('<small><i class="fa-solid fa-money-bill-1"></i>' + ' ' + desiredSalary + ' Triệu </small > ');
                        }
                    }
                    var liliteracy = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fas fa-sync me-1 font-awesome"></i>' + ' ' + value.jobApplication.literacy.displayName + '</small>');
                    var lilcreationTime = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fas fa-sync me-1 font-awesome"></i>' + ' ' + value.lastModificationTimeString + '</small>');

                    //var divbutton = $('<div>').addClass('d-flex justify-content-end')
                    //    .append($('<button>').click(function () {
                    //        window.location.href = abp.appPath + "UserJob/Detail?id=" + value.jobApplication.id;
                    //    }).text('Xem hồ sơ')
                    //        .addClass('btn btn-primary btn-lg'))
                    var divInfor = $('<div>')
                        .addClass('flex-grow-1 ms-3')
                        .append($('<div>')
                            .addClass('product-name pb-2')
                            .append($('<h6>')
                                .append($('<a>')
                                    .attr({
                                        'href': herf
                                    })
                                    .text(value.user.name))))
                        .append($('<small>').text(value.jobApplication.positions.displayName))
                        .append("<hr>")
                        .append($('<div>').addClass('row recruiment')
                            .append(liSalary)
                            .append(liliteracy))
                        .append($('<div>').addClass('row recruiment')
                            .append(spanWorkExp)
                            .append(lilcreationTime)
                        )
                    var htmldata = $('<div>').addClass('col-xxl-4 col-md-4 mt-3')
                        .append($('<div>').addClass('prooduct-details-box widget-hover')
                            .append($('<div>').addClass('d-flex')
                                .append(divAvatar)
                                .append(divInfor)));

                    listJobApp._arearJobApp.append(htmldata)
                    //UserCurrent.check(divbutton, value)
                })
            }
        },
        loadData: function (nameinput, datainput) {

            var url = abp.appPath + "UserJob/GetUserJob";
            var data = {
                pageSize: /*listJobApp._selectpagesize.val()*/ 9,
                page: 1,
                //search: $('input[type=search]').val(),
            };
            if ($('#Gender').val() != "" && $('#Gender').val() != 0 && $('#Gender').val() != undefined && $('#Gender').val() != null) {
                data.gender = $('#Gender').val();
            }
            if ($('#ExperiencesId').val() != "" && $('#ExperiencesId').val() != 0 && $('#ExperiencesId').val() != undefined && $('#ExperiencesId').val() != null) {
                data.experiencesId = $('#ExperiencesId').val();
            }
            if ($('#LiteracyId').val() != "" && $('#LiteracyId').val() != 0 && $('#LiteracyId').val() != undefined && $('#LiteracyId').val() != null) {
                data.literacyId = $('#LiteracyId').val();
            }
            if ($('#OccupationId').val() != "" && $('#OccupationId').val() != 0 && $('#OccupationId').val() != undefined && $('#OccupationId').val() != null) {
                data.occupationId = $('#OccupationId').val();
            }
            if ($('#SalaryMin').val() != "" && $('#SalaryMin').val() != 0 && $('#SalaryMin').val() != undefined && $('#SalaryMin').val() != null) {
                data.salaryMin = $('#SalaryMin').val();
            }
            if ($('#SalaryMax').val() != "" && $('#SalaryMax').val() != 0 && $('#SalaryMax').val() != undefined && $('#SalaryMax').val() != null) {
                data.salaryMax = $('#SalaryMax').val();
            }
            if (nameinput != undefined && nameinput === "WorkSite" && datainput != undefined) {
                data.workSite = [];
                $.each(datainput._location, function (index, value) {
                    if (value != '') {
                        data.workSite.push(value);
                    }
                });

            }

            callApi.ajax(url, data);
        },
        changePageSize: function () {
            listJobApp._selectpagesize.change(function () {
                var url = abp.appPath + "UserJob/GetUserJob";
                var data = {
                    pageSize: /*listJobApp._selectpagesize.val()*/ 12,
                    page: 1,
                    search: $('input[type=search]').val().trim(),
                };
                callApi.ajax(url, data);
            })
        },
        init: function () {
            this.changePageSize();
            this.loadData();
        }

    };

    var pagination = {
        _areaspagination: $('.pagination'),
        loadpagination: function (pageSize, totalItems, pagecurrent) {
            pagination._areaspagination.find('li').remove();
            if (pagecurrent != undefined && pagecurrent != null) {
                if (pagecurrent > 1) {
                    var prePage = pagecurrent - 1;
                    var li = $('<li>').addClass('page-item')
                        .append($('<a>').addClass('page-link').attr({
                            "aria-label": 'Previous',
                            "data-page": prePage,
                            "href": 'javascript:void(0)',
                        }).append($('<span>').attr({
                            "aria-hidden": true
                        }).text('«'))

                            .append($('<span>').addClass('sr-only').text('Previous')));
                    pagination._areaspagination.append(li);
                }
            }
            if (pageSize != undefined && pageSize != null && totalItems != undefined && totalItems != null) {
                var from = pagecurrent - pageSize;
                var to = pagecurrent + pageSize;
                if (from < 0) {
                    from = 1;
                    to = pageSize * 2;
                }
                if (to > totalItems) {
                    to = totalItems
                }
                for (var i = from; i <= to; i++) {
                    if (pagecurrent == i) {
                        var li = $('<li>').addClass('page-item active')
                            .append($('<a>').addClass('page-link').attr({
                                "data-page": i, "href": 'javascript:void(0)',
                            }).text(i));
                        pagination._areaspagination.append(li);
                    } else {
                        var li = $('<li>').addClass('page-item')
                            .append($('<a>').attr({
                                "href": 'javascript:void(0)',
                                "data-page": i,
                            }).addClass('page-link').text(i));
                        pagination._areaspagination.append(li);
                    }

                }
                if (pagecurrent < totalItems - (to / 2)) {
                    var li = $('<li>').addClass('page-item')
                        .append($('<a>').addClass('page-link').text("..."));
                    pagination._areaspagination.append(li);
                }
                if (pagecurrent < totalItems) {
                    var nextPage = parseInt(pagecurrent) + 1;
                    var li = $('<li>').addClass('page-item')
                        .append($('<a>').addClass('page-link').attr({
                            "aria-label": 'Next',
                            "data-page": nextPage,
                            "href": 'javascript:void(0)',
                        }).append($('<span>').attr({
                            "aria-hidden": true
                        }).text('»'))
                            .append($('<span>').addClass('sr-only').text('Next')));
                    pagination._areaspagination.append(li);
                }
            }
            pagination.changepagination();
        },
        changepagination: function () {
            pagination._areaspagination.find('a').click(function () {
                var page = $(this).attr('data-page');
                if (page != undefined) {
                    pagination._areaspagination.find('li').removeClass('active');
                    $(this).parents('li').addClass("active");
                    var url = abp.appPath + "UserJob/GetUserJob";
                    var data = {
                        pageSize: /*listJobApp._selectpagesize.val()*/ 10,
                        page: page,
                        search: $('input[type=search]').val().trim(),
                    }
                    callApi.ajax(url, data)
                }
            })
        },
        init: function () {
            pagination.changepagination();
        }
    };
    // check nut dat lich
    var UserCurrent = {
        IsCandidate: false,
        IsRecurters: false,
        check: function (parents, data) {
            if (UserCurrent.IsCandidate === false && UserCurrent.IsRecurters === true) {
                UserCurrent.addButtonOfCandidate(parents, data)
            }
        },
        addButtonOfCandidate: function (parents, data) {
            var text = app.localize("Đặt lịch")
            var divbutton = $('<a>')
                .addClass('d-flex justify-content-end')
                .append($('<strong>')
                    .append($('<a>').addClass('btn btn-primary btn-lg me-1 ms-2').attr({
                        "data-objId": data.jobApplication.id
                    }).click(function () {
                        var btnClick = $(this);
                        UserCurrent.eventOfCandidate(btnClick[0]);
                    }).text(text)))
            $(parents).append(divbutton);
        },
        eventOfCandidate: function (e) {
            _Modal.open({ jobId: $(e).attr('data-objId') })
        }
    }

    GeoUnits._multipleselect2 = true;
    GeoUnits._changeselect2 = true;
    // pagination.init();
    listJobApp.init();
    filter.init();
    filteradvanced.init();
})
