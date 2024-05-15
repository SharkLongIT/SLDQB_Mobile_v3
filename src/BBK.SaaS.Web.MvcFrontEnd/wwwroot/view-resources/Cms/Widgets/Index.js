(function () {
  $(function () {
	var _widgetsService = abp.services.app.widgets;
	var _$widgetsTable = $('#WidgetsTable');
	var _$widgetsTableFilter = $('#WidgetsTableFilter');
	//var _$numberOfFilteredPermission = $('#NumberOfFilteredPermission');

	//var _selectedPermissionNames = [];

	//var _$permissionFilterModal = app.modals.PermissionTreeModal.create({
	//  disableCascade: true,
	//  onSelectionDone: function (filteredPermissions) {
	//	_selectedPermissionNames = filteredPermissions;
	//	var filteredPermissionCount = filteredPermissions.length;

	//	_$numberOfFilteredPermission.text(filteredPermissionCount);
	//	abp.notify.success(app.localize('XCountPermissionFiltered', filteredPermissionCount));

	//	getWidgets();
	//  },
	//});

	var _permissions = {
	  create: abp.auth.hasPermission('Pages.Administration.CommFuncs.Create'),
	  edit: abp.auth.hasPermission('Pages.Administration.CommFuncs.Edit'),
	  changePermissions: abp.auth.hasPermission('Pages.Administration.CommFuncs.ChangePermissions'),
	  impersonation: abp.auth.hasPermission('Pages.Administration.CommFuncs.Impersonation'),
	  unlock: abp.auth.hasPermission('Pages.Administration.CommFuncs.Unlock'),
	  delete: abp.auth.hasPermission('Pages.Administration.CommFuncs.Delete'),
	};

	var _createModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/Widgets/CreateModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/Widgets/_CreateModal.js',
	  modalClass: 'CreateWidgetModal',
	});

	var _editModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/Widgets/EditModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/Widgets/_EditModal.js',
	  modalClass: 'EditWidgetModal',
	});

	var _configModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/Widgets/ConfigModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/Widgets/_ConfigModal.js',
	  modalClass: 'ConfigWidgetModal',
	});

	var getFilter = function () {
	  var filter = {
		filter: _$widgetsTableFilter.val()
	  };

	  //if (_$creationDateRangeActive.prop('checked')) {
	  //  filter.creationDateStart = _selectedCreationDateRange.startDate;
	  //  filter.creationDateEnd = _selectedCreationDateRange.endDate;
	  //}

	  return filter;
	};

	var dataTable = _$widgetsTable.DataTable({
	  paging: true,
	  serverSide: true,
	  processing: true,
	  listAction: {
		ajaxFunction: _widgetsService.getWidgets,
		inputFilter: function () {
		  return getFilter();
		},
	  },
	  columnDefs: [
		{
		  targets: 0,
		  data: null,
		  orderable: false,
		  autoWidth: false,
		  defaultContent: '',
		  rowAction: {
			text:
			  '<i class="fa fa-cog"></i> <span class="d-none d-md-inline-block d-lg-inline-block d-xl-inline-block">' +
			  app.localize('Actions') +
			  '</span> <span class="caret"></span>',
			items: [
			  {
				text: app.localize('Edit'),
				visible: function () {
				  return _permissions.edit;
				},
				action: function (data) {
				  _editModal.open({ id: data.record.id });
				  //window.open("/Cms/Widgets/Update/" + data.record.id, "_self");
				},
			  },
			  {
				text: app.localize('AddToZones'),
				//visible: function () {
				//  return _permissions.edit;
				//},
				action: function (data) {
				  _configModal.open({ id: data.record.id });
				},
			  },
			  {
				text: app.localize('Delete'),
				visible: function () {
				  return _permissions.delete;
				},
				action: function (data) {
				  deleteWidget(data.record);
				},
			  },
			],
		  },
		},
		{
		  targets: 1,
		  data: 'title',
		},
		{
		  targets: 2,
		  data: 'published',
		},
		//{
		//  targets: 3,
		//  data: 'startDate',
		//  render: function (startDate) {
		//	return moment(startDate).format('L');
		//  },
		//},
		//{
		//  targets: 4,
		//  data: 'endDate',
		//  render: function (endDate) {
		//	return moment(endDate).format('L');
		//  },
		//},
		{
		  targets: 3,
		  data: 'creationTime',
		  render: function (creationTime) {
			return moment(creationTime).format('L');
		  },
		},
	  ],
	});


	function deleteWidget(widget) {
	  abp.message.confirm(
		//app.localize('WidgetDeleteWarningMessage', widget.widgetName),
		'Bạn có chắc chắn xóa "' + widget.title + '"',
		app.localize('AreYouSure'),
		function (isConfirmed) {
		  if (isConfirmed) {
			_widgetsService
			  .deleteWidget({
				id: widget.id,
			  })
			  .done(function () {
				getWidgets(true);
				abp.notify.success(app.localize('SuccessfullyDeleted'));
			  });
		  }
		}
	  );
	}

	$('#ShowAdvancedFiltersSpan').click(function () {
	  $('#ShowAdvancedFiltersSpan').hide();
	  $('#HideAdvancedFiltersSpan').show();
	  $('#AdvacedAuditFiltersArea').slideDown();
	});

	$('#HideAdvancedFiltersSpan').click(function () {
	  $('#HideAdvancedFiltersSpan').hide();
	  $('#ShowAdvancedFiltersSpan').show();
	  $('#AdvacedAuditFiltersArea').slideUp();
	});

	$('#CreateNewButton').click(function (e) {
	  e.preventDefault();
	  _createModal.open();
	});

	//$('#ConfigWidgetButton').click(function (e) {
	//  e.preventDefault();
	//  _configModal.open();
	//});

	$('#GetWidgetsButton, #RefreshWidgetListButton').click(function (e) {
	  e.preventDefault();
	  getWidgets();
	});

	$('#WidgetsTableFilter').on('keydown', function (e) {
	  if (e.keyCode !== 13) {
		return;
	  }

	  e.preventDefault();
	  getWidgets();
	});

	function getWidgets() {
	  dataTable.ajax.reload();
	}

	abp.event.on('app.createOrEditTriggerModalSaved', function () {
      getWidgets();
    });

	_$widgetsTableFilter.focus();

  });
})();
