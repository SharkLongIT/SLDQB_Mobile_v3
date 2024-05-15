$(function () {

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


    //JobApplication
    var JobApplication = {
        _areasJobApplication: $('#jobapplication'),
        _$JobApplicationInformationForm: $('form[name=JobAppInfoForm]'),
        reloaddata: function (data) {
            var response = data.result;
            JobApplication._$JobApplicationInformationForm.find('div').remove();
            JobApplication._$JobApplicationInformationForm.find('input[name=JobApplicationId]').remove();
            var divJobAppId = $('<input>').attr({
                "name": "JobApplicationId",
                "value": response.id
            }).hide();
            var divTitle = $('<div>').addClass('col-md-12 col-lg-12')
                .append($('<p>').append($('<strong>')
                    .text(app.localize("Tiêu đề"))))
                .append($('<p>').attr({
                    "id": "Title",
                    "name": "Title",
                }).text(response.title));
            var divPositions = $('<div>').addClass('col-md-12 col-lg-12')
                .append($('<p>').append($('<strong>')
                    .text(app.localize("Vị trí muốn ứng tuyển"))))
                .append($('<p>').attr({
                    "id": "Positions",
                    "name": "Positions",
                }).text(response.positions));
            var divOccupations = $('<div>').addClass('col-lg-6 col-md-12')
                .append($('<p>').append($('<strong>').text(app.localize("Nghề nghiệp"))))
                .append($('<p>').attr({
                    "id": "Occupations",
                    "name": "Occupations",
                }).text(response.occupations));
            var divDesiredSalary = $('<div>').addClass('col-lg-6 col-md-12')
                .append($('<p>').append($('<strong>').text(app.localize("Mức lương mong muốn"))))
                .append($('<p>').attr({
                    "id": "DesiredSalary",
                    "name": "DesiredSalary",
                }).text(response.desiredSalary));
            var divLiteracy = $('<div>').addClass('col-lg-6 col-md-12')
                .append($('<p>').append($('<strong>').text(app.localize("Trình độ học vấn"))))
                .append($('<p>').attr({
                    "id": "Literacy",
                    "name": "Literacy",
                }).text(response.literacy));
            var divFormOfWork = $('<div>').addClass('col-lg-6 col-md-12')
                .append($('<p>')
                    .append($('<strong>').text(app.localize("Hình thức làm việc"))))
                .append($('<p>').attr({
                    "id": "FormOfWork",
                    "name": "FormOfWork",
                }).text(response.formOfWork));
            var divExperiences = $('<div>').addClass('col-lg-6 col-md-12')
                .append($('<p>').append($('<strong>').text(app.localize("Số năm kinh nghiệm"))))
                .append($('<p>').attr({
                    "id": "Experiences",
                    "name": "Experiences",
                }).text(response.experiences));
            var divWorkSite = $('<div>').addClass('col-lg-6 col-md-12')
                .append($('<p>').append($('<strong>').text(app.localize("Nơi làm việc"))))
                .append($('<p>').attr({
                    "id": "WorkSite",
                    "name": "WorkSite",
                }).text(response.workSite));
            var divCareer = $('<div>').addClass('col-12')
                .append($('<p>').append($('<strong>').text(app.localize("Career"))))
                .append($('<p>').attr({
                    "id": "Career",
                    "name": "Career",
                }).text(response.career));
            JobApplication._$JobApplicationInformationForm
                .append(divJobAppId)
                .append(divTitle)
                .append(divPositions)
                .append(divOccupations)
                .append(divDesiredSalary)
                .append(divLiteracy)
                .append(divFormOfWork)
                .append(divExperiences)
                .append(divWorkSite)
                .append(divCareer)

        },
       

        getdata: function (data) {
            var url = abp.appPath + "Profile/JobApplication/GetJobApp",
                data = {
                    id: $('input[name=JobApplicationId]').val(),
                }
            if ($('input[name=JobApplicationId]').val() != null && $('input[name=JobApplicationId]').val() != undefined && $('input[name=JobApplicationId]').val() != '') {
                 callApi.ajax(url, data, JobApplication.reloaddata);
            } 
        },
        init: function () {
            JobApplication.getdata();

        }
    }

    // WorkExperience
    var WorkExperience = {
        _$workexpInfoForm: $('form[name=WorkExperience]'),
        _$titleworkexp: $('div.titleworkexperience'),
        getdata: function () {

            var url = abp.appPath + "Profile/JobApplication/GetListWorkExp",
                data = {
                    id: $('input[name=JobApplicationId]').val(),
                }
            if ($('input[name=JobApplicationId]').val() != null && $('input[name=JobApplicationId]').val() != undefined && $('input[name=JobApplicationId]').val() != '' ) {
                 callApi.ajax(url, data, WorkExperience.reload);
            } 
        },
        reload: function (data) {
            if (data.result.length > 0) {
                WorkExperience._$workexpInfoForm.children().remove()
                $.each(data.result, function (index, value) {

                    var divPositions = $('<div>').addClass('col-md-12 col-lg-12 d-flex').attr({
                        "style": "justify-content: space-between",

                    })
                        .append($('<p>').attr({
                            "id": "Positions",
                            "name": "Positions",
                        })
                            .text(value.positions))

                    var divCompanyName = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "CompanyName",
                            "name": "CompanyName",
                        }).text(value.companyName));
                    var divStartTime = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "StartTime",
                            "name": "StartTime",
                        }).text(moment(value.startTime).format('L') + " - " + moment(value.endTime).format('L')));
                    var divDescription = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "Description",
                            "name": "Description",
                        }).text(value.description));
                    WorkExperience._$titleworkexp
                        .append($('<div>').addClass('pointworkexp')
                            .append(divPositions)
                            .append(divCompanyName)
                            .append(divStartTime)
                            .append(divDescription));
                })
            }
            else {
                WorkExperience._$titleworkexp.parents('div.card').remove();
            }

        },
        init: function () {
            WorkExperience.getdata();
        }

    }

    // LearningProcess
    var LearningProcess = {
        _$LearningProcessfoForm: $('form[name=LearningProcess]'),
        _$titlelearningprocess: $('div.titlelearningprocess'),
        getdata: function () {

            var url = abp.appPath + "Profile/JobApplication/GetListLearningProcess",
                data = {
                    id: 30,
                }
            if ($('input[name=JobApplicationId]').val() != null || $('input[name=JobApplicationId]').val() != undefined) {
                 callApi.ajax(url, data, LearningProcess.reload);
            } 
        },
        reload: function (data) {
            if (data.result.length > 0) {
                LearningProcess._$LearningProcessfoForm.children().remove()
                $.each(data.result, function (index, value) {

                    var divPositions = $('<div>').addClass('col-md-12 col-lg-12 d-flex').attr({
                        "style": "justify-content: space-between",

                    })
                        .append($('<p>').attr({
                            "id": "Positions",
                            "name": "Positions",
                        })
                            .text(value.positions))
                        
                    var divCompanyName = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "CompanyName",
                            "name": "CompanyName",
                        }).text(value.companyName));
                    var divStartTime = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "StartTime",
                            "name": "StartTime",
                        }).text(moment(value.startTime).format('L') + " - " + moment(value.endTime).format('L')));
                    var divDescription = $('<div>').addClass('col-md-12 col-lg-12')
                        .append($('<p>').attr({
                            "id": "Description",
                            "name": "Description",
                        }).text(value.description));
                    LearningProcess._$titleworkexp
                        .append($('<div>').addClass('pointworkexp')
                            .append(divPositions)
                            .append(divCompanyName)
                            .append(divStartTime)
                            .append(divDescription));
                })
            }
            else {
                LearningProcess._$titlelearningprocess.parents('div.card').remove();
            }

        },
        init: function () {
            LearningProcess.getdata();
        }

    }

    LearningProcess.init();
    JobApplication.init();
    WorkExperience.init();
})
