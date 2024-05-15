using Abp.Application.Services;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static BBK.SaaS.Mdls.Profile.Reports.Dto.ReportDto;

namespace BBK.SaaS.Mdls.Profile.Reports
{
    public interface IReportAppService : IApplicationService
    {
        Task<ReportArray> GetReportWebsite(int year, int month);
        Task<ReportArray> GetReportArticle(string StartTime, string EndTime);
        Task<ReportArray> GetReportCat(string StartTime, string EndTime,int Type);

        Task<FileDto> ExportReportWebsite(int year, int month);
        Task<FileDto> ExportReportArticle(string StartTime, string EndTime);
        Task<FileDto> ExportReportCat(string StartTime, string EndTime, int Type);
        Task<ReportArray> GetReportArticleApex(string StartTime, string EndTime);

        Task<ReportArray> GetReportWebsiteByYear(int ToYear, int FromYear);
        Task<FileDto> ExportReportWebsiteByYear(int ToYear, int FromYear);

        Task<ReportArray> GetAllReportArticle();
    }
}
