using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Net.Mail;
using Abp.UI;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Net;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace BBK.SaaS.Mdls.Profile.Contacts
{
    public class ContactAppService : SaaSAppServiceBase, IContactAppService
    {
        private readonly IRepository<Contact,long> _contacrepository;
        private readonly IEmailSender _emailSender;
        private IHostingEnvironment _Environment;
        public ContactAppService(IRepository<Contact, long> contacrepository, IEmailSender emailSender , IHostingEnvironment Environment) 
        { 
            _contacrepository = contacrepository;
            _emailSender = emailSender;
            _Environment = Environment;
        }

        public async Task<PagedResultDto<ContactDto>> GetAll(ContactSearch input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
               
                var query = _contacrepository
                            .GetAll().AsNoTracking().OrderByDescending(x=>x.CreationTime)
                            .WhereIf(input.Status.HasValue, x => x.Status.Equals(input.Status))
                            .WhereIf(!string.IsNullOrEmpty(input.StartDay), x => x.CreationTime >= DateTime.Parse(input.StartDay) && x.CreationTime <= DateTime.Parse(input.EndDay))
                            .Select(x => new ContactDto
                            {
                                Id = x.Id,
                                Description = x.Description,
                                FullName = x.FullName,
                                Email = x.Email,
                                Phone = x.Phone,
                                Status = x.Status,
                                CreationTime = x.CreationTime,
                                Answer = x.Answer,
                            }).ToList();

              

                var Count = query.Count();
                return new PagedResultDto<ContactDto>(
                    Count,
                    query.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<ContactDto>> GetAllOfMe(ContactSearch input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }

                var query = _contacrepository
                            .GetAll()
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreationTime)
                            .Where(x=>x.CreatorUserId == AbpSession.UserId.Value)
                            .WhereIf(input.Status.HasValue, x => x.Status.Equals(input.Status))
                            .WhereIf(!string.IsNullOrEmpty(input.Search), x=>x.Description.ToLower().Contains(input.Search.ToLower()))
                            .Select(x => new ContactDto
                            {
                                Id = x.Id,
                                Description = x.Description,
                                FullName = x.FullName,
                                Email = x.Email,
                                Phone = x.Phone,
                                Status = x.Status,
                                CreationTime = x.CreationTime,
                                Answer = x.Answer,
                                LastModificationTime = x.LastModificationTime,
                            }).ToList();



                var Count = query.Count();
                return new PagedResultDto<ContactDto>(
                    Count,
                    query.ToList()
                    );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<long> Create(ContactDto input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                input.Answer = "";
                Contact newItemId = ObjectMapper.Map<Contact>(input);
                var newId = await _contacrepository.InsertAndGetIdAsync(newItemId);
                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task<long> Update(ContactDto input)
        {
            try
            {
                var list = _contacrepository.GetAll();
                Contact Contact = list.FirstOrDefault(x => x.Id == input.Id);
                Contact.Status = true;
                Contact.Answer = input.Answer;
                await _contacrepository.UpdateAsync(Contact);
                return input.Id;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async void SendMail(ContactDto input)
        {
            var webRootPath = this._Environment.WebRootPath;
            int tenantId = AbpSession.TenantId ?? 1;
            string path = "";
            path = webRootPath + Path.DirectorySeparatorChar.ToString() + "data\\tenants\\" + tenantId + "\\EmailTemplates" + Path.DirectorySeparatorChar.ToString() + "sendMailCV.html";
            path = File.ReadAllText(path);
            path = path.Replace("{{TenNguoiNhan}}", input.FullName);
            path = path.Replace("{{Answer}}", input.Answer);
            //int tenantId = AbpSession.TenantId ?? 1;
            //var fileMgr = new FileMgr() { TenantId = tenantId, FilePath = "/sendMailCV.html", FileCategory = "EmailTemplates" };
           

            //path = File.ReadAllText(fileMgr.FilePath);
            //path = path.Replace("{{TenNguoiNhan}}", input.FullName);

            await Update(input);


            //Send a notification email
            await _emailSender.SendAsync(
                to: input.Email,
                subject: "Trả lời câu hỏi!",
                body: path,
                isBodyHtml: true
            );


        }

        public async Task<ContactDto> GetById(NullableIdDto<long> input)
        {
            try
            {
                ContactDto contactDto = new ContactDto();
                if (input.Id.HasValue)
                {
                    var contact = _contacrepository.Get(input.Id.Value);
                    contactDto = ObjectMapper.Map<ContactDto>(contact);
                }

                return contactDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
