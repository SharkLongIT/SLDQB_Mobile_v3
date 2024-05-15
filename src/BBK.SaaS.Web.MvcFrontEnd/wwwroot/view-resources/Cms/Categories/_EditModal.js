(function () {
  app.modals.EditCmsCatModal = function () {
    var _modalManager;
    var _categoryService = abp.services.app.cmsCats;

    var _$form = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;

      _$form = _modalManager.getModal().find('form[name=CategoryForm]');
      _$form.validate({ ignore: '' });
    };

    this.save = function () {
      if (!_$form.valid()) {
        return;
      }

      _$formSEO = _modalManager.getModal().find('form[name=CategorySEOForm]');
      var item = $.extend(_$form.serializeFormToObject(), _$formSEO.serializeFormToObject());

      console.log(item);

      _modalManager.setBusy(true);
      _categoryService
        .updateCmsCat(item)
        .done(function (result) {
          abp.notify.info(app.localize('SavedSuccessfully'));
          _modalManager.close();
          _modalManager.setResult(result);
        })
        .always(function () {
          _modalManager.setBusy(false);
        });
    };
  };
})();
