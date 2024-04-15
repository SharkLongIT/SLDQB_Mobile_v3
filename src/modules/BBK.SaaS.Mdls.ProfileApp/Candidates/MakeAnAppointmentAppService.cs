using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public class MakeAnAppointmentAppService : SaaSAppServiceBase, IMakeAnAppointmentAppService
    {
        private readonly IRepository<MakeAnAppointment, long> _MakeAnAppointmentRepo;
        private readonly IRepository<JobApplication, long> _JobApplicationRepo;
        private readonly IRepository<Recruiter, long> _RecruiterRepo;
        private readonly IRepository<Candidate, long> _CandidateRepo;
        private readonly IRepository<User,long> _UserRepo;
        private readonly IRepository<CatUnit, long> _CatRepo;
        private readonly IRepository<ApplicationRequest, long> _ApplicationRequestRepo;
        private readonly IRepository<Recruitment, long> _recruitmentRepo;
        public MakeAnAppointmentAppService(IRepository<MakeAnAppointment, long> MakeAnAppointmentRepo,
            IRepository<JobApplication, long> JobApplicationRepo,
            IRepository<Recruiter, long> RecruiterRepo,
            IRepository<Candidate, long> CandidateRepo,
            IRepository<User, long> UserRepo,
            IRepository<CatUnit, long> CatRepo,
            IRepository<ApplicationRequest, long> applicationRequestRepo,
            IRepository<Recruitment, long> recruitmentRepo)
        {
            _MakeAnAppointmentRepo = MakeAnAppointmentRepo;
            _JobApplicationRepo = JobApplicationRepo;
            _RecruiterRepo = RecruiterRepo;
            _CandidateRepo = CandidateRepo;
            _UserRepo = UserRepo;
            _CatRepo = CatRepo;
            _ApplicationRequestRepo = applicationRequestRepo;
            _recruitmentRepo = recruitmentRepo;
        }


        public async Task<PagedResultDto<MakeAnAppointmentDto>> GetAll(MakeAnAppointmentInput input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                var Cat = _CatRepo.GetAll();

                var query = _MakeAnAppointmentRepo
                            .GetAll()
                            .AsNoTracking()
                            .Where(x => x.CreatorUserId == AbpSession.UserId)
                            .Select(x => new MakeAnAppointmentDto
                            {
                                Id = x.Id,
                                RecruiterId = x.RecruiterId,
                                CandidateId = x.CandidateId,
                                JobApplicationId = x.JobApplicationId,
                                InterviewTime = x.InterviewTime,
                                TypeInterview = x.TypeInterview,
                                Rank = x.Rank,
                                InterviewResultLetter = x.InterviewResultLetter,
                                StatusOfCandidate = x.StatusOfCandidate,    
                            }).ToList();
                var result = (from q in query
                              join job in _JobApplicationRepo.GetAll() on q.JobApplicationId equals job.Id
                              join re in _RecruiterRepo.GetAll() on q.RecruiterId equals re.Id
                              join ca in _CandidateRepo.GetAll() on q.CandidateId equals ca.Id
                              join user in _UserRepo.GetAll() on ca.UserId equals user.Id
                              join cat in Cat on q.Rank equals cat.Id
                              select new MakeAnAppointmentDto
                              {
                                  Id = q.Id,
                                  RecruiterId = q.RecruiterId,
                                  Recruiter = ObjectMapper.Map<RecruiterEditDto>(re),
                                  CandidateId = q.CandidateId,
                                  Candidate = ObjectMapper.Map<CandidateEditDto>(ca),
                                  JobApplicationId = q.JobApplicationId,
                                  JobApplication = ObjectMapper.Map<JobApplicationEditDto>(job),
                                  InterviewTime = q.InterviewTime,
                                  TypeInterview = q.TypeInterview,
                                  Rank = q.Rank,
                                  Ranks = ObjectMapper.Map<CatUnitDto>(cat),
                                  Name = user.Name,
                                  InterviewResultLetter = q.InterviewResultLetter,
                                  StatusOfCandidate = q.StatusOfCandidate,
                              }).WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Name.ToLower().Contains(input.Search.ToLower()))
                              .WhereIf(input.Rank != 0, x => x.Rank == input.Rank)
                              .WhereIf(input.Experience != 0, x => x.JobApplication.ExperiencesId == input.Experience)
                              .WhereIf(input.InterviewResultLetter != 0, x => x.InterviewResultLetter == input.InterviewResultLetter)
                              .WhereIf(input.InterviewTime.HasValue, x => x.InterviewTime.ToShortDateString() == input.InterviewTime.Value.ToShortDateString()).OrderByDescending(x=>x.Id).ToList();

                var Count = result.Count();
                return new PagedResultDto<MakeAnAppointmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task<long> Create(MakeAnAppointmentDto input)
        { 
            try
            {
                MakeAnAppointment newItemId = new MakeAnAppointment();
                newItemId.TenantId = AbpSession.TenantId.Value;
                newItemId.Address = input.Address;
                newItemId.InterviewTime = input.InterviewTime;
                newItemId.CandidateId = input.CandidateId;
                newItemId.TypeInterview = input.TypeInterview;
                newItemId.ApplicationRequestId = input.ApplicationRequestId;
                newItemId.RecruiterId = input.RecruiterId;
                newItemId.StatusOfCandidate = 1;
                newItemId.Rank = input.Rank;
                newItemId.Message = input.Message;
                newItemId.ReasonForRefusal = string.Empty;

                newItemId.JobApplicationId = input.JobApplicationId;
                var newId = await _MakeAnAppointmentRepo.InsertAndGetIdAsync(newItemId);
                var application = _ApplicationRequestRepo.GetAll().Where(x => x.JobApplicationId == newItemId.JobApplicationId).FirstOrDefault();

                if (application != null)
                {
                    application.Status = 2;
                    await _ApplicationRequestRepo.UpdateAsync(application);
                }

                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task<MakeAnAppointmentDto> Update(MakeAnAppointmentDto input)
        {
            try
            {
                var output = new MakeAnAppointmentDto();
                if (input.Id.HasValue)
                {
                    MakeAnAppointment MakeAnAppointment = await _MakeAnAppointmentRepo.GetAsync(input.Id.Value);
                    if (MakeAnAppointment != null)
                    {
                        var application = _ApplicationRequestRepo.GetAll().Where(x => x.JobApplicationId == MakeAnAppointment.JobApplicationId).FirstOrDefault();

                        input.TenantId = MakeAnAppointment.TenantId;
                        MakeAnAppointment.InterviewResultLetter = input.InterviewResultLetter;
                        ObjectMapper.Map(input, MakeAnAppointment);
                        if (MakeAnAppointment != null)
                        {
                            MakeAnAppointment = await _MakeAnAppointmentRepo.UpdateAsync(MakeAnAppointment);
                        }
                        //output = ObjectMapper.Map<CandidateEditDto>(candidate);
                        //

                        if(application != null)
                        {
                            application.Status = 2;
                            await _ApplicationRequestRepo.UpdateAsync(application);
                        }
                      
                    }
                }
                return input;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<MakeAnAppointmentDto> GetDetail(long Id)
        {
            if (AbpSession.TenantId.HasValue == false)
            {
                var unit = UnitOfWorkManager.Current;
                unit.SetTenantId(1);
            }
            var output = new MakeAnAppointmentDto();
            var recruiter = _MakeAnAppointmentRepo.Get(Id);

            if (recruiter != null) { 
                recruiter.Recruitment = _recruitmentRepo.Get(recruiter.ApplicationRequestId);
                recruiter.Ranks = _CatRepo.Get(recruiter.Rank);
            }
            output = ObjectMapper.Map<MakeAnAppointmentDto>(recruiter);
            return output;

        }


        /// <summary>
        /// Lấy tất cả lịch phỏng vấn của người lao động
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<PagedResultDto<MakeAnAppointmentDto>> GetAllOfCandidate(MakeAnAppointmentInput input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                var Cat = _CatRepo.GetAll();

                var query = _MakeAnAppointmentRepo
                            .GetAll()
                            .Include(x=>x.Candidate)
                            .Include(x=>x.Recruitment)
                            .Include(x=>x.Recruiter)
                            .Where(x=>x.Candidate.UserId == AbpSession.UserId)
                            .AsNoTracking()
                            .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Recruiter.CompanyName.ToLower().Contains(input.Search.ToLower()) || x.Recruitment.Title.ToLower().Contains(input.Search.ToLower()))
                            //.WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Recruitment.Title.ToLower().Contains(input.Search.ToLower()))
                            .WhereIf(input.Rank != 0 , x => x.Rank == input.Rank)
                            .WhereIf(input.InterviewTime.HasValue , x => x.InterviewTime.Date == input.InterviewTime.Value.Date)
                            .WhereIf(input.StatusOfCandidate.HasValue, x => x.StatusOfCandidate == input.StatusOfCandidate.Value)
                            .ToList();
                var result = (from q in query
                              join job in _JobApplicationRepo.GetAll() on q.JobApplicationId equals job.Id
                              join re in _RecruiterRepo.GetAll() on q.RecruiterId equals re.Id
                              join ca in _CandidateRepo.GetAll() on q.CandidateId equals ca.Id
                              join user in _UserRepo.GetAll() on ca.UserId equals user.Id
                              join cat in Cat on q.Rank equals cat.Id
                              select new MakeAnAppointmentDto
                              {
                                  Id = q.Id,
                                  RecruiterId = q.RecruiterId,
                                  Recruiter = ObjectMapper.Map<RecruiterEditDto>(re),
                                  CandidateId = q.CandidateId,
                                  Candidate = ObjectMapper.Map<CandidateEditDto>(ca),
                                  JobApplicationId = q.JobApplicationId,
                                  JobApplication = ObjectMapper.Map<JobApplicationEditDto>(job),
                                  InterviewTime = q.InterviewTime,
                                  TypeInterview = q.TypeInterview,
                                  Rank = q.Rank,
                                  Ranks = ObjectMapper.Map<CatUnitDto>(cat),
                                  Name = user.FullName,
                                  InterviewResultLetter = q.InterviewResultLetter,
                                  Recruitment = ObjectMapper.Map<RecruitmentDto>(q.Recruitment),
                                  StatusOfCandidate = q.StatusOfCandidate,
                                  ReasonForRefusal = q.ReasonForRefusal,    

                              }).OrderByDescending(x=>x.Id).ToList();
                            

                var Count = result.Count();
                return new PagedResultDto<MakeAnAppointmentDto>(
                    Count,
                    result.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<MakeAnAppointmentDto> UpdateForCandidate(MakeAnAppointmentDto input)
        {
            try
            {
                var output = new MakeAnAppointmentDto();
                if (input.Id.HasValue)
                {
                    MakeAnAppointment MakeAnAppointment = await _MakeAnAppointmentRepo.GetAsync(input.Id.Value);
                    if (MakeAnAppointment != null)
                    {
                        MakeAnAppointment.StatusOfCandidate = input.StatusOfCandidate;
                        MakeAnAppointment.ReasonForRefusal = input.ReasonForRefusal;
                        if (MakeAnAppointment != null)
                        {
                            MakeAnAppointment = await _MakeAnAppointmentRepo.UpdateAsync(MakeAnAppointment);
                        }
                    }
                }
                else {
                    return output;
                        }
                return input;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }



        public async Task<MakeAnAppointmentForUpdateMobile> UpdateAppForMobile(MakeAnAppointmentForUpdateMobile input)
        {
            try
            {
                var output = new MakeAnAppointmentDto();
                if (input.Id.HasValue)
                {
                    MakeAnAppointment MakeAnAppointment = await _MakeAnAppointmentRepo.GetAsync(input.Id.Value);
                    if (MakeAnAppointment != null)
                    {
                        var application = _ApplicationRequestRepo.GetAll().Where(x => x.JobApplicationId == MakeAnAppointment.JobApplicationId).FirstOrDefault();

                        //input.TenantId = MakeAnAppointment.TenantId;
                        MakeAnAppointment.InterviewResultLetter = input.InterviewResultLetter;
 
                        if (application != null)
                        {
                            application.Status = 2;
                        }

                    }
                }
                return input;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}
