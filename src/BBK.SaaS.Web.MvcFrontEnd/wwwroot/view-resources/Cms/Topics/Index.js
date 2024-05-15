(function () {
  $(function () {
	var _topicService = abp.services.app.topic;
	var _$topicsTable = $('#TopicsTable');
    var _$topicsTableFilter = $('#TopicsTableFilter');
	var _$numberOfFilteredPermission = $('#NumberOfFilteredPermission');
	//var _dynamicEntityPropertyManager = new DynamicEntityPropertyManager();

	var _selectedPermissionNames = [];

	//var _$permissionFilterModal = app.modals.PermissionTreeModal.create({
	//  disableCascade: true,
	//  onSelectionDone: function (filteredPermissions) {
	//	_selectedPermissionNames = filteredPermissions;
	//	var filteredPermissionCount = filteredPermissions.length;

	//	_$numberOfFilteredPermission.text(filteredPermissionCount);
	//	abp.notify.success(app.localize('XCountPermissionFiltered', filteredPermissionCount));

	//	getTopics();
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

	var _createOrEditModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'App/Topics/CreateOrEditModal',
	  scriptUrl: abp.appPath + 'view-resources/App/Topics/_CreateOrEditModal.js',
	  modalClass: 'CreateOrEditTopicModal',
	});

	var _topicPermissionsModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'App/Topics/PermissionsModal',
	  scriptUrl: abp.appPath + 'view-resources/App/Topics/_PermissionsModal.js',
	  modalClass: 'TopicPermissionsModal',
	});

	var getFilter = function () {
      var filter = {
        filter: _$topicsTableFilter.val()
      };

      //if (_$creationDateRangeActive.prop('checked')) {
      //  filter.creationDateStart = _selectedCreationDateRange.startDate;
      //  filter.creationDateEnd = _selectedCreationDateRange.endDate;
      //}

      return filter;
    };

	var dataTable = _$topicsTable.DataTable({
	  paging: true,
	  serverSide: true,
	  processing: true,
	  listAction: {
		ajaxFunction: _topicService.getTopics,
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
				//visible: function () {
				//  return _permissions.edit;
				//},
				action: function (data) {
				  //_createOrEditModal.open({ id: data.record.id });
				  window.open("/Cms/Topics/Update/" + data.record.id, "_self");
				},
			  },
			  {
				text: app.localize('Unlock'),
				visible: function (data) {
				  return _permissions.unlock && data.record.lockoutEndDateUtc;
				},
				action: function (data) {
				  _topicService
					.unlockTopic({
					  id: data.record.id,
					})
					.done(function () {
					  abp.notify.success(app.localize('UnlockedTheTopic', data.record.topicName));
					  dataTable.ajax.reload()
					});
				},
			  },
			  {
				text: app.localize('Delete'),
				visible: function () {
				  return _permissions.delete;
				},
				action: function (data) {
				  deleteTopic(data.record);
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
		{
		  targets: 3,
		  data: 'creationTime',
		  render: function (creationTime) {
			return moment(creationTime).format('L');
		  },
		},
	  ],
	});

	function getTopics() {
	  dataTable.ajax.reload();
	}

	function deleteTopic(topic) {
	  abp.message.confirm(
		//app.localize('TopicDeleteWarningMessage', topic.topicName),
		'Bạn có chắc chắn xóa "' + topic.title + '"',
		app.localize('AreYouSure'),
		function (isConfirmed) {
		  if (isConfirmed) {
			_topicService
			  .deleteTopic({
				id: topic.id,
			  })
			  .done(function () {
				getTopics(true);
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

	$('#CreateNewButton').click(function () {
	  window.location = '/Cms/Topics/Create';
	});

	var getSortingFromDatatable = function () {
	  if (dataTable.ajax.params().order.length > 0) {
		var columnIndex = dataTable.ajax.params().order[0].column;
		var dir = dataTable.ajax.params().order[0].dir;
		var columnName = dataTable.ajax.params().columns[columnIndex].data;

		return columnName + ' ' + dir;
	  } else {
		return '';
	  }
	};

	$('#GetTopicsButton, #RefreshTopicListButton').click(function (e) {
	  e.preventDefault();
	  getTopics();
	});

	$('#TopicsTableFilter').on('keydown', function (e) {
	  if (e.keyCode !== 13) {
		return;
	  }

	  e.preventDefault();
	  getTopics();
	});

	abp.event.on('app.createOrEditTopicModalSaved', function () {
	  getTopics();
	});

	$('#TopicsTableFilter').focus();

	$('#ImportTopicsFromExcelButton')
	  .fileupload({
		url: abp.appPath + 'Topics/ImportFromExcel',
		dataType: 'json',
		maxFileSize: 1048576 * 100,
		dropZone: $('#TopicsTable'),
		done: function (e, response) {
		  var jsonResult = response.result;
		  if (jsonResult.success) {
			abp.notify.info(app.localize('ImportTopicsProcessStart'));
		  } else {
			abp.notify.warn(app.localize('ImportTopicsUploadFailed'));
		  }
		},
	  })
	  .prop('disabled', !$.support.fileInput)
	  .parent()
	  .addClass($.support.fileInput ? undefined : 'disabled');

	//$('#FilterByPermissionsButton').click(function () {
	//  _$permissionFilterModal.open({ grantedPermissionNames: _selectedPermissionNames });
	//});

	_$topicsTableFilter.focus();

  });
})();
