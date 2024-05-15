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
	  }
	};

	var widgets = {
	  $sortedlist: null,

	  add: function (selectedWidgets) {
		//console.log(selectedWidgets);

		var widgetIds = _.pluck(selectedWidgets, 'value');
		_widgetZoneService
          .addWidgetsToZone({
            zoneName: widgetZoneMgr.selectedZoneName.zoneName,
            widgetIds: widgetIds,
          })
          .done(function () {
            abp.notify.success(app.localize('SuccessfullyAdded'));
            //organizationTree.incrementRoleCount(ouId, roleIds.length);
			//roles.load();
			widgets.load();
          });
	  },

	  load: function () {
		if (!widgetZoneMgr.selectedZoneName.zoneName) {
		  //members.hideTable();
		  return;
		}

		_widgetZoneService.getWidgetZoneInfo(widgetZoneMgr.selectedZoneName.zoneName)
		  .done(function (result) {
			//console.log(result);
			if (result.widgets.length == 0) {
			  $('#widgets-pills-tabContent').hide();
			  $('#NoWidgetInZone').show();
			}
			else {
			  $('#widgets-pills-tabContent').show();
			  $('#NoWidgetInZone').hide();

			  $('#paginated-widgetlist').html('');

			  //<li class="inbox-data">
			  //	<div class="inbox-message">
			  //		<div class="email-data">
			  //			<span>New comments on MSR2023 draft presentation - <span>New Here's a list of all the topic challenges...</span></span>
			  //			<div class="badge badge-light-primary">new</div>
			  //		</div>
			  //		<div class="email-timing"><span>2:30 PM</span></div>
			  //		<div class="email-options"><i class="fa fa-envelope-o envelope-1 show"></i><i class="fa fa-envelope-open-o envelope-2 hide"></i><i class="fa fa-trash-o trash-3"></i><i class="fa fa-print"></i></div>
			  //	</div>
			  //</li>
			  $.each(result.widgets, function (index, data) {
				$('#paginated-widgetlist').append(
				  $('<li data-id="'+index+'">').addClass('inbox-data').append(
					  $("<div>").addClass('moveHandler').append($('<i class="fa fa-arrows moveWidget"></i>'))
					).append(
					$("<div>").addClass('inbox-message').append(
					  $("<div>").addClass('email-data').append(
						$("<span>").html(data.title)
					  )
					).append(
					  $("<div>").addClass('email-timing').append(
						$("<span>").html('2:30 PM')
					  )
					).append(
					  $("<div>").addClass('email-options').append(
						$("<i>").addClass('fa fa-trash-o trash-3')
					  )
					)
				  )
				);
			  });

			  var widgetlist = document.getElementById('paginated-widgetlist')
			  $sortedlist = new Sortable(widgetlist, {
				animation: 150,
				handle: '.moveHandler',
			  });

			}
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
