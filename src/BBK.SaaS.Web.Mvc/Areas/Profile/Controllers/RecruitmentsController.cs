using Abp.AspNetCore.Mvc.Authorization;
using Abp.UI;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
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
    public class RecruitmentsController : SaaSControllerBase
    {
        private readonly IRecruitmentAppService _recruitmentAppSerVice;
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        private readonly IRecruiterAppService _recruiterAppService;
        public RecruitmentsController(IRecruitmentAppService recruitmentAppSerVice, IGeoUnitAppService geoUnitAppService,
            ICatUnitAppService catUnitAppService, IPerRequestSessionCache perRequestSessionCache, IRecruiterAppService recruiterAppService)
        {
            _recruitmentAppSerVice = recruitmentAppSerVice;
            _geoUnitAppService = geoUnitAppService;
            _catUnitAppService = catUnitAppService;
            _perRequestSessionCache = perRequestSessionCache;
            _recruiterAppService = recruiterAppService;
        }

        [AbpMvcAuthorize]
        public ActionResult Recruitment()
        {

            #region chuyên môn kỹ thuật
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
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
            var CatUnitEX = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnit != null)
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

            return View();
        }

        [AbpMvcAuthorize]
        public async Task<IActionResult> CreateRecruitment()
        {
            //var List = _geoUnitAppService.GetAll();
            //if (List.Result.Count() == 0)
            //{
            //    await _geoUnitAppService.BuildDemoGeoAsync();
            //}


            #region Chuyên môn kỹ thuật
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
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

            #region Bằng cấp
            var CatUnitDegree = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
            if (CatUnitDegree != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitDegree.Id).Result;
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
            var CatUnitRank = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault();
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
                var FOWParent = _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id).Result;
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

        [AbpMvcAuthorize]
        public async Task<IActionResult> EditRecruitment(long Id)
        {
            var dto = await _recruitmentAppSerVice.GetDetail(Id);

            #region chuyên môn kỹ thuật
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
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

            #region Bằng cấp
            var CatUnitDegree = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
            if (CatUnitDegree != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitDegree.Id).Result;
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
            var CatUnitRank = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault();
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
                var FOWParent = _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id).Result;
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
            return PartialView(RecruitmentDto);
        }


        public async Task<IActionResult> ViewRecruitment(long Id)
        {
            var dto = await _recruitmentAppSerVice.GetDetail(Id);


            #region chuyên môn kỹ thuật
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
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


            #region Bằng cấp
            var CatUnitDegree = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
            if (CatUnitDegree != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitDegree.Id).Result;
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
            var CatUnitRank = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault();
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
			var rewcruimentDto = await _recruitmentAppSerVice.GetAllByNVNV(recruiterSearch);

			ViewBag.RecruimentAll = rewcruimentDto.Items.Where(x => x.Recruiter.UserId == RecruitmentDto.Recruiter.UserId && x.Id != Id).Take(5).ToList();
			return View(RecruitmentDto);
        }

        [AbpMvcAuthorize]
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

        [AbpMvcAuthorize]
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

        [AbpMvcAuthorize]
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

            var CatUnitFormOfWork = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Hình thức làm việc")).FirstOrDefault(); if (CatUnitFormOfWork != null)
                if (CatUnitFormOfWork != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id).Result;
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
            var CatUnitLiteracy = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
            if (CatUnitLiteracy != null)
                if (CatUnitLiteracy != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id).Result;
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
            var CatUnitPositionsId = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault(); if (CatUnitPositionsId != null)
                if (CatUnitPositionsId != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitPositionsId.Id).Result;
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
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
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

            #region mức lương
            var Salary = _recruitmentAppSerVice.GetAllBy();
            List<SelectListItem> listItemsMucLuong = new List<SelectListItem>();
            listItemsMucLuong.AddRange(Salary.Result.Items.Select(x => new SelectListItem
            {
                Text = x.MinSalary.ToString(),
                Value = x.MinSalary.ToString(),
            }));
            ViewBag.MucLuong = listItemsMucLuong;

            List<SelectListItem> listItemsMucLuongMax = new List<SelectListItem>();
            listItemsMucLuongMax.AddRange(Salary.Result.Items.Select(x => new SelectListItem
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
            //foreach (var item in viewModels)
            //{
            //    item.IsAppllied = await _applicationRequestAppService.CheckApplied(item.Id.Value);
            //}
            GetAllJobOfUser viewModel = new GetAllJobOfUser();
            viewModel.Recruiment = viewModels.Where(x => x.Recruiter.UserId == input.RecruiterUserId).Skip(recruiterSearch.SkipCount * (recruiterSearch.Paging - 1)).Take(input.PageSize).ToList();
            viewModel.Count = viewModels.Where(x => x.Recruiter.UserId == input.RecruiterUserId).Count();

            var userCurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            if (userCurrent != null)
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


        // NVNV
        public IActionResult NVNVRecruiment()
        {
            #region chuyên môn kỹ thuật
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
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
            var CatUnitEX = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnit != null)
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
            #region hinh thuc lam viec
            var CatUnitFormOfWork = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Hình thức làm việc")).FirstOrDefault();
            if (CatUnitFormOfWork != null)
            {
                var FOWParent = _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id).Result;
                List<SelectListItem> listItemsFOW = new List<SelectListItem>();
                listItemsFOW.AddRange(FOWParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsHinhThucLV = listItemsFOW;

            }
            #endregion

            #region Nhà tuyển dụng
            var Recruiter = _recruiterAppService.GetAll().Result;
            List<SelectListItem> listItemsRecruiter = new List<SelectListItem>();
            listItemsRecruiter.AddRange(Recruiter.Items.Select(x => new SelectListItem
            {
                Text = x.CompanyName,
                Value = x.Id.ToString(),
            }));
            ViewBag.listItemsRecruiter = listItemsRecruiter;

            #endregion

            return View();
        }

        public async Task<JsonResult> Active(RecruitmentDto model)
        {
            try
            {
                await _recruitmentAppSerVice.ActiveRecruiment(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

    }
}
