(function () {
  $(function () {
	var _categoryUnitService = abp.services.app.catUnit;

	//var _entityTypeFullName = 'Abp.Categorys.CategoryUnit';

	//var _permissions = {
	//  manageCategoryTree: abp.auth.hasPermission('Pages.Administration.CategoryUnits.ManageCategoryTree'),
	//  manageMembers: abp.auth.hasPermission('Pages.Administration.CategoryUnits.ManageMembers'),
	//  manageRoles: abp.auth.hasPermission('Pages.Administration.CategoryUnits.ManageRoles'),
	//};

	//var _entityTypeHistoryModal = app.modals.EntityTypeHistoryModal.create();

	//function entityHistoryIsEnabled() {
	//  return (
	//    abp.custom.EntityHistory &&
	//    abp.custom.EntityHistory.IsEnabled &&
	//    _.filter(abp.custom.EntityHistory.EnabledEntities, function (entityType) {
	//      return entityType === _entityTypeFullName;
	//    }).length === 1
	//  );
	//}

	var _createModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Lib/Indexings/CreateCategoryUnitModal',
	  scriptUrl: abp.appPath + 'view-resources/Lib/Indexings/_CreateCatUnitModal.js',
	  modalClass: 'CreateCategoryUnitModal',
	});

	var _editModal = new app.ModalManager({
	  viewUrl: abp.appPath + 'Lib/Indexings/EditCategoryUnitModal',
	  scriptUrl: abp.appPath + 'view-resources/Lib/Indexings/_EditCatUnitModal.js',
	  modalClass: 'EditCategoryUnitModal',
	});

	var rootEle = {
	  $tree: $('#RootUnitTree'),

	  selectedU: {
		id: null,
		displayName: null,
		code: null,

		set: function (ouInTree) {
		  console.log(ouInTree);

		  if (!ouInTree) {
			rootEle.selectedU.id = null;
			rootEle.selectedU.displayName = null;
			rootEle.selectedU.code = null;
		  } else {
			rootEle.selectedU.id = ouInTree[0].dataset.id;
			//rootEle.selectedU.displayName = ouInTree.original.displayName;
			//rootEle.selectedU.code = ouInTree.original.code;
		  }

		  catTree.reload();
		},
	  },

	  generateTextOnTree: function (ou) {
		return `<div class="d-flex list-group-item list-group-item-action rootItem" data-id="` + ou.id + `" data-code="` + ou.code + `">
		  <div class="flex-grow-1">
			<span class="f-w-600 m-r-10 item-label" title="` + ou.code + `" >` + ou.displayName + `</span>
		  </div>
		  <ul class="action">
			<li class="m-r-10" title="AddChild"> <a href="#" class="addBtn" data-id="` + ou.id + `"><i class="fa fa-plus"></i></a></li>
			<li class="edit"> <a href="#" class="editBtn" data-id="` + ou.id + `"><i class="icon-pencil-alt"></i></a></li>
			<li class="delete"><a href="#" class="deleteBtn" title="` + ou.displayName + `" data-id="` + ou.id + `"><i class="icon-trash"></i></a></li>
		  </ul>
		</div>`;
	  },

	  getTreeDataFromServer: function (callback) {
		_categoryUnitService.getRootCatUnits({}).done(function (result) {
		  var treeData = _.map(result.items, function (item) {
			return {
			  id: item.id,
			  parent: item.parentId ? item.parentId : '#',
			  code: item.code,
			  displayName: item.displayName,
			  //memberCount: item.memberCount,
			  //roleCount: item.roleCount,
			  text: rootEle.generateTextOnTree(item),
			  state: {
				opened: true,
			  },
			};
		  });

		  callback(treeData);
		});
	  },

	  init: function () {
		rootEle.getTreeDataFromServer(function (treeData) {
		  $.each(treeData, function () {
			console.log(this);
			rootEle.$tree.append(this.text);
		  });

		  rootEle.$tree.on('click', '.rootItem', function (e) {
			e.preventDefault();
			var selectedItem = $(this);
			rootEle.$tree.find("[data-id='" + rootEle.selectedU.id + "']").toggleClass('active');
			$(this).toggleClass('active');
			rootEle.selectedU.set(selectedItem);
		  });

		  rootEle.$tree.on('click', '.editBtn', function (e) {
			e.preventDefault();
			e.stopPropagation();

			var selectedItem = $(this);

			_editModal.open({ id: $(this)[0].dataset.id, },
			  function (updatedOu) {
				rootEle.reload();
			  }
			);
		  });

		  rootEle.$tree.on('click', '.deleteBtn', function (e) {
			e.preventDefault();
			e.stopPropagation();

			var selectedItem = $(this);
			//console.log(selectedItem[0].dataset, $(selectedItem[0]).dataset);
			abp.message.confirm(
			  //app.localize('OrganizationUnitDeleteWarningMessage', $(selectedItem[0]).attr('title')),
			  app.localize('Bạn có chắc chắn muốn xóa \"' + $(selectedItem[0]).attr('title') + '\" không?', $(selectedItem[0]).attr('title')),
			  app.localize('AreYouSure'),
			  function (isConfirmed) {
				if (isConfirmed) {
				  _categoryUnitService.deleteCatUnit({
					id: selectedItem[0].dataset.id,
				  })
					.done(function () {
					  abp.notify.success(app.localize('SuccessfullyDeleted'));
					  //instance.delete_node(node);
					  //organizationTree.refreshUnitCount();
					  rootEle.reload();
					})
					.fail(function (err) {
					  setTimeout(function () {
						abp.message.error(err.message);
					  }, 500);
					});
				}
			  }
			);

		  });

		  rootEle.$tree.on('click', '.addBtn', function (e) {
			e.preventDefault();
			e.stopPropagation();

			var selectedItem = $(this);

			_createModal.open({ parentId: $(this)[0].dataset.id, },
			  function (updatedOu) {
				//rootEle.reload();
				console.log(updatedOu);
				if (updatedOu.parentId != rootEle.selectedU.id) {
				  //rootEle.$tree.find("[data-id='" + rootEle.selectedU.id + "']").toggleClass('active');
				  //$(this).toggleClass('active');

				  rootEle.$tree.find("[data-id='" + rootEle.selectedU.id + "']").toggleClass('active');
				  rootEle.$tree.find("[data-id='" + updatedOu.parentId + "']").toggleClass('active');

				  console.log('active current root element');
				}

				console.log(rootEle.$tree.find("[data-id='" + updatedOu.parentId + "']")[0]);

				rootEle.selectedU.set($(rootEle.$tree.find("[data-id='" + updatedOu.parentId + "']")[0]));

			  }
			);
		  });

		});
	  },

	  reload: function () {
		rootEle.$tree.html('');
		rootEle.getTreeDataFromServer(function (treeData) {
		  $.each(treeData, function () {
			console.log(this);
			rootEle.$tree.append(this.text);
		  });
		});
	  },
	};

	var catTree = {
	  $tree: $('#CategoryUnitEditTree'),
	  $emptyInfo: $('#CategoryUnitTreeEmptyInfo'),

	  show: function () {
		catTree.$emptyInfo.hide();
		catTree.$tree.show();
	  },

	  hide: function () {
		catTree.$emptyInfo.show();
		catTree.$tree.hide();
	  },

	  unitCount: 0,
	  notInitilized: true,

	  setUnitCount: function (unitCount) {
		catTree.unitCount = unitCount;
		if (unitCount) {
		  catTree.show();
		} else {
		  catTree.hide();
		}
	  },

	  refreshUnitCount: function () {
		catTree.setUnitCount(catTree.$tree.jstree('get_json').length);
	  },

	  selectedOu: {
		id: null,
		displayName: null,
		code: null,

		set: function (ouInTree) {
		  if (!ouInTree) {
			catTree.selectedOu.id = null;
			catTree.selectedOu.displayName = null;
			catTree.selectedOu.code = null;
		  } else {
			catTree.selectedOu.id = ouInTree.id;
			catTree.selectedOu.displayName = ouInTree.original.displayName;
			catTree.selectedOu.code = ouInTree.original.code;
		  }

		  //members.load();
		  //roles.load();
		},
	  },

	  contextMenu: function (node) {
		var items = {
		  editUnit: {
			label: app.localize('Edit'),
			icon: 'la la-pencil',
			//_disabled: !_permissions.manageCategoryTree,
			action: function (data) {
			  var instance = $.jstree.reference(data.reference);

			  _editModal.open(
				{
				  id: node.id,
				},
				function (updatedOu) {
				  node.original.displayName = updatedOu.displayName;
				  instance.rename_node(node, catTree.generateTextOnTree(updatedOu));
				}
			  );
			},
		  },

		 // addSubUnit: {
			//label: app.localize('AddSubUnit'),
			//icon: 'la la-plus',
			////_disabled: !_permissions.manageCategoryTree,
			//action: function () {
			//  catTree.addUnit(node.id);
			//},
		 // },

		  // addMember: {
		  //label: app.localize('AddMember'),
		  //icon: 'la la-user-plus',
		  ////_disabled: !_permissions.manageMembers,
		  //action: function () {
		  //  members.openAddModal();
		  //},
		  // },

		  // addRole: {
		  //label: app.localize('AddRole'),
		  //icon: 'la la-user-plus',
		  ////_disabled: !_permissions.manageRoles,
		  //action: function () {
		  //  roles.openAddModal();
		  //},
		  // },

		  delete: {
			label: app.localize('Delete'),
			icon: 'la la-remove',
			//_disabled: !_permissions.manageCategoryTree,
			action: function (data) {
			  var instance = $.jstree.reference(data.reference);

			  abp.message.confirm(
				app.localize('CategoryUnitDeleteWarningMessage', node.original.displayName),
				app.localize('AreYouSure'),
				function (isConfirmed) {
				  if (isConfirmed) {
					_categoryUnitService
					  .deleteCatUnit({
						id: node.id,
					  })
					  .done(function () {
						abp.notify.success(app.localize('SuccessfullyDeleted'));
						instance.delete_node(node);
						catTree.refreshUnitCount();
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
		//	label: app.localize('History'),
		//	icon: 'la la-history',
		//	//_disabled: !_permissions.manageCategoryTree,
		//	action: function () {
		//	  _entityTypeHistoryModal.open({
		//		entityTypeFullName: _entityTypeFullName,
		//		entityId: node.original.id,
		//		entityTypeDescription: node.original.displayName,
		//	  });
		//	},
		//  };
		//}

		return items;
	  },

	  //addUnit: function (parentId) {
	  //  var instance = $.jstree.reference(catTree.$tree);

	  //  _createModal.open(
	  //    {
	  //      parentId: parentId,
	  //    },
	  //    function (newOu) {
	  //      instance.create_node(parentId ? instance.get_node(parentId) : '#', {
	  //        id: newOu.id,
	  //        parent: newOu.parentId ? newOu.parentId : '#',
	  //        code: newOu.code,
	  //        displayName: newOu.displayName,
	  //        memberCount: 0,
	  //        roleCount: 0,
	  //        text: catTree.generateTextOnTree(newOu),
	  //        state: {
	  //          opened: true,
	  //        },
	  //      });

	  //      catTree.refreshUnitCount();
	  //    }
	  //  );
	  //},

	  generateTextOnTree: function (ou) {
		return (
		  app.htmlUtils.htmlEncodeText(ou.displayName)
		);

		//var itemClass = ou.memberCount > 0 || ou.roleCount ? ' ou-text-has-members' : ' ou-text-no-members';
		//return (
		//  '<span title="' +
		//  ou.code +
		//  '" class="ou-text text-dark' +
		//  itemClass +
		//  '" data-ou-id="' +
		//  ou.id +
		//  '">' +
		//  app.htmlUtils.htmlEncodeText(ou.displayName) +
		//  ' <i class="fa fa-caret-down text-muted"></i> ' +
		//  ' <span style="font-size: .82em; opacity: .5;">' +
		//  '<span class="ou-text-member-count ml-2">' +
		//  ou.memberCount +
		//  ' ' +
		//  app.localize('Members') +
		//  ' ,</span> <span class="ou-text-role-count ml-1">' +
		//  ou.roleCount +
		//  ' ' +
		//  app.localize('Roles') +
		//  '</span></span></span>'
		//);
	  },

	  //incrementMemberCount: function (ouId, incrementAmount) {
	  //  var treeNode = catTree.$tree.jstree('get_node', ouId);
	  //  treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
	  //  catTree.$tree.jstree('rename_node', treeNode, catTree.generateTextOnTree(treeNode.original));
	  //},

	  //incrementRoleCount: function (ouId, incrementAmount) {
	  //  var treeNode = catTree.$tree.jstree('get_node', ouId);
	  //  treeNode.original.roleCount = treeNode.original.roleCount + incrementAmount;
	  //  catTree.$tree.jstree('rename_node', treeNode, catTree.generateTextOnTree(treeNode.original));
	  //},

	  getTreeDataFromServer: function (callback) {
		//console.log('rootEle.selectedU.id', rootEle.selectedU.id);
		if (rootEle.selectedU.id != null) {
		  //_categoryUnitService.getCatUnits(rootEle.selectedU.id).done(function (result) {
		  _categoryUnitService.getChildrenCatUnit(rootEle.selectedU.id).done(function (result) {
			var treeData = _.map(result.items, function (item) {
			  return {
				id: item.id,
				//parent: item.parentId ? item.parentId : '#',
				parent: '#',
				code: item.code,
				displayName: item.displayName,
				//memberCount: item.memberCount,
				//roleCount: item.roleCount,
				text: catTree.generateTextOnTree(item),
				state: {
				  opened: true,
				},
			  };
			});

			callback(treeData);
		  });
		}
	  },

	  init: function () {

		catTree.getTreeDataFromServer(function (treeData) {
		  catTree.$tree.on('click', '.ou-text .fa-caret-down', function (e) {
			e.preventDefault();

			var ouId = $(this).closest('.ou-text').attr('data-ou-id');
			setTimeout(function () {
			  catTree.$tree.jstree('show_contextmenu', ouId);
			}, 100);
		  });
		});

		console.log('initilized tree!');
	  },

	  reload: function () {
		catTree.getTreeDataFromServer(function (treeData) {
		  console.log(treeData);

		  //catTree.setUnitCount(0);
		  catTree.setUnitCount(treeData.length);
		  //catTree.setUnitCount(treeData.length);

		  if (catTree.notInitilized) {
			catTree.$tree
			  .on('changed.jstree', function (e, data) {
				if (data.selected.length != 1) {
				  catTree.selectedOu.set(null);
				} else {
				  var selectedNode = data.instance.get_node(data.selected[0]);
				  catTree.selectedOu.set(selectedNode);
				}
			  })
			  .on('move_node.jstree', function (e, data) {
				var parentNodeName =
				  !data.parent || data.parent == '#'
					? app.localize('Root')
					: catTree.$tree.jstree('get_node', data.parent).original.displayName;

				abp.message.confirm(
				  app.localize('CategoryUnitMoveConfirmMessage', data.node.original.displayName, parentNodeName),
				  app.localize('AreYouSure'),
				  function (isConfirmed) {
					if (isConfirmed) {
					  _categoryUnitService
						.moveCatUnit({
						  id: data.node.id,
						  //newParentId: data.parent === '#' ? null : data.parent,
						  newParentId: rootEle.selectedU.id,
						})
						.done(function () {
						  abp.notify.success(app.localize('SuccessfullyMoved'));
						  catTree.reload();
						})
						.fail(function (err) {
						  catTree.$tree.jstree('refresh'); //rollback
						  setTimeout(function () {
							abp.message.error(err.message);
						  }, 500);
						});
					} else {
					  catTree.$tree.jstree('refresh'); //rollback
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
				  items: catTree.contextMenu,
				},
				sort: function (node1, node2) {
				  if (this.get_node(node2).original.code < this.get_node(node1).original.code) {
					return 1;
				  }

				  return -1;
				},
				//plugins: ['types', 'contextmenu', 'wholerow', 'sort', 'dnd'],
				plugins: ['types', 'contextmenu', '', 'sort', 'dnd'],
			  });

			catTree.notInitilized = false;
		  }
		  else {
			catTree.$tree.jstree(true).settings.core.data = treeData;
			//catTree.$tree.jstree(true).sort();
			//catTree.$tree.jstree(true).redraw_node();
			catTree.$tree.jstree('refresh');
		  }



		});
	  },
	};

	rootEle.init();
	catTree.init();

	$('#AddRootUnitButton').click(function (e) {
	  e.preventDefault();

	  _createModal.open(
		{
		  parentId: null,
		},
		function (newOu) {
		  //setResult callback
		  // instance.create_node(parentId ? instance.get_node(parentId) : '#', {
		  //id: newOu.id,
		  //parent: newOu.parentId ? newOu.parentId : '#',
		  //code: newOu.code,
		  //displayName: newOu.displayName,
		  //memberCount: 0,
		  //roleCount: 0,
		  //text: organizationTree.generateTextOnTree(newOu),
		  //state: {
		  //  opened: true,
		  //},
		  // });

		  // organizationTree.refreshUnitCount();
		  rootEle.reload();
		}
	  );
	});
  });
})();
