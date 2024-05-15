using Abp.AspNetCore.Mvc.Authorization;
using BBK.SaaS.Authorization.Users.Profile;
using BBK.SaaS.Graphics;
using BBK.SaaS.Storage;

namespace BBK.SaaS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        public ProfileController(
            ITempFileCacheManager tempFileCacheManager,
            IProfileAppService profileAppService,
            IImageValidator imageValidator) :
            base(tempFileCacheManager, profileAppService, imageValidator)
        {
        }
    }
}