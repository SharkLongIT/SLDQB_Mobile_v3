using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Recruiters
{
    public class NVNVRecruiterAppService : SaaSAppServiceBase, INVNVRecruiterAppService
    {
        private readonly IRepository<Recruiter, long> _RecruiterRespo;
        public NVNVRecruiterAppService(IRepository<Recruiter, long> RecruiterRespo)
        {
            _RecruiterRespo = RecruiterRespo;
        }


        public async Task<PagedResultDto<RecruiterEditDto>> GetAllRecruiter(NVNVRecruiterSearch input)
        {
            var recruiterAcc = _RecruiterRespo.GetAll()
                .AsNoTracking()
                .Include(e => e.Account)
                .Include(e => e.HumanResSizeCat)
                .Include(e => e.SphereOfActivity)
                .Include(e => e.Province)
                .Include(e => e.District)
                .Include(e => e.Village)
                .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.CompanyName.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(input.SphereOfActivity !=0, x => x.SphereOfActivityId == input.SphereOfActivity).ToList()
                .WhereIf(input.Address !=0, x => x.ProvinceId == input.Address).ToList()
                .Select(x => new RecruiterEditDto
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    TaxCode = x.TaxCode,
                    HumanResSizeCat = ObjectMapper.Map<CatUnitDto>(x.HumanResSizeCat),
                    DateOfEstablishment = x.DateOfEstablishment,
                    SphereOfActivity = ObjectMapper.Map<CatUnitDto>(x.SphereOfActivity),
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    ContactPhone = x.ContactPhone,
                    UserId = x.UserId,
                })
               .ToList();


            return new PagedResultDto<RecruiterEditDto>(
                     recruiterAcc.Count(),
                     recruiterAcc.ToList()
                     );
        }


        public async Task<long> Update(RecruiterEditDto input)
        {
            try
            {
                Recruiter Recruiter = await _RecruiterRespo.FirstOrDefaultAsync(x => x.Id == input.Id);
                ObjectMapper.Map(input, Recruiter);
                await _RecruiterRespo.UpdateAsync(Recruiter);
                return input.Id.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task Delete(long? Id)
        {
            try
            {
                if (Id.HasValue)
                {
                    var Recruitment = _RecruiterRespo.Get(Id.Value);
                    Recruitment.IsDeleted = true;
                    Recruitment.DeletionTime = DateTime.Now;
                    await _RecruiterRespo.UpdateAsync(Recruitment);
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}
