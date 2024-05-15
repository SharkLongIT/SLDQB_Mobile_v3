using System.Threading.Tasks;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Sessions.Dto;

namespace BBK.SaaS.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
        Task<Recruiter> GetRecruiter();
        Task<Candidate> GetCandidate();
    }
}
