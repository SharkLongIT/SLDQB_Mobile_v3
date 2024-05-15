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
                type: "POST",
                url: url,
                data: data,
                caches: false,
                success: function (result) {
                    if (result.result.isCandidate == true) {
                        UserCurrent.IsCandidate = true;
                        //UserCurrent.IsRecurters = true;
                    } if (result.result.isRecruiters == true) {
                        // UserCurrent.IsCandidate = true;
                        UserCurrent.IsRecurters = true;
                    }
                    listJobApp.reload(result);
                    pagination.loadpagination(Number(data.pageSize), Math.ceil(result.result.count / Number(data.pageSize)), data.page)
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
            GeoUnits._geoUnitService.getGeoUnits({}).done(function (result) {
                GeoUnits.reloadProvince(location, result.items, select)
            })
        },
        changeselect2: function (location) {
            $(location).select2({
                placeholder: app.localize('Tất cả địa điểm'),
                multiple: GeoUnits._multipleselect2,
            });
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
                    var src = "Common/Images/default-profile-picture.png"
                    if (value.profilePictureId != null) {
                        src = '/Profile/GetProfilePictureByUser?userId=' + value.user.id + '&&profilePictureId=' + value.profilePictureId;
                    }
                    var divAvatar = $('<div>')
                        .addClass('col-lg-12 col-xl-3')
                        .append($('<div>')
                            .addClass('my-gallery')
                            .attr({
                                'id': "aniimated-thumbnials-3"
                            })
                            .append($('<img>')
                                .addClass('img-thumbnail img-rounded user-edit-dialog-profile-image m-r-10')
                                .attr({
                                    'src': src,
                                    'width': 128,
                                    'height': 128
                                })));
                    var labelDegree = $('<label>').addClass('me-3')
                        .html('<a><i class="fa fa-mortar-board me-1"></i></a>' + value.jobApplication.literacy.displayName);
                    var labelWorkExp = $('<label>').addClass('me-3')
                        .html('<a><i class="fa fa-calendar me-1"></i></a>' + value.jobApplication.experiences.displayName)
                     var labelSalary = $('<label>').addClass('me-3')
                          .html('<a><i class="fa fa-money me-1 me-1"></i></a>Lương thỏa thuận');
                     if (value.jobApplication.desiredSalary != null) {
                          labelSalary = $('<label>').addClass('me-3')
                               .html('<a><i class="fa fa-money me-1 me-1"></i></a>' + Math.floor(value.jobApplication.desiredSalary / 1000000) + " Triệu");
                     }
                    var labelDateUpdate = $('<label>').addClass('me-3')
                        .html('<a><i class="fas fa-sync me-1"></i></a>' + jQuery.timeago(value.jobApplication.creationTime));
                    var divbutton = $('<div>').addClass('d-flex justify-content-end')
                        //.append($('<button>').click(function () {
                        //    var dataFilter = { jobId: value.jobApplication.id };
                        //    _Modal.open(dataFilter);
                        //}).text('Đặt lịch')
                        //    .addClass('btn btn-primary btn-lg me-1'))
                        .append($('<button>').click(function () {
                            window.location.href = abp.appPath + "UserJob/Detail?id=" + value.jobApplication.id;
                        }).text('Xem hồ sơ')
                            .addClass('btn btn-primary btn-lg'))
                    var divInfor = $('<div>')
                        .addClass('col-xl-9')
                        .append($('<div>')
                            .addClass('d-flexjustify-content-between')
                            .append($('<h5>').addClass('text-black mb-3')
                                .append($('<a>')
                                    .attr({
                                        'href': abp.appPath + "UserJob/Detail?id=" + value.jobApplication.id
                                    })
                                    .text(/*value.user.surname + " " +*/ value.user.name))))
                        .append($('<p>')
                            .addClass('mb-3')
                            .text(value.jobApplication.positions.displayName))
                        .append($('<div>')
                            .append(labelDegree)
                            .append(labelWorkExp)
                            .append(labelSalary)
                            .append(labelDateUpdate)
                            .append(divbutton)
                        )
                    var htmldata = $('<div>')
                        .addClass('col-sm-6')
                        .append($('<div>')
                            .addClass('card')
                            .append($('<div>')
                                .addClass('profile-img-style')
                                .append($('<div>').addClass('row')
                                    .append(divAvatar).append(divInfor))))
                    listJobApp._arearJobApp.append(htmldata)
                    UserCurrent.check(divbutton, value)
                })
            }
        },
        loadData: function (nameinput, datainput) {

            var url = abp.appPath + "UserJob/GetUserJob";
            var data = {
                pageSize: /*listJobApp._selectpagesize.val()*/ 10,
                page: 1,
                //search: $('input[type=search]').val(),
                workSite: null,

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
            data.workSite = [];
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
                    pageSize: /*listJobApp._selectpagesize.val()*/ 10,
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
                        var li = $('<li>').addClass('page-item')
                            .append($('<a>').addClass('page-link active').attr({
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
                    var nextPage = pagecurrent + 1;
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
                    pagination._areaspagination.find('a').removeClass('active');
                    $(this).addClass("active");
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
