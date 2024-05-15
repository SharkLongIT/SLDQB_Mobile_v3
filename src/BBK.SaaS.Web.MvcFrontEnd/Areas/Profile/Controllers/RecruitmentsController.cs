using Abp.AspNetCore.Mvc.Authorization;
using Abp.HtmlSanitizer;
using Abp.UI;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Controllers;
using BBK.SaaS.Web.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    [AbpMvcAuthorize]
    public class RecruitmentsController : SaaSControllerBase
    {
        private readonly IRecruitmentAppService _recruitmentAppSerVice;
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        public RecruitmentsController(IRecruitmentAppService recruitmentAppSerVice, IGeoUnitAppService geoUnitAppService , ICatUnitAppService catUnitAppService, IPerRequestSessionCache perRequestSessionCache)
        {
            _recruitmentAppSerVice = recruitmentAppSerVice;
            _geoUnitAppService = geoUnitAppService;
            _catUnitAppService = catUnitAppService;
            _perRequestSessionCache = perRequestSessionCache;
        }

        public async Task<ActionResult> Recruitment()
        {

            #region chuyên môn kỹ thuật
            var CatUnit = ( await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
            if (CatUnit != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnit.Id).Result;
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKyThuat = listItemsKyThuat;

            }
            #endregion
            #region kinh nghiệm
            var CatUnitEX = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Kinh nghiệm làm việc"));
            if (CatUnitEX != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitEX.Id);
                List<SelectListItem> listItemsKinhNghiem = new List<SelectListItem>();
                listItemsKinhNghiem.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKinhNghiem = listItemsKinhNghiem;

            }
            #endregion
            #region hinh thuc lam viec
            var CatUnitFormOfWork = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Hình thức làm việc"));
            if (CatUnitFormOfWork != null)
            {
                var FOWParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id);
                List<SelectListItem> listItemsFOW = new List<SelectListItem>();
                listItemsFOW.AddRange(FOWParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsHinhThucLV = listItemsFOW;

            }
            #endregion

            return View();
        }

        public async Task<IActionResult> CreateRecruitment()
        {
            #region Chuyên môn kỹ thuật
            var CatUnit = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
            if (CatUnit != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnit.Id);
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKyThuat = listItemsKyThuat;

            }
            #endregion

            #region Bằng cấp
            var CatUnitDegree = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Bằng cấp"));
            if (CatUnitDegree != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitDegree.Id);
                List<SelectListItem> listItemsBangCap = new List<SelectListItem>();
                listItemsBangCap.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsBangCap = listItemsBangCap;

            }
            #endregion

            #region kinh nghiệm
            var CatUnitEX = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnitEX != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitEX.Id).Result;
                List<SelectListItem> listItemsKinhNghiem = new List<SelectListItem>();
                listItemsKinhNghiem.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKinhNghiem = listItemsKinhNghiem;

            }
            #endregion

            #region cấp bậc
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
            if (CatUnitRank != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id).Result;
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            #endregion

            #region hinh thuc lam viec
            var CatUnitFormOfWork = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Hình thức làm việc")).FirstOrDefault();
            if (CatUnitFormOfWork != null)
            {
                var FOWParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id);
                List<SelectListItem> listItemsFOW = new List<SelectListItem>();
                listItemsFOW.AddRange(FOWParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsHinhThucLV = listItemsFOW;

            }
            #endregion


            List<SelectListItem> listItemsGender = new List<SelectListItem>();

            listItemsGender.AddRange(Enum.GetValues(typeof(GenderEnum))
                 .Cast<GenderEnum>()
                 .Select(status =>
                     new SelectListItem
                     {
                         Text = EnumExtensions.GetDisplayName(status),
                         Value = ((int)status).ToString(),
                     })
             );
            ViewBag.listItemsGender = listItemsGender;
            return View();
        }

        public async Task<IActionResult> EditRecruitment(long Id)
        {
            var dto = await _recruitmentAppSerVice.GetDetail(Id);

            #region chuyên môn kỹ thuật
            var CatUnit = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
            if (CatUnit != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnit.Id);
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKyThuat = listItemsKyThuat;

            }
            #endregion

            #region Bằng cấp
            var CatUnitDegree = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Bằng cấp"));
            if (CatUnitDegree != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitDegree.Id);
                List<SelectListItem> listItemsBangCap = new List<SelectListItem>();
                listItemsBangCap.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsBangCap = listItemsBangCap;

            }
            #endregion

            #region kinh nghiệm
            var CatUnitEX = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnitEX != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitEX.Id).Result;
                List<SelectListItem> listItemsKinhNghiem = new List<SelectListItem>();
                listItemsKinhNghiem.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKinhNghiem = listItemsKinhNghiem;

            }
            #endregion

            #region cấp bậc
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
            if (CatUnitRank != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id).Result;
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            #endregion

            #region giới tính
            List<SelectListItem> listItemsGender = new List<SelectListItem>();

            listItemsGender.AddRange(Enum.GetValues(typeof(GenderEnum))
                 .Cast<GenderEnum>()
                 .Select(status =>
                     new SelectListItem
                     {
                         Text = EnumExtensions.GetDisplayName(status),
                         Value = ((int)status).ToString(),
                     })
             );
            ViewBag.listItemsGender = listItemsGender;
            #endregion

            #region hinh thuc lam viec
            var CatUnitFormOfWork = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Hình thức làm việc")).FirstOrDefault();
            if (CatUnitFormOfWork != null)
            {
                var FOWParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id);
                List<SelectListItem> listItemsFOW = new List<SelectListItem>();
                listItemsFOW.AddRange(FOWParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsHinhThucLV = listItemsFOW;

            }
            #endregion

            RecruitmentDto RecruitmentDto = new RecruitmentDto()
            {
                Id = Id,
                Title = dto.Title,
                JobCatUnitId = dto.JobCatUnitId,
                Status = dto.Status,
                FormOfWork = dto.FormOfWork,
                Degree = dto.Degree,
                Experience = dto.Experience,
                Rank = dto.Rank,
                MinAge = dto.MinAge,
                MaxAge = dto.MaxAge,
                GenderRequired = dto.GenderRequired,
                NumberOfRecruits = dto.NumberOfRecruits,
                ProbationPeriod = dto.ProbationPeriod,
                DeadlineSubmission = dto.DeadlineSubmission,
                MaxSalary = dto.MaxSalary,
                MinSalary = dto.MinSalary,
                NecessarySkills = dto.NecessarySkills,
                JobDesc = dto.JobDesc,
                JobRequirementDesc = dto.JobRequirementDesc,
                BenefitDesc = dto.BenefitDesc,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                RecruiterId = dto.RecruiterId,
                ProvinceId = dto.ProvinceId,
                DistrictCode = dto.DistrictCode,
                WorkAddress = dto.WorkAddress,
            };
            return View(RecruitmentDto);
        }


        public async Task<IActionResult> ViewRecruitment(long Id)
        {
            var dto = await _recruitmentAppSerVice.GetDetail(Id);
         
            RecruitmentDto RecruitmentDto = new RecruitmentDto()
            {
                Id = Id,
                Title = dto.Title,
                JobCatUnitId = dto.JobCatUnitId,
                Status = dto.Status,
                FormOfWork = dto.FormOfWork,
                Degree = dto.Degree,
                Experience = dto.Experience,
                Rank = dto.Rank,
                MinAge = dto.MinAge,
                MaxAge = dto.MaxAge,
                GenderRequired = dto.GenderRequired,
                NumberOfRecruits = dto.NumberOfRecruits,
                ProbationPeriod = dto.ProbationPeriod,
                DeadlineSubmission = dto.DeadlineSubmission,
                MaxSalary = dto.MaxSalary,
                MinSalary = dto.MinSalary,
                NecessarySkills = dto.NecessarySkills,
                JobDesc = dto.JobDesc,
                JobRequirementDesc = dto.JobRequirementDesc,
                BenefitDesc = dto.BenefitDesc,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                AddressName = dto.AddressName,
                Recruiter = dto.Recruiter,
                Experiences = dto.Experiences,
                JobCatUnitName = dto.JobCatUnitName,
                FormOfWorks = dto.FormOfWorks,
            };
            RecruimentInput recruiterSearch = new RecruimentInput();
            var rewcruimentDto = await _recruitmentAppSerVice.GetAllByAllUser(recruiterSearch);

            ViewBag.RecruimentAll = rewcruimentDto.Items.Where(x => x.Recruiter.UserId == RecruitmentDto.Recruiter.UserId && x.Id != Id).Take(5).ToList();
            return View(RecruitmentDto);
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Create(RecruitmentDto model)
        {
            try
            {
                var Recruiter = await _perRequestSessionCache.GetRecruiter();
                model.RecruiterId = Recruiter.Id;
                await _recruitmentAppSerVice.Create(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<long> Delete(long Id)
        {
            try
            {
                await _recruitmentAppSerVice.DeleteDoc(Id);
                return Id;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Update(RecruitmentDto model)
        {
            try
            {
                await _recruitmentAppSerVice.Update(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        public async Task<IActionResult> JobUser()
        {
            #region Danh mục kinh nghiệm làm việc 
            var CatUnitWorkExp = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnitWorkExp != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitWorkExp.Id).Result;
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.Experiences = listItemsKyThuat;

            }
            #endregion

            #region hình thức

            var CatUnitFormOfWork = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Hình thức làm việc")); if (CatUnitFormOfWork != null)
                if (CatUnitFormOfWork != null)
                {
                    var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id);
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.listItemFormOfWork = listItems;

                }

            #endregion

            #region Bằng cấp
            var CatUnitLiteracy = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Bằng cấp"));
            if (CatUnitLiteracy != null)
                if (CatUnitLiteracy != null)
                {
                    var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id);
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.listItemsCatUnitLiteracy = listItems;

                }
            #endregion
            #region cấp bậc
            var CatUnitPositionsId = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc")); if (CatUnitPositionsId != null)
                if (CatUnitPositionsId != null)
                {
                    var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitPositionsId.Id);
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.listItemPositions = listItems;

                }
            #endregion

            #region chuyên môn kỹ thuật
            var CatUnit = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
            if (CatUnit != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnit.Id);
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKyThuat = listItemsKyThuat;

            }
            #endregion

            #region mức lương
            var Salary = await _recruitmentAppSerVice.GetAllBy();
            List<SelectListItem> listItemsMucLuong = new List<SelectListItem>();
            listItemsMucLuong.AddRange(Salary.Items.Select(x => new SelectListItem
            {
                Text = x.MinSalary.ToString(),
                Value = x.MinSalary.ToString(),
            }));
            ViewBag.MucLuong = listItemsMucLuong;

            List<SelectListItem> listItemsMucLuongMax = new List<SelectListItem>();
            listItemsMucLuongMax.AddRange(Salary.Items.Select(x => new SelectListItem
            {
                Text = x.MaxSalary.ToString(),
                Value = x.MaxSalary.ToString(),
            }));
            ViewBag.MucLuongMax = listItemsMucLuongMax;
            #endregion



            return View();
        }

        public async Task<JsonResult> GetJobUser(RecruimentsInput input)
        {
            RecruimentInput recruiterSearch = new RecruimentInput();

            if (input != null)
            {
                recruiterSearch.Take = input.PageSize;
                recruiterSearch.Paging = input.Page;
                recruiterSearch.Filtered = input.Filtered;
                recruiterSearch.Experience = input.Experience;
                recruiterSearch.Rank = input.Rank;
                recruiterSearch.Job = input.Job;
                recruiterSearch.Degree = input.Degree;
                recruiterSearch.WorkSite = input.WorkSite;
                recruiterSearch.Salary = input.Salary;
                if (input.Page > 1)
                {
                    recruiterSearch.SkipCount = input.PageSize;

                }
            }
            int limit = 0;

            if (input.PageSize <= 0)
            {
                limit = input.Page;
            }
            int start;
            if (input.Page > 0)
            {
                input.Page = input.Page;
            }
            else
            {
                input.Page = 1;
            }
            start = (int)(input.Page - 1) * limit;
            ViewBag.pageCurrent = input.Page;

            var rewcruimentDto = await _recruitmentAppSerVice.GetAllByAllUser(recruiterSearch);

            var viewModels = ObjectMapper.Map<List<JobUserModel>>(rewcruimentDto.Items);

            GetAllJobOfUser viewModel = new GetAllJobOfUser();
            viewModel.Recruiment = viewModels.Where(x => x.Recruiter.UserId == input.RecruiterUserId).Skip(recruiterSearch.SkipCount * (recruiterSearch.Paging - 1)).Take(input.PageSize).ToList();
            viewModel.Count = rewcruimentDto.TotalCount;

            var userCurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            if (userCurrent.User != null)
            {
                if (userCurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                {
                    viewModel.IsRecuiters = true;
                }
                if (userCurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                {
                    viewModel.IsCandidate = true;
                }
            }
            return Json(viewModel);
        }


        public async Task<JsonResult> GetAll(RecruimentsInput input)
        {
            var output = await _recruitmentAppSerVice.GetAll(input);
            return Json(output);
        }
        public async Task<JsonResult> GetAllGeo()
        {
            var output = await _geoUnitAppService.GetAll();
            return Json(output);
        } 
        
        public async Task<JsonResult> GetChildrenGeoUnit(long Id)
        {
            var output = await _geoUnitAppService.GetChildrenGeoUnit(Id);
            return Json(output);
        }
        public async Task<JsonResult> GetDetail(long Id)
        {
            var output = await _recruitmentAppSerVice.GetDetail(Id);
            return Json(output);
        }

    }
}
