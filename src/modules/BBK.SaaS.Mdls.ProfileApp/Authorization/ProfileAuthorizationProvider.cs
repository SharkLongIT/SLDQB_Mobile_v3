using Abp.Authorization;
using Abp.Localization;

namespace BBK.SaaS.Mdls.Profile.Authorization
{
    public class ProfileAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            // nha tuyen dung
            var Recruiter = context.CreatePermission(ProfilePermission.Recruiter, L("Recruiter"));
            Recruiter.CreateChildPermission(ProfilePermission.Recruiter_Update, L("Recruiter_Update"));

            // nguoi lao dong 
            var Candidate = context.CreatePermission(ProfilePermission.Candidate, L("Candidate"));
            Candidate.CreateChildPermission(ProfilePermission.Candidate_Update, L("Candidate_Update"));
        }
        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, SaaSConsts.LocalizationSourceName);
        }
    }
}
