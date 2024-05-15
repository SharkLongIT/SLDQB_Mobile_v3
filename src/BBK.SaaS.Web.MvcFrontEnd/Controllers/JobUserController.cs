using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Controllers
{
    public class JobUserController : SaaSControllerBase
    {
        private readonly IRecruitmentAppService _recruitmentAppSerVice;
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        private readonly IRecruiterAppService _recruiterAppService;
        private readonly IApplicationRequestAppService _applicationRequestAppService;
        public JobUserController(IRecruitmentAppService recruitmentAppSerVice,
            IGeoUnitAppService geoUnitAppService,
            ICatUnitAppService catUnitAppService,
            IPerRequestSessionCache perRequestSessionCache,
            IRecruiterAppService recruiterAppService,
            IApplicationRequestAppService applicationRequestAppService)
        {
            _recruitmentAppSerVice = recruitmentAppSerVice;
            _geoUnitAppService = geoUnitAppService;
            _catUnitAppService = catUnitAppService;
            _perRequestSessionCache = perRequestSessionCache;
            _recruiterAppService = recruiterAppService;
            _applicationRequestAppService = applicationRequestAppService;
        }
        public async Task<IActionResult> Index()
        {
            #region Danh mục kinh nghiệm làm việc 
            var CatUnitWorkExp = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Kinh nghiệm làm việc"));
            if (CatUnitWorkExp != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitWorkExp.Id);
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

        public async Task<IActionResult> ViewRecruitment(long Id)
        {
            var dto = await _recruitmentAppSerVice.GetDetail(Id);
            var usercurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
          
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


            if (usercurrent != null)
            {
                if (usercurrent.User != null)
                {
                    if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                    {
                        RecruitmentDto.IsRecuiters = true;
                    }
                    if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                    {
                        RecruitmentDto.IsCandidate = true;
                    }
                }

            }
            RecruimentInput recruiterSearch = new RecruimentInput();
            var rewcruimentDto = await _recruitmentAppSerVice.GetAllByAllUser(recruiterSearch);

            ViewBag.RecruimentAll = rewcruimentDto.Items.Where(x => x.Recruiter.UserId == RecruitmentDto.Recruiter.UserId && x.Id != Id).Take(5).ToList();

            return View(RecruitmentDto);
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
            viewModel.Count = viewModels.Where(x => x.Recruiter.UserId == input.RecruiterUserId).Count();

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


        public async Task<ActionResult> NVNVViewDetailRecruiter(long id)
        {
            var recruiterDto = await _recruiterAppService.GetRecruiterForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));
            var viewModel = ObjectMapper.Map<RecruiterDetailViewModel>(recruiterDto);
            return View(viewModel);
        }


        public async Task<IActionResult> Search(RecruimentsInput input, int? page = 0)
        {
            #region Danh mục kinh nghiệm làm việc 
            var CatUnitWorkExp = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Kinh nghiệm làm việc"));
            if (CatUnitWorkExp != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitWorkExp.Id);
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
            RecruimentInput recruimentInput = new RecruimentInput();
            var Salary = await _recruitmentAppSerVice.GetAllByAllUser(recruimentInput);
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

            #region tinh thanh
            var List = (await _geoUnitAppService.GetAll()).Where(x => x.ParentId == null);
            if (List != null)
            {

                List<SelectListItem> listItemsTinhThanh = new List<SelectListItem>();
                listItemsTinhThanh.AddRange(List.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsTinhThanh = listItemsTinhThanh;

            }
            #endregion
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
                recruiterSearch.SalaryMax = input.SalaryMax;
                if (input.Page > 1)
                {
                    recruiterSearch.SkipCount = input.PageSize;

                }
            }
            var rewcruimentDto = await _recruitmentAppSerVice.GetAllByAllUser(recruiterSearch);
            int limit = 12;
            int start;
            if (page > 0)
            {
                page = page;
            }
            else
            {
                page = 1;
            }
            start = (int)(page - 1) * limit;

            ViewBag.pageCurrent = page;

            int totalProduct = rewcruimentDto.Items.Count();

            ViewBag.totalProduct = totalProduct;

            ViewBag.numberPage = (int)Math.Ceiling((float)totalProduct / limit);





            var viewModels = ObjectMapper.Map<List<JobUserModel>>(rewcruimentDto.Items);
            GetAllJobOfUser viewModel = new GetAllJobOfUser();
            viewModel.Recruiment = viewModels.Skip(start).Take(limit).ToList();
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
            return View("Search", viewModel);
        }
    }
}
