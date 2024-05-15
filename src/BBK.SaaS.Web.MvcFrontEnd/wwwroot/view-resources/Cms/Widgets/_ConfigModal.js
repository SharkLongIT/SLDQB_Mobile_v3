(function () {
  app.modals.ConfigWidgetModal = function () {
	var _modalManager;
	var _roleService = abp.services.app.role;
	var _$modalForm = null;
	var _permissionsTree;

	this.init = function (modalManager) {
	  _modalManager = modalManager;

	  //_permissionsTree = new PermissionsTree();
	  //_permissionsTree.init(_modalManager.getModal().find('.permission-tree'));

	  _$modalForm = _modalManager.getModal().find('form[name=ConfigWidgetForm]');
	  _$modalForm.validate({ ignore: '' });
	};

	this.save = function () {
	  if (!_$modalForm.valid()) {
		return;
	  }

	  var formData = _$modalForm.serializeFormToObject();

	  var zoneNameEles = $('input[name="ZoneNames"]');
	  var zoneSelected = [];
	  $.each(zoneNameEles, function (i, data) {
		var zoneEle = $(data); console.log(zoneEle);
		if (zoneEle[0].checked)
		  zoneSelected.push($(data).val());
	  });
	  formData.DisplayedInZones = zoneSelected;
	  console.log(formData);

	  //_modalManager.setBusy(true);
	  //_roleService
	  //  .createOrUpdateRole({
	  //    role: role,
	  //    grantedPermissionNames: _permissionsTree.getSelectedPermissionNames(),
	  //  })
	  //  .done(function () {
	  //    abp.notify.info(app.localize('SavedSuccessfully'));
	  //    _modalManager.close();
	  //    abp.event.trigger('app.createOrEditRoleModalSaved');
	  //  })
	  //  .always(function () {
	  //    _modalManager.setBusy(false);
	  //  });
	};
  };
})();
