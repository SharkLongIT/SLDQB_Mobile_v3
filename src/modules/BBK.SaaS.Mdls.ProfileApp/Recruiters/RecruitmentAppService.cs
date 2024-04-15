using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Recruiters
{
    public class RecruitmentAppService : SaaSAppServiceBase, IRecruitmentAppService
    {
        private readonly IRepository<Recruitment, long> _recruitmentRepo;
        private readonly IRepository<GeoUnit, long> _georepository;
        private readonly IRepository<CatUnit, long> _CatUnitrepository;
        private readonly IRepository<Recruiter, long> _recruiterrepository;
        public RecruitmentAppService(
            IRepository<Recruitment, long> recruitmentRepo,
            IRepository<GeoUnit, long> georepository,
            IRepository<CatUnit, long> CatUnitrepository,
            IRepository<Recruiter, long> recruiterrepository)
        {
            _recruitmentRepo = recruitmentRepo;
            _georepository = georepository;
            _CatUnitrepository = CatUnitrepository;
            _recruiterrepository = recruiterrepository;
        }

        public async Task<PagedResultDto<RecruitmentDto>> GetAll(RecruimentInput input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }

                var Cat = _CatUnitrepository.GetAll();
                var geo = _georepository.GetAll();

                var listCat = ObjectMapper.Map<List<CatUnitDto>>(Cat);
                var query = _recruitmentRepo
                            .GetAll().AsNoTracking().OrderByDescending(x=>x.Id).Include(x => x.FormOfWorks)
                            .Where(x => x.CreatorUserId == AbpSession.UserId)
                            .WhereIf(!string.IsNullOrEmpty(input.Filtered), u => u.Title.ToLower().Contains(input.Filtered.ToLower()))
                            .WhereIf(input.NumberOfRecruits.HasValue, x => x.NumberOfRecruits.Equals(input.NumberOfRecruits))
                            .WhereIf(input.FormOfWork.HasValue && input.FormOfWork != 0, x => x.FormOfWork.Equals(input.FormOfWork.Value))
                            .WhereIf(input.Job.HasValue && input.Job != 0, x => x.JobCatUnitId.Equals(input.Job.Value))
                            .WhereIf(input.Experience.HasValue && input.Experience != 0, x => x.Experience.Equals(input.Experience.Value))
                            .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.DeadlineSubmission >= DateTime.Parse(input.FromDate) && x.DeadlineSubmission <= DateTime.Parse(input.ToDate))
                            .Select(x => new RecruitmentDto
                            {
                                Id = x.Id,
                                Title = x.Title,
                                FormOfWork = x.FormOfWork,
                                Experience = x.Experience,
                                NumberOfRecruits = x.NumberOfRecruits,
                                DeadlineSubmission = x.DeadlineSubmission,
                                JobCatUnitId = x.JobCatUnitId,
                                Status = x.Status,
                                RecruiterId = x.RecruiterId,
                                MaxSalary = x.MaxSalary,
                                MinSalary = x.MinSalary,
                                ProvinceId = x.ProvinceId,
                                FormOfWorks = ObjectMapper.Map<CatUnitDto>(x.FormOfWorks)
                            }).ToList();

                var result = from c in query
                             join cat in listCat on c.JobCatUnitId equals cat.Id
                             join ex in listCat on c.Experience equals ex.Id
                             join recruiter in _recruiterrepository.GetAll() on c.RecruiterId equals recruiter.Id
                             join ge in geo on c.ProvinceId equals ge.Id

                             select new RecruitmentDto
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 FormOfWork = c.FormOfWork,
                                 Experience = c.Experience,
                                 Experiences = ex,
                                 NumberOfRecruits = c.NumberOfRecruits,
                                 DeadlineSubmission = c.DeadlineSubmission,
                                 JobCatUnitId = c.JobCatUnitId,
                                 Status = c.Status,
                                 JobCatUnitName = cat.DisplayName,
                                 MaxSalary = c.MaxSalary,
                                 MinSalary = c.MinSalary,
                                 RecruiterId = c.RecruiterId,
                                 WorkAddress = ge.DisplayName,
                                 FormOfWorks = c.FormOfWorks,
                                 Recruiter = ObjectMapper.Map<RecruiterEditDto>(recruiter),
                                 

                             };

                var Count = result.Count();
                return new PagedResultDto<RecruitmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<RecruitmentDto>> GetAllByAllUser(RecruimentInput input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }

                var Cat = _CatUnitrepository.GetAll();
                var geo = _georepository .GetAll();
                var query = _recruitmentRepo
                            .GetAll().AsNoTracking().OrderByDescending(x=>x.CreationTime).Include(x=>x.Experiences)
                            .WhereIf(!string.IsNullOrEmpty(input.Filtered), u => u.Title.ToLower().Contains(input.Filtered.ToLower()))
                            .WhereIf(input.NumberOfRecruits.HasValue, x => x.NumberOfRecruits == input.NumberOfRecruits)
                            .WhereIf(input.FormOfWork.HasValue && input.FormOfWork != 0, x => x.FormOfWork.Equals(input.FormOfWork.Value))
                            .WhereIf(input.Job.HasValue && input.Job != 0, x => x.JobCatUnitId.Equals(input.Job.Value))
                            .WhereIf(input.Experience.HasValue && input.Experience != 0, x => x.Experience.Equals(input.Experience.Value))
                            .WhereIf(input.Rank.HasValue && input.Rank != 0, x => x.Rank == input.Rank)
                            .WhereIf(input.Degree.HasValue && input.Degree != 0, x => x.Degree == input.Degree)
                            .WhereIf(input.Salary.HasValue && input.Salary != 0, x => x.MinSalary <= input.Salary &&  x.MaxSalary >= input.SalaryMax)
                            .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.ProvinceId))
                            .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.DeadlineSubmission >= DateTime.Parse(input.FromDate) && x.DeadlineSubmission <= DateTime.Parse(input.ToDate))
                            .Select(x => new RecruitmentDto
                            {
                                Id = x.Id,
                                Title = x.Title,
                                FormOfWork = x.FormOfWork,
                                NumberOfRecruits = x.NumberOfRecruits,
                                DeadlineSubmission = x.DeadlineSubmission,
                                JobCatUnitId = x.JobCatUnitId,
                                Status = x.Status,
                                RecruiterId = x.RecruiterId,
                                MaxSalary = x.MaxSalary,
                                MinAge = x.MinAge,
                                MinSalary = x.MinSalary,
                                Experience = x.Experience,
                                Experiences = ObjectMapper.Map<CatUnitDto>(x.Experiences),
                                Degree = x.Degree,
                                Rank = x.Rank,
                                ProvinceId = x.ProvinceId,
                            }).ToList();

                var result = (from c in query
                             join cat in Cat on c.JobCatUnitId equals cat.Id
                             join recruiter in _recruiterrepository.GetAll() on c.RecruiterId equals recruiter.Id
                             join ge in geo on c.ProvinceId equals ge.Id
                             select new RecruitmentDto
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 FormOfWork = c.FormOfWork,
                                 Experience = c.Experience,
                                 NumberOfRecruits = c.NumberOfRecruits,
                                 DeadlineSubmission = c.DeadlineSubmission,
                                 JobCatUnitId = c.JobCatUnitId,
                                 Status = c.Status,
                                 JobCatUnitName = cat.DisplayName,
                                 RecruiterId = c.RecruiterId,
                                 Recruiter = ObjectMapper.Map<RecruiterEditDto>(recruiter),
                                 MaxSalary = c.MaxSalary,
                                 MinSalary = c.MinSalary,
                                 MinAge = c.MinAge,
                                 Experiences = c.Experiences,
                                 Degree = c.Degree,
                                 Rank = c.Rank,
                                 WorkAddress = ge.DisplayName
                             }).Where(x =>x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Where(x=>x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0);

                var Count = result.Count();
                return new PagedResultDto<RecruitmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }



        public async Task<long> Create(RecruitmentDto input)
        {
            try
            {
                //Recruitment newItemId = ObjectMapper.Map<Recruitment>(input);
                Recruitment newItemId = new Recruitment();
                newItemId.Title = input.Title;
                newItemId.FormOfWork = input.FormOfWork;
                newItemId.Degree = input.Degree;
                newItemId.Experience = input.Experience;
                newItemId.Rank = input.Rank;
                newItemId.MinAge = input.MinAge;
                newItemId.MaxAge = input.MaxAge;
                newItemId.NumberOfRecruits = input.NumberOfRecruits;
                newItemId.ProbationPeriod = input.ProbationPeriod;
                newItemId.DeadlineSubmission = input.DeadlineSubmission;
                newItemId.MinSalary = input.MinSalary;
                newItemId.MaxSalary = input.MaxSalary;
                newItemId.NecessarySkills = input.NecessarySkills;
                newItemId.JobDesc = input.JobDesc;
                newItemId.JobRequirementDesc = input.JobRequirementDesc;
                newItemId.BenefitDesc = input.BenefitDesc;
                newItemId.TenantId = AbpSession.TenantId.Value;
                newItemId.Status = false;
                newItemId.JobCatUnitId = input.JobCatUnitId;
                newItemId.FullName = input.FullName;
                newItemId.Email = input.Email;
                newItemId.PhoneNumber = input.PhoneNumber;
                newItemId.Address = input.Address;
                newItemId.RecruiterId = input.RecruiterId;
                newItemId.TenantId = AbpSession.TenantId.Value;
                newItemId.WorkAddress = input.WorkAddress;
                newItemId.DistrictCode = input.DistrictCode;
                newItemId.ProvinceId = input.ProvinceId;
                if (input.GenderRequired == 1)
                {
                    newItemId.GenderRequired = GenderEnum.Male;
                }
                else { newItemId.GenderRequired = GenderEnum.Female; }
                var newId = await _recruitmentRepo.InsertAndGetIdAsync(newItemId);
                //if (input.RecruitmentAddress.Count > 0)
                //{
                //    foreach (var reAdress in input.RecruitmentAddress)
                //    {
                //        RecruitmentAddress recruitmentAddress = new RecruitmentAddress();
                //        recruitmentAddress.RecruitmentId = newId;
                //        recruitmentAddress.DistrictCode = reAdress.DistrictCode;
                //        recruitmentAddress.WorkAddress = reAdress.WorkAddress;
                //        await _recruitmentAddress.InsertAsync(recruitmentAddress);
                //    }
                //}
                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task<long> Update(RecruitmentDto input)
        {
            try
            {
                Recruitment Recruitment = await _recruitmentRepo.FirstOrDefaultAsync(x => x.Id == input.Id);
                Recruitment.Title = input.Title;
                Recruitment.FormOfWork = input.FormOfWork;
                Recruitment.Degree = input.Degree;
                Recruitment.Experience = input.Experience;
                Recruitment.Rank = input.Rank;
                Recruitment.MinAge = input.MinAge;
                Recruitment.MaxAge = input.MaxAge;
                Recruitment.NumberOfRecruits = input.NumberOfRecruits;
                Recruitment.ProbationPeriod = input.ProbationPeriod;
                Recruitment.DeadlineSubmission = input.DeadlineSubmission;
                Recruitment.MinSalary = input.MinSalary;
                Recruitment.MaxSalary = input.MaxSalary;
                Recruitment.NecessarySkills = input.NecessarySkills;
                Recruitment.JobDesc = input.JobDesc;
                Recruitment.JobRequirementDesc = input.JobRequirementDesc;
                Recruitment.BenefitDesc = input.BenefitDesc;
                Recruitment.TenantId = AbpSession.TenantId.Value;
                Recruitment.FullName = input.FullName;
                Recruitment.Email = input.Email;
                Recruitment.PhoneNumber = input.PhoneNumber;
                Recruitment.Address = input.Address;
                Recruitment.RecruiterId = input.RecruiterId;
                Recruitment.WorkAddress = input.WorkAddress;
                Recruitment.DistrictCode = input.DistrictCode;
                Recruitment.ProvinceId = input.ProvinceId;
                //Recruitment.TenantId = AbpSession.TenantId.Value;
                if (input.GenderRequired == 1)
                {
                    Recruitment.GenderRequired = GenderEnum.Male;
                }
                else { Recruitment.GenderRequired = GenderEnum.Female; }
                Recruitment.Status = input.Status;
                Recruitment.JobCatUnitId = input.JobCatUnitId;
                //if (input.RecruitmentAddress.Count > 0)
                //{
                //    var adress = _recruitmentAddress.GetAll().FirstOrDefault(x => x.RecruitmentId == input.Id.Value);
                //    foreach (var reAdress in input.RecruitmentAddress)
                //    {
                //        adress.RecruitmentId = input.Id.Value;
                //        adress.DistrictCode = reAdress.DistrictCode;
                //        adress.WorkAddress = reAdress.WorkAddress;
                //        await _recruitmentAddress.UpdateAsync(adress);
                //    }
                //}
                await _recruitmentRepo.UpdateAsync(Recruitment);



                return input.Id.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task DeleteDoc(long? Id)
        {
            try
            {
                if (Id.HasValue)
                {
                    var Recruitment = _recruitmentRepo.Get(Id.Value);
                    await _recruitmentRepo.DeleteAsync(Id.Value);
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<RecruitmentDto> GetDetail(long Id)
        {
            if (AbpSession.TenantId.HasValue == false)
            {
                var unit = UnitOfWorkManager.Current;
                unit.SetTenantId(1);
            }
            var item = await _recruitmentRepo.GetAsync(Id);
            var recruiter = await _recruiterrepository.GetAsync(item.RecruiterId);
            item.Recruiter = recruiter;
            
            
           // var adress = _recruitmentAddress.GetAll().FirstOrDefault(x => x.RecruitmentId == Id);
            var geo = await _georepository.GetAll().Where(x => x.Code == item.DistrictCode).FirstOrDefaultAsync();
            var geoProvince = await _georepository.GetAsync(item.ProvinceId);
            var catex = await _CatUnitrepository.GetAsync(item.Experience);
            var catjob = await _CatUnitrepository.GetAsync(item.JobCatUnitId);
            var rank = await _CatUnitrepository.GetAsync(item.Rank);
            var Formow = await _CatUnitrepository.GetAsync(item.FormOfWork);
          
            var output = ObjectMapper.Map<RecruitmentDto>(item);
            output.Recruiter = ObjectMapper.Map<RecruiterEditDto>(item.Recruiter);
            output.Experiences = ObjectMapper.Map<CatUnitDto>(catex);
            output.Ranks = ObjectMapper.Map<CatUnitDto>(rank);
            output.FormOfWorks = ObjectMapper.Map<CatUnitDto>(Formow);
            output.JobCatUnitName = catjob.DisplayName ;
            output.AddressName = item.WorkAddress + "," + geo.DisplayName + "," + geoProvince.DisplayName;
            if (recruiter.ProvinceId.HasValue)
            {
                output.Recruiter.Province = ObjectMapper.Map<GeoUnitDto>(_georepository.Get(item.Recruiter.ProvinceId.Value));
            }

            return output;
        }

        public async Task<PagedResultDto<RecruitmentDto>> GetAllBy()
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }

                var Cat = _CatUnitrepository.GetAll();
                var query = _recruitmentRepo
                            .GetAll().AsNoTracking().Where(x => x.IsDeleted == false)
							.Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0)
                            .Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0)
                            .Where(x=>x.CreatorUserId == AbpSession.UserId)
                            .Select(x => new RecruitmentDto
                            {
                                Id = x.Id,
                                Title = x.Title,
                                FormOfWork = x.FormOfWork,
                                Experience = x.Experience,
                                NumberOfRecruits = x.NumberOfRecruits,
                                DeadlineSubmission = x.DeadlineSubmission,
                                JobCatUnitId = x.JobCatUnitId,
                                Status = x.Status,
                                RecruiterId = x.RecruiterId,
                                MinSalary = x.MinSalary,
                                MaxSalary = x.MaxSalary,
                            }).ToList();

                var result = from c in query
                             join cat in Cat on c.JobCatUnitId equals cat.Id
                             join recruiter in _recruiterrepository.GetAll() on c.RecruiterId equals recruiter.Id
                             select new RecruitmentDto
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 FormOfWork = c.FormOfWork,
                                 Experience = c.Experience,
                                 NumberOfRecruits = c.NumberOfRecruits,
                                 DeadlineSubmission = c.DeadlineSubmission,
                                 JobCatUnitId = c.JobCatUnitId,
                                 Status = c.Status,
                                 JobCatUnitName = cat.DisplayName,
                                 RecruiterId = c.RecruiterId,
                                 Recruiter = ObjectMapper.Map<RecruiterEditDto>(recruiter),
                                 MinSalary = c.MinSalary,
                                 MaxSalary = c.MaxSalary,

                             };

                var Count = result.Count();
                return new PagedResultDto<RecruitmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }



        public async Task<PagedResultDto<RecruitmentDto>> GetAllByNVNV(RecruimentInput input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }

                var Cat = _CatUnitrepository.GetAll();
                var geo = _georepository.GetAll();

                var listCat = ObjectMapper.Map<List<CatUnitDto>>(Cat);
                var query = _recruitmentRepo
                            .GetAll().AsNoTracking().OrderByDescending(x => x.Id)
                            .Include(x=>x.FormOfWorks)
                            .WhereIf(!string.IsNullOrEmpty(input.Filtered), u => u.Title.ToLower().Contains(input.Filtered.ToLower()))
                            .WhereIf(input.NumberOfRecruits.HasValue, x => x.NumberOfRecruits.Equals(input.NumberOfRecruits))
                            .WhereIf(input.FormOfWork.HasValue && input.FormOfWork != 0, x => x.FormOfWork.Equals(input.FormOfWork.Value))
                            .WhereIf(input.Job.HasValue && input.Job != 0, x => x.JobCatUnitId.Equals(input.Job.Value))
                            .WhereIf(input.Experience.HasValue && input.Experience != 0, x => x.Experience.Equals(input.Experience.Value))
                            .WhereIf(input.RecruiterId.HasValue && input.RecruiterId != 0, x => x.RecruiterId.Equals(input.RecruiterId.Value))
                            .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.DeadlineSubmission >= DateTime.Parse(input.FromDate) && x.DeadlineSubmission <= DateTime.Parse(input.ToDate))
                            .Select(x => new RecruitmentDto
                            {
                                Id = x.Id,
                                Title = x.Title,
                                FormOfWork = x.FormOfWork,
                                Experience = x.Experience,
                                NumberOfRecruits = x.NumberOfRecruits,
                                DeadlineSubmission = x.DeadlineSubmission,
                                JobCatUnitId = x.JobCatUnitId,
                                Status = x.Status,
                                RecruiterId = x.RecruiterId,
                                MaxSalary = x.MaxSalary,
                                MinSalary = x.MinSalary,
                                ProvinceId = x.ProvinceId,
                                FormOfWorks = ObjectMapper.Map<CatUnitDto>(x.FormOfWorks)

                            }).ToList();

                var result = from c in query
                             join cat in listCat on c.JobCatUnitId equals cat.Id
                             join ex in listCat on c.Experience equals ex.Id
                             join recruiter in _recruiterrepository.GetAll() on c.RecruiterId equals recruiter.Id
                             join ge in geo on c.ProvinceId equals ge.Id

                             select new RecruitmentDto
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 FormOfWork = c.FormOfWork,
                                 Experience = c.Experience,
                                 Experiences = ex,
                                 NumberOfRecruits = c.NumberOfRecruits,
                                 DeadlineSubmission = c.DeadlineSubmission,
                                 JobCatUnitId = c.JobCatUnitId,
                                 Status = c.Status,
                                 JobCatUnitName = cat.DisplayName,
                                 MaxSalary = c.MaxSalary,
                                 MinSalary = c.MinSalary,
                                 RecruiterId = c.RecruiterId,
                                 WorkAddress = ge.DisplayName,
                                 FormOfWorks = c.FormOfWorks,
                                 Recruiter = ObjectMapper.Map<RecruiterEditDto>(recruiter),

                             };

                var Count = result.Count();
                return new PagedResultDto<RecruitmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<long> ActiveRecruiment(RecruitmentDto input)
        {
            try
            {
                Recruitment Recruitment = await _recruitmentRepo.FirstOrDefaultAsync(x => x.Id == input.Id);
                Recruitment.Status = input.Status;
                await _recruitmentRepo.UpdateAsync(Recruitment);



                return input.Id.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #region Mobile/Frontend
        public async Task<PagedResultDto<RecruitmentDto>> GetAllUser(RecruimentInput input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                var Cat = _CatUnitrepository.GetAll();
                var geo = _georepository.GetAll();
                var query = _recruitmentRepo
                            .GetAll().AsNoTracking().OrderByDescending(x => x.CreationTime).Include(x => x.Experiences)
                            .WhereIf(!string.IsNullOrEmpty(input.Filtered), u => u.Title.ToLower().Contains(input.Filtered.ToLower()))
                            .WhereIf(input.FormOfWork.HasValue && input.FormOfWork != 0, x => x.FormOfWork.Equals(input.FormOfWork.Value))
                            .WhereIf(input.Job.HasValue && input.Job != 0, x => x.JobCatUnitId.Equals(input.Job.Value))
                            .WhereIf(input.Experience.HasValue && input.Experience != 0, x => x.Experience.Equals(input.Experience.Value))
                            .WhereIf(input.Rank.HasValue && input.Rank != 0, x => x.Rank == input.Rank)
                            .WhereIf(input.Degree.HasValue && input.Degree != 0, x => x.Degree == input.Degree)
                            .WhereIf(input.Salary.HasValue && input.Salary != 0, x => x.MinSalary <= input.Salary && x.MaxSalary >= input.SalaryMax)
                            //.WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.ProvinceId))
                            .WhereIf(input.WorkSiteId.HasValue && input.WorkSiteId != 0, x => input.WorkSiteId.Value == x.ProvinceId)

                            .Select(x => new RecruitmentDto
                            {
                                Id = x.Id,
                                Title = x.Title,
                                FormOfWork = x.FormOfWork,
                                NumberOfRecruits = x.NumberOfRecruits,
                                DeadlineSubmission = x.DeadlineSubmission,
                                JobCatUnitId = x.JobCatUnitId,
                                Status = x.Status,
                                RecruiterId = x.RecruiterId,
                                MaxSalary = x.MaxSalary,
                                MinAge = x.MinAge,
                                MinSalary = x.MinSalary,
                                Experience = x.Experience,
                                Experiences = ObjectMapper.Map<CatUnitDto>(x.Experiences),
                                Degree = x.Degree,
                                Rank = x.Rank,
                                ProvinceId = x.ProvinceId,
                            }).ToList();

                var result = (from c in query
                              join cat in Cat on c.JobCatUnitId equals cat.Id
                              join recruiter in _recruiterrepository.GetAll() on c.RecruiterId equals recruiter.Id
                              join ge in geo on c.ProvinceId equals ge.Id
                              select new RecruitmentDto
                              {
                                  Id = c.Id,
                                  Title = c.Title,
                                  FormOfWork = c.FormOfWork,
                                  Experience = c.Experience,
                                  NumberOfRecruits = c.NumberOfRecruits,
                                  DeadlineSubmission = c.DeadlineSubmission,
                                  JobCatUnitId = c.JobCatUnitId,
                                  Status = c.Status,
                                  JobCatUnitName = cat.DisplayName,
                                  RecruiterId = c.RecruiterId,
                                  Recruiter = ObjectMapper.Map<RecruiterEditDto>(recruiter),
                                  MaxSalary = c.MaxSalary,
                                  MinSalary = c.MinSalary,
                                  MinAge = c.MinAge,
                                  Experiences = c.Experiences,
                                  Degree = c.Degree,
                                  Rank = c.Rank,
                                  WorkAddress = ge.DisplayName
                              }).Where(x => x.Status == false || x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0);

                var Count = result.Count();
                return new PagedResultDto<RecruitmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}
