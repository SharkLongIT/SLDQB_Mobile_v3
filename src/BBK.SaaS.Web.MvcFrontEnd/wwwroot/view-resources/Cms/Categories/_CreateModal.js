(function () {
  app.modals.CreateCmsCatModal = function () {
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
        .createCmsCat(item)
        .done(function (result) {
          abp.notify.info(app.localize('SavedSuccessfully'));
          _modalManager.setResult(result);
          _modalManager.close();
        })
        .always(function () {
          _modalManager.setBusy(false);
        });
    };
  };
})();
