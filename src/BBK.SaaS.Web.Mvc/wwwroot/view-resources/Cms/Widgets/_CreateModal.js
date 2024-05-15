(function () {
  app.modals.CreateWidgetModal = function () {
    var _modalManager;
    var _widgetService = abp.services.app.widgets;
    var _$form = null;


    this.init = function (modalManager) {
      _modalManager = modalManager;

      _$form = _modalManager.getModal().find('form[name=WidgetForm]');
      _$form.validate({ ignore: '' });

      var _insertMediaModal = new app.ModalManager({
	    viewUrl: abp.appPath + 'Cms/MediasMgr/InsertMediaModal',
	    scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_InsertMediaModal.js',
	    modalClass: 'InsertMediaModal',
	    cssClass: 'scrollable-modal',
	    modalSize: 'modal-xl'
      });

      $('.imgFieldBtn').click(function () {
        var fieldBtn = $(this)[0];
	    _insertMediaModal.open({},
		  function (selectedItems) {
            console.log(selectedItems);
            console.log(fieldBtn);

            $('#' + fieldBtn.dataset.fieldid).val('/file/get?c=' + selectedItems.publicUrl + '&ver=' + selectedItems.modified);

		    //$("#PrimaryImage").html('<em class="txt-secondary fw-bold text-underline">' + selectedItems.filename + '</em>');
		    //$("#PrimaryImage").attr('href', '/file/get?c=' + selectedItems.publicUrl + '&ver=' + selectedItems.modified);
		    //$("#PrimaryImageUrl").val(selectedItems.publicUrl);
		  });
	  });

    };

    this.save = function () {
      if (!_$form.valid()) {
        return;
      }

      var widget = _$form.serializeFormToObject();

      //_$formFields = _modalManager.getModal().find('form[name=WidgetTemplateForm]');
      //var widgetTemps = _$formFields.serializeFormToObject();
      var widgetTemps = _modalManager.getModal().find('form[name=WidgetTemplateForm]').serializeJSON();
      var injectValues = Object.keys(widgetTemps).map((key) => ({ name: key, value: widgetTemps[key] }));

      widget.widgetTemplateName = $('#WidgetTemplateName').val();
      widget.fields = injectValues;

      console.log(widget);

      //console.log(widget);
      _modalManager.setBusy(true);
      _widgetService
        .createWidget(widget)
        .done(function (result) {
          abp.notify.info(app.localize('SavedSuccessfully'));
          //_modalManager.setResult(result);
          _modalManager.close();
          abp.event.trigger('app.createOrEditTriggerModalSaved');

        })
        .always(function () {
          _modalManager.setBusy(false);
        });
    };
  };
})();
