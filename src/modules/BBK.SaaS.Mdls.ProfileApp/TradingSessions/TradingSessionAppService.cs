using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.IO.Extensions;
using Abp.UI;
using BBK.SaaS.Graphics;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.TradingSessions
{
    public class TradingSessionAppService : SaaSAppServiceBase, ITradingSessionAppService
    {
        private readonly IRepository<TradingSession, long> _tradingSessionService;
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IImageValidator _imageValidator;
        public TradingSessionAppService(IRepository<TradingSession, long> tradingSessionService, FileServiceFactory fileServiceFactory, IImageValidator imageValidator)
        {
            _tradingSessionService = tradingSessionService;
            _fileServiceFactory = fileServiceFactory;
            _imageValidator = imageValidator;
        }


        #region get tất cả

        public async Task<PagedResultDto<TradingSessionEditDto>> GetAll(TradingSessionSearch input)
        {
            var TradingList = _tradingSessionService.GetAll().OrderByDescending(x => x.CreationTime)
                .AsNoTracking()
                .Include(e => e.Province)
                .Include(e => e.District)
                .Include(e => e.Village)
                .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.NameTrading.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.StartTime >= DateTime.Parse(input.FromDate) && x.EndTime <= DateTime.Parse(input.ToDate))
                .WhereIf(input.WorkSite != null && input.WorkSite.Count != 0, x => input.WorkSite.Contains(x.ProvinceId))
                .WhereIf(input.Status.HasValue, x => input.Status == 2 ? x.StartTime > DateTime.Now :(input.Status == 1 ? x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now : x.StartTime < x.EndTime))
                .Select(x => new TradingSessionEditDto
                {
                    Id = x.Id,
                    NameTrading = x.NameTrading,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    VillageId = x.VillageId,
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Address = x.Address,
                    Description = x.Description,
                    CountRecruiterMax = x.CountRecruiterMax,
                    CountCandidateMax = x.CountCandidateMax,
                    Describe = x.Describe,
                    ImgUrl = x.ImgUrl,
                    CreationTime = x.CreationTime,
                })
               .ToList();


            return new PagedResultDto<TradingSessionEditDto>(
                     TradingList.Count(),
                     TradingList.ToList()
                     );
        }
        #endregion

        #region get phiên giao dịch sắp diễn ra
        public async Task<PagedResultDto<TradingSessionEditDto>> GetAllFuture(TradingSessionSearch input)
        {
            var TradingList = _tradingSessionService.GetAll().OrderByDescending(x => x.Id)
                .AsNoTracking()
                .Include(e => e.Province)
                .Include(e => e.District)
                .Include(e => e.Village)
                .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.NameTrading.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.StartTime >= DateTime.Parse(input.FromDate) && x.EndTime <= DateTime.Parse(input.ToDate))
                .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.ProvinceId))
                .Where(x => x.StartTime > DateTime.Now)
                .Select(x => new TradingSessionEditDto
                {
                    Id = x.Id,
                    NameTrading = x.NameTrading,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    VillageId = x.VillageId,
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Address = x.Address,
                    Description = x.Description,
                    CountRecruiterMax = x.CountRecruiterMax,
                    CountCandidateMax = x.CountCandidateMax,
                    Describe = x.Describe,
                    CreationTime = x.CreationTime,
                })
               .ToList();


            return new PagedResultDto<TradingSessionEditDto>(
                     TradingList.Count(),
                     TradingList.ToList()
                     );
        }

        #endregion

        #region get phiên giao dịch đang diễn ra
        public async Task<PagedResultDto<TradingSessionEditDto>> GetAllPresent(TradingSessionSearch input)
        {
            var TradingList = _tradingSessionService.GetAll().OrderByDescending(x => x.Id)
                .AsNoTracking()
                .Include(e => e.Province)
                .Include(e => e.District)
                .Include(e => e.Village)
                .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.NameTrading.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.StartTime >= DateTime.Parse(input.FromDate) && x.EndTime <= DateTime.Parse(input.ToDate))
                .Where(x => x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now)
                .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.ProvinceId))
                .Select(x => new TradingSessionEditDto
                {
                    Id = x.Id,
                    NameTrading = x.NameTrading,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    VillageId = x.VillageId,
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Address = x.Address,
                    Description = x.Description,
                    CountRecruiterMax = x.CountRecruiterMax,
                    CountCandidateMax = x.CountCandidateMax,
                    Describe = x.Describe,
                    CreationTime = x.CreationTime,
                })
               .ToList();


            return new PagedResultDto<TradingSessionEditDto>(
                     TradingList.Count(),
                     TradingList.ToList()
                     );
        }
        #endregion

        #region get phiên giao dịch đã diễn ra
        public async Task<PagedResultDto<TradingSessionEditDto>> GetAllPast(TradingSessionSearch input)
        {
            var TradingList = _tradingSessionService.GetAll().OrderByDescending(x => x.Id)
                .AsNoTracking()
                .Include(e => e.Province)
                .Include(e => e.District)
                .Include(e => e.Village)
                .WhereIf(!string.IsNullOrEmpty(input.Search), u => u.NameTrading.ToLower().Contains(input.Search.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(input.FromDate), x => x.StartTime >= DateTime.Parse(input.FromDate) && x.EndTime <= DateTime.Parse(input.ToDate))
                .WhereIf(input.WorkSite != null && input.WorkSite.Count > 0, x => input.WorkSite.Contains(x.ProvinceId))
                .Where(x => x.EndTime < DateTime.Now)
                .Select(x => new TradingSessionEditDto
                {
                    Id = x.Id,
                    NameTrading = x.NameTrading,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    VillageId = x.VillageId,
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Address = x.Address,
                    Description = x.Description,
                    CountRecruiterMax = x.CountRecruiterMax,
                    CountCandidateMax = x.CountCandidateMax,
                    Describe = x.Describe,
                    CreationTime = x.CreationTime,
                })
               .ToList();


            return new PagedResultDto<TradingSessionEditDto>(
                     TradingList.Count(),
                     TradingList.ToList()
                     );
        }
        #endregion


        #region tạo mới
        public async Task<long> Create(TradingSessionEditDto input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                else
                {
                    input.TenantId = AbpSession.TenantId.Value;
                }
                TradingSession newItemId = new TradingSession();
                newItemId.TenantId = input.TenantId;
                newItemId.NameTrading = input.NameTrading;
                newItemId.ProvinceId = input.ProvinceId;
                newItemId.DistrictId = input.DistrictId;
                newItemId.VillageId = input.VillageId;
                newItemId.Address = input.Address;
                newItemId.StartTime = input.StartTime;
                newItemId.EndTime = input.EndTime;
                newItemId.Description = input.Description;
                newItemId.CountCandidateMax = input.CountCandidateMax;
                newItemId.CountRecruiterMax = input.CountRecruiterMax;
                newItemId.Describe = input.Describe;
                var newId = await _tradingSessionService.InsertAndGetIdAsync(newItemId);

                var trading = _tradingSessionService.Get(newId);
                if (trading != null)
                {
                    if (!string.IsNullOrEmpty(input.ImgUrl) && input.ImgUrl.StartsWith("data:image/png"))
                    {
                        FileInputDto fileInput = new();
                        fileInput.FileName = $"Tradingimg_{AbpSession.TenantId.Value}_{newId}.webp";
                        fileInput.TenantId = AbpSession.TenantId.Value;
                        fileInput.CreatedAt = DateTime.Now;
                        fileInput.FileCategory = "CmsTradingSession";
                        fileInput.IsUniqueFileName = false;
                        fileInput.IsUniqueFolder = false;

                        using (var fileService = _fileServiceFactory.Get())
                        {
                            byte[] fileContent = Convert.FromBase64String(input.ImgUrl.Replace("data:image/png;base64,", ""));
                            using (var outputStream = new MemoryStream(_imageValidator.MakeWebp(fileContent, 800, 500)))
                            {
                                //var fileMgr = await fileService.Object.CreateOrUpdateImage(outputStream, fileInput);
                                var fileMgr = await fileService.Object.CreateOrUpdate(outputStream.GetAllBytes(), fileInput);
                                trading.ImgUrl = fileInput.FilePath;
                            }

                            // make thumbnail
                            fileInput.FileName = $"Tradingimg_{AbpSession.TenantId.Value}_{newId}_thumb.webp";
                            _ = fileService.Object.CreateOrUpdate(_imageValidator.MakeThumbnail(fileContent, 210, 160), fileInput);
                        }
                    }
                }
                await _tradingSessionService.UpdateAsync(trading);

                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region update
        public async Task<long> Update(TradingSessionEditDto input)
        {
            try
            {
                TradingSession TradingSession = await _tradingSessionService.FirstOrDefaultAsync(x => x.Id == input.Id);
                TradingSession.NameTrading = input.NameTrading;
                TradingSession.ProvinceId = input.ProvinceId;
                TradingSession.DistrictId = input.DistrictId;
                TradingSession.VillageId = input.VillageId;
                TradingSession.Address = input.Address;
                TradingSession.StartTime = input.StartTime;
                TradingSession.EndTime = input.EndTime;
                TradingSession.Description = input.Description;
                TradingSession.CountCandidateMax = input.CountCandidateMax;
                TradingSession.CountRecruiterMax = input.CountRecruiterMax;
                TradingSession.Describe = input.Describe;

                if (!string.IsNullOrEmpty(input.ImgUrl) && input.ImgUrl.StartsWith("data:image/png"))
                {
                    FileInputDto fileInput = new();
                    fileInput.FileName = $"Tradingimg_{AbpSession.TenantId.Value}_{input.Id}.webp";
                    fileInput.TenantId = AbpSession.TenantId.Value;
                    fileInput.CreatedAt = DateTime.Now;
                    fileInput.FileCategory = "CmsTradingSession";
                    fileInput.IsUniqueFileName = false;
                    fileInput.IsUniqueFolder = false;

                    using (var fileService = _fileServiceFactory.Get())
                    {
                        byte[] fileContent = Convert.FromBase64String(input.ImgUrl.Replace("data:image/png;base64,", ""));
                        using (var outputStream = new MemoryStream(_imageValidator.MakeWebp(fileContent, 800, 500)))
                        {
                            //var fileMgr = await fileService.Object.CreateOrUpdateImage(outputStream, fileInput);
                            var fileMgr = await fileService.Object.CreateOrUpdate(outputStream.GetAllBytes(), fileInput);
                            TradingSession.ImgUrl = fileInput.FilePath;
                        }

                        // make thumbnail
                        fileInput.FileName = $"Tradingimg_{AbpSession.TenantId.Value}_{input.Id}_thumb.webp";
                        _ = fileService.Object.CreateOrUpdate(_imageValidator.MakeThumbnail(fileContent, 210, 160), fileInput);
                    }
                }
                await _tradingSessionService.UpdateAsync(TradingSession);
                return input.Id.Value;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region xoá
        public async Task Delete(long? Id)
        {
            try
            {
                if (Id.HasValue)
                {
                    var Recruitment = _tradingSessionService.Get(Id.Value);
                    await _tradingSessionService.DeleteAsync(Recruitment);
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        #endregion

        #region getById
        public async Task<TradingSessionEditDto> GetById(NullableIdDto<long> input)
        {
            try
            {
                TradingSessionEditDto tradingSessionEditDto = new TradingSessionEditDto();
                if (input.Id.HasValue)
                {
                    var tradingSession = await _tradingSessionService.GetAll()
                        .Include(x => x.Province)
                        .Include(x => x.District)
                        .Include(x => x.Village)
                        .AsNoTracking()
                        .Where(x => x.Id == input.Id.Value)
                        .FirstOrDefaultAsync();
                    if (tradingSession != null)
                    {
                        tradingSessionEditDto = ObjectMapper.Map<TradingSessionEditDto>(tradingSessionEditDto);
                    }
                }

                return tradingSessionEditDto;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion  

        #region get tất cả

        public async Task<PagedResultDto<TradingSessionEditDto>> GetAllUnitOfWork()
        {
            using var uow = UnitOfWorkManager.Begin();

            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId.HasValue ? AbpSession.TenantId.Value : 1))
            {
                try
                {
                    var TradingList = _tradingSessionService.GetAll().OrderByDescending(x => x.CreationTime)
                .AsNoTracking()
                .Include(e => e.Province)
                .Include(e => e.District)
                .Include(e => e.Village)
                .Select(x => new TradingSessionEditDto
                {
                    Id = x.Id,
                    NameTrading = x.NameTrading,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    VillageId = x.VillageId,
                    Province = ObjectMapper.Map<GeoUnitDto>(x.Province),
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Address = x.Address,
                    Description = x.Description,
                    CountRecruiterMax = x.CountRecruiterMax,
                    CountCandidateMax = x.CountCandidateMax,
                    Describe = x.Describe,
                    ImgUrl = x.ImgUrl,
                    CreationTime = x.CreationTime,
                })
               .ToList();


                    return new PagedResultDto<TradingSessionEditDto>(
                             TradingList.Count(),
                             TradingList.ToList()
                             );
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    await uow.CompleteAsync();
                }
            }
        }
        #endregion
    }
}
