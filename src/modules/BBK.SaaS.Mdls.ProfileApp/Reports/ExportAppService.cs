using Abp.Timing;
using BBK.SaaS.DataExporting.Excel.MiniExcel;
using BBK.SaaS.Dto;
using BBK.SaaS.Net;
using BBK.SaaS.Storage;
using Microsoft.AspNetCore.Hosting;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BBK.SaaS.Mdls.Profile.Reports.Dto.ReportDto;

namespace BBK.SaaS.Mdls.Profile.Reports
{
    public class ExportAppService : MiniExcelExcelExporterBase, IExportAppService
    {
        private IHostingEnvironment _Environment;
        public ExportAppService(ITempFileCacheManager tempFileCacheManager, IHostingEnvironment Environment, FileServiceFactory fileServiceFactory) : base(tempFileCacheManager)
        {
            _Environment = Environment;
        }



        /// <summary>
        /// Xuất excel biểu đồ hoạt động website
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="reportArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileDto> ExportForReportTask(int year, int month, ReportArray reportArray)
        {
            try
            {
                var webRootPath = this._Environment.WebRootPath;
                string path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\ReportTemplate" + Path.DirectorySeparatorChar.ToString() + "ReportWebsite.xlsx";

                FileInfo fileInfo = new FileInfo(path);
                FileDto fileDto = new FileDto("Thống kê hoạt động_" + Clock.Now.ToString("yyyyMMdd") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                var items = new List<Dictionary<string, object>>();

                using (var stream = new MemoryStream())
                {
                    var rows = MiniExcel.Query(path).ToList(); // get all the rows as an IEnumerable<dynamic>
                    foreach (var user in reportArray.ListReport)
                    {
                        items.Add(new Dictionary<string, object>()
                             {
                                //{"STT", user.STT},
                                {L("Số lượng người lao động"), user.CountCandidate},
                                {L("Số Lượng Nhà tuyển dụng"), user.CountRecruiment},
                                {L("Ngày hoạt động"), user.Date},

                            });

                    }



                    stream.SaveAs(items);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return CreateExcelPackage(fileDto.FileName, items);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// export  excel  biểu đồ tin tức
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="reportArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileDto> ExportForReportArticle(string StartTime, string EndTime, ReportArray reportArray)
        {
            try
            {
                var webRootPath = this._Environment.WebRootPath;
                string path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\ReportTemplate" + Path.DirectorySeparatorChar.ToString() + "ReportArticle.xlsx";

                FileInfo fileInfo = new FileInfo(path);

                FileDto fileDto = new FileDto("Thống kê tin tức_" + Clock.Now.ToString("yyyyMMdd") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                var items = new List<Dictionary<string, object>>();

                using (var stream = new MemoryStream())
                {
                    var rows = MiniExcel.Query(path).ToList(); // get all the rows as an IEnumerable<dynamic>

                    foreach (var user in reportArray.ReportListArticle)
                    {
                        items.Add(new Dictionary<string, object>()
                             {
                                //{"STT", user.STT},
                                {L("Danh mục nghề nghiệp"), user.Cat},
                                {L("Số lượng tin tức"), user.CountArticle},
                            });

                    }



                    stream.SaveAs(items);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return CreateExcelPackage(fileDto.FileName, items);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// export excel biểu đồ danh mục (k dùng)
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Type"></param>
        /// <param name="reportArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileDto> ExportForReportCat(string StartTime, string EndTime, int Type, ReportArray reportArray)
        {
            try
            {
                var webRootPath = this._Environment.WebRootPath;
                string path;
                if (Type == 1) // tin tuyen dung
                {
                    path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\ReportTemplate" + Path.DirectorySeparatorChar.ToString() + "ReportRecruiment.xlsx";
                }
                else
                {
                    // ho so nguoi lao dong
                    path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\ReportTemplate" + Path.DirectorySeparatorChar.ToString() + "ReportJob.xlsx";
                }

                FileInfo fileInfo = new FileInfo(path);

                FileDto fileDto = new FileDto("Thống kê số lượng theo danh mục_" + Clock.Now.ToString("yyyyMMdd") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                var items = new List<Dictionary<string, object>>();

                using (var stream = new MemoryStream())
                {
                    var rows = MiniExcel.Query(path).ToList(); // get all the rows as an IEnumerable<dynamic>

                    foreach (var user in reportArray.ReportListCat)
                    {
                        if (Type == 1)
                        {
                            items.Add(new Dictionary<string, object>()
                             {
                                //{"STT", user.STT},
                                {L("Danh mục nghề nghiệp"), user.Cat},
                                {L("Số lượng tin tuyển dụng"), user.CountRecruiment},
                            });
                        }
                        else
                        {
                            items.Add(new Dictionary<string, object>()
                             {
                                //{"STT", user.STT},
                                {L("Danh mục nghề nghiệp"), user.Cat},
                                {L("Số lượng hồ sơ"), user.CountJob},
                            });
                        }
                    }
                    stream.SaveAs(items);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return CreateExcelPackage(fileDto.FileName, items);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// export  excel  biểu đồ tin tức tổng số tin
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="reportArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileDto> ExportForReportArticleApex(string StartTime, string EndTime, ReportArray reportArray)
        {
            try
            {
                //var webRootPath = this._Environment.WebRootPath;
                //string path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\ReportTemplate" + Path.DirectorySeparatorChar.ToString() + "ReportArticle.xlsx";

                //FileInfo fileInfo = new FileInfo(path);

                FileDto fileDto = new FileDto("Thống kê tin tức_" + Clock.Now.ToString("yyyyMMdd") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                var items = new List<Dictionary<string, object>>();

                using (var stream = new MemoryStream())
                {
                    //var rows = MiniExcel.Query(path).ToList(); // get all the rows as an IEnumerable<dynamic>

                    foreach (var user in reportArray.ReportListArticle)
                    {
                        items.Add(new Dictionary<string, object>()
                             {
                                //{"STT", user.STT},
                                {L("Ngày"), user.Cat},
                                {L("Số lượng tin tức"), user.CountArticle},
                            });

                    }
                    stream.SaveAs(items);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return CreateExcelPackage(fileDto.FileName, items);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// Xuất excel biểu đồ hoạt động website theo năm
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="reportArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileDto> ExportForReportWebsiteByYear(int ToYear, int FromYear, ReportArray reportArray)
        {
            try
            {
                var webRootPath = this._Environment.WebRootPath;
                string path = webRootPath + Path.DirectorySeparatorChar.ToString() + "assets" + "\\ReportTemplate" + Path.DirectorySeparatorChar.ToString() + "ReportWebsite.xlsx";

                FileInfo fileInfo = new FileInfo(path);

                FileDto fileDto = new FileDto("Thống kê hoạt động_" + Clock.Now.ToString("yyyyMMdd") + ".xlsx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                var items = new List<Dictionary<string, object>>();

                using (var stream = new MemoryStream())
                {
                    var rows = MiniExcel.Query(path).ToList(); // get all the rows as an IEnumerable<dynamic>
                    foreach (var user in reportArray.ListReport)
                    {
                        items.Add(new Dictionary<string, object>()
                             {
                                //{"STT", user.STT},
                                {L("Số lượng người lao động"), user.CountCandidate},
                                {L("Số Lượng nhà tuyển dụng"), user.CountRecruiment},
                                {L("Thời gian"), user.Date},
                            });
                    }
                    stream.SaveAs(items);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return CreateExcelPackage(fileDto.FileName, items);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
