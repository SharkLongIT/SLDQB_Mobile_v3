using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Recruiters
{
    public class RecruiterAppService : SaaSAppServiceBase, IRecruiterAppService
	{
		private readonly IRepository<Recruiter, long> _recruiterRepo;
        private readonly IRepository<GeoUnit, long> _GeoUnitRepo;

        public RecruiterAppService(IRepository<Recruiter, long> recruiter , IRepository<GeoUnit, long> GeoUnitRepo)
		{
			_recruiterRepo = recruiter;
            _GeoUnitRepo = GeoUnitRepo;
		}

		public async Task<GetRecruiterForEditOutput> GetRecruiterForEdit(NullableIdDto<long> input)
		{
            //Long tenant = 1
            var unit = UnitOfWorkManager.Current;
            if (unit.GetTenantId() == null)
            {
                unit.SetTenantId(1);
            }
            var output = new GetRecruiterForEditOutput { Recruiter = new RecruiterEditDto(), User = new UserEditDto() };

            if (input.Id != 0)
			{
				var recruiterAcc = await _recruiterRepo.GetAll()
					//.AsNoTracking()
					.Include(e => e.Account)
                    .Include(e=>e.HumanResSizeCat)
                    .Include(e=>e.SphereOfActivity)
                    .Include(e => e.Province)
                    .Include(e => e.District)
                    .Include(e => e.Village)
                    .FirstOrDefaultAsync(u => u.UserId == input.Id);

				////Editing an existing user
				//var user = await UserManager.GetUserByIdAsync(input.Id.Value);

				output.User = ObjectMapper.Map<UserEditDto>(recruiterAcc.Account);
				output.ProfilePictureId = recruiterAcc.Account.ProfilePictureId;

				output.Recruiter = ObjectMapper.Map<RecruiterEditDto>(recruiterAcc);
                output.Recruiter.Province = ObjectMapper.Map<GeoUnitDto>(recruiterAcc.Province);
                output.Recruiter.District = ObjectMapper.Map<GeoUnitDto>(recruiterAcc.District);
                output.Recruiter.Village = ObjectMapper.Map<GeoUnitDto>(recruiterAcc.Village);
                output.Recruiter.HumanResSizeCat = ObjectMapper.Map<CatUnitDto>(recruiterAcc.HumanResSizeCat);
                output.Recruiter.SphereOfActivity = ObjectMapper.Map<CatUnitDto>(recruiterAcc.SphereOfActivity);
            }

			return output;
		}

		//public async Task<RecruiterEditDto>

		public async Task<bool> UpdateRecruiterBL(NullableIdDto<long> input, string fileUrl)
		{
			if (input.Id.HasValue)
			{
				Recruiter recruiter = null;
				if (IsAdmin())
				{
					recruiter = await _recruiterRepo.FirstOrDefaultAsync(x => x.Id == input.Id);
				}
				else
				{
					recruiter = await _recruiterRepo.FirstOrDefaultAsync(x => x.Id == input.Id && x.CreatorUserId == AbpSession.UserId);
				}

				if (recruiter != null)
				{
					recruiter.BusinessLicenseUrl = fileUrl;
				}
			}

			return true;
		}

        /// <summary>
        /// Tạo mới nhà tuyển dụng 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<long> Create(RecruiterEditDto input)
        {
            try
            {
                Recruiter newItemId = ObjectMapper.Map<Recruiter>(input);
                newItemId.UserId = AbpSession.UserId.Value;
                var newId = await _recruiterRepo.InsertAndGetIdAsync(newItemId);
                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task<long> Update(RecruiterEditDto input)
        {
            try
            {
                Recruiter Recruiter = await _recruiterRepo.FirstOrDefaultAsync(x => x.Id == input.Id);
                ObjectMapper.Map(input, Recruiter);
                await _recruiterRepo.UpdateAsync(Recruiter);
                return input.Id.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<long> GetCrurrentUserId()
        {
            try
            {
                Recruiter Recruiter = await _recruiterRepo.FirstOrDefaultAsync(x => x.UserId == AbpSession.UserId);

                if(Recruiter == null)
                {
                    return 0;
                }
                else
                {
                    return Recruiter.UserId;
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }



        public async Task<PagedResultDto<RecruiterEditDto>> GetAll()
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }

                var query = _recruiterRepo
                            .GetAll().AsNoTracking()
                            .Select(x => new RecruiterEditDto
                            {
                               Id = x.Id,
                               CompanyName = x.CompanyName,
                            }).ToList();

                

                var Count = query.Count();
                return new PagedResultDto<RecruiterEditDto>(
                    Count,
                    query.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        #region Mobile
        public async Task<long> UpdateRecruiterForMobile(RecruiterEditDto input)
        {
            try
            {
                Recruiter Recruiter = new Recruiter();
                Recruiter = await _recruiterRepo.FirstOrDefaultAsync(x => x.Id == input.Id);
                if (Recruiter == null)
                {
                    return 0;
                }
                // Recruiter.Id = input.Id.Value;
                //Recruiter.UserId = input.UserId;
                Recruiter.CompanyName = input.CompanyName;
                Recruiter.ContactName = input.ContactName;
                Recruiter.ContactEmail = input.ContactEmail;
                Recruiter.ContactPhone = input.ContactPhone;
                if (input.HumanResSizeCatId > 0)
                {
                    Recruiter.HumanResSizeCatId = input.HumanResSizeCatId;
                }
                Recruiter.SphereOfActivityId = input.SphereOfActivityId;
                Recruiter.WebSite = input.WebSite;
                Recruiter.TaxCode = input.TaxCode;
                Recruiter.DateOfEstablishment = input.DateOfEstablishment;
                Recruiter.Address = input.Address;
                if (input.ProvinceId > 0)
                {
                    Recruiter.ProvinceId = input.ProvinceId;

                }
                if (input.DistrictId > 0)
                {
                    Recruiter.DistrictId = input.DistrictId;
                }
                if (input.VillageId > 0)
                {
                    Recruiter.VillageId = input.VillageId;
                }
                Recruiter.Description = input.Description;
                //await _recruiterRepo.UpdateAsync(Recruiter);
                return input.Id.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}
