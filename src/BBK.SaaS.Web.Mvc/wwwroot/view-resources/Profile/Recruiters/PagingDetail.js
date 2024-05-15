$(function () {
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
                    } if (result.result.isRecuiters == true) {
                        UserCurrent.IsRecurters = true;
                    }
                    listJobApp.reload(result);

                    pagination.loadpagination(Number(data.pageSize), Math.ceil(result.result.count / Number(data.pageSize)), data.page)
                }
            })
        }
    }



    var listJobApp = {
        _arearJobApp: $(".job-apps"),
        _selectpagesize: $(".pagesize"),
        _divdatarespone: $('.data-respone'),
        reload: function (respone) {
            this._arearJobApp.find('div').remove();
            if (respone.result != null && respone.result != undefined) {
                $.each(respone.result.recruiment, function (index, value) {
                    var src = "Common/Images/default-profile-picture.png"
                    if (value.recruiter.avatarUrl != null) {
                        src = value.recruiter.avatarUrl;
                    }
                    var divAvatar = $('<div>')
                        .addClass('col-lg-12 col-xl-3')
                        .append($('<div>')
                            .addClass('my-gallery')
                            .attr({
                                'id': "aniimated-thumbnials-3"
                            })
                            .append($('<a>')
                                .attr({
                                    'href': abp.appPath + "JobUser/ViewRecruitment?id=" + value.id
                                })
                            .append($('<img>')
                                .addClass('img-130 rounded-circle')
                                .attr({
                                    width: 100,
                                    height: 100,
                                    'src': src,
                                }))));
                    var labelDegree = $('<label>').addClass('m-1')
                        .html('<a><i class="fa fa-money me-1"></i></a>' + Math.floor(value.minSalary / 1000000)+ "-" + Math.floor(value.maxSalary / 1000000) + " Triệu");
                    var labelWorkExp = $('<label>').addClass('m-1')
                        .html('<a><i class="icofont icofont-ui-alarm me-1"></i></a>' + value.experiences.displayName);
                    var labelSalary = $('<label>').addClass('m-1')
                        .html('<a><i class="fa fa-calendar me-1 me-1"></i></a>' + moment(value.deadlineSubmission).format('L'));
                    var labelDateUpdate = $('<label>').addClass('m-1')
                        .html('<a><i class="fa fa-location-arrow me-1"></i></a>' + value.workAddress);
                    var divbutton = $('<div>').addClass('d-flex justify-content-end')
                      

                    var divInfor = $('<div>')
                        .addClass('col-xl-9')
                        .append($('<div>')
                            .addClass('d-flexjustify-content-between')
                            .append($('<h5>').addClass('text-black mb-3')
                                .append($('<a>')
                                    .attr({
                                        'href': abp.appPath + "Profile/Recruitments/ViewRecruitment?id=" + value.id
                                    })
                                    .text(value.title))))
                        .append($('<p>')
                            .addClass('mb-3')
                            .text(value.recruiter.companyName))
                        .append($('<div>')
                            .append(labelDegree)
                            .append(labelWorkExp).append("</br>")
                            .append(labelSalary)
                            .append(labelDateUpdate)
                            .append(divbutton)
                        )
                    var htmldata = $('<div>')
                        .addClass('col-sm-4')
                        .append($('<div>')
                            .addClass('card rounded-3')
                            .append($('<div>')
                                .addClass('profile-img-style')
                                .append($('<div>').addClass('row ')
                                    .append(divAvatar).append(divInfor))))
                    listJobApp._arearJobApp.append(htmldata)
                    //UserCurrent.check(divbutton, value)

                })
            }
        },
        loadData: function (nameinput, datainput) {

            var url = abp.appPath + "Profile/Recruitments/GetJobUser";
            var data = {
                pageSize: 6,
                page: 1,
            };
            data.recruiterUserId = $("#recruiterUserId").val();
            callApi.ajax(url, data);
        },
        changePageSize: function () {
            listJobApp._selectpagesize.change(function () {
                var url = abp.appPath + "Profile/Recruitments/GetJobUser";
                var data = {
                    pageSize: 6,
                    page: 1,
                };
                data.recruiterUserId = $("#recruiterUserId").val();
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
                        }).text('<'))

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
                            .append($('<a>').addClass('page-link active').attr({
                                "data-page": i, "href": 'javascript:void(0)',
                            }).text(i));
                        pagination._areaspagination.append(li);
                    } else {
                        var li = $('<li>').addClass('page-item ')
                            .append($('<a>').attr({
                                "href": 'javascript:void(0)',
                                "data-page": i,
                            }).addClass('page-link').text(i));
                        pagination._areaspagination.append(li);
                    }

                }
                if (pagecurrent < totalItems - (to / 2)) {
                    var li = $('<li>').addClass('page-item ')
                        .append($('<a>').addClass('page-link').text("..."));
                    pagination._areaspagination.append(li);
                }
                if (pagecurrent < totalItems) {
                    var nextPage = parseInt(pagecurrent) + 1;
                    console.log(pagecurrent)
                    var li = $('<li>').addClass('page-item ')
                        .append($('<a>').addClass('page-link').attr({
                            "aria-label": 'Next',
                            "data-page": nextPage,
                            "href": 'javascript:void(0)',
                        }).append($('<span>').attr({
                            "aria-hidden": true
                        }).text('>'))
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
                    var url = abp.appPath + "Profile/Recruitments/GetJobUser";
                    var data = {
                        pageSize: 6,
                        page: page,
                    }
                    data.recruiterUserId = $("#recruiterUserId").val();
                    callApi.ajax(url, data)
                }
            })
        },
        init: function () {
            pagination.changepagination();
        }
    };


    var UserCurrent = {
        IsCandidate: false,
        IsRecurters: false,
        check: function (parents, data) {
            if (UserCurrent.IsCandidate === true && UserCurrent.IsRecurters === false) {
                UserCurrent.addButtonOfCandidate(parents, data)
            }
            else if (UserCurrent.IsRecurters == true) { }
            else
            {
                UserCurrent.addButton(parents, data)
            }
        },
        addButtonOfCandidate: function (parents, data) {
            var text = app.localize("Ứng tuyển")
            if (data.isAppllied == true) {
                text = app.localize("Ứng tuyển lại")
            }
            var divbutton = $('<a>')
                .addClass('d-flex justify-content-end')
                .append($('<strong>')
                    .append($('<a>').addClass('btn btn-primary text-white btnMake ms-2').attr({
                        "data-objId": data.id
                    }).click(function () {
                        var btnClick = $(this);
                        UserCurrent.eventOfCandidate(btnClick[0]);
                    }).text(text)))
            $(parents).append(divbutton);
        },
        addButton: function (parents, data) {
            var text = app.localize("Ứng tuyển")
            if (data.isAppllied == true) {
                text = app.localize("Ứng tuyển lại")
            }
            var divbutton = $('<a>')
                .addClass('d-flex justify-content-end')
                .append($('<strong>')
                    .append($('<a>').addClass('btn btn-primary text-white btnMake ms-2').attr({
                        "data-objId": data.id
                    }).click(function () {
                        abp.message.confirm(
                            app.localize(''),
                            app.localize('Bạn cần đăng nhập !Nhấn Ok để tiếp tục'),
                            function (isConfirmed) {
                                if (isConfirmed) {
                                    window.location.href = abp.appPath + "Account/Login";
                                }
                            })
                    }).text(text)))
            $(parents).append(divbutton);
        },
        eventOfCandidate: function (e) {
            new app.ModalManager({
                viewUrl: abp.appPath + "Profile/ApplicationRequest/CreateApplicationRequestModal",
                scriptUrl: abp.appPath + 'view-resources/Profile/ApplicationRequest/CreateApplicationRequest.js',
                modalClass: "ApplicationRequest"
            }).open({ RecruitmentId: $(e).attr('data-objId') })
        }
    }
    listJobApp.init();
})
