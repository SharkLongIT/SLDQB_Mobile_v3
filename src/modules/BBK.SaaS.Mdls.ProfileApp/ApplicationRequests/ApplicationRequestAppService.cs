using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.ApplicationRequests
{
    public class ApplicationRequestAppService : SaaSAppServiceBase, IApplicationRequestAppService
    {
        private readonly IRepository<ApplicationRequest, long> _applicationRequestRepo;
        public ApplicationRequestAppService(IRepository<ApplicationRequest, long> applicationRequestRepo)
        {
            _applicationRequestRepo = applicationRequestRepo;
        }

        public async Task<bool> CheckApplied(long RecruitmentId)
        {
            bool ouput = false;
            if (RecruitmentId == 0) return ouput;
            try
            {
                var listAppRequest = _applicationRequestRepo.GetAll().AsTracking().Where(x => x.RecruitmentId == RecruitmentId && x.CreatorUserId == AbpSession.UserId).FirstOrDefaultAsync();
                if (listAppRequest != null)
                {
                    ouput = true;
                }
                else
                {
                    ouput = false;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            return ouput;
        }

        public async Task<ApplicationRequestEditDto> Create(ApplicationRequestEditDto dto)
        {
            try
            {

                ApplicationRequest applicationRequest = new ApplicationRequest();
                applicationRequest = ObjectMapper.Map<ApplicationRequest>(dto);
                applicationRequest.Status = 1;
                applicationRequest.Id = await _applicationRequestRepo.InsertAndGetIdAsync(applicationRequest);

                applicationRequest = _applicationRequestRepo.Get(applicationRequest.Id);

                var output = ObjectMapper.Map<ApplicationRequestEditDto>(applicationRequest);
                return output;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task DeleteApplicationRequest(NullableIdDto<long> dto)
        {
            try
            {
                if (dto.Id.HasValue)
                {
                    var WorkExperience = _applicationRequestRepo.Get(dto.Id.Value);
                    await _applicationRequestRepo.DeleteAsync(WorkExperience);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ApplicationRequestEditDto>> GetAll(ApplicationRequestSearch input)
        {
            try
            {
                PagedResultDto<ApplicationRequestEditDto> Ouput = new PagedResultDto<ApplicationRequestEditDto>();
                var query = await _applicationRequestRepo.GetAll()
              .Include(x => x.JobApplication)
              .Include(x => x.Recruitment.Recruiter)
              .Include(x => x.Recruitment.Ranks)
              .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Recruitment.Recruiter.CompanyName.ToLower().Contains(input.Search.ToLower()) || x.Recruitment.Title.ToLower().Contains(input.Search.ToLower()))
              .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value)
              .WhereIf(input.StartTime.HasValue, x => x.CreationTime.Date >= input.StartTime.Value.Date)
              .WhereIf(input.EndTime.HasValue, x => x.CreationTime.Date <= input.EndTime.Value.Date)
              .AsNoTracking()
              .Where(x => x.CreatorUserId == AbpSession.UserId)
              .OrderByDescending(x => x.CreationTime)
              .ToListAsync();

                var listappRequest = ObjectMapper.Map<List<ApplicationRequestEditDto>>(query);

                //foreach (var item in listappRequest)
                //{
                //    item.

                //}


                Ouput.Items = listappRequest;

                Ouput.TotalCount = listappRequest.Count();
                return Ouput;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }

        public async Task<ApplicationRequestEditDto> GetById(NullableIdDto<long> dto)
        {
            ApplicationRequestEditDto AppRequestEditDto = new ApplicationRequestEditDto();
            if (dto != null)
            {
                if (dto.Id.HasValue)
                {
                    var AppRequest = await _applicationRequestRepo.GetAll()
                          .Include(e => e.JobApplication)
                          .Include(e => e.JobApplication.Candidate)
                          .Include(e => e.JobApplication.Candidate.Account)
                          .Include(e => e.JobApplication.Candidate.Province)
                          .Include(e => e.JobApplication.Occupations)
                          .Include(e => e.JobApplication.Literacy)
                          .Include(e => e.JobApplication.FormOfWork)
                          .Include(e => e.JobApplication.Experiences)
                          .Include(e => e.JobApplication.WorkExperiences)
                          .Include(e => e.JobApplication.LearningProcess)
                          .Include(e => e.JobApplication.Positions)
                          .Include(e => e.JobApplication.Province)
                          .Include(e => e.Recruitment)
                          .AsTracking()
                          .Where(x => x.Id == dto.Id).FirstOrDefaultAsync();
                    if (AppRequest != null)
                    {

                        AppRequestEditDto = ObjectMapper.Map<ApplicationRequestEditDto>(AppRequest);
                        if (AppRequest.JobApplication.Candidate.Account != null)
                        {
                            AppRequestEditDto.User = ObjectMapper.Map<UserEditDto>(AppRequest.JobApplication.Candidate.Account);
                            AppRequestEditDto.ProfilePictureId = AppRequest.JobApplication.Candidate.Account.ProfilePictureId;
                        }

                    }
                }
            }
            return AppRequestEditDto;
        }

        public async Task<PagedResultDto<ApplicationRequestEditDto>> GetAllByRecruiter(ApplicationRequestSearch input)
        {
            try
            {
                PagedResultDto<ApplicationRequestEditDto> Ouput = new PagedResultDto<ApplicationRequestEditDto>();
                var query = await _applicationRequestRepo.GetAll()
              .Include(x => x.JobApplication)
              .Include(x => x.JobApplication.Candidate)
              .Include(x => x.JobApplication.Candidate.Account)
              .Include(x => x.Recruitment.Recruiter)
              .Include(x => x.Recruitment.Experiences)
              .Include(x => x.Recruitment.Ranks)
              .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.Recruitment.Title.ToLower().Contains(input.Search.ToLower()))
              .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value)
              .WhereIf(input.Rank.HasValue, x => x.Recruitment.Rank == input.Rank.Value)
              .WhereIf(input.Experience.HasValue, x => x.Recruitment.Experience == input.Experience.Value)
              .AsNoTracking()
              .Where(x => x.Recruitment.Recruiter.UserId == AbpSession.UserId).ToListAsync();

                var listappRequest = ObjectMapper.Map<List<ApplicationRequestEditDto>>(query);

                Ouput.Items = listappRequest;

                Ouput.TotalCount = listappRequest.Count();
                return Ouput;

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
                    var Recruitment = _applicationRequestRepo.Get(Id.Value);
                    await _applicationRequestRepo.DeleteAsync(Id.Value);
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<long> CreateUT(ApplicationRequestEditDto input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                ApplicationRequest newItemId = new ApplicationRequest();
               
                newItemId.Status = 1;
                newItemId.RecruitmentId = input.RecruitmentId;
                newItemId.JobApplicationId = input.JobApplicationId;
                newItemId.Content = input.Content;
                var newId = await _applicationRequestRepo.InsertAndGetIdAsync(newItemId);
                return newId;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }




}
