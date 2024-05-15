(function () {
  $(function () {
	var _articlesService = abp.services.app.articles;
	var _$articlesTable = $('#ArticlesTable');
    var _$articlesTableFilter = $('#ArticlesTableFilter');
	var _$numberOfFilteredPermission = $('#NumberOfFilteredPermission');

	var _selectedPermissionNames = [];

	//var _$permissionFilterModal = app.modals.PermissionTreeModal.create({
	//  disableCascade: true,
	//  onSelectionDone: function (filteredPermissions) {
	//	_selectedPermissionNames = filteredPermissions;
	//	var filteredPermissionCount = filteredPermissions.length;

	//	_$numberOfFilteredPermission.text(filteredPermissionCount);
	//	abp.notify.success(app.localize('XCountPermissionFiltered', filteredPermissionCount));

	//	getArticles();
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
	
	var getFilter = function () {
      var filter = {
        filter: _$articlesTableFilter.val()
      };

      //if (_$creationDateRangeActive.prop('checked')) {
      //  filter.creationDateStart = _selectedCreationDateRange.startDate;
      //  filter.creationDateEnd = _selectedCreationDateRange.endDate;
      //}

      return filter;
    };

	var dataTable = _$articlesTable.DataTable({
	  paging: true,
	  serverSide: true,
	  processing: true,
	  listAction: {
		ajaxFunction: _articlesService.getArticles,
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
				  //_createOrEditModal.open({ id: data.record.id });
				  window.open("/Cms/Articles/Update/" + data.record.id, "_self");
				},
			  },
			  {
				text: app.localize('Delete'),
				visible: function () {
				  return _permissions.delete;
				},
				action: function (data) {
				  deleteArticle(data.record);
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
		  render: function (published) {
			if (published) return 'Có';
			else return 'Không';
		  }
		},
		{
		  targets: 3,
		  data: 'startDate',
		  render: function (startDate) {
			return moment(startDate).format('L');
		  },
		},
		{
		  targets: 4,
		  data: 'endDate',
		  render: function (endDate) {
			return moment(endDate).format('L');
		  },
		},
		{
		  targets: 5,
		  data: 'creationTime',
		  render: function (creationTime) {
			//return moment(creationTime).format('L');
			return moment(creationTime).format('YYYY-MM-DD HH:mm:ss');
		  },
		},
	  ],
	});

	function getArticles() {
	  dataTable.ajax.reload();
	}

	function deleteArticle(article) {
	  abp.message.confirm(
		//app.localize('ArticleDeleteWarningMessage', article.articleName),
		'Bạn có chắc chắn xóa "' + article.title + '"',
		app.localize('AreYouSure'),
		function (isConfirmed) {
		  if (isConfirmed) {
			_articlesService
			  .deleteArticle({
				id: article.id,
			  })
			  .done(function () {
				getArticles(true);
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
	  window.location = '/Cms/Articles/Create';
	});
	
	$('#GetArticlesButton, #RefreshArticleListButton').click(function (e) {
	  e.preventDefault();
	  getArticles();
	});

	$('#ArticlesTableFilter').on('keydown', function (e) {
	  if (e.keyCode !== 13) {
		return;
	  }

	  e.preventDefault();
	  getArticles();
	});
		
	_$articlesTableFilter.focus();

  });
})();
