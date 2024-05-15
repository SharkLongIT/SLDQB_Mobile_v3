(function () {
  $(function () {
	var _geoUnitService = abp.services.app.geoUnit;

	$('#TextBaseLanguageSelectionCombobox').select2({
	  //theme: 'bootstrap5',
	  selectionCssClass: 'form-select',
	  language: abp.localization.currentCulture.name,
	  //width: '100%'
	});

	var cityProvinceSelect = {
	  $list: $('#ProvinceCity'),

	  //$emptyInfo: $('#GeoUnitTreeEmptyInfo'),

	  //show: function () {
	  //  cityProvinceSelect.$emptyInfo.hide();
	  //  cityProvinceSelect.$list.show();
	  //},

	  //hide: function () {
	  //  cityProvinceSelect.$emptyInfo.show();
	  //  cityProvinceSelect.$list.hide();
	  //},

	  //unitCount: 0,

	  //setUnitCount: function (unitCount) {
	  //  cityProvinceSelect.unitCount = unitCount;
	  //  if (unitCount) {
	  //    cityProvinceSelect.show();
	  //  } else {
	  //    cityProvinceSelect.hide();
	  //  }
	  //},

	  //refreshUnitCount: function () {
	  //  cityProvinceSelect.setUnitCount(cityProvinceSelect.$list.jstree('get_json').length);
	  //},

	  selectedOu: {
		id: null,
		displayName: null,
		code: null,

		set: function (ouInTree) {
		  //console.log(ouInTree);
		  if (!ouInTree) {
			cityProvinceSelect.selectedOu.id = null;
			cityProvinceSelect.selectedOu.displayName = null;
			cityProvinceSelect.selectedOu.code = null;
		  } else {
			cityProvinceSelect.selectedOu.id = ouInTree[0].dataset.geounitid;
			//cityProvinceSelect.selectedOu.displayName = ouInTree.original.displayName;
			cityProvinceSelect.selectedOu.code = ouInTree[0].dataset.code;
		  }

		  //console.log(cityProvinceSelect.selectedOu);
		  //members.load();
		  districtSelect.reload();
		},
	  },

	  //addUnit: function (parentId) {
	  //  var instance = $.jstree.reference(cityProvinceSelect.$list);

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
	  //        text: cityProvinceSelect.generateTextOnTree(newOu),
	  //        state: {
	  //          opened: true,
	  //        },
	  //      });

	  //      cityProvinceSelect.refreshUnitCount();
	  //    }
	  //  );
	  //},

	  generateTextOnTree: function (ou) {
		return (
		  '<li class="list-group-item cityProvinceItem" data-code="' + ou.code + '" data-geounitid="' + ou.id + '">' + ou.code + '<i class="icofont icofont-arrow-right"> </i>' + ou.displayName + '</li>'
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
	  //  var treeNode = cityProvinceSelect.$list.jstree('get_node', ouId);
	  //  treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
	  //  cityProvinceSelect.$list.jstree('rename_node', treeNode, cityProvinceSelect.generateTextOnTree(treeNode.original));
	  //},

	  //incrementRoleCount: function (ouId, incrementAmount) {
	  //  var treeNode = cityProvinceSelect.$list.jstree('get_node', ouId);
	  //  treeNode.original.roleCount = treeNode.original.roleCount + incrementAmount;
	  //  cityProvinceSelect.$list.jstree('rename_node', treeNode, cityProvinceSelect.generateTextOnTree(treeNode.original));
	  //},

	  getTreeDataFromServer: function (callback) {
		_geoUnitService.getGeoUnits({}).done(function (result) {

		  //console.log(result);

		  //var treeData = _.map(result.items, function (item) {
		  //  return {
		  //    id: item.id,
		  //    parent: item.parentId ? item.parentId : '#',
		  //    code: item.code,
		  //    displayName: item.displayName,
		  //    //memberCount: item.memberCount,
		  //    //roleCount: item.roleCount,
		  //    text: cityProvinceSelect.generateTextOnTree(item),
		  //    state: {
		  //      opened: true,
		  //    },
		  //  };
		  //});

		  //callback(treeData);
		  callback(result.items);


		});
	  },

	  init: function () {
		cityProvinceSelect.getTreeDataFromServer(function (treeData) {
		  //cityProvinceSelect.setUnitCount(treeData.length);

		  console.log(treeData);
		  $.each(treeData, function () {
			//console.log(i);
			cityProvinceSelect.$list.append(cityProvinceSelect.generateTextOnTree(this));
		  });

		  $('#AddRootUnitButton').click(function (e) {
			e.preventDefault();
			cityProvinceSelect.addUnit(null);
		  });

		  //cityProvinceSelect.$list.on('click', '.ou-text .fa-caret-down', function (e) {
		  //  e.preventDefault();

		  //  var ouId = $(this).closest('.ou-text').attr('data-ou-id');
		  //  setTimeout(function () {
		  //    cityProvinceSelect.$list.jstree('show_contextmenu', ouId);
		  //  }, 100);
		  //});
		});

		cityProvinceSelect.$list.on('click', '.cityProvinceItem', function () {
		  var selectedItem = $(this); console.log(selectedItem[0].dataset);
		  cityProvinceSelect.$list.find("[data-geounitid='" + cityProvinceSelect.selectedOu.id + "']").toggleClass('active');
		  $(this).toggleClass('active');
		  cityProvinceSelect.selectedOu.set(selectedItem);
		});
	  },

	  reload: function () {
		cityProvinceSelect.getTreeDataFromServer(function (treeData) {
		  //cityProvinceSelect.setUnitCount(treeData.length);
		  //cityProvinceSelect.$list.jstree(true).settings.core.data = treeData;
		  //cityProvinceSelect.$list.jstree('refresh');
		});
	  },
	};

	var districtSelect = {
	  $list: $('#District'),

	  //$emptyInfo: $('#GeoUnitTreeEmptyInfo'),

	  //show: function () {
	  //  districtSelect.$emptyInfo.hide();
	  //  districtSelect.$list.show();
	  //},

	  //hide: function () {
	  //  districtSelect.$emptyInfo.show();
	  //  districtSelect.$list.hide();
	  //},

	  //unitCount: 0,

	  //setUnitCount: function (unitCount) {
	  //  districtSelect.unitCount = unitCount;
	  //  if (unitCount) {
	  //    districtSelect.show();
	  //  } else {
	  //    districtSelect.hide();
	  //  }
	  //},

	  //refreshUnitCount: function () {
	  //  districtSelect.setUnitCount(districtSelect.$list.jstree('get_json').length);
	  //},

	  selectedOu: {
		id: null,
		displayName: null,
		code: null,

		set: function (ouInTree) {
		  if (!ouInTree) {
			districtSelect.selectedOu.id = null;
			districtSelect.selectedOu.displayName = null;
			districtSelect.selectedOu.code = null;
		  } else {
			//districtSelect.selectedOu.id = ouInTree.id;
			//districtSelect.selectedOu.displayName = ouInTree.original.displayName;
			//districtSelect.selectedOu.code = ouInTree.original.code;
			districtSelect.selectedOu.id = ouInTree[0].dataset.geounitid;
			districtSelect.selectedOu.code = ouInTree[0].dataset.code;
		  }

		  wardCommuneSelect.reload();
		},
	  },

	  //contextMenu: function (node) {
	  //  var items = {
	  //    editUnit: {
	  //      label: app.localize('Edit'),
	  //      icon: 'la la-pencil',
	  //      _disabled: !_permissions.manageOrganizationTree,
	  //      action: function (data) {
	  //        var instance = $.jstree.reference(data.reference);

	  //        _editModal.open(
	  //          {
	  //            id: node.id,
	  //          },
	  //          function (updatedOu) {
	  //            node.original.displayName = updatedOu.displayName;
	  //            instance.rename_node(node, districtSelect.generateTextOnTree(updatedOu));
	  //          }
	  //        );
	  //      },
	  //    },

	  //    addSubUnit: {
	  //      label: app.localize('AddSubUnit'),
	  //      icon: 'la la-plus',
	  //      _disabled: !_permissions.manageOrganizationTree,
	  //      action: function () {
	  //        districtSelect.addUnit(node.id);
	  //      },
	  //    },

	  //    addMember: {
	  //      label: app.localize('AddMember'),
	  //      icon: 'la la-user-plus',
	  //      _disabled: !_permissions.manageMembers,
	  //      action: function () {
	  //        members.openAddModal();
	  //      },
	  //    },

	  //    addRole: {
	  //      label: app.localize('AddRole'),
	  //      icon: 'la la-user-plus',
	  //      _disabled: !_permissions.manageRoles,
	  //      action: function () {
	  //        roles.openAddModal();
	  //      },
	  //    },

	  //    delete: {
	  //      label: app.localize('Delete'),
	  //      icon: 'la la-remove',
	  //      _disabled: !_permissions.manageOrganizationTree,
	  //      action: function (data) {
	  //        var instance = $.jstree.reference(data.reference);

	  //        abp.message.confirm(
	  //          app.localize('GeoUnitDeleteWarningMessage', node.original.displayName),
	  //          app.localize('AreYouSure'),
	  //          function (isConfirmed) {
	  //            if (isConfirmed) {
	  //              _geoUnitService
	  //                .deleteGeoUnit({
	  //                  id: node.id,
	  //                })
	  //                .done(function () {
	  //                  abp.notify.success(app.localize('SuccessfullyDeleted'));
	  //                  instance.delete_node(node);
	  //                  districtSelect.refreshUnitCount();
	  //                })
	  //                .fail(function (err) {
	  //                  setTimeout(function () {
	  //                    abp.message.error(err.message);
	  //                  }, 500);
	  //                });
	  //            }
	  //          }
	  //        );
	  //      },
	  //    },
	  //  };

	  //  if (entityHistoryIsEnabled()) {
	  //    items.history = {
	  //      label: app.localize('History'),
	  //      icon: 'la la-history',
	  //      _disabled: !_permissions.manageOrganizationTree,
	  //      action: function () {
	  //        _entityTypeHistoryModal.open({
	  //          entityTypeFullName: _entityTypeFullName,
	  //          entityId: node.original.id,
	  //          entityTypeDescription: node.original.displayName,
	  //        });
	  //      },
	  //    };
	  //  }

	  //  return items;
	  //},

	  //addUnit: function (parentId) {
	  //  var instance = $.jstree.reference(districtSelect.$list);

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
	  //        text: districtSelect.generateTextOnTree(newOu),
	  //        state: {
	  //          opened: true,
	  //        },
	  //      });

	  //      districtSelect.refreshUnitCount();
	  //    }
	  //  );
	  //},

	  generateTextOnTree: function (ou) {
		return (
		  '<li class="list-group-item districtItem" data-code="' + ou.code + '" data-geounitid="' + ou.id + '">' + ou.code + '<i class="icofont icofont-arrow-right"> </i>' + ou.displayName + '</li>'
		);
	  },

	  //incrementMemberCount: function (ouId, incrementAmount) {
	  //  var treeNode = districtSelect.$list.jstree('get_node', ouId);
	  //  treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
	  //  districtSelect.$list.jstree('rename_node', treeNode, districtSelect.generateTextOnTree(treeNode.original));
	  //},

	  //incrementRoleCount: function (ouId, incrementAmount) {
	  //  var treeNode = districtSelect.$list.jstree('get_node', ouId);
	  //  treeNode.original.roleCount = treeNode.original.roleCount + incrementAmount;
	  //  districtSelect.$list.jstree('rename_node', treeNode, districtSelect.generateTextOnTree(treeNode.original));
	  //},

	  getTreeDataFromServer: function (callback) {
		//console.log(cityProvinceSelect.selectedOu.id);
		if (cityProvinceSelect.selectedOu.id) {
		  _geoUnitService.getChildrenGeoUnit(cityProvinceSelect.selectedOu.id).done(function (result) {

			//console.log(result);

			//var treeData = _.map(result.items, function (item) {
			//  return {
			//    id: item.id,
			//    parent: item.parentId ? item.parentId : '#',
			//    code: item.code,
			//    displayName: item.displayName,
			//    //memberCount: item.memberCount,
			//    //roleCount: item.roleCount,
			//    text: districtSelect.generateTextOnTree(item),
			//    state: {
			//      opened: true,
			//    },
			//  };
			//});

			//callback(treeData);
			callback(result.items);


		  });
		}

	  },

	  init: function () {
		//districtSelect.getTreeDataFromServer(function (treeData) {
		//  //districtSelect.setUnitCount(treeData.length);
		//  //districtSelect.$list.on('click', '.ou-text .fa-caret-down', function (e) {
		//  //  e.preventDefault();

		//  //  var ouId = $(this).closest('.ou-text').attr('data-ou-id');
		//  //  setTimeout(function () {
		//  //    districtSelect.$list.jstree('show_contextmenu', ouId);
		//  //  }, 100);
		//  //});
		//});

		$('#AddRootUnitButton').click(function (e) {
		  e.preventDefault();
		  districtSelect.addUnit(null);
		});

		//districtSelect.$list.on('click', '.cityProvinceItem', function () { cityProvinceSelect.selectedOu.set($(this)); });

		districtSelect.$list.on('click', '.districtItem', function () {
		  //var selectedItem = $(this);
		  districtSelect.$list.find("[data-geounitid='" + districtSelect.selectedOu.id + "']").toggleClass('active');
		  $(this).toggleClass('active');
		  districtSelect.selectedOu.set($(this));
		});
		wardCommuneSelect.reload();
	  },

	  reload: function () {
		districtSelect.$list.html('');
		districtSelect.selectedOu.set(null);

		districtSelect.getTreeDataFromServer(function (treeData) {
		  //console.log(treeData);

		  $.each(treeData, function () {
			districtSelect.$list.append(districtSelect.generateTextOnTree(this));
		  });


		});
	  },
	};

	var wardCommuneSelect = {
	  $list: $('#WardCommune'),

	  selectedOu: {
		id: null,
		displayName: null,
		code: null,

		set: function (ouInTree) {
		  if (!ouInTree) {
			wardCommuneSelect.selectedOu.id = null;
			wardCommuneSelect.selectedOu.displayName = null;
			wardCommuneSelect.selectedOu.code = null;
		  } else {
			//wardCommuneSelect.selectedOu.id = ouInTree.id;
			//wardCommuneSelect.selectedOu.displayName = ouInTree.original.displayName;
			//wardCommuneSelect.selectedOu.code = ouInTree.original.code;
			wardCommuneSelect.selectedOu.id = ouInTree[0].dataset.geounitid;
			wardCommuneSelect.selectedOu.code = ouInTree[0].dataset.code;
		  }

		  members.load();
		  roles.load();
		},
	  },

	  generateTextOnTree: function (ou) {
		return (
		  '<li class="list-group-item wardCommuneItem" data-code="' + ou.code + '" data-geounitid="' + ou.id + '">' + ou.code + '<i class="icofont icofont-arrow-right"> </i>' + ou.displayName + '</li>'
		);
	  },

	  getTreeDataFromServer: function (callback) {

		if (districtSelect.selectedOu.id) {
		  _geoUnitService.getChildrenGeoUnit(districtSelect.selectedOu.id).done(function (result) {

			//callback(treeData);
			callback(result.items);


		  });
		}
		else {
		  wardCommuneSelect.$list.html('');
		}

	  },

	  init: function () {
		//wardCommuneSelect.getTreeDataFromServer(function (treeData) {
		//  //wardCommuneSelect.setUnitCount(treeData.length);
		//  //wardCommuneSelect.$list.on('click', '.ou-text .fa-caret-down', function (e) {
		//  //  e.preventDefault();

		//  //  var ouId = $(this).closest('.ou-text').attr('data-ou-id');
		//  //  setTimeout(function () {
		//  //    wardCommuneSelect.$list.jstree('show_contextmenu', ouId);
		//  //  }, 100);
		//  //});
		//});

		//$('#AddRootUnitButton').click(function (e) {
		//  e.preventDefault();
		//  wardCommuneSelect.addUnit(null);
		//});

		wardCommuneSelect.$list.on('click', '.wardCommuneItem', function () {
		  //console.log(this);
		  wardCommuneSelect.$list.find("[data-geounitid='" + wardCommuneSelect.selectedOu.id + "']").toggleClass('active');
		  $(this).toggleClass('active');
		  wardCommuneSelect.selectedOu.set($(this));

		});
	  },

	  reload: function () {
		wardCommuneSelect.$list.html('');
		wardCommuneSelect.getTreeDataFromServer(function (treeData) {
		  console.log(treeData);

		  $.each(treeData, function () {
			wardCommuneSelect.$list.append(wardCommuneSelect.generateTextOnTree(this));
		  });


		});
	  },
	};

	cityProvinceSelect.init();
	districtSelect.init();
	wardCommuneSelect.init();

  });
})();
