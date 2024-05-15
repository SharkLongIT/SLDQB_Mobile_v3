using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Authorization.Users.Profile;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public class JobApplicationAppService : SaaSAppServiceBase, IJobApplicationAppService
    {
        private readonly IRepository<JobApplication, long> _jobApplicationRepo;
        private readonly IRepository<WorkExperience, long> _workExperienceRepo;
        private readonly IRepository<LearningProcess, long> _learningProcessRepo;
        private readonly IRepository<Candidate, long> _candidateRepo;
        private readonly IProfileAppService _profileAppService;

        public JobApplicationAppService(IRepository<JobApplication, long> jobApplication,
            IRepository<WorkExperience, long> workExperience,
            IRepository<LearningProcess, long> learningProcess,
            IRepository<Candidate, long> candidate,
            IProfileAppService profileAppService)
        {
            _jobApplicationRepo = jobApplication;
            _workExperienceRepo = workExperience;
            _learningProcessRepo = learningProcess;
            _candidateRepo = candidate;
            _profileAppService = profileAppService;

        }

        public async Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobApps(JobAppSearch input)
        {
            try
            {
                List<Candidate> candidates = new List<Candidate>();


                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                    candidates = await _candidateRepo.GetAll()
                        .WhereIf(input.Gender.HasValue, x => (int)x.Gender == input.Gender.Value)
                        .Include(e => e.Account).ToListAsync();
                }
                else
                {
                    candidates = await _candidateRepo.GetAll()
                        .WhereIf(input.Gender.HasValue, x => (int)x.Gender == input.Gender.Value)
                        .Include(e => e.Account).ToListAsync();

                };
                var output = new PagedResultDto<GetJobApplicationForEditOutput> { Items = new List<GetJobApplicationForEditOutput>(), TotalCount = 0 };

                if (candidates == null || candidates.Count() <= 0)
                {
                    return output;
                }
                List<UserEditDto> users = new List<UserEditDto>();
                foreach (var item in candidates)
                {
                    var user = ObjectMapper.Map<UserEditDto>(item.Account);
                    users.Add(user);
                }


                var listCadidates = ObjectMapper.Map<List<CandidateEditDto>>(candidates);

                var queryJobApps = _jobApplicationRepo.GetAll()
                    .Include(e => e.Province)
                    .Include(e => e.Literacy)
                    .Include(e => e.Experiences)
                    .Include(e => e.FormOfWork)
                    .Include(e => e.Positions)
                    .Include(e => e.Candidate)
                    .Include(e => e.Candidate.Account)
                    .AsNoTracking()
                    .Where(x => x.IsPublished == true)
                    .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.WorkSite))
                    .WhereIf(input.ExperiencesId.HasValue, x => x.ExperiencesId.Equals(input.ExperiencesId.Value))
                    .WhereIf(input.OccupationId.HasValue, x => x.OccupationId.Equals(input.OccupationId.Value))
                    .WhereIf(input.Gender.HasValue, x => (int)x.Candidate.Gender == input.Gender.Value)
                    .WhereIf(input.FormOfWorkId != null && input.FormOfWorkId.Count > 0, x => input.FormOfWorkId.Contains(x.FormOfWorkId))
                    .WhereIf(input.SalaryMax.HasValue, x => input.SalaryMax >= x.DesiredSalary)
                    .WhereIf(input.SalaryMin.HasValue, x => input.SalaryMin <= x.DesiredSalary)
                    .WhereIf(input.LiteracyId.HasValue, x => x.LiteracyId.Equals(input.LiteracyId.Value)).OrderByDescending(x => x.CreationTime);

                if (queryJobApps == null || queryJobApps.Count() <= 0)
                {
                    return output;
                }
                //var listJobApps = ObjectMapper.Map<List<JobApplicationEditDto>>(queryJobApps);
                if (input.Take == 0) input.Take = 10;
                List<GetJobApplicationForEditOutput> queryCommon = new List<GetJobApplicationForEditOutput>();

                foreach (var item in queryJobApps.Skip(input.SkipCount * (input.Paging - 1)).Take(input.Take).ToList())
                {

                    queryCommon.Add(new GetJobApplicationForEditOutput
                    {
                        ProfilePictureId = item.Candidate.Account.ProfilePictureId,
                        Candidate = ObjectMapper.Map<CandidateEditDto>(item.Candidate),
                        JobApplication = ObjectMapper.Map<JobApplicationEditDto>(item),
                        User = ObjectMapper.Map<UserEditDto>(item.Candidate.Account)

                    });


                }

                //var queryCommon = from JobApp in listJobApps
                //                  join candidate in listCadidates on JobApp.CandidateId equals candidate.Id
                //                  join user in users on candidate.UserId equals user.Id
                //                  select (new GetJobApplicationForEditOutput
                //                  {
                //                      User = user,
                //                      Candidate = candidate,
                //                      JobApplication = JobApp,
                //                  });
                if (queryCommon.Count() <= 0)
                { return output; }

                output.Items = queryCommon.ToList();

                output.TotalCount = queryJobApps.Count();

                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<JobApplicationEditDto> CreateJobApplication(JobApplicationEditDto input)
        {
            try
            {
                JobApplication jobApplication = new JobApplication();
                #region Cập nhật làm hồ sơ chính 
                if (input.IsPublished == true)
                {
                    //var ListJobApp = _jobApplicationRepo.GetAll()
                    //                                     .Where(x => x.CandidateId == input.CandidateId).ToList();
                    //foreach (var item in ListJobApp)
                    //{
                    //    item.IsPublished = false;
                    //    UpdatePushlish(item);
                    //}

                    var jobAppPublished = await _jobApplicationRepo.FirstOrDefaultAsync(x => x.CandidateId == input.CandidateId && x.IsPublished == true);
                    if (jobAppPublished != null)
                        jobAppPublished.IsPublished = false;

                    //await UnitOfWorkManager.Current.SaveChangesAsync();
                }
                #endregion

                #region Encrypt File 
                if(input.FileCVUrl != null)
                {
                    input.FileCVUrl = StringCipher.Instance.Decrypt(input.FileCVUrl);   
                }
                #endregion
                jobApplication = ObjectMapper.Map<JobApplication>(input);
                jobApplication.Id = await _jobApplicationRepo.InsertAndGetIdAsync(jobApplication);

                jobApplication = _jobApplicationRepo.Get(jobApplication.Id);

                var output = ObjectMapper.Map<JobApplicationEditDto>(jobApplication);
                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<LearningProcessEditDto> CreateLearningProcess(LearningProcessEditDto input)
        {
            try
            {
                LearningProcess learningProcess = new LearningProcess();
                learningProcess = ObjectMapper.Map<LearningProcess>(input);
                learningProcess.Id = await _learningProcessRepo.InsertAndGetIdAsync(learningProcess);

                learningProcess = _learningProcessRepo.Get(learningProcess.Id);
                var output = ObjectMapper.Map<LearningProcessEditDto>(learningProcess);
                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<List<WorkExperienceEditDto>> CreateWorkExperience(WorkExperienceEditDto input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue)
                {
                    input.TenantId = AbpSession.TenantId.Value;
                }
                WorkExperience workExperience = new WorkExperience();
                workExperience = ObjectMapper.Map<WorkExperience>(input);

                workExperience.Id = await _workExperienceRepo.InsertAndGetIdAsync(workExperience);
                List<WorkExperience> multiWorkExperience = new List<WorkExperience>();
                multiWorkExperience = await _workExperienceRepo.GetAll().Where(x => x.JobApplicationId == input.JobApplicationId).ToListAsync();

                var output = ObjectMapper.Map<List<WorkExperienceEditDto>>(multiWorkExperience);
                return output;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<JobApplicationEditDto> DeleteJobApplication(NullableIdDto<long> input)
        {
            try
            {
                JobApplication jobApplication = new JobApplication();
                if (input.Id.HasValue)
                {
                    jobApplication = _jobApplicationRepo.Get(input.Id.Value);
                    await _jobApplicationRepo.DeleteAsync(jobApplication);
                }
                var ouput = ObjectMapper.Map<JobApplicationEditDto>(jobApplication);
                return ouput;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<LearningProcessEditDto> DeleteLearningProcess(LearningProcessEditDto input)
        {
            try
            {
                if (input.Id.HasValue)
                {
                    var LearningProcess = _learningProcessRepo.Get(input.Id.Value);
                    await _learningProcessRepo.DeleteAsync(LearningProcess);
                }
                return input;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<WorkExperienceEditDto> DeleteWorkExperience(WorkExperienceEditDto input)
        {
            try
            {
                if (input.Id.HasValue)
                {
                    var WorkExperience = _workExperienceRepo.Get(input.Id.Value);
                    await _workExperienceRepo.DeleteAsync(WorkExperience);
                }
                return input;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<GetJobApplicationForEditOutput> GetJobApplicationForEdit(NullableIdDto<long> input)
        {
            var unit = UnitOfWorkManager.Current;
            if (unit.GetTenantId() == null)
            {
                unit.SetTenantId(1);
            }
            var output = new GetJobApplicationForEditOutput { JobApplication = new JobApplicationEditDto(), User = new UserEditDto(), Candidate = new CandidateEditDto() };
            if (input.Id.HasValue)
            {
                //var jobApplication = await _jobApplicationRepo.GetAsync(input.Id.Value);
                var jobApplication = await _jobApplicationRepo.GetAll()
                         .Where(x => x.Id == input.Id.Value)
                         .Include(x => x.Province)
                         .Include(x => x.Positions)
                         .Include(x => x.Literacy)
                         .Include(x => x.FormOfWork)
                         .Include(x => x.Occupations)
                         .Include(x => x.Experiences)
                         .FirstOrDefaultAsync();

                if (jobApplication != null)
                {
                    output.JobApplication = ObjectMapper.Map<JobApplicationEditDto>(jobApplication);
                    if (output.JobApplication != null)
                    {
                        if (!string.IsNullOrEmpty(jobApplication.FileCVUrl))
                            output.JobApplication.FileMgr = new FileMgr(jobApplication.FileCVUrl);

                        List<WorkExperience> workExperiences = new List<WorkExperience>();
                        workExperiences = await _workExperienceRepo.GetAll().Where(x => x.JobApplicationId == output.JobApplication.Id).ToListAsync();
                        if (workExperiences != null)
                        {
                            output.JobApplication.WorkExperiences = ObjectMapper.Map<List<WorkExperienceEditDto>>(workExperiences);
                        }
                        List<LearningProcess> learningProcess = new List<LearningProcess>();
                        learningProcess = await _learningProcessRepo.GetAll().Where(x => x.JobApplicationId == output.JobApplication.Id).ToListAsync();
                        output.JobApplication.LearningProcess = ObjectMapper.Map<List<LearningProcessEditDto>>(learningProcess);
                        #region Candidate

                        Candidate candidate = new Candidate();
                        candidate = await _candidateRepo.GetAll()
                            .Include(e => e.Account)
                            .Include(e => e.Province)
                            .Include(e => e.District)
                            .FirstOrDefaultAsync(x => x.Id == jobApplication.CandidateId);
                        output.User = ObjectMapper.Map<UserEditDto>(candidate.Account);
                        if (candidate.Account != null)
                        {
                            output.ProfilePictureId = candidate.Account.ProfilePictureId;
                        }
                        output.Candidate = ObjectMapper.Map<CandidateEditDto>(candidate);
                        output.Candidate.Province = ObjectMapper.Map<GeoUnitDto>(candidate.Province);
                        output.Candidate.District = ObjectMapper.Map<GeoUnitDto>(candidate.District);

                        #endregion

                    }
                }
            }
            return output;
        }
        public async Task<JobApplicationEditDto> GetJobApplication(NullableIdDto<long> input)
        {
            JobApplicationEditDto jobApplicationEditDto = new JobApplicationEditDto();
            if (input != null)
            {
                if (input.Id.HasValue)
                {
                    var jobApplication = await _jobApplicationRepo.GetAll()
                          .Include(e => e.Province)
                          .Include(e => e.Experiences)
                          .Include(e => e.FormOfWork)
                          .Include(e => e.Occupations)
                          .Include(e => e.Literacy)
                          .Include(e => e.Positions)
                          .AsTracking()
                          .Where(x => x.Id == input.Id).FirstOrDefaultAsync();
                    jobApplicationEditDto = ObjectMapper.Map<JobApplicationEditDto>(jobApplication);
                    if (jobApplication != null)
                    {
                        if (!string.IsNullOrEmpty(jobApplication.FileCVUrl))
                        {
                            jobApplicationEditDto.FileMgr = new FileMgr(jobApplication.FileCVUrl); 
                            
                        }
                    }
                }
            }

            return jobApplicationEditDto;
        }

        public async Task<LearningProcessEditDto> GetLearningProcess(NullableIdDto<long> input)
        {
            LearningProcessEditDto LearningProcessEditDto = new LearningProcessEditDto();
            if (input != null)
            {
                if (input.Id.HasValue)
                {
                    var LearningProcess = await _learningProcessRepo.GetAll().Where(x => x.Id == input.Id).FirstOrDefaultAsync();
                    LearningProcessEditDto = ObjectMapper.Map<LearningProcessEditDto>(LearningProcess);

                }
            }
            return LearningProcessEditDto;
        }

        public async Task<List<LearningProcessEditDto>> GetLearningProcessForList(NullableIdDto<long> IdJobApp)
        {
            List<LearningProcessEditDto> LearningProcessEditDto = new List<LearningProcessEditDto>();
            if (IdJobApp != null)
            {
                if (IdJobApp.Id.HasValue)
                {
                    var LearningProcess = await _learningProcessRepo.GetAll().Where(x => x.JobApplicationId == IdJobApp.Id)
                        .OrderBy(x => x.StartTime)
                        .ToListAsync();
                    LearningProcessEditDto = ObjectMapper.Map<List<LearningProcessEditDto>>(LearningProcess);
                }
            }
            return LearningProcessEditDto;
        }

        public async Task<WorkExperienceEditDto> GetWorkExperience(NullableIdDto<long> input)
        {
            WorkExperienceEditDto workExperienceEditDto = new WorkExperienceEditDto();
            if (input != null)
            {
                if (input.Id.HasValue)
                {
                    var workExperience = await _workExperienceRepo.GetAll().Where(x => x.Id == input.Id).FirstOrDefaultAsync();
                    workExperienceEditDto = ObjectMapper.Map<WorkExperienceEditDto>(workExperience);
                }
            }
            return workExperienceEditDto;
        }

        public async Task<List<WorkExperienceEditDto>> GetWorkExperiencesForList(NullableIdDto<long> IdJobApp)
        {
            List<WorkExperienceEditDto> workExperienceEditDto = new List<WorkExperienceEditDto>();
            if (IdJobApp != null)
            {
                if (IdJobApp.Id.HasValue)
                {
                    var workExperience = await _workExperienceRepo.GetAll().Where(x => x.JobApplicationId == IdJobApp.Id)
                        .OrderBy(x => x.StartTime)
                        .ToListAsync();
                    workExperienceEditDto = ObjectMapper.Map<List<WorkExperienceEditDto>>(workExperience);
                }
            }
            return workExperienceEditDto;
        }
        public async Task<JobApplicationEditDto> UpdateJobApplicationForWeb(JobApplicationCreate input)
        {
            try
            {
                var output = new JobApplicationEditDto();
                if (input.Id.HasValue)
                {
                    JobApplication JobApp = await _jobApplicationRepo.GetAsync(input.Id.Value);
                    if (JobApp != null)
                    {
                        input.TenantId = JobApp.TenantId;
                        #region Cập nhật làm hồ sơ chính 
                        if (input.IsPublished == true)
                        {
                            if (JobApp.IsPublished != true)
                            {
                                var ListJobApp = await _jobApplicationRepo.GetAll()
                                                                     .Where(x => x.CandidateId == JobApp.CandidateId)
                                                                     .Where(x => x.IsPublished == true)
                                                                     .ToListAsync();

                                foreach (var item in ListJobApp)
                                {
                                    item.IsPublished = false;
                                }

                            }
                        }
                        #endregion
                        if (input.FileCVUrl != null)
                        {
                            input.FileCVUrl = StringCipher.Instance.Decrypt(input.FileCVUrl);
                        }
                        #region ObjectMap 
                        JobApp.Id = input.Id.Value;
                        JobApp.TenantId = input.TenantId;
                        JobApp.DesiredSalary = input.DesiredSalary;
                        JobApp.CurrencyUnit = input.CurrencyUnit;
                        JobApp.FormOfWorkId = input.FormOfWorkId;
                        JobApp.Career = input.Career;
                        JobApp.LiteracyId = input.LiteracyId;
                        JobApp.PositionsId = input.PositionsId;
                        JobApp.OccupationId = input.OccupationId;
                        JobApp.WorkSite = input.WorkSite;
                        JobApp.ExperiencesId = input.ExperiencesId;
                        JobApp.Title = input.Title;
                        JobApp.IsPublished = input.IsPublished;
                        JobApp.CandidateId = input.CandidateId;
                        JobApp.FileCVUrl = input.FileCVUrl;
                        #endregion
                        if (JobApp != null)
                        {
                            JobApp = await _jobApplicationRepo.UpdateAsync(JobApp);
                            JobApp = await _jobApplicationRepo.GetAsync(JobApp.Id);
                            if (JobApp != null)
                            {
                                #region ObjectMap 
                                output.Id = JobApp.Id;
                                output.FormOfWorkId = JobApp.FormOfWorkId.Value;
                                output.TenantId = JobApp.TenantId;
                                output.DesiredSalary = JobApp.DesiredSalary;
                                output.CurrencyUnit = JobApp.CurrencyUnit.Value;
                                output.Career = JobApp.Career;
                                output.LiteracyId = JobApp.LiteracyId.Value;
                                output.PositionsId = JobApp.PositionsId;
                                output.OccupationId = JobApp.OccupationId.Value;
                                output.WorkSite = JobApp.WorkSite;
                                output.ExperiencesId = JobApp.ExperiencesId;
                                output.Title = JobApp.Title;
                                output.IsPublished = JobApp.IsPublished;
                                output.CandidateId = JobApp.CandidateId;
                                output.FileCVUrl = StringCipher.Instance.Encrypt(JobApp.FileCVUrl);
                                #endregion  
                            }
                            //output = ObjectMapper.Map<JobApplicationEditDto>(JobApp);

                        }
                        
                    }

                }

                return output;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<JobApplicationEditDto> UpdateJobApplication(JobApplicationEditDto input)
        {
            try
            {
                var output = new JobApplicationEditDto();
                if (input.Id.HasValue)
                {
                    JobApplication JobApp = await _jobApplicationRepo.GetAsync(input.Id.Value);
                    if (JobApp != null)
                    {
                        input.TenantId = JobApp.TenantId;
                        #region Cập nhật làm hồ sơ chính 
                        if (input.IsPublished == true)
                        {
                            if (JobApp.IsPublished != true)
                            {
                                var ListJobApp = await _jobApplicationRepo.GetAll()
                                                                     .Where(x => x.CandidateId == JobApp.CandidateId)
                                                                     .Where(x => x.IsPublished == true)
                                                                     .ToListAsync();

                                foreach (var item in ListJobApp)
                                {
                                    item.IsPublished = false;
                                    //UpdatePushlish(item);
                                }

                            }
                        }
                        #endregion
                        #region ObjectMap 
                        JobApp.Id = input.Id.Value;
                        JobApp.TenantId = input.TenantId;
                        JobApp.DesiredSalary = input.DesiredSalary;
                        JobApp.CurrencyUnit = input.CurrencyUnit;
                        JobApp.FormOfWorkId = input.FormOfWorkId;
                        JobApp.Career = input.Career;
                        JobApp.LiteracyId = input.LiteracyId;
                        if (input.PositionsId.HasValue)
                        {
                            JobApp.PositionsId = input.PositionsId.Value;

                        }
                        JobApp.OccupationId = input.OccupationId;
                        JobApp.WorkSite = input.WorkSite;
                        JobApp.ExperiencesId = input.ExperiencesId;
                        JobApp.Title = input.Title;
                        JobApp.IsPublished = input.IsPublished;
                        JobApp.CandidateId = input.CandidateId;
                        JobApp.FileCVUrl = StringCipher.Instance.Decrypt(input.FileCVUrl);
                        #endregion
                        if (JobApp != null)
                        {
                            JobApp = await _jobApplicationRepo.UpdateAsync(JobApp);
                            JobApp = await _jobApplicationRepo.GetAsync(JobApp.Id);
                            if (JobApp != null)
                            {
                                #region ObjectMap 
                                output.Id = JobApp.Id;
                                output.FormOfWorkId = JobApp.FormOfWorkId.Value;
                                output.TenantId = JobApp.TenantId;
                                output.DesiredSalary = JobApp.DesiredSalary;
                                output.CurrencyUnit = JobApp.CurrencyUnit.Value;
                                output.Career = JobApp.Career;
                                output.LiteracyId = JobApp.LiteracyId.Value;
                                output.PositionsId = JobApp.PositionsId;
                                output.OccupationId = JobApp.OccupationId.Value;
                                output.WorkSite = JobApp.WorkSite;
                                output.ExperiencesId = JobApp.ExperiencesId;
                                output.Title = JobApp.Title;
                                output.IsPublished = JobApp.IsPublished;
                                output.CandidateId = JobApp.CandidateId;
                                output.FileCVUrl = JobApp.FileCVUrl;
                                #endregion  
                            }
                            //output = ObjectMapper.Map<JobApplicationEditDto>(JobApp);

                        }
                    }

                }

                return output;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public Task<bool> UpdateJobApplicationBL(NullableIdDto<long> input, string fileUrl)
        {
            throw new NotImplementedException();
        }

        public async Task<LearningProcessEditDto> UpdateLearningProcess(LearningProcessEditDto input)
        {
            try
            {
                var output = new LearningProcessEditDto();
                if (input.Id.HasValue)
                {
                    LearningProcess learingProcess = await _learningProcessRepo.GetAsync(input.Id.Value);
                    if (learingProcess != null)
                    {
                        input.TenantId = learingProcess.TenantId;
                        ObjectMapper.Map(input, learingProcess);
                        if (learingProcess != null)
                        {
                            learingProcess = await _learningProcessRepo.UpdateAsync(learingProcess);
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

        public async Task<WorkExperienceEditDto> UpdateWorkExperience(WorkExperienceEditDto input)
        {
            try
            {
                var output = new WorkExperienceEditDto();
                if (input.Id.HasValue)
                {
                    WorkExperience workExperience = await _workExperienceRepo.GetAsync(input.Id.Value);
                    if (workExperience != null)
                    {
                        input.TenantId = workExperience.TenantId;
                        ObjectMapper.Map(input, workExperience);
                        if (workExperience != null)
                        {
                            workExperience = await _workExperienceRepo.UpdateAsync(workExperience);
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

        public async Task<PagedResultDto<JobApplicationEditDto>> GetListJobAppOfCandidate(JobAppSearch input)
        {
            try
            {
                PagedResultDto<JobApplicationEditDto> Ouput = new PagedResultDto<JobApplicationEditDto>();

                var candidate = _candidateRepo.GetAll().Where(x => x.UserId == AbpSession.UserId).FirstOrDefault();
                if (candidate != null)
                {
                    var queryJobApps = _jobApplicationRepo.GetAll()
                        .Include(x => x.Province)
                        .Include(x => x.Occupations)
                        .Include(x => x.Literacy)
                        .Include(x => x.Experiences)
                        .Include(x => x.FormOfWork)
                    .Where(x => x.CandidateId == candidate.Id)
                    .AsNoTracking()
                   .WhereIf(input.FormOfWorkId != null && input.FormOfWorkId.Count > 0, x => input.FormOfWorkId.Contains(x.FormOfWorkId))
                   .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.WorkSite))
                   .WhereIf(input.OccupationId.HasValue, x => x.OccupationId == input.OccupationId)
                   .WhereIf(input.ExperiencesId.HasValue, x => x.ExperiencesId == input.ExperiencesId)
                   .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Title.ToLower().Contains(input.Search.ToLower()))
                   .WhereIf(input.LiteracyId.HasValue, x => x.LiteracyId.Equals(input.LiteracyId.Value))
                    .WhereIf(input.SalaryMax.HasValue, x => input.SalaryMax >= x.DesiredSalary)
                    .WhereIf(input.SalaryMin.HasValue, x => input.SalaryMin <= x.DesiredSalary)
                   .OrderByDescending(x => x.IsPublished)
                   .ThenByDescending(x => x.CreationTime);

                    var ListJobApplicationEditDto = ObjectMapper.Map<List<JobApplicationEditDto>>(queryJobApps);

                    Ouput.Items = ListJobApplicationEditDto;
                    Ouput.TotalCount = ListJobApplicationEditDto.Count();
                    return Ouput;
                }
                return Ouput;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task UpdatePushlishById(NullableIdDto<long> input)
        {
            try
            {
                if (input != null)
                {
                    var JobApp = _jobApplicationRepo.Get(input.Id.Value);
                    var Candidate = _candidateRepo.Get(JobApp.CandidateId);
                    if (Candidate.UserId == AbpSession.UserId)
                    {
                        #region Cập nhật làm hồ sơ chính 
                        if (JobApp.IsPublished != true)
                        {
                            var ListJobApp = await _jobApplicationRepo.GetAll()
                                                                 //.Where(x => x.CandidateId == JobApp.CandidateId && x.IsPublished == true && x.Id != input.Id)
                                                                 .Where(x => x.CandidateId == JobApp.CandidateId && x.IsPublished == true)
                                                                 .ToListAsync();

                            foreach (var item in ListJobApp)
                            {
                                item.IsPublished = false;
                                //UpdatePushlish(item);
                            }

                            JobApp.IsPublished = true;
                            //_jobApplicationRepo.Update(JobApp);
                        }
                        else
                        {
                            var ListJobApp = await _jobApplicationRepo.GetAll()
                                                                 //.Where(x => x.CandidateId == JobApp.CandidateId && x.IsPublished == true && x.Id != input.Id)
                                                                 .Where(x => x.CandidateId == JobApp.CandidateId && x.IsPublished == true)
                                                                 .ToListAsync();

                            foreach (var item in ListJobApp)
                            {
                                item.IsPublished = false;
                                //UpdatePushlish(item);
                            }

                            JobApp.IsPublished = false;
                        }
                        #endregion
                    }
                }

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<JobApplication> ConvertJobApplicationCreateToJobApplication(JobApplicationCreate jobApplicationCreate)
        {
            JobApplication jobApplication = new JobApplication();
            if (jobApplicationCreate != null)
            {
                #region FullAuditedEntityDto
                if (jobApplicationCreate.Id.HasValue)
                {
                    jobApplication.Id = jobApplicationCreate.Id.Value;
                }
                jobApplication.CreationTime = jobApplicationCreate.CreationTime;
                jobApplication.CreatorUserId = jobApplicationCreate.CreatorUserId;
                jobApplication.DeleterUserId = jobApplicationCreate.DeleterUserId;
                jobApplication.DeletionTime = jobApplicationCreate.DeletionTime;
                jobApplication.IsDeleted = jobApplicationCreate.IsDeleted;
                jobApplication.LastModifierUserId = jobApplicationCreate.LastModifierUserId;
                jobApplication.LastModificationTime = jobApplicationCreate.LastModificationTime;
                jobApplication.TenantId = jobApplicationCreate.TenantId;
                #endregion


                #region JobApplication
                jobApplication.DesiredSalary = jobApplicationCreate.DesiredSalary;
                jobApplication.CurrencyUnit = jobApplicationCreate.CurrencyUnit;
                jobApplication.FormOfWorkId = jobApplicationCreate.FormOfWorkId;
                jobApplication.Career = jobApplicationCreate.Career;
                jobApplication.LiteracyId = jobApplicationCreate.LiteracyId;
                jobApplication.PositionsId = jobApplicationCreate.PositionsId;
                jobApplication.OccupationId = jobApplicationCreate.OccupationId;
                jobApplication.WorkSite = jobApplicationCreate.WorkSite;
                jobApplication.ExperiencesId = jobApplicationCreate.ExperiencesId;
                jobApplication.JobGrade = jobApplicationCreate.JobGrade;
                jobApplication.Title = jobApplicationCreate.Title;
                jobApplication.IsPublished = jobApplicationCreate.IsPublished;
                jobApplication.Word = jobApplicationCreate.Word;
                jobApplication.Excel = jobApplicationCreate.Excel;
                jobApplication.PowerPoint = jobApplicationCreate.PowerPoint;
                jobApplication.CandidateId = jobApplicationCreate.CandidateId;
                jobApplication.FileCVUrl = jobApplicationCreate.FileCVUrl;
                #endregion 
            }
            return jobApplication;
        }

        public async Task<int> CountJob()
        {
            using var uow = UnitOfWorkManager.Begin();

            using (CurrentUnitOfWork.SetTenantId(1))
            {
                try
                {
                    var List = (await _jobApplicationRepo.GetAllListAsync()).Where(x => x.IsPublished == true).ToList();
                    return List.Count();
                }
                catch (Exception) {return 0; }
                finally
                {
                    await uow.CompleteAsync();
                }
               
            }
        }

        #region Mobile/Frontend
        public async Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobAppsMobile(JobAppSearch input)
        {
            try
            {
                List<Candidate> candidates = new List<Candidate>();


                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                    candidates = await _candidateRepo.GetAll()
                        .WhereIf(input.Gender.HasValue, x => (int)x.Gender == input.Gender.Value)
                        .Include(e => e.Account).ToListAsync();
                }
                else
                {
                    candidates = await _candidateRepo.GetAll()
                        .WhereIf(input.Gender.HasValue, x => (int)x.Gender == input.Gender.Value)
                        .Include(e => e.Account).ToListAsync();

                };
                var output = new PagedResultDto<GetJobApplicationForEditOutput> { Items = new List<GetJobApplicationForEditOutput>(), TotalCount = 0 };

                if (candidates == null || candidates.Count() <= 0)
                {
                    return output;
                }
                List<UserEditDto> users = new List<UserEditDto>();
                foreach (var item in candidates)
                {
                    var user = ObjectMapper.Map<UserEditDto>(item.Account);
                    users.Add(user);
                }


                var listCadidates = ObjectMapper.Map<List<CandidateEditDto>>(candidates);

                var queryJobApps = _jobApplicationRepo.GetAll()
                    .Include(e => e.Province)
                    .Include(e => e.Literacy)
                    .Include(e => e.Experiences)
                    .Include(e => e.FormOfWork)
                    .Include(e => e.Positions)
                    .Include(e => e.Candidate)
                    .Include(e => e.Candidate.Account)
                    .Where(e => e.IsPublished == true)
                    .AsNoTracking()
                    .WhereIf(input.Gender.HasValue, x => (int)x.Candidate.Gender == input.Gender.Value)
                    .WhereIf(input.WorkSiteId.HasValue && input.WorkSiteId != 0, x => input.WorkSiteId.Value == x.WorkSite)
                    .WhereIf(input.ExperiencesId.HasValue, x => x.ExperiencesId.Equals(input.ExperiencesId.Value))
                    .WhereIf(input.OccupationId.HasValue, x => x.OccupationId.Equals(input.OccupationId.Value))
                    .WhereIf(input.FormOfWorkId != null && input.FormOfWorkId.Count > 0, x => input.FormOfWorkId.Contains(x.FormOfWorkId))
                     .WhereIf(input.SalaryMax.HasValue, x => input.SalaryMax >= x.DesiredSalary)
                    .WhereIf(input.SalaryMin.HasValue, x => input.SalaryMin <= x.DesiredSalary)
                    .WhereIf(input.LiteracyId.HasValue, x => x.LiteracyId.Equals(input.LiteracyId.Value)).OrderByDescending(x => x.CreationTime);

                if (queryJobApps == null || queryJobApps.Count() <= 0)
                {
                    return output;
                }
                if (input.Take == 0) input.Take = 10;
                List<GetJobApplicationForEditOutput> queryCommon = new List<GetJobApplicationForEditOutput>();

                foreach (var item in queryJobApps.Skip(input.SkipCount * (input.Paging - 1)).Take(input.Take).ToList())
                {
                    item.Candidate.AvatarUrl = await GetProfilePicture(item.Candidate.Account.Id);
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

                output.Items = queryCommon.ToList();

                output.TotalCount = queryCommon.Count();

                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<string> GetProfilePicture(long userId)
        {
            var result = await _profileAppService.GetProfilePictureByUser(userId);
            if (string.IsNullOrWhiteSpace(result.ProfilePicture))
            {
                return GetDefaultProfilePicture();
            }

            return "data:image/png;base64, " + result.ProfilePicture;
        }

        private string GetDefaultProfilePicture()
        {
            return "media/default-profile-picture.png";
        }

        public async Task<JobApplicationEditDto> CreateJobApplicationForWeb(JobApplicationCreate input)
        {
            try
            {
                JobApplication jobApplication = new JobApplication();
                #region Cập nhật làm hồ sơ chính 
                if (input.IsPublished == true)
                {
                    //var ListJobApp = _jobApplicationRepo.GetAll()
                    //                                     .Where(x => x.CandidateId == input.CandidateId).ToList();
                    //foreach (var item in ListJobApp)
                    //{
                    //    item.IsPublished = false;
                    //    UpdatePushlish(item);
                    //}

                    var jobAppPublished = await _jobApplicationRepo.FirstOrDefaultAsync(x => x.CandidateId == input.CandidateId && x.IsPublished == true);
                    if (jobAppPublished != null)
                        jobAppPublished.IsPublished = false;

                    //await UnitOfWorkManager.Current.SaveChangesAsync();
                }
                #endregion
                #region
                if (input.FileCVUrl!= null)
                {
                    input.FileCVUrl = StringCipher.Instance.Decrypt(input.FileCVUrl);  
                }
                #endregion
                jobApplication = await ConvertJobApplicationCreateToJobApplication(input);
                jobApplication.Id = await _jobApplicationRepo.InsertAndGetIdAsync(jobApplication);

                jobApplication = _jobApplicationRepo.Get(jobApplication.Id);

                var output = ObjectMapper.Map<JobApplicationEditDto>(jobApplication);
                output.FileCVUrl = StringCipher.Instance.Encrypt(input.FileCVUrl);
                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        //public async Task<FileDto> ExportCV(GetJobApplicationForEditOutput input)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ;
        //    }
        //}
    }
}
