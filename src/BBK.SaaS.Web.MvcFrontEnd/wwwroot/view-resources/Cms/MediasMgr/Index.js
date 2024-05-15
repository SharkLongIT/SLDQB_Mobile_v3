(function () {
  $(function () {
	var _mediasMgrService = abp.services.app.mediasMgr;
	//var _entityTypeFullName = 'Abp.Organizations.OrganizationUnit';

	var _permissions = {
	  manageOrganizationTree: abp.auth.hasPermission('Pages.Administration.OrganizationUnits.ManageOrganizationTree'),
	  manageMembers: abp.auth.hasPermission('Pages.Administration.OrganizationUnits.ManageMembers'),
	  manageRoles: abp.auth.hasPermission('Pages.Administration.OrganizationUnits.ManageRoles'),
	};

	var _createModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/MediasMgr/CreateMediaFolderModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_CreateMediaFolderModal.js',
	  modalClass: 'CreateMediaFolderModal',
	});

	var _editModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Cms/MediasMgr/EditModal',
	  scriptUrl: abp.appPath + 'view-resources/Cms/MediasMgr/_EditModal.js',
	  modalClass: 'EditMediaFolderModal',
	});

	var _addRoleModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'App/OrganizationUnits/AddRoleModal',
	  scriptUrl: abp.appPath + 'view-resources/App/OrganizationUnits/_AddRoleModal.js',
	  modalClass: 'AddRoleModal',
	  addRoleOptions: {
		title: app.localize('SelectARole'),
		serviceMethod: _mediasMgrService.findRoles,
	  },
	});

	var mediaFolderTree = {
	  $tree: $('#MediaFolderEditTree'),

	  $emptyInfo: $('#MediaFolderTreeEmptyInfo'),

	  show: function () {
		mediaFolderTree.$emptyInfo.hide();
		mediaFolderTree.$tree.show();
	  },

	  hide: function () {
		mediaFolderTree.$emptyInfo.show();
		mediaFolderTree.$tree.hide();
	  },

	  unitCount: 0,

	  setUnitCount: function (unitCount) {
		mediaFolderTree.unitCount = unitCount;
		if (unitCount) {
		  mediaFolderTree.show();
		} else {
		  mediaFolderTree.hide();
		}
	  },

	  refreshUnitCount: function () {
		mediaFolderTree.setUnitCount(mediaFolderTree.$tree.jstree('get_json').length);
	  },

	  selectedOu: {
		id: null,
		displayName: null,
		code: null,

		set: function (ouInTree) {
		  if (!ouInTree) {
			mediaFolderTree.selectedOu.id = null;
			mediaFolderTree.selectedOu.displayName = null;
			mediaFolderTree.selectedOu.code = null;
		  } else {
			mediaFolderTree.selectedOu.id = ouInTree.id;
			mediaFolderTree.selectedOu.displayName = ouInTree.original.displayName;
			mediaFolderTree.selectedOu.code = ouInTree.original.code;
		  }

		  //members.load();
		  medias.load();
		},
	  },

	  contextMenu: function (node) {
		var items = {
		  editUnit: {
			label: app.localize('Edit'),
			icon: 'la la-pencil',
			_disabled: !_permissions.manageOrganizationTree,
			action: function (data) {
			  var instance = $.jstree.reference(data.reference);

			  _editModal.open(
				{
				  id: node.id,
				},
				function (updatedOu) {
				  node.original.displayName = updatedOu.displayName;
				  instance.rename_node(node, mediaFolderTree.generateTextOnTree(updatedOu));
				}
			  );
			},
		  },

		  addSubUnit: {
			label: app.localize('AddSubUnit'),
			icon: 'la la-plus',
			_disabled: !_permissions.manageOrganizationTree,
			action: function () {
			  mediaFolderTree.addUnit(node.id);
			},
		  },

		  delete: {
			label: app.localize('Delete'),
			icon: 'la la-remove',
			_disabled: !_permissions.manageOrganizationTree,
			action: function (data) {
			  var instance = $.jstree.reference(data.reference);

			  abp.message.confirm(
				app.localize('MediaFolderDeleteWarningMessage', node.original.displayName),
				app.localize('AreYouSure'),
				function (isConfirmed) {
				  if (isConfirmed) {
					_mediasMgrService
					  .deleteMediaFolder({
						id: node.id,
					  })
					  .done(function () {
						abp.notify.success(app.localize('SuccessfullyDeleted'));
						instance.delete_node(node);
						mediaFolderTree.refreshUnitCount();
					  })
					  .fail(function (err) {
						setTimeout(function () {
						  abp.message.error(err.message);
						}, 500);
					  });
				  }
				}
			  );
			},
		  },
		};

		//if (entityHistoryIsEnabled()) {
		//  items.history = {
		//    label: app.localize('History'),
		//    icon: 'la la-history',
		//    _disabled: !_permissions.manageOrganizationTree,
		//    action: function () {
		//      _entityTypeHistoryModal.open({
		//        entityTypeFullName: _entityTypeFullName,
		//        entityId: node.original.id,
		//        entityTypeDescription: node.original.displayName,
		//      });
		//    },
		//  };
		//}

		return items;
	  },

	  addUnit: function (parentId) {
		var instance = $.jstree.reference(mediaFolderTree.$tree);

		_createModal.open(
		  {
			parentId: parentId,
		  },
		  function (newOu) {
			instance.create_node(parentId ? instance.get_node(parentId) : '#', {
			  id: newOu.id,
			  parent: newOu.parentId ? newOu.parentId : '#',
			  code: newOu.code,
			  displayName: newOu.displayName,
			  memberCount: 0,
			  roleCount: 0,
			  text: mediaFolderTree.generateTextOnTree(newOu),
			  state: {
				opened: true,
			  },
			});

			mediaFolderTree.refreshUnitCount();
		  }
		);
	  },

	  generateTextOnTree: function (ou) {
		var itemClass = ou.itemCount ? ' ou-text-has-items' : ' ou-text-no-item';
		return (
		  '<span title="' +
		  ou.code +
		  '" class="ou-text text-dark' +
		  itemClass +
		  '" data-ou-id="' +
		  ou.id +
		  '">' +
		  app.htmlUtils.htmlEncodeText(ou.displayName) +
		  ' <i class="fa fa-caret-down text-muted"></i> ' +
		  ' <span style="font-size: .82em; opacity: .5;">' +
		  '<span class="ou-text-member-count ml-2"> (' +
		  ou.itemCount +
		  ')</span></span></span>'
		);
	  },

	  incrementRoleCount: function (ouId, incrementAmount) {
		var treeNode = mediaFolderTree.$tree.jstree('get_node', ouId);
		treeNode.original.roleCount = treeNode.original.roleCount + incrementAmount;
		mediaFolderTree.$tree.jstree('rename_node', treeNode, mediaFolderTree.generateTextOnTree(treeNode.original));
	  },

	  getTreeDataFromServer: function (callback) {
		_mediasMgrService.getMediaFolders({}).done(function (result) {
		  var treeData = _.map(result.items, function (item) {
			return {
			  id: item.id,
			  parent: item.parentId ? item.parentId : '#',
			  code: item.code,
			  displayName: item.displayName,
			  memberCount: item.memberCount,
			  roleCount: item.roleCount,
			  text: mediaFolderTree.generateTextOnTree(item),
			  state: {
				opened: true,
			  },
			};
		  });

		  callback(treeData);
		});
	  },

	  init: function () {
		mediaFolderTree.getTreeDataFromServer(function (treeData) {
		  mediaFolderTree.setUnitCount(treeData.length);

		  mediaFolderTree.$tree
			.on('changed.jstree', function (e, data) {
			  if (data.selected.length != 1) {
				mediaFolderTree.selectedOu.set(null);
			  } else {
				var selectedNode = data.instance.get_node(data.selected[0]);
				mediaFolderTree.selectedOu.set(selectedNode);
			  }
			})
			.on('move_node.jstree', function (e, data) {
			  var parentNodeName =
				!data.parent || data.parent == '#'
				  ? app.localize('Root')
				  : mediaFolderTree.$tree.jstree('get_node', data.parent).original.displayName;

			  abp.message.confirm(
				app.localize('MediaFolderMoveConfirmMessage', data.node.original.displayName, parentNodeName),
				app.localize('AreYouSure'),
				function (isConfirmed) {
				  if (isConfirmed) {
					_mediasMgrService
					  .moveMediaFolder({
						id: data.node.id,
						newParentId: data.parent === '#' ? null : data.parent,
					  })
					  .done(function () {
						abp.notify.success(app.localize('SuccessfullyMoved'));
						mediaFolderTree.reload();
					  })
					  .fail(function (err) {
						mediaFolderTree.$tree.jstree('refresh'); //rollback
						setTimeout(function () {
						  abp.message.error(err.message);
						}, 500);
					  });
				  } else {
					mediaFolderTree.$tree.jstree('refresh'); //rollback
				  }
				}
			  );
			})
			.jstree({
			  core: {
				data: treeData,
				multiple: false,
				check_callback: function (operation, node, node_parent, node_position, more) {
				  return true;
				},
			  },
			  types: {
				default: {
				  icon: 'fa fa-folder text-warning',
				},
				file: {
				  icon: 'fa fa-file text-warning',
				},
			  },
			  contextmenu: {
				items: mediaFolderTree.contextMenu,
			  },
			  sort: function (node1, node2) {
				if (this.get_node(node2).original.displayName < this.get_node(node1).original.displayName) {
				  return 1;
				}

				return -1;
			  },
			  plugins: ['types', 'contextmenu', 'wholerow', 'sort', 'dnd'],
			});

		  $('#AddRootUnitButton').click(function (e) {
			e.preventDefault();
			mediaFolderTree.addUnit(null);
		  });

		  mediaFolderTree.$tree.on('click', '.ou-text .fa-caret-down', function (e) {
			e.preventDefault();

			var ouId = $(this).closest('.ou-text').attr('data-ou-id');
			setTimeout(function () {
			  mediaFolderTree.$tree.jstree('show_contextmenu', ouId);
			}, 100);
		  });
		});

		$('#MediaFolderRootBtn').click(function (e) {
		  e.preventDefault();
		  console.log('go to root');
		  //mediaFolderTree.selectedOu.set(null);
		  //mediaFolderTree.$tree.deselect_all(true);
		  mediaFolderTree.$tree.jstree("deselect_all");
		});
	  },

	  reload: function () {
		mediaFolderTree.getTreeDataFromServer(function (treeData) {
		  mediaFolderTree.setUnitCount(treeData.length);
		  mediaFolderTree.$tree.jstree(true).settings.core.data = treeData;
		  mediaFolderTree.$tree.jstree('refresh');
		});
	  },
	};

	var medias = {
	  $table: $('#FolderMediasTable'),
	  $emptyInfo: $('#OuRolesEmptyInfo'),
	  $addRoleToOuButton: $('#AddRoleToOuButton'),
	  $selectedOuRightTitle: $('#SelectedOuRightTitle'),
	  dataTable: null,

	  // showTable: function () {
	  //medias.$emptyInfo.hide();
	  //medias.$table.show();
	  //medias.$addRoleToOuButton.show();
	  //medias.$selectedOuRightTitle.text(mediaFolderTree.selectedOu.displayName).show();
	  // },

	  // hideTable: function () {
	  //medias.$selectedOuRightTitle.hide();
	  //medias.$addRoleToOuButton.hide();
	  //medias.$table.hide();
	  //medias.$emptyInfo.show();
	  // },

	  load: function () {
		//if (!mediaFolderTree.selectedOu.id) {
		//  //medias.hideTable();
		//  return;
		//}

		//medias.showTable();
		this.dataTable.ajax.reload();
	  },

	  add: function (roleList) {
		var ouId = mediaFolderTree.selectedOu.id;
		if (!ouId) {
		  return;
		}

		var roleIds = _.pluck(roleList, 'value');
		_mediasMgrService
		  .addRolesToMediaFolder({
			organizationUnitId: ouId,
			roleIds: roleIds,
		  })
		  .done(function () {
			abp.notify.success(app.localize('SuccessfullyAdded'));
			mediaFolderTree.incrementRoleCount(ouId, roleIds.length);
			medias.load();
		  });
	  },

	  delete: function (media) {
		abp.message.confirm(
		  //app.localize('RemoveRoleFromOuWarningMessage', media.filename, mediaFolderTree.selectedOu.displayName),
		  'Delete "' + media.filename + '"?',
		  app.localize('AreYouSure'),
		  function (isConfirmed) {
			if (isConfirmed) {
			  _mediasMgrService
				.deleteMedia({
				  id: media.id
				})
				.done(function () {
				  abp.notify.success(app.localize('SuccessfullyRemoved'));
				  //mediaFolderTree.incrementRoleCount(ouId, -1);
				  medias.load();
				});
			}
		  }
		);
	  },

	  openAddModal: function () {
		var ouId = mediaFolderTree.selectedOu.id;
		if (!ouId) {
		  return;
		}

		_addRoleModal.open(
		  {
			title: app.localize('SelectARole'),
			organizationUnitId: ouId,
		  },
		  function (selectedItems) {
			medias.add(selectedItems);
		  }
		);
	  },

	  init: function () {
		this.dataTable = medias.$table.find('.folder-medias-table').DataTable({
		  paging: true,
		  serverSide: true,
		  processing: true,
		  //deferLoading: 0, //prevents table for ajax request on initialize
		  //responsive: false,
		  listAction: {
			ajaxFunction: _mediasMgrService.getMediaFolderFiles,
			inputFilter: function () {
			  console.log(mediaFolderTree.selectedOu);
			  return {

				folderId: mediaFolderTree.selectedOu.id
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
				  .addClass('btn btn-danger btn-sm')
				  .attr('title', app.localize('Delete'))
				  .html(app.localize('Delete'))
				  //.append($('<i/>').addClass('fa fa-times'))
				  .click(function () {
					var record = $(this).data();
					medias.delete(record);
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
				console.log(data);
				//var eleStr = `<a href="#" data-bs-toggle="popover" data-bs-trigger="hover focus" data-bs-content="<div class='hover-content'>
				//<img src='https://s3-us-west-2.amazonaws.com/s.cdpn.io/344846/500px-P1040705_copyGemeentehuis_Zundert.jpg' />
				//<p>Zundert is a municipality and town in the south of the Netherlands, in the province of North Brabant.</p>
				//</div>" />`+data+`</a>`;

				var eleStr = `<img class="align-self-center img-fluid img-60 p-r-10" src="/file/get?c=` + row.thumbUrl + `&ver=` + row.modified + `" alt="#"><a href="/file/get?c=` + row.publicUrl + `&ver=` + row.modified + `" target="_blank" />` + data + `</a>`;
				//var eleStr = `<a href="/file/get?c=` + row.publicUrl + `&ver=` + row.modified + `" target="_blank" /><img class="align-self-center img-fluid img-60 p-r-10" src="/file/get?c=` + row.thumbUrl + `&ver=` + row.modified + `" alt="#">` + data + `</a>`;

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

		//$('#AddRoleToOuButton').click(function (e) {
		//  e.preventDefault();
		//  medias.openAddModal();
		//});

		//medias.hideTable();
	  },
	};
	
	medias.init();
	mediaFolderTree.init();

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
		  //console.log(formData, mediaFolderTree.selectedOu.id);
		  if (mediaFolderTree.selectedOu.id)
			formData.append("Model.FolderId", mediaFolderTree.selectedOu.id);
		});

		//$("#btnupload").click(function () {
		//  dz.processQueue();
		//  $(this).attr("disabled", "disabled");
		//});

		//console.log('init ok');
	  },
	  success: function (file) {
		var preview = $(file.previewElement);
		preview.addClass("dz-success text-success");
		setTimeout(function () {
		  dz.removeFile(file);

		}, 2000);

	  },
	  queuecomplete: function () {
		alert('Files Uploaded Successfully!');
		dz.removeAllFiles(true);

		// reload lists of file in current folder
		medias.load();
	  },
	  dictDefaultMessage: "You can drag and drop your images here.",
	  dictRemoveFile: "File Remove"
	});

	//console.log(dz);

  });
})();
