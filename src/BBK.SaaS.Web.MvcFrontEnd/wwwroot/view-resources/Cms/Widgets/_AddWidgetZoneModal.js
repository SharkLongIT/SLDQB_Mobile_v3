(function () {
  app.modals.AddWidgetZoneModal = function () {
    var _modalManager;

    var _options = {
      serviceMethod: null, //Required
      title: app.localize('SelectAnItem'),
      loadOnStartup: true,
      showFilter: true,
      filterText: '',
      pageSize: app.consts.grid.defaultPageSize,
    };

    var _$table;
    var _$filterInput;
    var dataTable;

    function refreshTable() {
      dataTable.ajax.reload();
    }

    function updateSaveButtonState() {

      var rowData = dataTable.rows('.selected').data().toArray();
      console.log(rowData);
      var $saveButton = _modalManager.getModal().find('#btnAddWidgetsToZoneName');
      if (rowData.length > 0) {
        $saveButton.removeAttr('disabled');
      } else {
        $saveButton.attr('disabled', 'disabled');
      }
    }

    this.init = function (modalManager) {
      _modalManager = modalManager;
      _options = $.extend(_options, _modalManager.getOptions().addWidgetOptions);

      _$table = _modalManager.getModal().find('#addWidgetModalTable');

      _$filterInput = _modalManager.getModal().find('.add-widget-filter-text');
      _$filterInput.val(_options.filterText);

      dataTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        processing: true,
        deferLoading: 0,
        listAction: {
          ajaxFunction: _options.serviceMethod,
          inputFilter: function () {
            return {
              filter: _$filterInput.val(),
              organizationUnitId: _modalManager.getArgs().organizationUnitId,
            };
          },
        },
        
        columnDefs: [
          {
            targets: 0,
            data: null,
            defaultContent: '',
            "orderable": false,
            "width": "12%",
            render: function (data) {
              //return (
              //  '<label for="checkbox_' +
              //  data.value +
              //  '" class="checkbox form-check">' +
              //  '<input type="checkbox" id="checkbox_' +
              //  data.value +
              //  '" class="form-check-input" />&nbsp;' +
              //  '<span class="form-check-label"></span>' +
              //  '</label>'
              //);

              return (
                `<input class="form-check-input" id="checkbox_` + data.value + `" name="Published" type="checkbox">
			    <label class="checkbox form-check-label" for="checkbox_` + data.value + `"></label>`
              );
            },
          },
          {
            targets: 1,
            "orderable": false,
            data: 'name',
          },
          {
            targets: 2,
            visible: false,
            data: 'value',
          },
        ],
        select: {
          style: 'multi',
          info: false,
          selector: 'td:first-child input',
        },
      });

      dataTable
        .on('select', function (e, dt, type, indexes) {
          updateSaveButtonState();
        })
        .on('deselect', function (e, dt, type, indexes) {
          updateSaveButtonState();
        });

      _modalManager
        .getModal()
        .find('.add-widget-filter-button')
        .click(function (e) {
          e.preventDefault();
          refreshTable();
        });

      _modalManager
        .getModal()
        .find('.modal-body')
        .keydown(function (e) {
          if (e.which === 13) {
            e.preventDefault();
            refreshTable();
          }
        });

      if (_options.loadOnStartup) {
        refreshTable();
      }

      _modalManager
        .getModal()
        .find('#btnAddWidgetsToZoneName')
        .click(function () {
          _modalManager.setResult(dataTable.rows('.selected').data().toArray());
          _modalManager.close();
        });
    };
  };
})();


//(function () {
//  app.modals.AddWidgetZoneModal = function () {
//	var _modalManager;
//	var _widgetService = abp.services.app.widget;
//	var _$modalForm = null;
//	var _permissionsTree;

//	this.init = function (modalManager) {
//	  _modalManager = modalManager;

//	  //_permissionsTree = new PermissionsTree();
//	  //_permissionsTree.init(_modalManager.getModal().find('.permission-tree'));

//	  _$modalForm = _modalManager.getModal().find('form[name=ConfigWidgetForm]');
//	  _$modalForm.validate({ ignore: '' });
//	};

//	this.save = function () {
//	  if (!_$modalForm.valid()) {
//		return;
//	  }

//	  var formData = _$modalForm.serializeFormToObject();

//	  var zoneNameEles = $('input[name="ZoneNames"]');
//	  var zoneSelected = [];
//	  $.each(zoneNameEles, function (i, data) {
//		var zoneEle = $(data); console.log(zoneEle);
//		if (zoneEle[0].checked)
//		  zoneSelected.push($(data).val());
//	  });
//	  formData.DisplayedInZones = zoneSelected;
//	  console.log(formData);

//	  //_modalManager.setBusy(true);
//	  //_widgetService
//	  //  .createOrUpdateWidget({
//	  //    widget: widget,
//	  //    grantedPermissionNames: _permissionsTree.getSelectedPermissionNames(),
//	  //  })
//	  //  .done(function () {
//	  //    abp.notify.info(app.localize('SavedSuccessfully'));
//	  //    _modalManager.close();
//	  //    abp.event.trigger('app.createOrEditWidgetModalSaved');
//	  //  })
//	  //  .always(function () {
//	  //    _modalManager.setBusy(false);
//	  //  });
//	};
//  };
//})();
