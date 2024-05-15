$(function () {
    var _Modal = new app.ModalManager({
        viewUrl: abp.appPath + 'Profile/JobApplication/MakeAnAppointment',
        scriptUrl: abp.appPath + 'view-resources/Profile/Candidate/MakeAnAppointment.js',
        modalClass: 'CreateModal',
        modalType: 'modal-xl'
    });

    $('.makeanappointment').click(function () {
        _Modal.open({ jobId: $('#JobAppId').val() })
    })
})
