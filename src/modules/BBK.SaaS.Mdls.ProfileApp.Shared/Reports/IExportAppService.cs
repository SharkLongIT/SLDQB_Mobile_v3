using Abp.Application.Services;
using BBK.SaaS.Dto;
using System.Threading.Tasks;
using static BBK.SaaS.Mdls.Profile.Reports.Dto.ReportDto;

namespace BBK.SaaS.Mdls.Profile.Reports
{
    public interface IExportAppService : IApplicationService
    {
        Task<FileDto> ExportForReportTask(int year, int month, ReportArray reportArray);
        Task<FileDto> ExportForReportArticle(string StartTime, string EndTime, ReportArray reportArray);
        Task<FileDto> ExportForReportCat(string StartTime, string EndTime, int Type, ReportArray reportArray);
        Task<FileDto> ExportForReportArticleApex(string StartTime, string EndTime, ReportArray reportArray);
    }
}
