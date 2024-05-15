(function () {
  $(function () {
    var _cmsSettingsService = abp.services.app.cmsSettings;

    $('#SaveAllSettingsButton').click(function () {

      var data = {
        general: $('#GeneralSettingsForm').serializeFormToObject(),
        robotsTxt: $('#RobotsTxtForm').serializeFormToObject(),
      };
      console.log(data);

      //_cmsSettingsService
      //  .updateAllSettings(data)
      //  .done(function () {
      //    abp.notify.info(app.localize('SavedSuccessfully'));
      //  });

    });

  });
})();
