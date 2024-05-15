(function () {
  $(function () {
	var _widgetsService = abp.services.app.widgets;
	var _widgetZoneService = abp.services.app.widgetZone;
	var _$widgetsTable = $('#WidgetsTable');
	var _$widgetsTableFilter = $('#WidgetsTableFilter');

	var _permissions = {
	  create: abp.auth.hasPermission('Pages.Administration.CommFuncs.Create'),
	  edit: abp.auth.hasPermission('Pages.Administration.CommFuncs.Edit'),
	  changePermissions: abp.auth.hasPermission('Pages.Administration.CommFuncs.ChangePermissions'),
	  impersonation: abp.auth.hasPermission('Pages.Administration.CommFuncs.Impersonation'),
	  unlock: abp.auth.hasPermission('Pages.Administration.CommFuncs.Unlock'),
	  delete: abp.auth.hasPermission('Pages.Administration.CommFuncs.Delete'),
	};

	var _addWidgetZoneModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/Widgets/AddWidgetZoneModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/Widgets/_AddWidgetZoneModal.js',
	  modalClass: 'AddWidgetZoneModal',
	  addWidgetOptions: {
		title: app.localize('SelectAWidget'),
		serviceMethod: _widgetZoneService.findWidgets,
	  },
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

	var widgetZoneMgr = {
	  selectedZoneName: {
		zoneName: null,
		set: function (zoneName) {
		  widgetZoneMgr.selectedZoneName.zoneName = zoneName;

		  widgets.load();
		}
	  },

	  init: function () {
		$('#NoneSelectedZoneName').show();

		$('#widget-list').hide();
		$('#NoWidgetInZone').hide();

		$('.zoneNameEle').click(function (e) {
		  e.preventDefault();

		  $('#widget-list').show();
		  $('#NoneSelectedZoneName').hide();

		  widgetZoneMgr.selectedZoneName.set($(this)[0].id);

		});

		$('#widget-list').on('click', '.remove-widget', function () {
		  //console.log('click: ', this, $(this)[0].dataset.id);
		  //console.log($(this)[0].closest('li'));
		  $(this)[0].closest('li').remove();
		});

	  }
	};

	var widgets = {
	  $sortablelist: null,
	  //$widgetlist: $('#paginated-widgetlist'),
	  $widgetlist: document.getElementById('paginated-widgetlist'),
	  $widgetdata: [],
	  $isChanged: false,

	  generate: function (data, isNew) {
		//console.log(data);
		//return $('<li data-id="' + data.value + '">').addClass('inbox-data').append(
		//  $("<div>").addClass('moveHandler').append($('<i class="fa fa-arrows moveWidget"></i>'))
		//).append(
		//  $("<div>").addClass('inbox-message').append(
		//	$("<div>").addClass('email-data').append(
		//	  $("<span>").html(data.name)
		//	)
		//  ).append(
		//	$("<div>").addClass('email-timing').append(
		//	  $("<span>").html('')
		//	)
		//  ).append(
		//	$("<div>").addClass('email-options text-danger').append(
		//	  $('<i data-id="' + data.value + '">').addClass('fa fa-trash-o remove-widget')
		//	)
		//  )
		//);

		return $('<li data-id="' + (data.value ? data.value : data.widgetId) + '">').addClass('inbox-data').append(
		  $("<div>").addClass('moveHandler').append($('<i class="fa fa-arrows moveWidget"></i>'))
		).append(
		  $("<div>").addClass('inbox-message').append(
			$("<div>").addClass('email-data').append(
			  $("<span>").html(data.name ? data.name : data.title)
			)
		  ).append(`<div class="d-flex align-items-center gap-3"><span class="badge badge-light-danger">` + (isNew ? 'Chưa lưu' : '') + `</span>
                      <span class="task-action-btn"><span class="action-box large delete-btn" title="Delete Task"><i class="icon"><i class="icon-trash remove-widget"></i></i></span></span>
                    </div>`)
		);
	  },

	  add: function (selectedWidgets) {
		//console.log(selectedWidgets);

		//$widgetdata.push(selectedWidgets);
		//console.log(selectedWidgets);

		$.each(selectedWidgets, function (index, data) {
		  $('#paginated-widgetlist').append(widgets.generate(data, true));
		});

		//var widgetIds = _.pluck(selectedWidgets, 'value');
		//console.log(widgetIds);
		//_widgetZoneService
		//  .addWidgetsToZone({
		//	zoneName: widgetZoneMgr.selectedZoneName.zoneName,
		//	widgetIds: widgetIds,
		//  })
		//  .done(function () {
		//	abp.notify.success(app.localize('SuccessfullyAdded'));
		//	//widgets.load();
		//  });
	  },

	  load: function () {
		if (!widgetZoneMgr.selectedZoneName.zoneName) {
		  //members.hideTable();
		  return;
		}

		$('#paginated-widgetlist').html('');

		_widgetZoneService.getWidgetZoneInfo(widgetZoneMgr.selectedZoneName.zoneName)
		  .done(function (result) {
			//console.log(result);
			if (result.widgets.length == 0) {
			  //$('#widgets-pills-tabContent').hide();
			  //$('#NoWidgetInZone').show();
			}
			else {
			  //$('#widgets-pills-tabContent').show();
			  //$('#NoWidgetInZone').hide();

			  $('#paginated-widgetlist').html('');

			  $.each(result.widgets, function (index, data) {
				$('#paginated-widgetlist').append(
				  widgets.generate(data, false)
				);
			  });
			}
		  })
		  .always(function () {

		  });


		var widgetlist = document.getElementById('paginated-widgetlist')
		widgets.$sortablelist = new Sortable(widgets.$widgetlist, {
		  animation: 150,
		  handle: '.moveHandler',
		});
	  },

	  save: function () {

		var savedData = {
		  zoneName: widgetZoneMgr.selectedZoneName.zoneName,
		  widgetIds: []
		};

		$(this.$widgetlist).find('li').each(function () {
		  //console.log($(this)[0].dataset.id);
		  savedData.widgetIds.push($(this)[0].dataset.id);
		});

		//console.log(savedData);

		_widgetZoneService.saveWidgetZoneInfo(savedData)
		  .done(function (result) {
			abp.notify.success('Lưu thành công');
			widgets.load();
			//console.log(result);
		  })
		  .always(function () {

		  });

	  }
	};

	widgetZoneMgr.init();

	$('#AddWidgetModal').click(function (e) {
	  e.preventDefault();
	  //_addWidgetZoneModal.open({ zoneName: widgetZoneMgr.selectedZoneName.zoneName });
	  _addWidgetZoneModal.open({
		title: app.localize('SelectAWidget'),
		zoneName: widgetZoneMgr.selectedZoneName.zoneName,
	  },
		function (selectedItems) {
		  widgets.add(selectedItems);
		});
	});

	$('#TestSortable').click(function (e) {
	  e.preventDefault();

	  $('#paginated-widgetlist').append(
		$('<li data-id="' + 2 + '">').addClass('inbox-data').append(
		  $("<div>").addClass('moveHandler').append($('<i class="fa fa-arrows moveWidget"></i>'))
		).append(
		  $("<div>").addClass('inbox-message').append(
			$("<div>").addClass('email-data').append(
			  $("<span>").html('my test')
			)
		  ).append(
			$("<div>").addClass('email-timing').append(
			  $("<span>").html('2:30 PM')
			)
		  ).append(
			$("<div>").addClass('email-options').append(
			  $('<i data-id="' + 2 + '">').addClass('fa fa-trash-o remove-widget')
			)
		  )
		)
	  );

	  //console.log('test call');

	});

	$('#SaveWidgetsInZone').click(function (e) {
	  e.preventDefault();

	  widgets.save();

	  //console.log('test call');

	});

	$('#SyncZone').click(function (e) {
	  e.preventDefault();

	  if (!widgetZoneMgr.selectedZoneName.zoneName) {
		return;
	  }
	  //console.log(this, $(this)[0].dataset.value);

	  if ($(this)[0].dataset.value) {
		window.open($(this)[0].dataset.value + 'Cache/ClearZone?zoneName=' + widgetZoneMgr.selectedZoneName.zoneName, '_blank');
	  }

	});

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

	_$widgetsTableFilter.focus();

  });
})();
