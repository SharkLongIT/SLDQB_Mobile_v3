using Abp;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Authorization.Users.Profile;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public class CandidateAppService : SaaSAppServiceBase, ICandidateAppService
    {
        private readonly IRepository<Candidate, long> _candidateRepo;
        private readonly IRepository<JobApplication, long> _jobApplicationRepo;
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IProfileAppService _profileAppService;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IUnitOfWorkManager _unitOfWork;
        private IHostingEnvironment _Environment;

        public CandidateAppService(
            IRepository<Candidate, long> candidate,
            IRepository<JobApplication, long> jobApplicationRepo,
            FileServiceFactory fileServiceFactory,
            IHostingEnvironment Environment,
            IProfileAppService profileAppService,
            IUnitOfWorkManager unitOfWork,
            IBinaryObjectManager binaryObjectManager)
        {
            _candidateRepo = candidate;
            _jobApplicationRepo = jobApplicationRepo;
            _fileServiceFactory = fileServiceFactory;
            _Environment = Environment;
            _binaryObjectManager = binaryObjectManager;
            _profileAppService = profileAppService;
            _unitOfWork = unitOfWork;


        }

        public async Task<GetCandidateForEditOutput> GetCandidateForEdit(NullableIdDto<long> input)
        {
            var output = new GetCandidateForEditOutput { Candidate = new CandidateEditDto(), User = new UserEditDto() };
            if (input.Id.HasValue)
            {
                var unit = UnitOfWorkManager.Current;
                if (unit.GetTenantId() == null)
                {
                    unit.SetTenantId(1);
                }
                var candidate = await _candidateRepo.GetAll()
                        .Include(e => e.Account)
                        .Include(e => e.Province)
                        .Include(e => e.District)
                        .FirstOrDefaultAsync(u => u.UserId == input.Id);
                if (candidate != null)
                {
                    output.User = ObjectMapper.Map<UserEditDto>(candidate.Account);
                    if (candidate.Account != null)
                    {
                        output.ProfilePictureId = candidate.Account.ProfilePictureId;
                    }
                    //output.District = ObjectMapper.Map<GeoUnitDto>(candidate.District);

                    output.Candidate = ObjectMapper.Map<CandidateEditDto>(candidate);
                    output.Candidate.Province = ObjectMapper.Map<GeoUnitDto>(candidate.Province);
                    output.Candidate.District = ObjectMapper.Map<GeoUnitDto>(candidate.District);

                    // output.Candidate.BusinessLicenseFile = new Storage.FileMgr(output.Candidate.BusinessLicenseUrl);
                }
            }
            return output;
        }



        public async Task<CandidateEditDto> Update(CandidateEditDto input)
        {
            try
            {
                var output = new CandidateEditDto();
                if (input.Id.HasValue)
                {
                    Candidate candidate = await _candidateRepo.GetAsync(input.Id.Value);
                    if (candidate != null)
                    {
                        input.TenantId = candidate.TenantId;
                        ObjectMapper.Map(input, candidate);
                        if (candidate != null)
                        {
                            candidate = await _candidateRepo.UpdateAsync(candidate);
                        }
                        //output = ObjectMapper.Map<CandidateEditDto>(candidate);     
                    }
                }
                return input;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<long> UpdateMobile(CandidateEditDto input)
        {
            try
            {
                //_profileAppService.UpdateProfilePicture(new SaaS.Authorization.Users.Profile.Dto.UpdateProfilePictureInput(){});

                var output = new CandidateEditDto();
                if (input.Id.HasValue)
                {
                    Candidate candidate = await _candidateRepo.GetAsync(input.Id.Value);
                    if (candidate != null)
                    {
                        input.TenantId = candidate.TenantId;
                        ObjectMapper.Map(input, candidate);
                        if (candidate != null)
                        {
                            candidate = await _candidateRepo.UpdateAsync(candidate);
                        }
                    }
                }
                return input.Id.Value;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task UpdateProfilePictureFromMobile(byte[] imageBytes)
        {
            UserIdentifier userIdentifier = new UserIdentifier(base.AbpSession.TenantId, base.AbpSession.UserId.Value);

            //byte[] imageBytes = _tempFileCacheManager.GetFile(input.FileToken);
            if (imageBytes == null)
            {
                throw new UserFriendlyException("Not allow null");
            }

            byte[] byteArray = imageBytes;
            if (byteArray.Length > 5242880)
            {
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit", 1024));
            }

            User user = await base.UserManager.GetUserByIdAsync(userIdentifier.UserId);
            if (user.ProfilePictureId.HasValue)
            {
                await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
            }

            BinaryObject storedFile = new BinaryObject(userIdentifier.TenantId, byteArray, $"Profile picture of user {userIdentifier.UserId}. {DateTime.UtcNow}");
            await _binaryObjectManager.SaveAsync(storedFile);
            user.ProfilePictureId = storedFile.Id;
        }

        public async Task<bool> UpdateCandidateBL(NullableIdDto<long> input, string fileUrl)
        {
            if (input.Id.HasValue)
            {
                Candidate candidate = null;
                if (IsAdmin())
                {
                    candidate = await _candidateRepo.FirstOrDefaultAsync(x => x.Id == input.Id);
                }
                else
                {
                    candidate = await _candidateRepo.FirstOrDefaultAsync(x => x.Id == input.Id && x.CreatorUserId == AbpSession.UserId);
                }

                if (candidate != null)
                {
                    candidate.AvatarUrl = fileUrl;
                }
            }
            return true;

        }


        public async Task GeneratePdf(long JobId, int TemplateId)
        {
            try
            {
                var job = _jobApplicationRepo.GetAll()
                    .Include(x => x.Positions)
                    .Include(x => x.Candidate)
                    .Include(x => x.Candidate.Account)
                    .Include(x => x.Candidate.Province)
                    .Include(x => x.Candidate.District)
                    .Include(x => x.Province)
                    .Include(x => x.LearningProcess)
                    .Include(x => x.WorkExperiences)
                    .FirstOrDefault(x => x.Id == JobId);


                List<Templatepdf> widgettemps = null;
                using (var fileService = _fileServiceFactory.Get())
                {

                    
                    //var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
                    var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CVTemplates\\TemplateCV.json" });
                    widgettemps = JsonConvert.DeserializeObject<List<Templatepdf>>(Encoding.UTF8.GetString(fileMgr.Content));
                }

                var webRootPath = this._Environment.WebRootPath;
                string path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\CVTemplates" + Path.DirectorySeparatorChar.ToString() + "CV_1.html";
                string Learning = string.Empty;
                string contentLearning = string.Empty;

                string Experience = string.Empty;
                string contentExperience = string.Empty;
                if (widgettemps != null && TemplateId > 0)
                {
                    foreach (var item in widgettemps)
                    {
                        if(item.TemplatepdfId == TemplateId)
                        {
                            path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\CVTemplates" + Path.DirectorySeparatorChar.ToString() + item.TemplateName;
                            path = File.ReadAllText(path);

                            path = path.Replace("{{FullName}}", job.Candidate.Account.Name.ToUpper());
                            path = path.Replace("{{Phone}}", job.Candidate.Account.PhoneNumber);
                            path = path.Replace("{{Positions}}", job.Positions.DisplayName.ToUpper());
                            path = path.Replace("{{Provice}}", job.Candidate.Province.DisplayName);
                            path = path.Replace("{{Village}}", job.Candidate.Address);
                            path = path.Replace("{{Distrist}}", job.Candidate.District.DisplayName);
                            path = path.Replace("{{Email}}", job.Candidate.Account.EmailAddress);
                            path = path.Replace("{{Career}}", job.Career);
                            //path = path.Replace("{{avt}}", url);
                            path = path.Replace("{{avt}}", "/Profile/GetProfilePictureByUser?userId=" + job.Candidate.Account.Id + "&&profilePictureId=" + job.Candidate.Account.ProfilePictureId);
                            path = path.Replace("{{DateOfBirth}}", job.Candidate.DateOfBirth.ToString());

                            #region learning
                            if(job.LearningProcess != null && job.LearningProcess.Count() > 0)
                            {
                                path = path.Replace("{{titlelearning}}", item.TitleLearning);
                                foreach (var learning in job.LearningProcess)
                                {
                                    Learning = Learning + item.ContentLearningProcessTemplatepdf;
                                    Learning = Learning.Replace("{{SchoolName}}", learning.SchoolName);
                                    Learning = Learning.Replace("{{StartTime}}", learning.StartTime.ToShortDateString());
                                    Learning = Learning.Replace("{{EndTime}}", learning.EndTime.ToShortDateString());
                                    Learning = Learning.Replace("{{Description}}", learning.Description);
                                    Learning = Learning.Replace("{{AcademicDiscipline}}", learning.AcademicDiscipline);
                                }
                                path = path.Replace("{{learning}}", Learning);
                            }
                            else
                            {
                                path = path.Replace("{{titlelearning}}","");
                                path = path.Replace("{{learning}}", "");
                            }

                            #endregion

                            if (job.WorkExperiences != null && job.WorkExperiences.Count() > 0)
                            {
                                path = path.Replace("{{titleexperience}}", item.TitleWorkExp);
                                foreach (var WorkExperiences in job.WorkExperiences)
                                {
                                    Experience = Experience + item.ContentWorkExpTemplatepdf;
                                    Experience = Experience.Replace("{{CompanyName}}", WorkExperiences.CompanyName);
                                    Experience = Experience.Replace("{{StartTime}}", WorkExperiences.StartTime.ToShortDateString());
                                    Experience = Experience.Replace("{{EndTime}}", WorkExperiences.EndTime.ToShortDateString());
                                    Experience = Experience.Replace("{{Description}}", WorkExperiences.Description);
                                    Experience = Experience.Replace("{{Positions}}", WorkExperiences.Positions);

                                }
                                path = path.Replace("{{experience}}", Experience);
                            }
                            else
                            {
                                path = path.Replace("{{titleexperience}}", "");
                                path = path.Replace("{{experience}}", "");
                            }
                                
                        }
                    }
                }
                else
                {

                    path = File.ReadAllText(path);
                    path = path.Replace("{{FullName}}", job.Candidate.Account.Name.ToUpper());
                    path = path.Replace("{{Phone}}", job.Candidate.Account.PhoneNumber);
                    path = path.Replace("{{Positions}}", job.Positions.DisplayName.ToUpper());
                    path = path.Replace("{{Provice}}", job.Candidate.Province.DisplayName);
                    path = path.Replace("{{Village}}", job.Candidate.Address);
                    path = path.Replace("{{Distrist}}", job.Candidate.District.DisplayName);
                    path = path.Replace("{{Email}}", job.Candidate.Account.EmailAddress);
                    path = path.Replace("{{Career}}", job.Career);
                    //path = path.Replace("{{avt}}", url);
                    path = path.Replace("{{avt}}", "/Profile/GetProfilePictureByUser?userId=" + job.Candidate.Account.Id + "&&profilePictureId=" + job.Candidate.Account.ProfilePictureId);
                    path = path.Replace("{{DateOfBirth}}", job.Candidate.DateOfBirth.ToString());

                    #region learning


                    foreach (var learning in job.LearningProcess)
                    {
                        contentLearning += " <table style='padding:-10px; margin: 0px;'><tbody><tr><th style='text-align:left; width:250px;' > <p style='padding: 0px; margin: 0px; color: #387484; font-size: 14px;'>"+ learning.SchoolName + "</p></th><th style='text-align:right; width : auto'><p style=' text-align :right ; padding : 0px;  margin:0px; color: #7c9997; font-size: 13px; font-weight:normal;'>" + learning.StartTime.ToShortDateString() + "-" + learning.EndTime.ToShortDateString() + "</p></th></tr></tbody></table>";

                        //contentLearning = "<p style='float: left; padding: 0px; color: #387484; font-weight: bold; margin-bottom: 5px; margin-top: 5px'>" + learning.SchoolName + "</p>";
                        //contentLearning += "<p style=' float: right; padding : 0px; color: #7c9997; font-size: 14px; margin-top: -32px; padding-right: 24px; text-align: right;'>" + learning.StartTime.ToShortDateString() + "-" + learning.EndTime.ToShortDateString() + "</p>";
                        contentLearning += "<div>";
                        contentLearning += " <p style='color: #387484; font-size: 15px; font-style: italic; margin-top: -10px;    margin-top: -10px; font-weight:normal;'>" + learning.AcademicDiscipline + "</p>";
                        contentLearning += "<ul style='color: #6a8d8d; font-size: 15px; margin-top: 4px;  margin-top: -10px ; margin-left:10px'>";
                        contentLearning += "<p style='padding : 0px;  margin : 0px; text-align :left; white-space: pre-line; font-weight:normal;'>" + learning.Description + "</p>";
                        contentLearning += "</ul>";
                        contentLearning += " </div>";
                        Learning += contentLearning;
                    }
                    path = path.Replace("{{learning}}", Learning);
                    #endregion

                    #region Experience

                    foreach (var WorkExperiences in job.WorkExperiences)
                    {
                        //contentExperience = "<p style=' float: left; padding: 0px; color: #387484; font-weight: bold; margin-bottom: 5px; margin-top: 5px'>" + WorkExperiences.CompanyName + "</p>";
                        //contentExperience += "<p style=' float: right; padding:0px; color: #7c9997; font-size: 14px; margin-top: -32px; padding-right: 24px; text-align: right;'>" + WorkExperiences.StartTime.ToShortDateString() + "-" + WorkExperiences.EndTime.ToShortDateString() + "</p>";
                        contentExperience += "<table style='padding:-10px; margin : 0px;'><tbody><tr><th style='text-align:left; width:250px;' > <p style='padding: 0px; margin : 0px; color: #387484; font-size: 14px;'>" + WorkExperiences.CompanyName + "</p></th><th style='text-align:right; width : auto'><p style=' text-align :right ; padding : 0px; margin:0px; color: #7c9997; font-size: 13px;  font-weight:normal;'>" + WorkExperiences.StartTime.ToShortDateString() + "-" + WorkExperiences.EndTime.ToShortDateString() + "</p></th></tr></tbody></table>";
                        contentExperience += "<div>";
                        contentExperience += " <p style='color: #387484; font-size: 15px; font-style: italic; margin-top: -10px; margin-bottom: -10px;  font-weight:normal;'>" + WorkExperiences.Positions + "</p>";
                        contentExperience += "<ul style='color: #6a8d8d; font-size: 15px; margin-top: 4px; margin-top: -10px; margin-left:10px '>";
                        contentExperience += "<p style='padding : 0px;  margin : 0px; text-align :left; white-space: pre-line; font-weight:normal;'>" + WorkExperiences.Description + "</p>";
                        contentExperience += "</ul>";
                        contentExperience += " </div>";
                        Experience += contentExperience;
                    }
                    path = path.Replace("{{experience}}", Experience);
                    #endregion
                }




                string testStylesheet = ".bold{font-weight:700;font-size:20px;text-transform:uppercase}.semi-bold{font-weight:500;font-size:16px}.resume{width:800px;height:auto;display:block;margin:50px auto}.resume .resume_left{width:280px;background:#0bb5f4}.resume .resume_left .resume_profile{width:100%;height:280px}.resume .resume_left .resume_profile img{width:100%;height:100%}.resume .resume_left .resume_content{padding:0 25px}.resume .resume_right ul li .info,.resume .title{margin-bottom:20px}.resume .resume_left .bold{color:#fff}.resume .resume_left .regular,.resume .resume_left ul li .data{color:#b1eaff}.resume .resume_item{padding:25px 0;border-bottom:2px solid #b1eaff}.resume .resume_left .resume_item:last-child,.resume .resume_right .resume_item:last-child{border-bottom:0}.resume .resume_left ul li{display:block;margin-bottom:10px;align-items:center}.resume .resume_left ul li:last-child,.resume .resume_right ul li:last-child .info{margin-bottom:0}.resume .resume_left ul li .icon{width:35px;height:35px;background:#fff;color:#0bb5f4;border-radius:50%;margin-right:15px;font-size:16px;position:relative}.resume .icon i,.resume .resume_right .resume_hobby ul li i{position:absolute;top:50%;left:50%;transform:translate(-50%,-50%)}.resume .resume_left .resume_skills ul li{display:block;margin-bottom:10px;color:#b1eaff;justify-content:space-between;align-items:center}.resume .resume_left .resume_skills ul li .skill_name{width:25%}.resume .resume_left .resume_skills ul li .skill_progress{width:60%;margin:0 5px;height:5px;background:#009fd9;position:relative}.resume .resume_left .resume_skills ul li .skill_per{width:15%}.resume .resume_left .resume_skills ul li .skill_progress span{position:absolute;top:0;left:0;height:100%;background:#fff}.resume .resume_left .resume_social .semi-bold{color:#fff;margin-bottom:3px}.resume .resume_right{width:520px;background:#fff;padding:25px}.resume .resume_right .bold{color:#0bb5f4}.resume .resume_right .resume_education ul,.resume .resume_right .resume_work ul{padding-left:40px;overflow:hidden}.resume .resume_right ul li{position:relative}.resume .resume_right ul li .date{font-size:16px;font-weight:500;margin-bottom:15px}.resume .resume_right .resume_education ul li:before,.resume .resume_right .resume_work ul li:before{content:\"\";position:absolute;top:5px;left:-25px;width:6px;height:6px;border-radius:50%;border:2px solid #0bb5f4}.resume .resume_right .resume_education ul li:after,.resume .resume_right .resume_work ul li:after{content:\"\";position:absolute;top:14px;left:-21px;width:2px;height:115px;background:#0bb5f4}.resume .resume_right .resume_hobby ul{display:block;justify-content:space-between}.resume .resume_right .resume_hobby ul li{width:80px;height:80px;border:2px solid #0bb5f4;border-radius:50%;position:relative;color:#0bb5f4}.resume .resume_right .resume_hobby ul li i{font-size:30px}.resume .resume_right .resume_hobby ul li:before{content:\"\";position:absolute;top:40px;right:-52px;width:50px;height:2px;background:#0bb5f4}.resume .resume_right .resume_hobby ul li:last-child:before{display:none}";  
                var cssData = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.ParseStyleSheet(testStylesheet, true);

                var pdf = PdfGenerator.GeneratePdf(path, PdfSharp.PageSize.A4, cssData: cssData);

                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.Save(ms);

                    FileInputDto fileInput = new()
                    {
                        FileName = $"svlqb_cv_{AbpSession.UserId}.pdf",
                        TenantId = AbpSession.TenantId.Value,
                        CreatedAt = DateTime.Now,
                        FileCategory = "UserProfile",
                        IsUniqueFileName = false,
                        IsUniqueFolder = false
                    };

                    using (var fileService = _fileServiceFactory.Get())
                    {
                        var fileDto = await fileService.Object.CreateOrUpdate(ms.ToArray(), fileInput);
                        using (var uow = _unitOfWork.Begin())
                        {

                            job.FileCVUrl = StringCipher.Instance.Decrypt(fileDto.FileUrl);
                            await _jobApplicationRepo.UpdateAsync(job);
                            uow.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
