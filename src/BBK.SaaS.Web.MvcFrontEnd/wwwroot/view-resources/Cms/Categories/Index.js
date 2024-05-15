(function () {
  $(function () {
    var _categoryService = abp.services.app.cmsCats;

    var _permissions = {
      manageOrganizationTree: abp.auth.hasPermission('Pages.Administration.CommFuncs'),
      //manageMembers: abp.auth.hasPermission('Pages.Administration.CmsCats.ManageMembers'),
      //manageRoles: abp.auth.hasPermission('Pages.Administration.CmsCats.ManageRoles'),
    };

    var _createModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Cms/Categories/CreateModal',
      scriptUrl: abp.appPath + 'view-resources/Cms/Categories/_CreateModal.js',
      modalClass: 'CreateCmsCatModal',
    });

    var _editModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Cms/Categories/EditModal',
      scriptUrl: abp.appPath + 'view-resources/Cms/Categories/_EditModal.js',
      modalClass: 'EditCmsCatModal',
    });
    
    
    var categoryTree = {
      $tree: $('#CategoryEditTree'),

      $emptyInfo: $('#CategoryEditTreeEmptyInfo'),

      show: function () {
        categoryTree.$emptyInfo.hide();
        categoryTree.$tree.show();
      },

      hide: function () {
        categoryTree.$emptyInfo.show();
        categoryTree.$tree.hide();
      },

      unitCount: 0,

      setUnitCount: function (unitCount) {
        categoryTree.unitCount = unitCount;
        if (unitCount) {
          categoryTree.show();
        } else {
          categoryTree.hide();
        }
      },

      refreshUnitCount: function () {
        categoryTree.setUnitCount(categoryTree.$tree.jstree('get_json').length);
      },

      selectedOu: {
        id: null,
        displayName: null,
        code: null,

        set: function (ouInTree) {
          if (!ouInTree) {
            categoryTree.selectedOu.id = null;
            categoryTree.selectedOu.displayName = null;
            categoryTree.selectedOu.slug = null;
            categoryTree.selectedOu.code = null;
          } else {
            categoryTree.selectedOu.id = ouInTree.id;
            categoryTree.selectedOu.displayName = ouInTree.original.displayName;
            categoryTree.selectedOu.slug = ouInTree.original.slug;
            categoryTree.selectedOu.code = ouInTree.original.code;
          }


          categoryDetail.load();
          //roles.load();
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
                  instance.rename_node(node, categoryTree.generateTextOnTree(updatedOu));
                }
              );
            },
          },

          addSubUnit: {
            label: app.localize('AddSubUnit'),
            icon: 'la la-plus',
            _disabled: !_permissions.manageOrganizationTree,
            action: function () {
              categoryTree.addUnit(node.id);
            },
          },

          //addMember: {
          //  label: app.localize('AddMember'),
          //  icon: 'la la-user-plus',
          //  _disabled: !_permissions.manageMembers,
          //  action: function () {
          //    members.openAddModal();
          //  },
          //},

          //addRole: {
          //  label: app.localize('AddRole'),
          //  icon: 'la la-user-plus',
          //  _disabled: !_permissions.manageRoles,
          //  action: function () {
          //    roles.openAddModal();
          //  },
          //},

          delete: {
            label: app.localize('Delete'),
            icon: 'la la-remove',
            _disabled: !_permissions.manageOrganizationTree,
            action: function (data) {
              var instance = $.jstree.reference(data.reference);

              abp.message.confirm(
                app.localize('CmsCatDeleteWarningMessage', node.original.displayName),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                  if (isConfirmed) {
                    _categoryService
                      .deleteCmsCat({
                        id: node.id,
                      })
                      .done(function () {
                        abp.notify.success(app.localize('SuccessfullyDeleted'));
                        instance.delete_node(node);
                        categoryTree.refreshUnitCount();
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

        return items;
      },

      addUnit: function (parentId) {
        var instance = $.jstree.reference(categoryTree.$tree);

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
              slug: newOu.slug,
              memberCount: 0,
              roleCount: 0,
              text: categoryTree.generateTextOnTree(newOu),
              state: {
                opened: true,
              },
            });

            categoryTree.refreshUnitCount();
          }
        );
      },

      generateTextOnTree: function (ou) {
        var itemClass = ou.memberCount > 0 || ou.roleCount ? ' ou-text-has-members' : ' ou-text-no-members';
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
          '<span class="ou-text-member-count ml-2">' +
          ou.memberCount +
          ' ' +
          app.localize('Members') +
          ' ,</span> <span class="ou-text-role-count ml-1">' +
          ou.roleCount +
          ' ' +
          app.localize('Roles') +
          '</span></span></span>'
        );
      },

      incrementMemberCount: function (ouId, incrementAmount) {
        var treeNode = categoryTree.$tree.jstree('get_node', ouId);
        treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
        categoryTree.$tree.jstree('rename_node', treeNode, categoryTree.generateTextOnTree(treeNode.original));
      },

      incrementRoleCount: function (ouId, incrementAmount) {
        var treeNode = categoryTree.$tree.jstree('get_node', ouId);
        treeNode.original.roleCount = treeNode.original.roleCount + incrementAmount;
        categoryTree.$tree.jstree('rename_node', treeNode, categoryTree.generateTextOnTree(treeNode.original));
      },

      getTreeDataFromServer: function (callback) {
        _categoryService.getCmsCats({}).done(function (result) {
          var treeData = _.map(result.items, function (item) {
            return {
              id: item.id,
              parent: item.parentId ? item.parentId : '#',
              code: item.code,
              displayName: item.displayName,
              slug: item.slug,
              memberCount: item.memberCount,
              roleCount: item.roleCount,
              text: categoryTree.generateTextOnTree(item),
              state: {
                opened: true,
              },
            };
          });

          callback(treeData);
        });
      },

      init: function () {
        categoryTree.getTreeDataFromServer(function (treeData) {
          categoryTree.setUnitCount(treeData.length);

          categoryTree.$tree
            .on('changed.jstree', function (e, data) {
              if (data.selected.length != 1) {
                categoryTree.selectedOu.set(null);
              } else {
                var selectedNode = data.instance.get_node(data.selected[0]);
                //console.log(selectedNode);
                categoryTree.selectedOu.set(selectedNode);
              }
            })
            .on('move_node.jstree', function (e, data) {
              var parentNodeName =
                !data.parent || data.parent == '#'
                  ? app.localize('Root')
                  : categoryTree.$tree.jstree('get_node', data.parent).original.displayName;

              abp.message.confirm(
                app.localize('CmsCatMoveConfirmMessage', data.node.original.displayName, parentNodeName),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                  if (isConfirmed) {
                    _categoryService
                      .moveCmsCat({
                        id: data.node.id,
                        newParentId: data.parent === '#' ? null : data.parent,
                      })
                      .done(function () {
                        abp.notify.success(app.localize('SuccessfullyMoved'));
                        categoryTree.reload();
                      })
                      .fail(function (err) {
                        categoryTree.$tree.jstree('refresh'); //rollback
                        setTimeout(function () {
                          abp.message.error(err.message);
                        }, 500);
                      });
                  } else {
                    categoryTree.$tree.jstree('refresh'); //rollback
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
                items: categoryTree.contextMenu,
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
            categoryTree.addUnit(null);
          });

          categoryTree.$tree.on('click', '.ou-text .fa-caret-down', function (e) {
            e.preventDefault();

            var ouId = $(this).closest('.ou-text').attr('data-ou-id');
            setTimeout(function () {
              categoryTree.$tree.jstree('show_contextmenu', ouId);
            }, 100);
          });
        });
      },

      reload: function () {
        categoryTree.getTreeDataFromServer(function (treeData) {
          categoryTree.setUnitCount(treeData.length);
          categoryTree.$tree.jstree(true).settings.core.data = treeData;
          categoryTree.$tree.jstree('refresh');
        });
      },
    };

    var categoryDetail = {
      $table: $('#catDetailInfo'),
      $emptyInfo: $('#CatDetailEmptyInfo'),
      $addUserToOuButton: $('#AddUserToOuButton'),
      $selectedOuRightTitle: $('#SelectedOuRightTitle'),

      load: function () {
        if (!categoryTree.selectedOu.id) {
          categoryDetail.hideInfo();
          return;
        }

        categoryDetail.$table.find('#DisplayName').html(categoryTree.selectedOu.displayName);
        categoryDetail.$table.find('#Slug').html(categoryTree.selectedOu.slug);
        categoryDetail.showInfo();
      },

      hideInfo: function () {
        categoryDetail.$table.hide();
        categoryDetail.$emptyInfo.show();
      },

      showInfo: function () {
        categoryDetail.$table.show();
        categoryDetail.$emptyInfo.hide();
      },
      
      init: function () {
        
      },
    };
    
    categoryTree.init();
  });
})();
