using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Net;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public class ReportJobApplication : SaaSAppServiceBase, IReportJobApplicaiton
    {
        private readonly FileServiceFactory _fileServiceFactory;

        public ReportJobApplication(  FileServiceFactory fileServiceFactory) { 
            _fileServiceFactory = fileServiceFactory;
        }
        public async Task<FileDto> ExportJobApplication(JobApplicationEditDto input)
        {

            int tenantId = AbpSession.TenantId ?? 1;
            var fileMgr = new FileMgr() { TenantId = tenantId, FilePath = "\\ProfileDataTemplate\\CandidateTemp.docx" };
            using (var profileImageService = _fileServiceFactory.Get())
            {
                var profileImage = await profileImageService.Object.Download(fileMgr);
                var File = Convert.FromBase64String(profileImage.ContentString);
                byte[] byteArray = Encoding.UTF8.GetBytes(profileImage.ContentString);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    MemoryStream streamContent = new MemoryStream(byteArray);
                    FileMgr fileInput = new FileMgr();
                    fileInput.FileName = fileMgr.FileName;
                    fileInput.TenantId = AbpSession.TenantId.Value;
                    fileInput.CreatedAt = DateTime.Now;

                  
                        fileInput = await profileImageService.Object.CreateOrUpdate(streamContent, fileInput);
                        fileInput.FileUrl = $"/file/get?c={HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(fileInput.FilePath))}";
                        fileInput.FilePath = StringCipher.Instance.Encrypt(fileInput.FilePath);

                }
            }
            return null;   
        }
    }
}
