using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Profile.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BBK.SaaS.Mdls.Profile.Reports.Dto.ReportDto;

namespace BBK.SaaS.Mdls.Profile.Reports
{
    public class ReportAppService : SaaSAppServiceBase, IReportAppService
    {
        private readonly IRepository<Recruitment, long> _Recruitment;
        private readonly IRepository<CatUnit, long> _CatUnit;
        private readonly IRepository<User, long> _User;
        private readonly IRepository<Article, long> _Article;
        private readonly IRepository<CmsCat, long> _CmsCat;
        private readonly IRepository<CmsCatArticle, long> _CmsCatArticle;
        private readonly IRepository<Recruitment, long> _RecruitmentRes;
        private readonly IRepository<JobApplication, long> _JobApplicationRes;
        private readonly IExportAppService _exportAppService;
        public ReportAppService(IRepository<Recruitment, long> Recruitment, IRepository<CatUnit, long> CatUnit, IRepository<User, long> User,
            IRepository<Article, long> Article, IRepository<CmsCatArticle, long> cmsCatArticle, IRepository<CmsCat, long> cmsCat, IRepository<Recruitment, long> recruitmentRes,
            IExportAppService exportAppService, IRepository<JobApplication, long> jobApplicationRes)
        {
            _CatUnit = CatUnit;
            _Recruitment = Recruitment;
            _User = User;
            _Article = Article;
            _CmsCatArticle = cmsCatArticle;
            _CmsCat = cmsCat;
            _RecruitmentRes = recruitmentRes;
            _exportAppService = exportAppService;
            _JobApplicationRes = jobApplicationRes;
        }

        #region get Data
        /// <summary>
        /// biểu đồ hoạt động
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ReportArray> GetReportWebsite(int year, int month)
        {
            ReportArray returnObject = new ReportArray();
            int day = DateTime.DaysInMonth(year, month);

            var UserList = (await _User.GetAllListAsync()).Where(x => x.CreationTime.Year == year && x.CreationTime.Month == month).ToList();
            for (int i = 1; i <= day; i++)
            {
                ReportList reportList = new ReportList();
                reportList.Date = i + "/" + month + "/" + year;
                reportList.CountRecruiment = UserList.Where(x => x.UserType.Equals(UserTypeEnum.Type1) && x.CreationTime.Day == i && x.CreationTime.Month == month && x.CreationTime.Year == year).Count();
                reportList.CountCandidate = UserList.Where(x => x.UserType.Equals(UserTypeEnum.Type2) && x.CreationTime.Day == i && x.CreationTime.Month == month && x.CreationTime.Year == year).Count();
                returnObject.ListReport.Add(reportList);
            }
            return returnObject;
        }

        /// <summary>
        /// biểu đồ tin tức
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public async Task<ReportArray> GetReportArticle(string StartTime, string EndTime)
        {
            ReportArray returnObject = new ReportArray();

            var CatArticle = _CmsCatArticle.GetAll().Include(x => x.Article)
                  .WhereIf(!string.IsNullOrEmpty(StartTime), x => x.Article.CreationTime >= DateTime.Parse(StartTime) && x.Article.CreationTime <= DateTime.Parse(EndTime)).ToList();

            var ArrayCat = (await _CmsCat.GetAllListAsync()).ToList();

            foreach (var CmsCat in ArrayCat)
            {
                ReportListArticle reportListArticle = new ReportListArticle();
                reportListArticle.Cat = CmsCat.DisplayName;
                reportListArticle.CountArticle = CatArticle.Where(x => x.CmsCatId == CmsCat.Id).Select(x => x.ArticleId).Count();
                returnObject.ReportListArticle.Add(reportListArticle);
            }

            return returnObject;

        }

        /// <summary>
        /// biểu đồ danh mục
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ReportArray> GetReportCat(string StartTime, string EndTime, int type)
        {
            // type = 1 : tin tuyển dụng
            // type = 2 : hồ sơ người lao động
            ReportArray reportArray = new ReportArray();

            var RecruitmentJob = (await _RecruitmentRes.GetAllListAsync()).WhereIf(!string.IsNullOrEmpty(StartTime), x => x.CreationTime >= DateTime.Parse(StartTime) && x.CreationTime <= DateTime.Parse(EndTime));
            var JobList = (await _JobApplicationRes.GetAllListAsync()).WhereIf(!string.IsNullOrEmpty(StartTime), x => x.CreationTime >= DateTime.Parse(StartTime) && x.CreationTime <= DateTime.Parse(EndTime));

            var Cat = (await _CatUnit.GetAllListAsync()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));

            if (Cat != null)
            {
                var ListCat = (await _CatUnit.GetAllListAsync()).Where(x => x.ParentId == Cat.Id).ToList();
                foreach (var Name in ListCat)
                {
                    ReportListCat reportList = new ReportListCat();
                    reportList.Cat = Name.DisplayName;
                    if (type == 1)
                    {
                        reportList.CountRecruiment = RecruitmentJob.Where(x => x.JobCatUnitId == Name.Id).Count();
                        //if (reportList.CountRecruiment != 0)
                        //{
                            reportArray.ReportListCat.Add(reportList);

                        //}
                    }
                    else
                    {
                        reportList.CountJob = JobList.Where(x => x.OccupationId == Name.Id).Count();
                        //if (reportList.CountJob != 0)
                        //{
                            reportArray.ReportListCat.Add(reportList);
                       // }
                    }
                }
            }
            return reportArray;
        }

        /// <summary>
        /// get bieu do tổng số tin tức theo từng ngày
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public async Task<ReportArray> GetReportArticleApex(string StartTime, string EndTime)
        {
            var selectedDates = Enumerable
                            .Range(0, int.MaxValue)
                            .Select(index => new DateTime?(DateTime.Parse(StartTime).AddDays(index)))
                            .TakeWhile(date => date <= DateTime.Parse(EndTime))
                            .ToList();

            ReportArray returnObject = new ReportArray();

            var CatArticle = _CmsCatArticle.GetAll().Include(x => x.Article)
                  .WhereIf(!string.IsNullOrEmpty(StartTime), x => x.Article.CreationTime >= DateTime.Parse(StartTime) && x.Article.CreationTime <= DateTime.Parse(EndTime)).ToList();
            foreach (var day in selectedDates)
            {
                ReportListArticle reportListArticle = new ReportListArticle();
                reportListArticle.Cat = day.Value.Date.ToShortDateString();
                reportListArticle.CountArticle = CatArticle.Where(x => x.CreationTime.ToShortDateString() == day.Value.Date.ToShortDateString()).Select(x => x.ArticleId).Count();
                returnObject.ReportListArticle.Add(reportListArticle);
            }
            return returnObject;

        }

        /// <summary>
        /// Biểu đồ số lượng từng danh mục theo từng ngày
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ReportArray> GetReportCatApex(string StartTime, string EndTime, int type)
        {
            // type = 1 : tin tuyển dụng
            // type = 2 : hồ sơ người lao động
            var selectedDates = Enumerable
                               .Range(0, int.MaxValue)
                               .Select(index => new DateTime?(DateTime.Parse(StartTime).AddDays(index)))
                               .TakeWhile(date => date <= DateTime.Parse(EndTime))
                               .ToList();




            ReportArray reportArray = new ReportArray();

            var RecruitmentJob = (await _RecruitmentRes.GetAllListAsync()).WhereIf(!string.IsNullOrEmpty(StartTime), x => x.CreationTime >= DateTime.Parse(StartTime) && x.CreationTime <= DateTime.Parse(EndTime));
            var JobList = (await _JobApplicationRes.GetAllListAsync()).WhereIf(!string.IsNullOrEmpty(StartTime), x => x.CreationTime >= DateTime.Parse(StartTime) && x.CreationTime <= DateTime.Parse(EndTime));

            var Cat = (await _CatUnit.GetAllListAsync()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));

            if (Cat != null)
            {
                var ListCat = (await _CatUnit.GetAllListAsync()).Where(x => x.ParentId == Cat.Id).ToList();
                foreach (var item in selectedDates)
                {
                    foreach (var Name in ListCat)
                    {
                        ReportListCat reportList = new ReportListCat();
                        reportList.Date = item.Value.Date.ToShortDateString();
                        reportList.Cat = Name.DisplayName;
                        if (type == 1)
                        {
                            reportList.CountRecruiment = RecruitmentJob.Where(x => x.JobCatUnitId == Name.Id && x.CreationTime.ToShortDateString().Equals(item.Value.Date.ToShortDateString())).Count();
                            if (reportList.CountRecruiment != 0)
                            {
                                reportArray.ReportListCat.Add(reportList);

                            }
                        }
                        else
                        {
                            reportList.CountJob = JobList.Where(x => x.OccupationId == Name.Id).Count();
                            if (reportList.CountJob != 0)
                            {
                                reportArray.ReportListCat.Add(reportList);
                            }
                        }
                    }
                }

            }
            return reportArray;
        }

        #endregion


        #region export

        /// <summary>
        /// export excel bieu do hoat dong
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportReportWebsite(int year, int month)
        {
            var res = await GetReportWebsite(year, month);

            return await _exportAppService.ExportForReportTask(year, month, res);

        }

        /// <summary>
        /// export excel bieu do so luong tin tuc
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportReportArticle(string StartTime, string EndTime)
        {
            var res = await GetReportArticle(StartTime, EndTime);
            return await _exportAppService.ExportForReportArticle(StartTime, EndTime, res);

        }


        /// <summary>
        /// export excel biểu đồ danh mục
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportReportCat(string StartTime, string EndTime, int Type)
        {
            var res = await GetReportCat(StartTime, EndTime, Type);
            return await _exportAppService.ExportForReportCat(StartTime, EndTime, Type, res);
        }

        /// <summary>
        /// export excel bieu do so luong tin tuc
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportReportArticleApex(string StartTime, string EndTime)
        {
            var res = await GetReportArticleApex(StartTime, EndTime);
            return await _exportAppService.ExportForReportArticleApex(StartTime, EndTime, res);

        }
        #endregion



    }
}
