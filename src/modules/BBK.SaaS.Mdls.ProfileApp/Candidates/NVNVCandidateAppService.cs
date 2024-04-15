using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public class NVNVCandidateAppService : SaaSAppServiceBase, INVNVCandidateAppService
    {
        private readonly IRepository<JobApplication, long> _jobApplicationRepo;
        private readonly IRepository<Candidate, long> _candidateRepo;
        public NVNVCandidateAppService(
            IRepository<JobApplication, long> jobApplicationRepo,
            IRepository<Candidate, long> candidateRepo)
        {
            _jobApplicationRepo = jobApplicationRepo;
            _candidateRepo = candidateRepo;
        }
        public async Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobOfProfessionalStaff(JobAppSearchOfProfessionalStaff input)
        {
            try
            {
                var output = new PagedResultDto<GetJobApplicationForEditOutput> { Items = new List<GetJobApplicationForEditOutput>(), TotalCount = 0 };

                var queryJobApps = _jobApplicationRepo.GetAll()
                    .Include(e => e.Province)
                    .Include(e => e.Literacy)
                    .Include(e => e.Experiences)
                    .Include(e => e.FormOfWork)
                    .Include(e => e.Positions)
                    .Include(e => e.Occupations)
                    .Include(e => e.Candidate)
                    .Include(e => e.Candidate.Account)
                    .AsNoTracking()
					.WhereIf(input.CandidateId.HasValue, x => x.CandidateId == input.CandidateId)
					.WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Candidate.Account.UserName.ToLower().Contains(input.Search.ToLower()) || x.Candidate.Account.Name.ToLower().Contains(input.Search.ToLower()) || x.Title.ToLower().Contains(input.Search.ToLower()))
                    .WhereIf(input.Gender.HasValue, x => (int)x.Candidate.Gender == input.Gender.Value)
                    .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.WorkSite))
                    .WhereIf(input.ExperiencesId.HasValue, x => x.ExperiencesId.Equals(input.ExperiencesId.Value))
                    .WhereIf(input.LiteracyId.HasValue, x => x.LiteracyId.Equals(input.LiteracyId.Value))
                    .WhereIf(input.OccupationId.HasValue, x => x.OccupationId.Equals(input.OccupationId.Value))
                    .WhereIf(input.DesiredSalary.HasValue, x => x.DesiredSalary.Equals(input.DesiredSalary.Value))
                    ;

                if (queryJobApps == null || queryJobApps.Count() <= 0)
                {
                    return output;
                }
                List<GetJobApplicationForEditOutput> queryCommon = new List<GetJobApplicationForEditOutput>();

                foreach (var item in queryJobApps)
                {

                    queryCommon.Add(new GetJobApplicationForEditOutput
                    {
                        ProfilePictureId = item.Candidate.Account.ProfilePictureId,
                        Candidate = ObjectMapper.Map<CandidateEditDto>(item.Candidate),
                        JobApplication = ObjectMapper.Map<JobApplicationEditDto>(item),
                        User = ObjectMapper.Map<UserEditDto>(item.Candidate.Account)

                    });


                }

                if (queryCommon.Count() <= 0)
                { return output; }

                output.Items = queryCommon.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

                output.TotalCount = queryCommon.Count();

                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<GetCandidateForEditOutput>> GetAllCandidateOfProfessionalStaff(JobAppSearchOfProfessionalStaff input)
        {
            try
            {
                List<GetCandidateForEditOutput> GetCandidateForEdits = new List<GetCandidateForEditOutput>();
                
                List<CandidateEditDto> Candidates = new List<CandidateEditDto>();

                var query = _candidateRepo.GetAll()
                    .Include(x => x.Account)
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Account.Name.ToLower().Contains(input.Search.ToLower()))
                    .WhereIf(input.Gender.HasValue, x => (int)x.Gender == input.Gender.Value)
                    .WhereIf(input.ProvinceId.HasValue, x => x.ProvinceId == input.ProvinceId.Value)
                    .WhereIf(input.DistrictId.HasValue, x => x.DistrictId == input.DistrictId.Value)
                    .OrderByDescending(x=>x.Account.CreationTime);

                if(query == null || query.Count() == 0) {
                    return new PagedResultDto<GetCandidateForEditOutput>
                    {
                        Items = GetCandidateForEdits,
                        TotalCount =Candidates .Count,
                    };
                }
                foreach (var item in query)
                {
                    GetCandidateForEdits.Add(new GetCandidateForEditOutput
                    {
                        Candidate = ObjectMapper.Map<CandidateEditDto>(item),
                        ProfilePictureId = item.Account.ProfilePictureId,
                        User = ObjectMapper.Map<UserEditDto>(item.Account)

                    }); 
                }

                return new PagedResultDto<GetCandidateForEditOutput>
                {
                    Items = GetCandidateForEdits,
                    TotalCount = Candidates.Count,
                };
            }
            catch (Exception ex)
            {

                throw new  Exception(ex.Message) ;
            }
        }

    }
}
