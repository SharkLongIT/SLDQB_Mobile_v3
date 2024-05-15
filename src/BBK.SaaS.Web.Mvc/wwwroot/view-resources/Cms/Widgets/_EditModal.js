(function () {
  app.modals.EditWidgetModal = function () {
    var _modalManager;
    var _widgetService = abp.services.app.widgets;
    var _$form = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;

      _$form = _modalManager.getModal().find('form[name=WidgetForm]');
      _$form.validate({ ignore: '' });
    };

    this.save = function () {
      if (!_$form.valid()) {
        return;
      }

      var widget = _$form.serializeFormToObject();
      //console.log(widget);
      _modalManager.setBusy(true);
      _widgetService
        .updateWidget(widget)
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
