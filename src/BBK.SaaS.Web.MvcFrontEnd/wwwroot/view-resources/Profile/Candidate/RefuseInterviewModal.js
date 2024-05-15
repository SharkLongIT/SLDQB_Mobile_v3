(function ($) {
    app.modals.RefuseInterviewModal = function () {
        var _recruiment = abp.services.app.recruitment;
        var _modalManager;
        var _frmMakeAnAppointmentForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmMakeAnAppointmentForm = _modalManager.getModal().find('form[name=RefuseInterviewModal]');
        }

        $('#ApplicationRequestId').change(function () {
            var optionSelected = $(this).find("option:selected");
            var valueSelected = optionSelected.val();
            _recruiment.getDetail(valueSelected).done(function (e) {
                $('#Rank').val(e.rank).trigger("change");
            })

        });

        //sự kiện khi đóng modal
        $('.cancel-work-button').click(function () {
            abp.libs.sweetAlert.config = {
                confirm: {
                    icon: 'warning',
                    buttons: ['Không', 'Có']
                }
            };

            abp.message.confirm(
                app.localize(abp.localization.localize("Close")),
                app.localize(abp.localization.localize("AreYouSure")),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _modalManager.close();
                        return true;

                    }
                }
            );

        });
        this.save = function () {
            _frmMakeAnAppointmentForm.addClass('was-validated');
            if (_frmMakeAnAppointmentForm[0].checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }
            // trạng thái của người lao động // 1.Chờ pv, 2.Xác nhận pv, 3.Từ chối pv, 4.Đỗ pv 
            var data = _frmMakeAnAppointmentForm.serializeFormToObject();
            data.statusOfCandidate = 3;
            _modalManager.setBusy(true);
            var url = abp.appPath + "Profile/Candidate/UpdateMakeAnApppointment";
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                success: function (result) {
                    _modalManager.close();
                },
                error: function (result) {
                    alert(result);
                }
            }).done(function () {
                abp.event.trigger('app.reloadDocTable');
                abp.notify.info(abp.localization.localize("Từ chối phỏng vấn"));

            })
           ;
        };
    };
})(jQuery);