(function ($) {
    app.modals.CreateModal = function () {
        var _Service = abp.services.app.makeAnAppointment;
        var _recruiment = abp.services.app.recruitment;
        var _modalManager;
        var _frmMakeAnAppointmentForm = null;
        flatpickr("#Time", {
            enableTime: true,
            dateFormat: "d-m-Y H:i",
            minDate: 'today',
            locale: {
                months: {
                    shorthand: ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'],
                    longhand: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                },
            },
        });
     
        this.init = function (modalManager) {
            _modalManager = modalManager;
            _frmMakeAnAppointmentForm = _modalManager.getModal().find('form[name=MakeAnAppointmentForm]');

       


            //let currentDate = new Date().toISOString().slice(0, -8); //yyyy-MM-ddThh:mm
            //document.querySelector("#Time").min = currentDate;

           
            //$('#Time').on('change', function () {
            //    var startDate = new Date().toISOString().slice(0, -8);
            //    var endDate = new Date($('#Time').val()).toISOString().slice(0, -8);
            //    if (endDate < startDate) {
            //        abp.message.warn('Giờ không được nhỏ hơn giờ hiện tại.', '');
            //        $('#Time').val('');
            //    }
            //});
        }
        if ($("#ApplicationId").val() != undefined) {
            $.ajax({
                url: '/Profile/Recruitments/GetDetail/' + $("#ApplicationId").val(),
                type: "get",
                cache: false,
                success: function (results) {
                    $('#Rank').val(results.result.rank).trigger("change");
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }

                //});
                //_recruiment.getDetail(valueSelected).done(function (e) {

            })
            //_recruiment.getDetail($("#ApplicationId").val()).done(function (e) {
            //    $('#Rank').val(e.rank).trigger("change");
            //})
        }
       
        $('#ApplicationRequestId').change(function () {
            var optionSelected = $(this).find("option:selected");
            var valueSelected = optionSelected.val();

            $.ajax({
                url: '/Profile/Recruitments/GetDetail/' + valueSelected ,
                //data: data,
                type: "get",
                cache: false,
                success: function (results) {
                    $('#Rank').val(results.result.rank).trigger("change");
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
              
            //});
            //_recruiment.getDetail(valueSelected).done(function (e) {
               
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
            var data = _frmMakeAnAppointmentForm.serializeFormToObject();
            _modalManager.setBusy(true);
            $.ajax({
                url: '/Profile/Appointments/Create',
                data: data,
                type: "post",
                cache: false,
                success: function (results) {
                    _modalManager.close();
                    abp.notify.info(abp.localization.localize("Đặt lịch thành công"));
                    window.setTimeout(function () {
                        window.location.reload();
                    }, 2000)
                    return results;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("error");
                }
                //});
                //_Service.create(data)
                //    .done(function () {
                //        _modalManager.close();
                //        abp.notify.info(abp.localization.localize("Ứng tuyển thành công"));
                //    }).always(function () {
                //        _modalManager.setBusy(false);
                //    });
            });
            //_Service.create(data)
            //    .done(function () {
            //        _modalManager.close();
            //        abp.notify.info(abp.localization.localize("Đặt lịch thành công"));
            //        window.setTimeout(function () {
            //            window.location.reload();
            //        }, 2000)
            //    }).always(function () {
            //        _modalManager.setBusy(false);
            //    });
        };
    };
})(jQuery);