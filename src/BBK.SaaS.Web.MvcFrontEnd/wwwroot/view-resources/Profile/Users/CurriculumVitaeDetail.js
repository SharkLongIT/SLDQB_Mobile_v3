(function () {
    $(function () {
        var _jobApplicationService = abp.services.app.jobApplication;

        $('button.DeleteJobApplication').click(function () {
                
            var user = {};
            if ($('#Title').text() == '') {
                user.userName = $('#Title').val();
            } else {
                user.userName = $('#Title').text();
            }
            user.UserId = $('input[name=UserId]').val();
            user.id = $(this).attr("data-jobapp");
            deleteUser(user)	
        })
        function deleteUser(user) {
        	//if (user.userName === app.consts.userManagement.defaultAdminUserName) {
        	//	abp.message.warn(app.localize('{0}UserCannotBeDeleted', app.consts.userManagement.defaultAdminUserName));
        	//	return;
        	//}

        	abp.message.confirm(
                app.localize('Hồ sơ ứng tuyển ' + user.userName + ' sẽ bị xóa'),
        		app.localize('AreYouSure'),
        		function (isConfirmed) {
        			if (isConfirmed) {
                        _jobApplicationService
                            .deleteJobApplication({
        						id: user.id,
        					})
                            .done(function () {
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                                window.location.href = abp.appPath + "Profile/UsersType1/CurriculumVitaeJobApplication?userId=" + user.UserId
        					});
        			}
        		}
        	);
        }
    });
})();
