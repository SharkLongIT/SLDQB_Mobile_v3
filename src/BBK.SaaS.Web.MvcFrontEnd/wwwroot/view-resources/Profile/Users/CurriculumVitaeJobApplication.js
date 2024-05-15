(function () {
    $(function () {
        var _$usersTable = $('#UsersTable');
        var _nVNVCandidateService = abp.services.app.nVNVCandidate;
        var _jobApplication = abp.services.app.jobApplication
        var _$numberOfFilteredPermission = $('#NumberOfFilteredPermission');

        var _selectedPermissionNames = [];

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.Users.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.Users.Edit'),
            changePermissions: abp.auth.hasPermission('Pages.Administration.Users.ChangePermissions'),
            impersonation: abp.auth.hasPermission('Pages.Administration.Users.Impersonation'),
            unlock: abp.auth.hasPermission('Pages.Administration.Users.Unlock'),
            delete: abp.auth.hasPermission('Pages.Administration.Users.Delete'),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'App/Users/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/App/Users/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditUserModal',
        });

        var _userPermissionsModal = new app.ModalManager({
            viewUrl: abp.appPath + 'App/Users/PermissionsModal',
            scriptUrl: abp.appPath + 'view-resources/App/Users/_PermissionsModal.js',
            modalClass: 'UserPermissionsModal',
        });

        var filter = function () {
            var datafilter = {};
            //datafilter.search = $('#SearchTerm').val();
            //if ($('#Gender').val() != 0 && $('#Gender').val() != undefined && $('#Gender').val() != null) {
            //    datafilter.gender = $('#Gender').val();
            //}
            datafilter.candidateId = $('input[name=CandidateId]').val();
            if ($('#WorkSite').val() != 0 && $('#WorkSite').val() != undefined && $('#WorkSite').val() != null) {
                datafilter.workSite = $('#WorkSite').val();
            }
            if ($('#ExperiencesId').val() != 0 && $('#ExperiencesId').val() != undefined && $('#ExperiencesId').val() != null) {

                datafilter.experiencesId = $('#ExperiencesId').val();
            }
            if ($('#LiteracyId').val() != 0 && $('#LiteracyId').val() != undefined && $('#LiteracyId').val() != null) {

                datafilter.literacyId = $('#LiteracyId').val();
            }
            if ($('#OccupationId').val() != null && $('#OccupationId').val() != undefined && $('#OccupationId').val() != 0) {

                datafilter.occupationId = $('#OccupationId').val();
            }
            if ($('#salary').val() != 0 && $('#salary').val() != null && $('#salary').val() != undefined) {
                datafilter.salary = $('#salary').val();
            }
            return datafilter

        }

        var dataTable = _$usersTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _nVNVCandidateService.getAllJobOfProfessionalStaff,
                inputFilter: filter,

                //function () {
                //return {
                //	filter: $('#UsersTableFilter').val(),
                //	permissions: _selectedPermissionNames,
                //	role: $('#RoleSelectionCombo').val(),
                //	onlyLockedUsers: $('#UsersTable_OnlyLockedUsers').is(':checked'),
                //	userType: 2
                //};

                //     },
            },
            columnDefs: [

                {
                    targets: 0,
                    data: 'userName',
                    render: function (userName, type, row, meta) {
                        //var $container = $('<span/>');
                        //var $userName = $('<span/>');
                        //var lockedIcon = '<i class="fas fa-lock ms-2"></i>';
                        //var profilePicture =
                        //	abp.appPath + 'Profile/GetProfilePictureByUser?userId=' + row.id + '&profilePictureId=' + row.profilePictureId;

                        //if (profilePicture) {
                        //	var $link = $('<a/>').attr('href', profilePicture).attr('target', '_blank');
                        //	var $img = $('<img/>').addClass('img-circle').attr('src', profilePicture);

                        //	$link.append($img);
                        //	$container.append($link);
                        //}

                        //$userName.append(userName);

                        //if (row.lockoutEndDateUtc) {
                        //	if (moment.utc(row.lockoutEndDateUtc) > moment.utc()) {
                        //		$userName.append(lockedIcon);
                        //	}
                        //}

                        //$container.append($userName);
                        //return $container[0].outerHTML;
                        return meta.row + 1;
                    },
                },
                {
                    targets: 1,
                    data: 'name',
                    render: function (userName, type, row, meta) {
                        return row.jobApplication.title;
                    }
                },
                {
                    targets: 2,
                    data: 'surname',
                    render: function (userName, type, row, meta) {
                        return row.candidate.gender == 1 ? app.localize("Male") : app.localize("Female");
                    }
                },
                {
                    targets: 3,
                    data: 'roles',
                    orderable: false,
                    render: function (userName, type, row, meta) {
                        return row.jobApplication.experiences.displayName;
                    }
                },
                {
                    targets: 4,
                    data: 'emailAddress',
                    render: function (userName, type, row, meta) {
                        return row.jobApplication.literacy.displayName;
                    }
                },
                {
                    targets: 5,
                    data: 'isEmailConfirmed',
                    //render: function (isEmailConfirmed) {
                    //	var $span = $('<span/>').addClass('label');
                    //	if (isEmailConfirmed) {
                    //		$span.addClass('badge badge-success').text(app.localize('Yes'));
                    //	} else {
                    //		$span.addClass('badge badge-dark').text(app.localize('No'));
                    //	}
                    //	return $span[0].outerHTML;
                    //},
                    render: function (userName, type, row, meta) {
                        return row.jobApplication.occupations.displayName;
                    }
                },
                {
                    targets: 6,
                    data: 'isActive',
                    //render: function (isActive) {
                    //	var $span = $('<span/>').addClass('label');
                    //	if (isActive) {
                    //		$span.addClass('badge badge-success').text(app.localize('Yes'));
                    //	} else {
                    //		$span.addClass('badge badge-dark').text(app.localize('No'));
                    //	}
                    //	return $span[0].outerHTML;
                    //},
                    render: function (userName, type, row, meta) {
                        return row.jobApplication.province.displayName;
                    }
                },
                {
                    targets: 7,
                    data: 'creationTime',
                    render: function (userName, type, row, meta) {
                        if (row.jobApplication.desiredSalary == null) {
                            return `<span> Lương thỏa thuận <\span>`
                        }
                        return row.jobApplication.desiredSalary;
                    }
                },
                {
                    targets: 8,
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
                            //{
                            //    text: app.localize('Danh sách hồ sơ ứng tuyển'),
                            //    visible: function () {
                            //        return _permissions.edit;
                            //    },
                            //    action: function (data) {
                            //        window.open("/Profile/Candidate/Detail?Id=" + data.record.user.id, "_blank");
                            //    },
                            //},
                            {
                                text: app.localize('Edit'),
                                //visible: function () {
                                //    return _permissions.edit;
                                //},
                                action: function (data) {
                                     // window.open("/Profile/UsersType1/CurriculumVitaeDetail?JobAppId=" + data.record.jobApplication.id, "_blank");
                                     window.location.href ="/Profile/UsersType1/CurriculumVitaeDetail?JobAppId=" + data.record.jobApplication.id;
                                },
                            },
                            {
                                text: app.localize('Delete'),
                                //visible: function (data) {
                                //    return _permissions.delete;
                                //},
                                action: function (data) {
                                    abp.libs.sweetAlert.config = {
                                        confirm: {
                                            icon: 'warning',
                                            buttons: ['Không', 'Có']
                                        }
                                    };
                                    deleteUser(data.record.user, data.record.jobApplication);
                                },
                            },
                        ],
                    },
                },
            ],
        });

        function getUsers() {
            dataTable.ajax.reload();
        }

        function deleteUser(user,jobapplicaiton) {
            if (user.userName === app.consts.userManagement.defaultAdminUserName) {
                abp.message.warn(app.localize('{0}UserCannotBeDeleted', app.consts.userManagement.defaultAdminUserName));
                return;
            }

            abp.message.confirm(
                app.localize('Hồ sơ ứng tuyển ' + jobapplicaiton.title + ' sẽ bị xóa'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _jobApplication
                            .deleteJobApplication({
                                id: jobapplicaiton.id,
                            })
                            .done(function () {
                                getUsers(true);
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                    }
                }
            );
        }

        abp.event.on('app.createOrEditUserModalSaved', function () {
            getUsers();
        });

        $('#UsersTableFilter').focus();

        $('#Search').click(function () {
            getUsers();
        });
    });
})();
