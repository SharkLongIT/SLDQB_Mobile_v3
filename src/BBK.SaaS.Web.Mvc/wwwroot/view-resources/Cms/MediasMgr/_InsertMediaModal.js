(function () {
  app.modals.InsertMediaModal = function () {
	var _mediasMgrService = abp.services.app.mediasMgr;
	var _modalManager;

	var _options = {
	  serviceMethod: null, //Required
	  title: app.localize('SelectAnItem'),
	  loadOnStartup: true,
	  showFilter: true,
	  filterText: '',
	  pageSize: app.consts.grid.defaultPageSize,
	};

	var _$selectedFolderId = null;
	var _$table;
	var _$filterInput;
	var dataTable;
	var folderBreadcrumb;

	function refreshTable() {
	  dataTable.ajax.reload();
	}

	folderBreadcrumb = {
	  $breadCrumb: $('#mediafolderbreadcrumb'),
	  $folderPath: [],
	  $childFolder: [],

	  change: function (folderId) {
		_mediasMgrService.getMediaFolder(_$selectedFolderId)
		  .done(function (result) {
			console.log(result);

			folderBreadcrumb.$folderPath = result.path;
			folderBreadcrumb.$childFolder = result.childFolders;

			folderBreadcrumb.draw();
		  });

		refreshTable();
	  },

	  draw: function () {
		this.$breadCrumb.html('');

		this.$breadCrumb.append($('<li class="breadcrumb-item"><a href="#" class="mediaFolder-selecting" data-selid=""><i class="fa fa-home"></i></a></li>'));

		if (folderBreadcrumb.$folderPath.length > 0) {
		  $.each(folderBreadcrumb.$folderPath, function (key, data) {
			folderBreadcrumb.$breadCrumb.append($('<li class="breadcrumb-item active"><a href="#" class="mediaFolder-selecting" data-selid=' + data.value + '>' + data.name + '</a></li>'));
		  });
		}

		this.$breadCrumb.append($(`<li class="breadcrumb-item">
			<div class="btn-group" role="group">
				<button class="btn btn-square btn-outline-secondary btn-xs btn-line-0 dropdown-toggle" id="btnGroupVerticalDrop1" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Thư mục con</button>
				<div id="childFolderMenu" class="dropdown-menu" aria-labelledby="btnGroupVerticalDrop1">
				</div>
			</div>
		</li>`));

		//console.log(folderBreadcrumb.$childFolder);
		if (folderBreadcrumb.$childFolder.length > 0) {
		  $.each(folderBreadcrumb.$childFolder, function (key, data) {
			folderBreadcrumb.$breadCrumb.find('#childFolderMenu').append('<a class="dropdown-item mediaFolder-selecting" data-selid=' + data.value + ' href="#">' + data.name + '</a>');
		  });

		}

	  },

	  init: function () {
		this.change();

		folderBreadcrumb.$breadCrumb.on('click', '.mediaFolder-selecting', function (e) {
		  e.preventDefault();
		  console.log(this.dataset.selid, $(this));
		  _$selectedFolderId = this.dataset.selid;
		  folderBreadcrumb.change(this.dataset.selid);

		});
	  }

	};

	this.init = function (modalManager) {
	  _modalManager = modalManager;
	  //_options = $.extend(_options, _modalManager.getOptions().addRoleOptions);

	  var dz = null;
	  $("#multiFileUpload").dropzone({
		autoProcessQueue: true,
		paramName: "Uploads",
		maxFilesize: 5, //mb
		maxThumbnailFilesize: 1, //mb
		maxFiles: 5,
		//parallelUploads: 5,
		acceptedFiles: ".jpeg,.png,.jpg",
		//uploadMultiple: true,
		//addRemoveLinks: true,
		url: '/CMS/MediasMgr/UploadFiles',

		init: function () {
		  dz = this;

		  this.on("sending", function (file, xhr, formData) {
			if (_$selectedFolderId)
			  formData.append("Model.FolderId", _$selectedFolderId);
		  });
		},
		success: function (file) {
		  var preview = $(file.previewElement);
		  preview.addClass("dz-success text-success");
		  setTimeout(function () {
			dz.removeFile(file);

		  }, 2000);

		},
		queuecomplete: function () {
		  //alert('Files Uploaded Successfully!');
		  dz.removeAllFiles(true);

		  // reload lists of file in current folder
		  refreshTable();
		},
		dictDefaultMessage: "You can drag and drop your images here.",
		dictRemoveFile: "File Remove"
	  });

	  _$table = _modalManager.getModal().find('.folder-medias-table');

	  _$filterInput = _modalManager.getModal().find('.insert-media-filter-text');
	  //_$filterInput.val(_options.filterText);

	  dataTable = _$table.DataTable({
		paging: true,
		serverSide: true,
		processing: true,
		deferLoading: 0, //prevents table for ajax request on initialize
		//responsive: false,
		listAction: {
		  ajaxFunction: _mediasMgrService.getMediaFolderFiles,
		  inputFilter: function () {
			return {
			  filter: _$filterInput.val(),
			  folderId: _$selectedFolderId
			};
		  },
		},
		columnDefs: [
		  {
			targets: 0,
			data: null,
			orderable: false,
			defaultContent: '',
			className: 'text-center',
			rowAction: {
			  targets: 0,
			  data: null,
			  orderable: false,
			  defaultContent: '',
			  element: $('<button type="button"/>')
				.addClass('btn btn-info btn-sm selected-img')
				.attr('title', app.localize('Select'))
				//.attr('value', $(this).data())
				.html(app.localize('Select'))
				//.append($('<i/>').addClass('fa fa-times'))
				.click(function () {
				  console.log($(this).data());
				  var data = $(this).data();
				  //
				  //_modalManager.setResult('/file/get?c=' + data.publicUrl + '&ver=' + data.modified);
				  _modalManager.setResult(data);
				  _modalManager.close();
				}),
			  visible: function () {
				return true;
			  },
			},
		  },
		  {
			targets: 1,
			data: 'filename',
			render: function (data, type, row) {
			  //var eleStr = `<img class="align-self-center img-fluid img-60 p-r-10" src="/file/get?c=` + row.publicUrl + `&ver=` + row.modified + `" alt="#"><a href="/file/get?c=` + row.publicUrl + `&ver=` + row.modified + `" target="_blank" />` + data + `</a>`;
			  var eleStr = `<a href="/file/get?c=` + row.publicUrl + `&ver=` + row.modified + `" target="_blank" title='Click để hiển thị đầy đủ'/><img class="align-self-center img-fluid img-60 p-r-10" src="/file/get?c=` + row.thumbUrl + `&ver=` + row.modified + `" alt="Click để hiển thị đầy đủ">` + data + `</a>`;

			  return eleStr;
			}
		  },
		  {
			targets: 2,
			data: 'contentType',
		  },
		  {
			targets: 3,
			data: 'size',
		  },
		  {
			targets: 4,
			data: 'modified',
			render: function (addedTime) {
			  //return moment(addedTime).format('L LLL');
			  return moment(addedTime).format('YYYY-MM-DD HH:mm:ss');
			},
		  },
		],
	  });

	  _modalManager
		.getModal()
		.find('.insert-media-filter-button')
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

	  //_modalManager
	  //  .getModal()
	  //  .find('#btnAddRolesToOrganization')
	  //  .click(function () {
	  //    _modalManager.setResult(dataTable.rows({ selected: true }).data().toArray());
	  //    _modalManager.close();
	  //  });

	  //_modalManager.getModal().on('click', '.selected-img', function (e) {
	  //  _modalManager.setResult($(this).attr('src'));
	  //  _modalManager.close();
	  //});

	  folderBreadcrumb.init();
	};


  };
})();
