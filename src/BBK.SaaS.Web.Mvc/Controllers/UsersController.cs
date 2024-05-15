using Abp.AspNetCore.Mvc.Authorization;
using BBK.SaaS.Authorization;
using BBK.SaaS.Storage;
using Abp.BackgroundJobs;
using Abp.Authorization;

namespace BBK.SaaS.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}