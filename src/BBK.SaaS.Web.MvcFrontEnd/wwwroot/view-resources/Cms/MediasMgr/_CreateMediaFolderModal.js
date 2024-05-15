(function () {
  app.modals.CreateMediaFolderModal = function () {
    var _modalManager;
    var _mediasMgrService = abp.services.app.mediasMgr;

    var _$form = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;

      _$form = _modalManager.getModal().find('form[name=MediaFolderForm]');
      _$form.validate({ ignore: '' });
    };

    this.save = function () {
      if (!_$form.valid()) {
        return;
      }

      var organizationUnit = _$form.serializeFormToObject();

      _modalManager.setBusy(true);
      _mediasMgrService
        .createMediaFolder(organizationUnit)
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
