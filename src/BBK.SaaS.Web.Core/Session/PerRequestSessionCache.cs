using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using BBK.SaaS.Sessions;
using BBK.SaaS.Sessions.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters;
using Abp.Domain.Repositories;
using System.Linq;

namespace BBK.SaaS.Web.Session
{
    public class PerRequestSessionCache : IPerRequestSessionCache, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionAppService _sessionAppService;
        private readonly IRepository<Recruiter,long> _recruiter;
        private readonly IRepository<Candidate,long> _candidate;

        public PerRequestSessionCache(
            IHttpContextAccessor httpContextAccessor,
            ISessionAppService sessionAppService,
            IRepository<Recruiter, long> recruiter,
            IRepository<Candidate, long> candidate)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionAppService = sessionAppService;
            _recruiter = recruiter;
            _candidate = candidate;
        }

        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return await _sessionAppService.GetCurrentLoginInformations();
            }

            var cachedValue = httpContext.Items["__PerRequestSessionCache"] as GetCurrentLoginInformationsOutput;
            if (cachedValue == null)
            {
                cachedValue = await _sessionAppService.GetCurrentLoginInformations();
                httpContext.Items["__PerRequestSessionCache"] = cachedValue;
            }

            return cachedValue;
        }

        public async Task<Recruiter> GetRecruiter()
        {
            var currenUser = await GetCurrentLoginInformationsAsync();

            var getRecruiter =  _recruiter.GetAll().Where(x=>x.UserId == currenUser.User.Id).FirstOrDefault();

            return getRecruiter;
        }
        public async Task<Candidate> GetCandidate()
        {
            var currenUser = await GetCurrentLoginInformationsAsync();

            var getCandidate = _candidate.GetAll().Where(x => x.UserId == currenUser.User.Id).FirstOrDefault();

            return getCandidate;
        }
    }
}