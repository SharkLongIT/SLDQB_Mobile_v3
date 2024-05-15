(function () {
  app.modals.CreateCategoryUnitModal = function () {
    var _modalManager;
    var _categoryUnitService = abp.services.app.catUnit;
    var _$form = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;

      _$form = _modalManager.getModal().find('form[name=CategoryUnitForm]');
      _$form.validate({ ignore: '' });
    };

    this.save = function () {
      if (!_$form.valid()) {
        return;
      }

      var categoryUnit = _$form.serializeFormToObject();

      _modalManager.setBusy(true);
      _categoryUnitService
        .createCatUnit(categoryUnit)
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
