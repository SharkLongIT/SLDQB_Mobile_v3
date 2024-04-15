using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.TradingSessions
{
    public class TradingSessionAccountAppService : SaaSAppServiceBase, ITradingSessionAccountAppService
    {
        private readonly IRepository<TradingSessionAccount, long> _TradingSessionAccountService;
        private readonly IRepository<TradingSession, long> _tradingSessionService;
        private readonly IRepository<Recruiter, long> _RecruiterRes;
        private readonly IRepository<Candidate, long> _CandidateRes;
        private readonly IRepository<JobApplication, long> _JobApplicationRes;
        private readonly IRepository<Recruitment, long> _RecruitmentRes;
        private readonly IRepository<CatUnit, long> _Cat;

        public TradingSessionAccountAppService(IRepository<TradingSessionAccount, long> TradingSessionAccountService, 
            IRepository<TradingSession, long> tradingSessionService, IRepository<Recruiter, long> recruiterRes,
            IRepository<Candidate, long> candidateRes, IRepository<JobApplication, long> jobApplicationRes, 
            IRepository<Recruitment, long> RecruitmentRes, IRepository<CatUnit, long> Cat)
        {
            _TradingSessionAccountService = TradingSessionAccountService;
            _tradingSessionService = tradingSessionService;
            _RecruiterRes = recruiterRes;
            _CandidateRes = candidateRes;
            _JobApplicationRes = jobApplicationRes;
            _RecruitmentRes = RecruitmentRes;
            _Cat = Cat;
        }



        #region get tất cả

        public async Task<PagedResultDto<TradingSessionEditDto>> GetAll(TradingSessionSearch input)
        {
            var TradingList = _tradingSessionService.GetAll().Include(e => e.Province).AsNoTracking().ToList();
            var tradingAccount = _TradingSessionAccountService.GetAll().ToList();


            var query = (from t in TradingList
                         join ta in tradingAccount on t.Id equals ta.TradingSessionId
                         where ta.UsertId == AbpSession.UserId
                         select new TradingSessionEditDto
                         {
                             Id = t.Id,
                             NameTrading = t.NameTrading,
                             ProvinceId = t.ProvinceId,
                             DistrictId = t.DistrictId,
                             VillageId = t.VillageId,
                             Province = ObjectMapper.Map<GeoUnitDto>(t.Province),
                             StartTime = t.StartTime,
                             EndTime = t.EndTime,
                             Address = t.Address,
                             Description = t.Description,
                             Status = ta.Status,
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.NameTrading.ToLower().Contains(input.Search.ToLower()))
                         .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.StartTime >= DateTime.Parse(input.FromDate) && x.EndTime <= DateTime.Parse(input.ToDate))
                         .WhereIf(input.WorkSite != null && input.WorkSite.Count != 0, x => input.WorkSite.Contains(x.ProvinceId)).ToList();


            return new PagedResultDto<TradingSessionEditDto>(
                     query.Count(),
                     query.ToList()
                     );
        }
        #endregion

        #region get all nha tuyen dung
        public async Task<PagedResultDto<TradingSessionAccountEditDto>> GetAllRecuiter(TradingSessionAccountSeach input)
        {
            var TradingAccountList = _TradingSessionAccountService.GetAll().OrderByDescending(x => x.CreationTime).Include(x => x.TradingSession)
                .Include(x => x.Recruiter)
                .Include(x => x.Recruiter.HumanResSizeCat)
                .Include(x => x.Recruiter.Province)
                .Include(x => x.Recruiter.SphereOfActivity)
                .AsNoTracking()
                .Where(x=>x.Status == 1)
                .Where(x => x.RecruiterId.HasValue && x.TradingSessionId == input.Id)
                .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.Recruiter.CompanyName.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(input.WorkSite != null && input.WorkSite.Count != 0, x => input.WorkSite.Contains(x.Recruiter.ProvinceId.Value))
                .Select(x => new TradingSessionAccountEditDto
                {
                    Id = x.Id,
                    UsertId = x.UsertId,
                    RecruiterId = x.RecruiterId,
                    TradingSessionId = x.TradingSessionId,
                    Status = x.Status,
                    Recruiter = ObjectMapper.Map<RecruiterEditDto>(x.Recruiter),
                })
               .ToList();


            return new PagedResultDto<TradingSessionAccountEditDto>(
                     TradingAccountList.Count(),
                     TradingAccountList.ToList()
                     );
        }
        #endregion

        #region get all nguoi lao dong
        public async Task<PagedResultDto<TradingSessionAccountEditDto>> GetAllCandidate(TradingSessionAccountSeach input)
        {
            //var job = _JobApplicationRes.GetAll().Include(x => x.Positions).Include(x => x.Province).Include(x => x.Occupations).Include(x=>x.Experiences).Include(x=>x.Literacy);
            var TradingAccountList = _TradingSessionAccountService.GetAll().OrderByDescending(x => x.CreationTime)
                .Include(x => x.TradingSession)
                .Include(x => x.Candidate)
                .Include(x => x.Candidate.Account)
                .Include(x=>x.JobApplication)
                .Include(x=>x.JobApplication.Experiences)
                .Include(x=>x.JobApplication.Occupations)
                .Include(x=>x.JobApplication.Positions)
                .Include(x=>x.JobApplication.Literacy)
                .AsNoTracking()
                .Where(x => x.Status == 1)
                .Where(x => x.JobApplicationId.HasValue && x.TradingSessionId == input.Id)
               
                .Select(x => new TradingSessionAccountEditDto
                {
                    Id = x.Id,
                    UsertId = x.UsertId,
                    CandidateId = x.CandidateId,
                    TradingSessionId = x.TradingSessionId,
                    Status = x.Status,
                    Candidate = ObjectMapper.Map<CandidateEditDto>(x.Candidate),
                    JobApplicationId = x.JobApplicationId,
                    JobApplication = ObjectMapper.Map<JobApplicationEditDto>(x.JobApplication),
                    ProfilePictureId = x.Candidate.Account.ProfilePictureId,
                    WorkSite = x.JobApplication.Province.DisplayName,
                    WorkSiteId = x.JobApplication.Province.Id,
                })
                 .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.Candidate.Account.Name.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(input.WorkSite != null && input.WorkSite.Count != 0, x => input.WorkSite.Contains(x.WorkSiteId))
               .ToList();
            //var query = (from t in TradingAccountList
            //             join j in job on t.CandidateId equals j.CandidateId
            //             where j.IsPublished == true
            //             select new TradingSessionAccountEditDto
            //             {
            //                 Id = t.Id,
            //                 UsertId = t.UsertId,
            //                 CandidateId = t.CandidateId,
            //                 TradingSessionId = t.TradingSessionId,
            //                 Status = t.Status,
            //                 Candidate = ObjectMapper.Map<CandidateEditDto>(t.Candidate),
            //                 Positions = j.Positions.DisplayName,
            //                 WorkSite = j.Province.DisplayName,
            //                 Occupations = j.Occupations.DisplayName,
            //                 WorkSiteId = j.Province.Id,
            //                 JobApplicationId = j.Id,
            //                 ProfilePictureId = t.ProfilePictureId,
            //                 Experiences = j.Experiences.DisplayName,
            //                 DesiredSalary = j.DesiredSalary,
            //                 Literacy = j.Literacy.DisplayName
            //             })
            //             .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.Candidate.Account.Name.ToLower().Contains(input.Search.ToLower()))
            //             .WhereIf(input.WorkSite != null && input.WorkSite.Count != 0, x => input.WorkSite.Contains(x.WorkSiteId))
            //              .ToList();

            return new PagedResultDto<TradingSessionAccountEditDto>(
                     TradingAccountList.Count(),
                     TradingAccountList.ToList()
                     );
        }
        #endregion


        //Tạo mới
        public async Task<long> Create(TradingSessionAccountEditDto input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                else
                {
                    input.TenantId = AbpSession.TenantId.Value;
                }
                TradingSessionAccount newItemId = new TradingSessionAccount();
                newItemId.TenantId = input.TenantId;
                newItemId.UsertId = input.UsertId;
                newItemId.CandidateId = input.CandidateId;
                newItemId.JobApplicationId = input.JobApplicationId;
                newItemId.RecruiterId = input.RecruiterId;
                newItemId.TradingSessionId = input.TradingSessionId;
                newItemId.Status = input.Status;
                var newId = await _TradingSessionAccountService.InsertAndGetIdAsync(newItemId);
                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }




        /// <summary>
        /// Check ntd/nld có trong bảng Tradding Account hay chưa
        /// </summary>
        /// <param name="TradingSessionId"></param>
        /// <returns></returns>
        public int CheckAccount(long? TradingSessionId)
        {
            var tradingAccount = _TradingSessionAccountService.GetAll().ToList();
            if (tradingAccount.Where(x=>x.UsertId == AbpSession.UserId && x.TradingSessionId == TradingSessionId).Count() == 0)
            {
                return 0;
            }
            else if (tradingAccount.Where(x => x.Status == 0 && x.UsertId == AbpSession.UserId && x.TradingSessionId == TradingSessionId).Count() > 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }


        // get ntd chua tham gia
        public async Task<PagedResultDto<RecruiterEditDto>> GetAllRecruiterNot(TradingSessionAccountSeach input)
        {
            var tradingAccount = _TradingSessionAccountService.GetAll().Where(x => x.TradingSessionId == input.Id).Select(x => x.RecruiterId).ToList();
            var recruiter = _RecruiterRes.GetAll()
                .Include(x=>x.Province)
                .Include(x=>x.SphereOfActivity)
                .Include(x=>x.HumanResSizeCat)
                .Where(x => !tradingAccount.Contains(x.Id))
                .Select(x => new RecruiterEditDto
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    UserId = x.UserId,
                    DateOfEstablishment = x.DateOfEstablishment,
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    HumanResSizeCat = ObjectMapper.Map<CatUnitDto>(x.HumanResSizeCat),
                    SphereOfActivity = ObjectMapper.Map<CatUnitDto>(x.SphereOfActivity),
                    
                }).ToList();
            return new PagedResultDto<RecruiterEditDto>(
                     recruiter.Count(),
                     recruiter.ToList()
                     );
        }


        // get nld chua tham gia
        public async Task<PagedResultDto<JobApplicationEditDto>> GetAllCandidateNot(TradingSessionAccountSeach input)
        {
            var tradingAccount = _TradingSessionAccountService.GetAll().Where(x => x.TradingSessionId == input.Id).Select(x => x.CandidateId).ToList();
            var recruiter = _JobApplicationRes.GetAll()
                .Include(x => x.Province)
                .Include(x => x.Candidate)
                .Include(x => x.Candidate.Province)
                .Include(x => x.Candidate.Account)
                .Where(x=>x.IsPublished == true)
                .Where(x => !tradingAccount.Contains(x.CandidateId))
                .Select(x => new JobApplicationEditDto
                {
                    Id = x.Id,
                    Candidate = ObjectMapper.Map<CandidateEditDto>(x.Candidate),
                    Experiences = ObjectMapper.Map<CatUnitDto>(x.Experiences),
                    Occupations = ObjectMapper.Map<CatUnitDto>(x.Occupations),
                    Literacy = ObjectMapper.Map<CatUnitDto>(x.Literacy),

                }).ToList();
            return new PagedResultDto<JobApplicationEditDto>(
                     recruiter.Count(),
                     recruiter.ToList()
                     );
        }


        #region lấy phiên giao dịch theo Id người tham gia
        public async Task<PagedResultDto<TradingSessionAccountEditDto>> GetAllByUserId(TradingSessionSearch input)
        {
            try
            {
                PagedResultDto<TradingSessionAccountEditDto> output = new PagedResultDto<TradingSessionAccountEditDto>();
                var TradingList = _TradingSessionAccountService.GetAll().OrderByDescending(x => x.CreationTime)
               .AsNoTracking()
               .Include(x => x.TradingSession)
               .Include(x => x.TradingSession.Province)
               .Where(x => x.UsertId == AbpSession.UserId.Value)
               .WhereIf(input.Status.HasValue, u => u.Status == input.Status.Value)
               .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.TradingSession.NameTrading.ToLower().Contains(input.Search.ToLower()))
               .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.TradingSession.StartTime.Date >= DateTime.Parse(input.FromDate).Date)
               .WhereIf(!string.IsNullOrEmpty(input.ToDate),x => x.TradingSession.EndTime.Date <= DateTime.Parse(input.ToDate).Date)
               .WhereIf(input.WorkSite != null && input.WorkSite.Count != 0, x => input.WorkSite.Contains(x.TradingSession.ProvinceId))
               .Select(x => new TradingSessionAccountEditDto
               {
                   Id = x.Id,
                   TradingSession = new TradingSessionEditDto() {
                    Address = x.TradingSession.Address,
                    CountCandidateMax = x.TradingSession.CountCandidateMax,
                    CountRecruiterMax = x.TradingSession.CountRecruiterMax,
                    Describe = x.TradingSession.Describe,
                    Description = x.TradingSession.Description, 
                   // District = ObjectMapper.Map<GeoUnitDto>(x.TradingSession.District),
                    Province = ObjectMapper.Map<GeoUnitDto>(x.TradingSession.Province),
                    //Village = ObjectMapper.Map<GeoUnitDto>(x.TradingSession.Village),
                    ImgUrl = x.TradingSession.ImgUrl,
                    NameTrading = x.TradingSession.NameTrading,
                    StartTime = x.TradingSession.StartTime, 
                    EndTime = x.TradingSession.EndTime, 
                    TenantId = x.TradingSession.TenantId,   
                    Id = x.TradingSession.Id,   
                   }, /*ObjectMapper.Map<TradingSessionEditDto>(x.TradingSession),*/
                   Status = x.Status,
                   JoiningDate = x.CreationTime
               })
              
              .ToList();

                #region Trạng thái phiên dao dịch sắp diễn ra, đang diễn ra, đã diễn ra
                List<TradingSessionAccountEditDto> tradingSessions = new List<TradingSessionAccountEditDto>();
                if (TradingList != null && TradingList.Count > 0)
                {
                    
                    foreach (var item in TradingList)
                    {

                        if (item.TradingSession.StartTime > DateTime.Now)
                        {
                            item.StatusOfTradingSession = 1; // sắp diễn ra  
                            if (input.StatusOfTradingSession.HasValue)
                            {
                                if (input.StatusOfTradingSession == 1)
                                {
                                    tradingSessions.Add(item);
                                }
                            }
                            else
                            {
                                tradingSessions.Add(item);
                            }
                          
                        }
                        if (item.TradingSession.StartTime <= DateTime.Now && item.TradingSession.EndTime >= DateTime.Now)
                        {
                            item.StatusOfTradingSession = 2; // Đang diễn ra
                            if (input.StatusOfTradingSession.HasValue)
                            {
                                if (input.StatusOfTradingSession == 2)
                                {
                                    tradingSessions.Add(item);
                                }
                            }
                            else
                            {
                                tradingSessions.Add(item);
                            }
                        }
                        if (item.TradingSession.EndTime < DateTime.Now)
                        {
                            item.StatusOfTradingSession = 3; // Đã diễn ra  
                            if (input.StatusOfTradingSession.HasValue)
                            {
                                if (input.StatusOfTradingSession == 3)
                                {
                                    tradingSessions.Add(item);
                                }
                            }
                            else
                            {
                                tradingSessions.Add(item);
                            }
                        }
                    }
                }
                #endregion
                if (tradingSessions.Count() > 0)
                {
                    output.TotalCount= tradingSessions.Count();
                    output.Items = tradingSessions.OrderBy(x=>x.StatusOfTradingSession).ToList();
                }else {
                
                    return output;

                }
                return output;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
           


         
        }



        #endregion

        #region Cập nhật trạng thái từ chối, xác nhận tham gia phiên giao dịch

        public async Task UpdateStatus(TradingSessionAccountEditDto input)
        {
            try
            {
                if(input.Id.HasValue)
                {
                    var tradingSessionAccount =  await _TradingSessionAccountService.GetAsync(input.Id.Value);
                    if(tradingSessionAccount != null) {
                        tradingSessionAccount.Status = input.Status;
                    }
                }

            }
            catch (Exception ex)
            {

                throw new  UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Cập nhật trạng thái từ chối, xác nhận tham gia phiên giao dịch

        public async Task UpdateStatusByTradingId(TradingSessionAccountEditDto input)
        {
            try
            {
                if (input.TradingSessionId != 0)
                {
                    var tradingSessionAccount =   _TradingSessionAccountService.FirstOrDefault(x=>x.TradingSessionId == input.TradingSessionId && x.UsertId == AbpSession.UserId);
                    if (tradingSessionAccount != null)
                    {
                        tradingSessionAccount.Status = input.Status;
                        await _TradingSessionAccountService.UpdateAsync(tradingSessionAccount);
                    }
                }

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion



        #region get bieu do
        public async Task<ReportArray> GetByChart(long Id)
        {
            var RecruitmentJob = _RecruitmentRes.GetAll()
                .Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0)
                .Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Select(x=>x.JobCatUnitId).Distinct().ToList();


            var RecruiterList = _TradingSessionAccountService.GetAll().AsNoTracking().Where(x => x.TradingSessionId == Id && x.Status == 1).Select(x=>x.RecruiterId).ToList();
            var JobApplicationList = _TradingSessionAccountService.GetAll().AsNoTracking().Where(x => x.TradingSessionId == Id && x.Status == 1).Select(x=>x.JobApplicationId).ToList();

            var RecruitmentRes = _RecruitmentRes.GetAll().Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0)
                .Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Where(x => RecruiterList.Contains(x.RecruiterId)).ToList();
            var JobApplication = _JobApplicationRes.GetAll().Where(x => JobApplicationList.Contains(x.Id)).ToList();



            ReportArray reportArray = new ReportArray();

            foreach (var jobid in RecruitmentJob)
            {
                ReportList reportList = new ReportList();

                var cat = _Cat.Get(jobid);
                reportList.NameRecruitment = cat.DisplayName;

                foreach (var job in JobApplication)
                {
                    if(jobid == job.OccupationId)
                    {
                        reportList.CountJob++;
                    }
                }

                foreach (var rec in RecruitmentRes)
                {
                    if(jobid == rec.JobCatUnitId)
                    {
                        reportList.CountRecruiment++;
                    }
                }

                reportArray.ListReport.Add(reportList);

            }

            return reportArray;



          
        }
        #endregion

    }
}
