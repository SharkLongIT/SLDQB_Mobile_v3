$(function () {

    var callApi = {
        ajax: function (url, data, callBack, beforeSend, handleData) {
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                caches: false,
                beforeSend: beforeSend,
                contentType: app.consts.contentTypes.formUrlencoded,
                success: function (result) {
                    if (callBack != undefined || callBack != null) {

                        callBack(result)
                    }
                    //if (handleData != undefined) {
                    //    handleData
                    //}
                },
              
                
            }).done(function () {
                handleData
            }).fail(function (error) {
                abp.ajax.showError(error);
            })
        }
    };
    //#region Add validate 
    $.validator.addMethod("valueDefault", function (value, element) {
        if (value > 0) {
            return true;

        } else {
            return false;
        }
    }, "Invalid Date!");


    //#endregion

    //#region  JobApplication
    var JobApplication = {
        _$cardJobApplication: $('div.card.card-jobapplication'),
        _areasJobApplication: $('#JobApplication'),
        _$JobApplicationInformationForm: $('form[name=JobAppInfoForm]'),
        _Isform: false,
        _IsPreView: false,
        _IsRedirect: false,
        fileMgr: null,

        edit: function () {
            JobApplication._IsPreView = false;
            JobApplication._Isform = true;

            var url = abp.appPath + "Profile/JobApplication/PartialViewCommonJobApp";
            callApi.ajax(url, null, JobApplication.getdata);

        },
        redirect: function (data) {
            window.location.href = abp.appPath + "Profile/JobApplication/CreateJobOfCandidate?jobAppId=" + data.result.id;
            /*callApi.ajax(url, data);*/
        },
        preview: function (data) {

            $('#JobApplicationId').attr({
                value: data.result.id
            })
            var url = abp.appPath + "Profile/JobApplication/ViewJobApp";
            JobApplication._IsPreView = true;
            JobApplication._Isform = false;
            callApi.ajax(url, null, JobApplication.getdata);
        },
        getdata: function (data) {
            var url = abp.appPath + "Profile/JobApplication/GetJobApp?id=" + $('#JobApplicationId').val();

            JobApplication._areasJobApplication.find('div').remove();

            JobApplication._areasJobApplication.append(data);
            JobApplication.fileMgr = null;
            JobApplication.uploadfile();
            callApi.ajax(url, null, JobApplication.viewdata)
        },
        viewdata: function (data) {
            var response = data.result;
            if (JobApplication._IsPreView == true) {
                JobApplication._$cardJobApplication.find('div.card-header-right').find('a.edit').remove();

                JobApplication._$cardJobApplication.find('div.card-header-right')
                    .append($('<a>')
                        .text('Cập nhật')
                        .addClass('edit')
                        .prepend($('<span>')
                            .addClass("fa fa-pencil me-1")
                        ));
                JobApplication._$cardJobApplication.find('div.card-header-right').find('a.edit').click(function () {
                    JobApplication.edit();

                    $(this).hide()

                })
                $('#JobApplicationId').attr({
                    "value": response.id
                });
                $('#Title').text(response.title);
                $('#Positions').text(response.positions.displayName);
                if (response.desiredSalary == null) {
                    $('#DesiredSalary').text(app.localize("Lương thỏa thuận"));
                } else {
                    $('#DesiredSalary').text(response.desiredSalary);
                }
                $('#LiteracyId').text(response.literacy.displayName);
                $('#ExperiencesId').text(response.experiences.displayName)
                $('#FormOfWork').text(response.formOfWork.displayName);
                $('#Occupations').text(response.occupations.displayName);
                $('#WorkSite').text(response.province.displayName);
                $("#Pushlish").toggle(response.isPublished);
                $('#Career').text(response.career);
                if (response.fileMgr != null) {
                    $('#fileView').append(
                        '<div class="list-group m-r-10"><a class="list-group-item list-group-item-action list-light-info" data-token="' + response.fileMgr.fileToken + '" data-path=' + response.fileMgr.filePath + ' href="' + response.fileMgr.fileUrl + '" id="PrimaryImage" target="_blank">' + response.fileMgr.fileName + '</a></div>'
                    );
                }
                /*$('#formFile1').text()*/
            } else if (JobApplication._Isform === true) {
                $('#Title').attr({
                    "value": response.title
                });
                $('#JobApplicationId').attr({
                    "value": response.id
                });
                $("#PositionsId option[value=" + response.positions.id + "]").attr("selected", "selected");
                $("#LiteracyId option[value=" + response.literacy.id + "]").attr("selected", "selected");
                $("#FormOfWorkId option[value=" + response.formOfWork.id + "]").attr("selected", "selected");
                $("#ExperiencesId option[value=" + response.experiences.id + "]").attr("selected", "selected");
                $("#WorkSite option[value=" + response.province.id + "]").attr("selected", "selected");
                $("#OccupationId option[value=" + response.occupationId + "]").attr("selected", "selected");
                $("#Career").text(response.career);
                $("#Pushlish").attr('checked', response.isPublished);
                $("#DesiredSalary").attr({
                    "value": response.desiredSalary
                });
                JobApplication._$JobApplicationInformationForm = $('form[name=JobAppInfoForm]');
                JobApplication._$JobApplicationInformationForm.find('button[type=reset]').click(function () {
                    if (JobApplication._$cardJobApplication.find('div.card-header-right').find('a.edit:hidden').length === 1) {
                        var data = {};
                        data.result = {};
                        data.result.id = response.id
                        JobApplication.preview(data);
                    }
                })
                if (response.fileMgr != null) {
                    $('#fileView').append(

                        '<div class="d-flex"> <div class="list-group m-r-10"><a class="list-group-item list-group-item-action list-light-info" data-token="' + response.fileMgr.fileToken + '" data-path="' + response.fileMgr.filePath + '" href="' + response.fileMgr.fileUrl + '" id="PrimaryImage" target="_blank">' + response.fileMgr.fileName + '</a> </div><div class="header-top"><a class="btn badge-light-primary f-w-500" ><i class="icofont icofont-ui-close"></i></a></div></div > '
                    );
                    $('#fileView').find('a.btn.badge-light-primary').click(function () {
                        $('#fileView').remove();
                    })
                }

                JobApplication._$JobApplicationInformationForm.find('button[type=button]').click(function () {
                    JobApplication.update();

                })
            }
        },
        create: function (event) {

            if ($('#JobApplicationId').val() != "" && $('#JobApplicationId').val() != null && $('#JobApplicationId').val() != undefined && $('#JobApplicationId').val() != null) {
                JobApplication.update();
                var data = {};
                data.result = {};
                data.result.id = $('#JobApplicationId').val();
                JobApplication.redirect(data);
            } else {
                JobApplication.validate(JobApplication._$JobApplicationInformationForm)
                if (JobApplication._$JobApplicationInformationForm.valid() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                    return false;
                }
                var data = JobApplication._$JobApplicationInformationForm.serializeFormToObject();
                data.candidateId = $("input[name=CandidateId]").val();
                data.fileMgr = {};
                data.fileMgr.fileName = data.Title;
                if (JobApplication.fileMgr != null) {
                    data.fileMgr = JobApplication.fileMgr;
                    data.fileMgr.fileName = JobApplication.fileMgr.fileName;
                    data.FileCVUrl = JobApplication.fileMgr.fileUrl;
                }
                var url = abp.appPath + "Profile/JobApplication/CreateJobApp";
                if (JobApplication._IsRedirect === true) {
                    callApi.ajax(url, data, JobApplication.redirect);
                } else {
                    callApi.ajax(url, data, JobApplication.reload);
                }
            }


        },
        update: function () {
            JobApplication.validate(JobApplication._$JobApplicationInformationForm)
            if (JobApplication._$JobApplicationInformationForm.valid() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            var data = JobApplication._$JobApplicationInformationForm.serializeFormToObject();
            data.userId = $("input[name=UserId]").val();
            data.Id = $('#JobApplicationId').val();
            data.CandidateId = $('input[name=CandidateId]').val();
            data.fileMgr = {};
            data.fileMgr.fileName = data.Title;
            data.FileCVUrl = $('#fileView').find('a.list-group-item-action').attr('data-path')

            if ($('#fileView').find('a').length == 0) {
                data.FileCVUrl = null;
            }


            if (JobApplication.fileMgr != null) {
                data.fileMgr = JobApplication.fileMgr;
                data.fileMgr.fileName = JobApplication.fileMgr.fileName;
                data.FileCVUrl = JobApplication.fileMgr.fileUrl;
            }

            var url = abp.appPath + "Profile/JobApplication/UpdateJobApplication";
            callApi.ajax(url, data, JobApplication.preview, null, abp.notify.info(abp.localization.localize("Cập nhật thành công")));
        },
        uploadfile: function () {

            $("#formFile1").filestyle({
                'onChange': async function (files) {
                    abp.ui.setBusy($('body'));

                    let form_data = new FormData();
                    jQuery.each(jQuery('#formFile1')[0].files, function (i, file) {
                        form_data.append('file', file);
                    });
                    await $.ajax({
                        'url': abp.appPath + 'Profile/JobApplication/UploadFile',
                        'type': 'POST',
                        'data': form_data,
                        'contentType': false,
                        'processData': false
                    }).done(function (results) {
                        JobApplication.fileMgr = results.result.files[0];
                        //$("#FileCVUrl").val(results.result.files[0].fileUrl);
                        //$("#FileName").val(results.result.files[0].fileName);
                        $('#fileView').remove();
                        abp.ui.clearBusy($('body'));
                    })
                        .catch(e => console.log(e));
                }
            });
            $("span.buttonText").text("Tải tệp");


        },
        validate: function (form) {
            form.validate({
                validClass: "valid",  // default
                errorClass: "invalid-feedback", // default is "error"
                highlight: function (element, errorClass, validClass) {
                    $(element).addClass('is-invalid').removeClass('is-valid');
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).addClass('is-valid').removeClass('is-invalid');
                },
                rules: {
                    "Title": {
                        required: true,
                    },
                    "PositionsId": {
                        required: true,
                        valueDefault: true,
                    },
                    "OccupationId": {
                        valueDefault: true,
                    },
                    "LiteracyId": {
                        valueDefault: true,
                    },
                    "FormOfWorkId": {
                        valueDefault: true,
                    },
                    "ExperiencesId": {
                        valueDefault: true,
                    },
                    "WorkSite": {
                        valueDefault: true,
                    },
                    "Career": {
                        required: true,
                    }

                },
                messages: {
                    "Title": {
                        required: app.localize("Tiêu đề không được để trống"),
                    },
                    "PositionsId": {
                        valueDefault: app.localize("Vui lòng chọn cấp bậc mong muốn"),
                    },
                    "OccupationId": {
                        valueDefault: app.localize("Vui lòng chọn nghề nghiệp"),
                    },
                    "LiteracyId": {
                        valueDefault: app.localize("Vui lòng chọn bằng cấp"),
                    },
                    "FormOfWorkId": {
                        valueDefault: app.localize("Vui lòng chọn hình thức làm việc"),
                    },
                    "ExperiencesId": {
                        valueDefault: app.localize("Vui lòng chọn kinh nghiệm làm việc"),
                    },
                    "WorkSite": {
                        valueDefault: app.localize("Vui lòng chọn nơi làm việc"),
                    },
                    "Career": {
                        required: app.localize("Mục tiêu nghề được để trống"),
                    }
                }
            });

        },
        init: function () {
            if ($('#JobApplicationId').val() != "" && $('#JobApplicationId').val() != null && $('#JobApplicationId').val() != undefined && $('#JobApplicationId').val() != null) {
                var url = abp.appPath + "Profile/JobApplication/GetJobApp?id=" + $('#JobApplicationId').val();
                JobApplication._IsPreView = true;
                JobApplication._Isform = false;
                callApi.ajax(url, null, JobApplication.viewdata);
                //JobApplication._areasJobApplication.find('button.edit').click(function () {
                //    JobApplication.edit();
                //});
            } else {
                JobApplication._$JobApplicationInformationForm.find('button[type=button]').click(function (event) {
                    JobApplication._IsRedirect = true;
                    JobApplication.create(event);
                })
                JobApplication._$JobApplicationInformationForm.find('button[type=reset]').click(function (event) {
                    window.location.href = abp.appPath + "Profile/Candidate/JobAppOfCandidate";
                })
            }
            JobApplication.fileMgr = null;
            JobApplication.uploadfile();


        }
    }

    JobApplication.init();



    //#endregion

    //#region  WorkExperience
    var WorkExperience = {
        _$areasworkexp: $('#workexperience'),
        _$partialview: $('div.partialview'),
        _$workexpInfoForm: $('form[name=WorkExperience]'),
        _$buttonaddform: $('a.addworkexp'),
        _IsAddform: false,
        _Isfilldata: false,
        _Id: null,
        updateJobAppIdAndCreateWorkExp: function (data) {
            $("#JobApplicationId").attr({
                "value": data.result.id
            })
            var data = WorkExperience._$workexpInfoForm.serializeFormToObject();
            data.JobApplicationId = $('#JobApplicationId').val();
            var url = abp.appPath + "Profile/JobApplication/CreateWorkExp";
            callApi.ajax(url, data, WorkExperience.getdata, null, abp.notify.info(app.localize('Thêm mới thành công')))
        },
        create: function () {
            WorkExperience._$areasworkexp.find('button[type=button]').click(function (event) {
                WorkExperience._$workexpInfoForm = WorkExperience._$areasworkexp.find('form[name=WorkExperience]');
                var IsId = WorkExperience._$workexpInfoForm.find('input[name=Id]').val();
                if (IsId == undefined || IsId == null || IsId == '') {
                    //WorkExperience._$workexpInfoForm.addClass('was-validated');
                    WorkExperience.validate(WorkExperience._$workexpInfoForm);
                    if (WorkExperience._$workexpInfoForm.valid() === false) {
                        event.preventDefault();
                        event.stopPropagation();
                        return;
                    }
                    var data = WorkExperience._$workexpInfoForm.serializeFormToObject();
                    if ($('#JobApplicationId').val() != undefined && $('#JobApplicationId').val() != null && $('#JobApplicationId').val() != "") {
                        data.JobApplicationId = $('#JobApplicationId').val();
                        var url = abp.appPath + "Profile/JobApplication/CreateWorkExp";
                        callApi.ajax(url, data, WorkExperience.getdata, null, abp.notify.info(app.localize('Thêm mới thành công')))
                    } else {
                        var respone = JobApplication.create(event)
                        if (respone === false) {
                            abp.message.warn(app.localize("Vui lòng điền đầy đủ thông tin cơ bản hồ sơ ứng tuyển", null));
                        } else {
                            dataJobApp.candidateId = $("input[name=CandidateId]").val()
                            callApi.ajax(url, dataJobApp, WorkExperience.updateJobAppIdAndCreateWorkExp);
                        }
                        /*  console.log(respone);*/
                        //var dataJobApp = $('form[name=JobAppInfoForm]').serializeFormToObject();
                        //var url = abp.appPath + "Profile/JobApplication/CreateJobApp";
                        //dataJobApp.candidateId = $("input[name=CandidateId]").val();
                        // callApi.ajax(url, dataJobApp, WorkExperience.updateJobAppIdAndCreateWorkExp);
                    }
                }

            })
        },
        update: function () {
            WorkExperience._$workexpInfoForm.find('button[type=button]').click(function (event) {
                WorkExperience.validate($(WorkExperience._$workexpInfoForm))
                if (WorkExperience._$workexpInfoForm.valid() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                    return false;
                }
                var data = WorkExperience._$workexpInfoForm.serializeFormToObject();
                data.id = $('input[name=WorkExperienceId]').val();
                data.JobApplicationId = $('#JobApplicationId').val();
                var url = abp.appPath + "Profile/JobApplication/UpdateWorkExp";
                callApi.ajax(url, data, WorkExperience.getdata, null, abp.notify.info(app.localize('Cập nhật thành công')))
            })
        },
        delete: function (data, event) {
            abp.message.confirm(
                app.localize(abp.localization.localize("Delete")),
                app.localize(abp.localization.localize("AreYouSure")),
                function (isConfirmed) {
                    if (isConfirmed) {
                        var url = abp.appPath + "Profile/JobApplication/DeleteWorkExp";
                        callApi.ajax(url, data, WorkExperience.getdata, null, abp.notify.success(app.localize('Xóa thành công')))
                    }
                }
            )
        },
        getdata: function () {

            var url = abp.appPath + "Profile/JobApplication/GetListWorkExp",
                data = {
                    id: $('#JobApplicationId').val(),
                }
            if ($('#JobApplicationId').val() != null && $('#JobApplicationId').val() != undefined && $('#JobApplicationId').val() != '') {
                callApi.ajax(url, data, WorkExperience.reload);
            } else {
                WorkExperience._$areasworkexp.hide();

                WorkExperience._$areasworkexp.children('div.card-body.card-wrapper').hide();
                WorkExperience._$buttonaddform.click(function () {
                    WorkExperience._$buttonaddform.hide();
                    WorkExperience._IsAddform = true,
                        WorkExperience._$areasworkexp.children('div.card-body.card-wrapper').show();

                    var url = abp.appPath + "Profile/JobApplication/PartialViewWorkExp";
                    callApi.ajax(url, null, WorkExperience.addform);
                })
            }
        },
        reload: function (data) {
            WorkExperience._$workexpInfoForm = WorkExperience._$areasworkexp.find('form[name=WorkExperience]')
            WorkExperience._$areasworkexp.find('div.partialview').children().remove();
            $('div.addworkexp').remove();
            if (data.result.length > 0) {
                WorkExperience._$buttonaddform.show();
                WorkExperience._$buttonaddform.click(function () {
                    WorkExperience._$areasworkexp.children('div.card-body.card-wrapper').show();
                    WorkExperience._IsAddform = true;
                    WorkExperience._$buttonaddform.hide();
                    var url = abp.appPath + "Profile/JobApplication/PartialViewWorkExp";
                    callApi.ajax(url, null, WorkExperience.addform);
                })
                $.each(data.result, function (index, value) {

                    var aTextUpdate = $('<a>').addClass('m-2').attr({
                        "href": "javascript:void(0)",
                    }).text(app.localize("Cập nhật")).prepend($('<span>')
                        .addClass("fa fa-pencil me-1")
                    ).click(function () {
                        WorkExperience._Isfilldata = true;
                        WorkExperience._Id = value.id;

                        WorkExperience._$areasworkexp.find('div.pointworkexp').show();
                        $(this).parents('div.pointworkexp').hide();
                        WorkExperience._IsAddform = false;
                        var url = abp.appPath + "Profile/JobApplication/PartialViewWorkExp?id=" + value.id;

                        callApi.ajax(url, null, WorkExperience.addform);


                    });
                    var aTextDelete = $('<a>').addClass('m-2').attr({
                        "href": "javascript:void(0)",
                    }).text(app.localize("Xóa")).prepend($('<span>')
                        .addClass("fas fa-trash me-1")
                    ).click(function () {
                        data.id = value.id
                        WorkExperience.delete(data)
                    });

                    var divPositions = $('<div>').addClass('col-md-12 col-lg-12 d-flex mt-3').attr({
                        "style": "justify-content: space-between",

                    })
                        .append($('<span>').addClass('fw-bolder').attr({
                            "id": "Positions",
                            "name": "Positions",
                        }).append($('<h5>')
                            .text(value.positions)))
                        .append($('<div>')
                            .append(aTextDelete)
                            .append(aTextUpdate));
                    var divCompanyName = $('<div>').addClass('col-md-12 col-lg-12 fw-bolder')
                        .append($('<p>').attr({
                            "id": "CompanyName",
                            "name": "CompanyName",
                        }).append($('<h5>').text(value.companyName)));
                    var divStartTime = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "StartTime",
                            "name": "StartTime",
                        }).text(moment(value.startTime).format('L') + " - " + moment(value.endTime).format('L')));
                    var divDescription = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>')
                            .addClass('m-3 description')
                            .attr({
                                "id": "Description",
                                "name": "Description",
                            }).text(value.description));
                    if (index > 0) {
                        WorkExperience._$partialview.append("<hr></hr>")
                    }
                    WorkExperience._$partialview
                        .append($('<div>').addClass('pointworkexp')
                            .append(divPositions)
                            .append(divCompanyName)
                            .append(divStartTime)
                            .append(divDescription));
                })

            } else {
                WorkExperience._$buttonaddform.show();
                WorkExperience._$areasworkexp.children('div.card-body.card-wrapper').hide();
                WorkExperience._$buttonaddform.click(function () {
                    WorkExperience._IsAddform = true,
                        WorkExperience._$areasworkexp.children('div.card-body.card-wrapper').show();
                    WorkExperience._$buttonaddform.hide();
                    var url = abp.appPath + "Profile/JobApplication/PartialViewWorkExp";
                    callApi.ajax(url, null, WorkExperience.addform);
                })
            }
        },
        filldata: function (data) {
            var response = data.result;
            WorkExperience._$workexpInfoForm = WorkExperience._$areasworkexp.find('form[name=WorkExperience]')
            WorkExperience._$workexpInfoForm.find('input[name=Id').attr({
                "value": response.id
            });

            WorkExperience._$workexpInfoForm.find('input[name=Positions]').attr({
                "value": response.positions
            });
            var now = new Date(response.startTime)
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);

            var today = now.getFullYear() + "-" + (month) + "-" + (day);


            WorkExperience._$workexpInfoForm.find('input[name=StartTime]').attr({
                "value": today
            });

            var now = new Date(response.endTime)
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);

            var today = now.getFullYear() + "-" + (month) + "-" + (day);
            WorkExperience._$workexpInfoForm.find('input[name=EndTime]').attr({
                "value": today
            });
            WorkExperience._$workexpInfoForm.find('input[name=CompanyName]').attr({
                "value": response.companyName
            });
            WorkExperience._$workexpInfoForm.find('textarea[name=Description]').text(response.description);

            if (WorkExperience._$workexpInfoForm.find('input[name=Id]').val() != ""
                && WorkExperience._$workexpInfoForm.find('input[name=Id]').val() != undefined
                && WorkExperience._$workexpInfoForm.find('input[name=Id]').val() != null
                && WorkExperience._$workexpInfoForm.find('input[name=Id]').val() != 0) {
                WorkExperience.update();
            } else {
                WorkExperience.create();
            }
        },
        addform: function (data) {

            WorkExperience._$areasworkexp.find('div.partialview').find('form').remove();
            var pointwork = WorkExperience._$areasworkexp.find('div.pointworkexp');
            var isEdit = WorkExperience._$areasworkexp.find('div.pointworkexp:hidden');

            if (pointwork.length > 0) {
                if (WorkExperience._IsAddform == true) {
                    WorkExperience._$areasworkexp.find('div.pointworkexp:last').nextAll('hr').remove();
                    WorkExperience._$areasworkexp.find('div.partialview').append('<hr></hr>');
                    WorkExperience._$areasworkexp.find('div.pointworkexp:hidden').show();
                    var newform = WorkExperience._$areasworkexp.find('div.partialview').append(data);
                } else {
                    var newform = $(data).insertAfter("div.pointworkexp:hidden");
                }
            } else {
                WorkExperience._$areasworkexp.find('div.partialview').append('<hr></hr>');
                var newform = WorkExperience._$areasworkexp.find('div.partialview').append(data);
            }

            //#region event button
            //button cancel 
            newform.find('button[type=reset]').click(function () {
                if (isEdit.length === 1) // đang chỉnh sửa 
                {
                    var url = abp.appPath + "Profile/JobApplication/GetListWorkExp",
                        data = {
                            id: $('#JobApplicationId').val(),
                        }
                    callApi.ajax(url, data, WorkExperience.reload);

                } else {
                    WorkExperience._$areasworkexp.find('div.partialview').find('hr:last').remove();
                    WorkExperience._$areasworkexp.find('div.partialview').find('form').remove();
                    WorkExperience._$buttonaddform.show();
                }
            })
            //button save

            WorkExperience.create();

            //#endregion

            //#region validate starttime and endtime

            newform.find('input#EndTime').attr({
                "max": new Date().toISOString().split("T")[0],
            }).change(function () {
                datemax = $(this).val()
                newform.find('input#StartTime').attr({
                    "max": datemax,
                })
            })
            newform.find('input#StartTime').attr({
                "max": new Date().toISOString().split("T")[0],
            }).change(function () {
                datemin = $(this).val()
                newform.find('input#EndTime').attr({
                    "min": datemin,
                })
            })

            //#endregion
            if (WorkExperience._Isfilldata === true) {
                WorkExperience._Isfilldata = false;
                var url = abp.appPath + "Profile/JobApplication/GetWorkExp";
                var data = {
                    id: WorkExperience._Id
                }
                callApi.ajax(url, data, WorkExperience.filldata);
            }
        },
        validate: function (form) {
            form.validate({
                validClass: "valid",  // default
                errorClass: "invalid-feedback", // default is "error"
                highlight: function (element, errorClass, validClass) {
                    $(element).addClass('is-invalid').removeClass('is-valid');
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).addClass('is-valid').removeClass('is-invalid');
                },
                rules: {
                    "Positions": {
                        required: true,
                    },
                    "CompanyName": {
                        required: true,
                    },
                    "Description": {
                        required: true,
                    },
                    "StartTime": {
                        required: true,
                        date: true,
                    },
                    "EndTime": {
                        required: true,
                        date: true,
                    }

                },
                messages: {
                    "Positions": {
                        required: app.localize("Vị trí công việc không được để trống"),
                    },
                    "CompanyName": {
                        required: app.localize("Tên công ty không được để trống"),
                    },
                    "Description": {
                        required: app.localize("Mô tả không được để trống"),
                    },
                    "StartTime": {
                        required: app.localize("Ngày bắt đầu không được để trống"),
                        date: app.localize("Ngày bắt đầu không không hợp lệ"),
                        maxDate: app.localize("Ngày bắt đầu không không hợp lệ"),
                    },
                    "EndTime": {
                        required: app.localize("Ngày kết thúc không được để trống"),
                        date: app.localize("Ngày kết thúc  không hợp lệ"),
                    }
                }
            });

        },
        init: function () {
            WorkExperience.getdata();
        }

    }

    //#endregion

    //#region LearningProcess
    var LearningProcess = {
        _$areaslearningprocess: $('#learningprocess'),
        _$LearningProcessfoForm: $('form[name=LearningProcess]'),
        _$partialview: $('div.partialviewlearn'),
        _$buttonaddform: $('a.addlearnprocess'),
        _IsAddform: false,
        _Isfilldata: false,
        _Id: null,
        updateJobAppIdAndCreateLearnProcess: function (data) {
            $("#JobApplicationId").attr({
                "value": data.result.id
            })
            var data = LearningProcess._$LearningProcessfoForm.serializeFormToObject();
            data.JobApplicationId = $('#JobApplicationId').val();
            var url = abp.appPath + "Profile/JobApplication/CreateLearningProcess";
            callApi.ajax(url, data, LearningProcess.getdata, null, abp.notify.info(app.localize('Thêm mới thành công')))
        },
        create: function () {
            LearningProcess._$LearningProcessfoForm = LearningProcess._$areaslearningprocess.find('form');
            LearningProcess.validate(LearningProcess._$LearningProcessfoForm);
            LearningProcess._$LearningProcessfoForm.find('button[type=button]').click(function (event) {
                if (!LearningProcess._$LearningProcessfoForm.valid()) {
                    event.preventDefault();
                    event.stopPropagation();
                    return;
                }
                if (LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != ""
                    && LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != undefined
                    && LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != null
                    && LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != 0) {
                    return;
                }
                if ($('#JobApplicationId').val() != undefined && $('#JobApplicationId').val() != null && $('#JobApplicationId').val() != "") {
                    var data = LearningProcess._$LearningProcessfoForm.serializeFormToObject();
                    data.JobApplicationId = $('#JobApplicationId').val();
                    var url = abp.appPath + "Profile/JobApplication/CreateLearningProcess";
                    callApi.ajax(url, data, LearningProcess.getdata, null, abp.notify.info(app.localize('Thêm mới thành công')))
                } else {
                    var dataJobApp = $('form[name=JobAppInfoForm]').serializeFormToObject();
                    var url = abp.appPath + "Profile/JobApplication/CreateJobApp";
                    dataJobApp.candidateId = $("input[name=CandidateId]").val();
                    callApi.ajax(url, dataJobApp, LearningProcess.updateJobAppIdAndCreateLearnProcess);
                }

            })
        },
        update: function () {
            LearningProcess._$LearningProcessfoForm = LearningProcess._$areaslearningprocess.find('form');
            LearningProcess.validate(LearningProcess._$LearningProcessfoForm);
            LearningProcess._$LearningProcessfoForm.find('button[type=button]').click(function (event) {
                if (!LearningProcess._$LearningProcessfoForm.valid()) {
                    event.preventDefault();
                    event.stopPropagation();
                    return;
                }
                var data = LearningProcess._$LearningProcessfoForm.serializeFormToObject();
                data.id = $('input[name=LearningProcessId]').val();
                data.JobApplicationId = $('#JobApplicationId').val();
                var url = abp.appPath + "Profile/JobApplication/UpdateLearningProcess";
                callApi.ajax(url, data, LearningProcess.getdata, null, abp.notify.info(app.localize('Cập nhật thành công')))
            })
        },
        delete: function (data) {
            abp.message.confirm(
                app.localize(abp.localization.localize("Delete")),
                app.localize(abp.localization.localize("AreYouSure")),
                function (isConfirmed) {
                    if (isConfirmed) {
                        var url = abp.appPath + "Profile/JobApplication/DeleteLearningProcess";
                        callApi.ajax(url, data, LearningProcess.getdata, null, abp.notify.success(app.localize('Xóa thành công')))
                    }
                }
            )
        },
        getdata: function () {

            var url = abp.appPath + "Profile/JobApplication/GetListLearningProcess",
                data = {
                    id: $('#JobApplicationId').val(),
                }
            if ($('#JobApplicationId').val() != null && $('#JobApplicationId').val() != undefined && $('#JobApplicationId').val() != '') {
                callApi.ajax(url, data, LearningProcess.reload);
            } else {
                LearningProcess._$areaslearningprocess.hide();

                LearningProcess._$areaslearningprocess.children('div.card-body.card-wrapper').hide();
                LearningProcess._$buttonaddform.click(function () {
                    LearningProcess._IsAddform = true;
                    LearningProcess._$areaslearningprocess.children('div.card-body.card-wrapper').show();
                    LearningProcess._$buttonaddform.hide();

                    var url = abp.appPath + "Profile/JobApplication/PartialViewLearnProcess";
                    callApi.ajax(url, null, LearningProcess.addform);
                    ////#region event button
                    ////button cancel 
                    //LearningProcess._$LearningProcessfoForm.find('button[type=reset]').click(function () {
                    //    LearningProcess._$areaslearningprocess.children('div.card-body.card-wrapper').hide();
                    //    LearningProcess._$buttonaddform.show();
                    //})
                    ////button save
                    ////#endregion
                    //LearningProcess.create();
                })
            }
        },
        reload: function (data) {
            LearningProcess._$partialview.children().remove();
            if (data.result.length > 0) {
                LearningProcess._$buttonaddform.show();
                LearningProcess._$buttonaddform.click(function () {
                    LearningProcess._IsAddform = true;
                    LearningProcess._$areaslearningprocess.children('div.card-body.card-wrapper').show();
                    LearningProcess._$buttonaddform.hide();
                    var url = abp.appPath + "Profile/JobApplication/PartialViewLearnProcess";
                    callApi.ajax(url, null, LearningProcess.addform);
                })
                $.each(data.result, function (index, value) {

                    var aTextUpdate = $('<a>').addClass('m-2').attr({
                        "href": "javascript:void(0)",
                    }).text(app.localize("Cập nhật")).prepend($('<span>')
                        .addClass("fa fa-pencil me-1")
                    ).click(function () {
                        LearningProcess._Isfilldata = true;
                        $(this).parents('div.pointlearnProcess').hide();
                        LearningProcess._IsAddform = false;
                        var url = abp.appPath + "Profile/JobApplication/PartialViewLearnProcess"
                        callApi.ajax(url, null, LearningProcess.addform);

                        LearningProcess._Id = value.id

                    });

                    var aTextDelete = $('<a>').addClass('m-2').attr({
                        "href": "javascript:void(0)",
                    }).text(app.localize("Xóa")).prepend($('<span>')
                        .addClass("fas fa-trash me-1")
                    ).click(function () {
                        data.id = value.id
                        LearningProcess.delete(data)
                    });

                    var divPositions = $('<div>').addClass('col-md-12 col-lg-12 d-flex mt-3').attr({
                        "style": "justify-content: space-between",

                    })
                        .append($('<span>').attr({
                            "id": "AcademicDiscipline",
                            "name": "AcademicDiscipline",
                        }).append($('<h5>')
                            .text(value.academicDiscipline)))
                        .append($('<div>')
                            .append(aTextDelete)
                            .append(aTextUpdate));
                    var divschoolName = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "SchoolName",
                            "name": "SchoolName",
                        }).append($('<h5>').text(value.schoolName)));
                    var divStartTime = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "StartTime",
                            "name": "StartTime",
                        }).text(moment(value.startTime).format('L') + " - " + moment(value.endTime).format('L')));
                    var divDescription = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>')
                            .addClass('m-3 description')
                            .attr({
                                "id": "Description",
                                "name": "Description",
                            }).html(value.description));
                    if (index > 0) {
                        LearningProcess._$partialview.append("<hr></hr>")
                    }
                    LearningProcess._$partialview
                        .append($('<div>').addClass('pointlearnProcess')
                            .append(divPositions)
                            .append(divschoolName)
                            .append(divStartTime)
                            .append(divDescription));
                })
            } else {
                LearningProcess._$buttonaddform.show();
                LearningProcess._$areaslearningprocess.children('div.card-body.card-wrapper').hide();
                LearningProcess._$buttonaddform.click(function () {
                    LearningProcess._IsAddform = true;
                    LearningProcess._$areaslearningprocess.children('div.card-body.card-wrapper').show();
                    LearningProcess._$buttonaddform.hide();
                    var url = abp.appPath + "Profile/JobApplication/PartialViewLearnProcess";
                    callApi.ajax(url, null, LearningProcess.addform);
                })

            }


        },
        filldata: function (data) {
            var response = data.result;
            LearningProcess._$LearningProcessfoForm.find('input[name=Id').attr({
                "value": response.id
            });

            LearningProcess._$LearningProcessfoForm.find('input[name=AcademicDiscipline]').attr({
                "value": response.academicDiscipline
            });
            var now = new Date(response.startTime)
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);

            var today = now.getFullYear() + "-" + (month) + "-" + (day);


            LearningProcess._$LearningProcessfoForm.find('input[name=StartTime]').attr({
                "value": today
            });

            var now = new Date(response.endTime)
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);

            var today = now.getFullYear() + "-" + (month) + "-" + (day);
            LearningProcess._$LearningProcessfoForm.find('input[name=EndTime]').attr({
                "value": today
            });
            LearningProcess._$LearningProcessfoForm.find('input[name=SchoolName]').attr({
                "value": response.schoolName
            });
            LearningProcess._$LearningProcessfoForm.find('textarea[name=Description]').text(response.description);

            if (LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != ""
                && LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != undefined
                && LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != null
                && LearningProcess._$LearningProcessfoForm.find('input[name=Id]').val() != 0) {
                LearningProcess.update();
            } else {
                LearningProcess.create();
            }
        },
        addform: function (data) {
            LearningProcess._$areaslearningprocess.find('div.partialviewlearn').find('form').remove();
            var pointwork = LearningProcess._$areaslearningprocess.find('div.pointlearnProcess');
            var isEdit = LearningProcess._$areaslearningprocess.find('div.pointlearnProcess:hidden');

            if (pointwork.length > 0) {
                if (LearningProcess._IsAddform == true) {
                    LearningProcess._$areaslearningprocess.find('div.pointlearnProcess:last').nextAll('hr').remove();
                    LearningProcess._$areaslearningprocess.find('div.partialviewlearn').append('<hr></hr>');
                    LearningProcess._$areaslearningprocess.find('div.pointlearnProcess:hidden').show();
                    var newform = LearningProcess._$areaslearningprocess.find('div.partialviewlearn').append(data);
                } else {
                    var newform = $(data).insertAfter("div.pointlearnProcess:hidden");
                }
            } else {
                LearningProcess._$areaslearningprocess.find('div.partialviewlearn').append('<hr></hr>');
                var newform = LearningProcess._$areaslearningprocess.find('div.partialviewlearn').append(data);
            }

            //#region event button
            //button cancel 
            newform.find('button[type=reset]').click(function () {
                if (isEdit.length === 1) // đang chỉnh sửa 
                {
                    var url = abp.appPath + "Profile/JobApplication/GetListLearningProcess",
                        data = {
                            id: $('#JobApplicationId').val(),
                        }
                    callApi.ajax(url, data, LearningProcess.reload);

                } else {
                    LearningProcess._$areaslearningprocess.find('div.partialviewlearn').find('hr:last').remove();
                    LearningProcess._$areaslearningprocess.find('div.partialviewlearn').find('form').remove();
                    LearningProcess._$buttonaddform.show();
                }
            })
            //button save
            LearningProcess.create();
            //#endregion

            //#region validate starttime and endtime

            newform.find('input#EndTime').attr({
                "max": new Date().toISOString().split("T")[0],
            }).change(function () {
                datemax = $(this).val()
                newform.find('input#StartTime').attr({
                    "max": datemax,
                })
            })
            newform.find('input#StartTime').attr({
                "max": new Date().toISOString().split("T")[0],
            }).change(function () {
                datemin = $(this).val()
                newform.find('input#EndTime').attr({
                    "min": datemin,
                })
            })

            //#endregion

            if (LearningProcess._Isfilldata === true) {
                LearningProcess._Isfilldata = false;
                var url = abp.appPath + "Profile/JobApplication/GetLearnProcess";
                var data = {
                    id: LearningProcess._Id
                }
                callApi.ajax(url, data, LearningProcess.filldata);
            }

        },
        validate: function (form) {
            form.validate({
                validClass: "valid",  // default
                errorClass: "invalid-feedback", // default is "error"
                highlight: function (element, errorClass, validClass) {
                    $(element).addClass('is-invalid').removeClass('is-valid');
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).addClass('is-valid').removeClass('is-invalid');
                },
                rules: {
                    "AcademicDiscipline": {
                        required: true,
                    },
                    "SchoolName": {
                        required: true,
                    },
                    "Description": {
                        required: true,
                    },
                    "StartTime": {
                        required: true,
                        date: true,
                    },
                    "EndTime": {
                        required: true,
                        date: true,
                    }

                },
                messages: {
                    "AcademicDiscipline": {
                        required: app.localize("Ngành học/Môn học không được để trống"),
                    },
                    "SchoolName": {
                        required: app.localize("Tên trường không được để trống"),
                    },
                    "Description": {
                        required: app.localize("Mô tả không được để trống"),
                    },
                    "StartTime": {
                        required: app.localize("Ngày bắt đầu không được để trống"),
                        date: app.localize("Ngày bắt đầu không không hợp lệ"),
                        maxDate: app.localize("Ngày bắt đầu không không hợp lệ"),
                    },
                    "EndTime": {
                        required: app.localize("Ngày kết thúc không được để trống"),
                        date: app.localize("Ngày kết thúc  không hợp lệ"),
                    }
                }
            });

        },
        init: function () {
            LearningProcess.getdata();
        }

    }

    //#endregion
    WorkExperience.init();
    LearningProcess.init();

})
