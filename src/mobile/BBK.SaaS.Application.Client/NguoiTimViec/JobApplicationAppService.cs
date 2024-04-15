using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBK.SaaS.NguoiTimViec
{
    public class JobApplicationAppService : ProxyAppServiceBase, IJobApplicationAppService
    {
        public async Task<GetJobApplicationForEditOutput> GetJobApplicationForEdit(NullableIdDto<long> input)
        {
            return await ApiClient.GetAnonymousAsync<GetJobApplicationForEditOutput>(GetEndpoint(nameof(GetJobApplicationForEdit)), input);

        }
        public async Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobAppsMobile(JobAppSearch input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<GetJobApplicationForEditOutput>>(GetEndpoint(nameof(GetAllJobAppsMobile)), input);
        }
        public async Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobApps(JobAppSearch input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<GetJobApplicationForEditOutput>>(GetEndpoint(nameof(GetAllJobApps)), input);
        }
        public async Task<PagedResultDto<JobApplicationEditDto>> GetListJobAppOfCandidate(JobAppSearch input)
        {
            return await ApiClient.GetAsync<PagedResultDto<JobApplicationEditDto>>(GetEndpoint(nameof(GetListJobAppOfCandidate)), input);
        }
        public async Task<JobApplicationEditDto> UpdateJobApplication(JobApplicationEditDto input)
        {
            return await ApiClient.PutAsync<JobApplicationEditDto>(GetEndpoint(nameof(UpdateJobApplication)), input);
        }
        #region
        public async Task<JobApplicationEditDto> CreateJobApplication(JobApplicationEditDto input)
        {
            return await ApiClient.PostAsync<JobApplicationEditDto>(GetEndpoint(nameof(CreateJobApplication)), input);
        }

        public async Task<LearningProcessEditDto> CreateLearningProcess(LearningProcessEditDto input)
        {
            return await ApiClient.PostAsync<LearningProcessEditDto>(GetEndpoint(nameof(CreateLearningProcess)), input);
        }

        //public async Task<WorkExperienceEditDto> CreateWorkExperience(WorkExperienceEditDto input)
        //{
        //    return await ApiClient.PostAsync<WorkExperienceEditDto>(GetEndpoint(nameof(CreateWorkExperience)), input);
        //}
        public  async Task<JobApplicationEditDto> DeleteJobApplication(NullableIdDto<long> input)
        {
            return await ApiClient.DeleteAsync<JobApplicationEditDto>(GetEndpoint(nameof(DeleteJobApplication)), input);
        }

        public async Task<LearningProcessEditDto> DeleteLearningProcess(LearningProcessEditDto input)
        {
            return await ApiClient.DeleteAsync<LearningProcessEditDto>(GetEndpoint(nameof(DeleteLearningProcess)), input);
        }

        public async Task<WorkExperienceEditDto> DeleteWorkExperience(WorkExperienceEditDto input)
        {
            return await ApiClient.DeleteAsync<WorkExperienceEditDto>(GetEndpoint(nameof(DeleteWorkExperience)), input);
        }

        public Task<JobApplicationEditDto> GetJobApplication(NullableIdDto<long> input)
        {
            throw new System.NotImplementedException();
        }

      

        public Task<LearningProcessEditDto> GetLearningProcess(NullableIdDto<long> id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<LearningProcessEditDto>> GetLearningProcessForList(NullableIdDto<long> IdJobApp)
        {
            throw new System.NotImplementedException();
        }

        public Task<WorkExperienceEditDto> GetWorkExperience(NullableIdDto<long> id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<WorkExperienceEditDto>> GetWorkExperiencesForList(NullableIdDto<long> IdJobApp)
        {
            throw new System.NotImplementedException();
        }

       

        public Task<bool> UpdateJobApplicationBL(NullableIdDto<long> input, string fileUrl)
        {
            throw new System.NotImplementedException();
        }

        public async Task<LearningProcessEditDto> UpdateLearningProcess(LearningProcessEditDto input)
        {
            return await ApiClient.PutAsync<LearningProcessEditDto>(GetEndpoint(nameof(UpdateLearningProcess)), input);
        }

        public async Task<WorkExperienceEditDto> UpdateWorkExperience(WorkExperienceEditDto input)
        {
            return await ApiClient.PutAsync<WorkExperienceEditDto>(GetEndpoint(nameof(UpdateWorkExperience)), input);
        }

        public async Task<List<WorkExperienceEditDto>> CreateWorkExperience(WorkExperienceEditDto input)
        {
            return await ApiClient.PostAsync<List<WorkExperienceEditDto>>(GetEndpoint(nameof(CreateWorkExperience)), input);
        }

        public Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobOfProfessionalStaff(JobAppSearchOfProfessionalStaff input)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdatePushlishById(NullableIdDto<long> input)
        {
            throw new System.NotImplementedException();
        }

        public Task<JobApplicationEditDto> CreateJobApplicationForWeb(JobApplicationCreate input)
        {
            throw new System.NotImplementedException();
        }

        public async Task<JobApplicationEditDto> UpdateJobApplicationForWeb(JobApplicationCreate input)
        {
            return await ApiClient.PutAsync<JobApplicationEditDto>(GetEndpoint(nameof(UpdateJobApplicationForWeb)), input);
        }






        #endregion
    }
}
