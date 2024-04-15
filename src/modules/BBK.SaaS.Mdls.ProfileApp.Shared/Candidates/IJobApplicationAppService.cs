using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public interface IJobApplicationAppService : IApplicationService, ITransientDependency
    {
		Task<GetJobApplicationForEditOutput> GetJobApplicationForEdit(NullableIdDto<long> input);
		Task<bool> UpdateJobApplicationBL(NullableIdDto<long> input, string fileUrl);

		Task<List<WorkExperienceEditDto>> CreateWorkExperience(WorkExperienceEditDto input);
		Task<WorkExperienceEditDto> UpdateWorkExperience(WorkExperienceEditDto input);
		Task<WorkExperienceEditDto> DeleteWorkExperience(WorkExperienceEditDto input);
		Task<WorkExperienceEditDto> GetWorkExperience(NullableIdDto<long> id);
		Task<List<WorkExperienceEditDto>> GetWorkExperiencesForList(NullableIdDto<long> IdJobApp);


		Task<LearningProcessEditDto> CreateLearningProcess(LearningProcessEditDto input);
		Task<LearningProcessEditDto> UpdateLearningProcess(LearningProcessEditDto input);
		Task<LearningProcessEditDto> DeleteLearningProcess(LearningProcessEditDto input);
        Task<LearningProcessEditDto> GetLearningProcess(NullableIdDto<long> id);
        Task<List<LearningProcessEditDto>> GetLearningProcessForList(NullableIdDto<long> IdJobApp);


		Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobApps(JobAppSearch input);
		Task<PagedResultDto<JobApplicationEditDto>> GetListJobAppOfCandidate(JobAppSearch input);
        Task<JobApplicationEditDto> CreateJobApplication(JobApplicationEditDto input);
        Task<JobApplicationEditDto> CreateJobApplicationForWeb(JobApplicationCreate input);
		Task<JobApplicationEditDto> UpdateJobApplication(JobApplicationEditDto input);
		Task<JobApplicationEditDto> UpdateJobApplicationForWeb(JobApplicationCreate input);
		Task<JobApplicationEditDto> DeleteJobApplication(NullableIdDto<long> input);
		Task<JobApplicationEditDto> GetJobApplication(NullableIdDto<long> input);

		Task UpdatePushlishById(NullableIdDto<long> input);


        #region Mobile/Frontend
        Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobAppsMobile(JobAppSearch input);

        #endregion

    }
}
