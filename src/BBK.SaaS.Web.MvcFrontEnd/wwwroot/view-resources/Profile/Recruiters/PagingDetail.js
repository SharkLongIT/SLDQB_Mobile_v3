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
                            .append($('<a>')
                                .attr({
                                    'href': abp.appPath + "JobUser/ViewRecruitment?id=" + value.id
                                })
                            .append($('<img>')
                                .addClass('img-90 align-self-center img-fluid mt-3')
                                .attr({
                                    width: 90,
                                    height: 90,
                                    'src': src,
                                })));
                    var labelDegree = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fa-solid fa-money-bill-1"></i>'+' '+ Math.floor(value.minSalary / 1000000) + "-" + Math.floor(value.maxSalary / 1000000) + " Triệu" +'</small>');
                    var labelDateUpdate = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fa fa-map-marker font-awesome"></i>' + ' ' + value.workAddress + '</small>');

                    var labelWorkExp = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fa fa-briefcase font-awesome"></i>' + ' ' + value.experiences.displayName +'</small>');
                    var labelSalary = $('<div>').addClass('col-md-6')
                        .html('<small><i class="fa fa-calendar font-awesome"></i>' + ' ' + moment(value.deadlineSubmission).format('L') + '</small>');
                   
                   // var divbutton = $('<div>').addClass('d-flex justify-content-end')
                      

                    var divInfor = $('<div>')
                        .addClass('flex-grow-1 ms-3')
                        .append($('<div>')
                            .addClass('product-name pb-2')
                            .append($('<h6>')
                                .append($('<a>')
                                    .attr({
                                        'href': abp.appPath + "JobUser/ViewRecruitment?id=" + value.id
                                    })
                                    .text(value.title))))
                        .append($('<small>').html('<i class="fa fa-building font-awesome"></i> ' + value.recruiter.companyName))
                        .append("<hr>")
                        .append($('<div>').addClass('row recruiment')
                            .append(labelDegree)
                            .append(labelDateUpdate))
                        .append($('<div>').addClass('row recruiment')
                            .append(labelWorkExp)
                            .append(labelSalary)
                        )
                    var htmldata = $('<div>')
                        .addClass('col-xxl-4 col-md-6')
                        .append($('<div>')
                            .addClass('prooduct-details-box widget-hover')
                            .append($('<div>')
                                .addClass('d-flex')
                                    .append(divAvatar).append(divInfor)))
                    listJobApp._arearJobApp.append(htmldata)
                   // UserCurrent.check(divbutton, value)

                })
            }
        },
        loadData: function (nameinput, datainput) {

            var url = abp.appPath + "JobUser/GetJobUser";
            var data = {
                pageSize: 6,
                page: 1,
            };
            data.recruiterUserId = $("#recruiterUserId").val();
            callApi.ajax(url, data);
        },
        changePageSize: function () {
            listJobApp._selectpagesize.change(function () {
                var url = abp.appPath + "JobUser/GetJobUser";
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
                    var url = abp.appPath + "JobUser/GetJobUser";
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
