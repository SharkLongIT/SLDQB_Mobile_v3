using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using BBK.SaaS.Authorization;
using System.Linq.Dynamic.Core;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using BBK.SaaS.Authorization.Roles;
using BBK.SaaS.MultiTenancy;
using Abp.UI;
using Microsoft.AspNetCore.Mvc.Filters;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Medias.Dto;
using System;
using Abp.Timing;
using BBK.SaaS.Security;
using System.Web;
using System.IO;
using Abp.Collections.Extensions;

namespace BBK.SaaS.Mdls.Cms.Medias
{
	//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders)]
	public class MediasMgrAppService : SaaSAppServiceBase, IMediasMgrAppService
	{
		private readonly MediaFolderManager _mediaFolderManager;
		private readonly IRepository<MediaFolder, long> _mediaFolderRepository;
		private readonly IRepository<Media, long> _mediaRepository;

		public MediasMgrAppService(
			 MediaFolderManager MediaFolderManager,
			 IRepository<MediaFolder, long> MediaFolderRepository,
			 IRepository<Media, long> mediaRepository
			 )
		{
			_mediaFolderManager = MediaFolderManager;
			_mediaFolderRepository = MediaFolderRepository;
			_mediaRepository = mediaRepository;
		}

		public async Task<ListResultDto<MediaFolderDto>> GetMediaFolders()
		{
			var mediaFolders = await _mediaFolderRepository.GetAllListAsync();

			var mediaInFolderCounts = await _mediaRepository.GetAll()
				.GroupBy(x => x.FolderId)
				.Select(groupedFolders => new
				{
					mediaFolderId = groupedFolders.Key,
					count = groupedFolders.Count()
				}).ToDictionaryAsync(x => x.mediaFolderId ?? 0, y => y.count);

			return new ListResultDto<MediaFolderDto>(
				 mediaFolders.Select(ou =>
				 {
					 var mediaFolderDto = ObjectMapper.Map<MediaFolderDto>(ou);
					 mediaFolderDto.ItemCount = mediaInFolderCounts.ContainsKey(ou.Id)
						? mediaInFolderCounts[ou.Id]
						: 0;
					 return mediaFolderDto;
				 }).ToList());
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders_ManageOrganizationTree)]
		public async Task<ListResultDto<MediaFolderDto>> GetRootMediaFolders()
		{
			var mediaFolders = await _mediaFolderRepository.GetAll()
				 .Where(x => x.ParentId == null).ToListAsync();

			return new ListResultDto<MediaFolderDto>(
				 mediaFolders.Select(ou =>
				 {
					 var MediaFolderDto = ObjectMapper.Map<MediaFolderDto>(ou);
					 return MediaFolderDto;
				 }).ToList());
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders_ManageOrganizationTree)]
		public async Task<ListResultDto<MediaFolderDto>> GetChildrenMediaFolder(long id)
		{
			var mediaFolders = await _mediaFolderRepository.GetAll()
				 .Where(x => x.ParentId == id).ToListAsync();

			return new ListResultDto<MediaFolderDto>(
				 mediaFolders.Select(ou =>
				 {
					 var MediaFolderDto = ObjectMapper.Map<MediaFolderDto>(ou);

					 return MediaFolderDto;
				 }).ToList());
		}

		public async Task<MediaFolderInfo> GetMediaFolder(long? folderId)
		{
			MediaFolderInfo mediaFolderInfo = new MediaFolderInfo();

			if (folderId.HasValue)
			{
				var mediaFolder = await _mediaFolderRepository.GetAsync(folderId.Value);
				mediaFolderInfo.Path.Add(new NameValueDto<long>() { Name = mediaFolder.DisplayName, Value = mediaFolder.Id });


				var splittedCode = mediaFolder.Code.Split('.');
				if (splittedCode.Length > 1)
				{
					string parentCode = string.Empty;
					for (int i = 1; i < splittedCode.Length; i++)
					{
						parentCode = splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
						var parentFolder = await _mediaFolderRepository
							//.GetAll()
							//.AsNoTracking()
							//.Where(x => x.Code == parentCode)
							//.FirstOrDefaultAsync();
							.FirstOrDefaultAsync(x => x.Code == parentCode);

						mediaFolderInfo.Path.Insert(0, new NameValueDto<long>() { Name = parentFolder.DisplayName, Value = parentFolder.Id });
						parentCode = parentFolder.Code;
					}
				}
			}

			List<MediaFolder> mediaFolders = await _mediaFolderManager.FindChildrenAsync(folderId);
			foreach (var mediaFolder in mediaFolders)
			{
				if (mediaFolder.Id == folderId)
				{
					mediaFolderInfo.Path.Add(new NameValueDto<long>() { Name = mediaFolder.DisplayName, Value = mediaFolder.Id });
				}
				else
				{
					mediaFolderInfo.ChildFolders.Add(new NameValueDto<long>() { Name = mediaFolder.DisplayName, Value = mediaFolder.Id });
				}
			}

			return mediaFolderInfo;
		}

		public async Task<PagedResultDto<MediaDto>> GetMediaFolderFiles(GetMediasInput input)
		{
			var query = _mediaRepository.GetAll()
				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), media => media.Filename.Contains(input.Filter))
				.Where(media => media.FolderId == input.FolderId);

			var totalCount = await query.CountAsync();
			var items = await query.OrderByDescending(x => x.CreationTime).PageBy(input).ToListAsync();

			return new PagedResultDto<MediaDto>(
				totalCount,
				items.Select(item =>
				{
					var dto = ObjectMapper.Map<MediaDto>(item);
					dto.Modified = item.LastModificationTime ?? item.CreationTime;
					//dto.RelativePath = item.PublicUrl;
					dto.PublicUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(item.PublicUrl));
					dto.ThumbUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(GetThumbnailImage(item.PublicUrl)));

					return dto;
				}).ToList()
				);

			//var query = from ouRole in _organizationUnitRoleRepository.GetAll()
			//            join ou in _organizationUnitRepository.GetAll() on ouRole.OrganizationUnitId equals ou.Id
			//            join role in _roleManager.Roles on ouRole.RoleId equals role.Id
			//            where ouRole.OrganizationUnitId == input.Id
			//            select new
			//            {
			//                ouRole,
			//                role
			//            };

			//var totalCount = await query.CountAsync();
			//var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			//return new PagedResultDto<MediaDto>(
			//    totalCount,
			//    items.Select(item =>
			//    {
			//        var organizationUnitRoleDto = ObjectMapper.Map<MediaDto>(item.role);
			//        organizationUnitRoleDto.AddedTime = item.ouRole.CreationTime;
			//        return organizationUnitRoleDto;
			//    }
			//    ).ToList());
			return new PagedResultDto<MediaDto>(
				10, new List<MediaDto>() { new() { Filename = "abc.txt", Size = "124", Title = "la la", ContentType = "image/png", Modified = DateTime.Now } }
				);
		}

		private string GetThumbnailImage(string publicUrl)
		{
			if (string.IsNullOrEmpty(publicUrl)) return string.Empty;

			string fileNameOnly = Path.GetFileNameWithoutExtension(publicUrl);
			string fileExt = Path.GetExtension(publicUrl);
			string filePath = Path.GetDirectoryName(publicUrl);

			return $"{filePath}\\{fileNameOnly}_thumb{fileExt}";
		}

		public async Task DeleteMedia(EntityDto<long> input)
		{
			await _mediaRepository.DeleteAsync(input.Id);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders_ManageOrganizationTree)]
		public async Task<MediaFolderDto> CreateMediaFolder(CreateMediaFolderInput input)
		{
			var MediaFolder = new MediaFolder(AbpSession.TenantId, input.DisplayName, input.ParentId);

			await _mediaFolderManager.CreateAsync(MediaFolder);
			await CurrentUnitOfWork.SaveChangesAsync();

			return ObjectMapper.Map<MediaFolderDto>(MediaFolder);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders_ManageOrganizationTree)]
		public async Task<MediaFolderDto> UpdateMediaFolder(UpdateMediaFolderInput input)
		{
			var MediaFolder = await _mediaFolderRepository.GetAsync(input.Id);

			MediaFolder.DisplayName = input.DisplayName;

			await _mediaFolderManager.UpdateAsync(MediaFolder);

			return await CreateMediaFolderDto(MediaFolder);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders_ManageOrganizationTree)]
		public async Task<MediaFolderDto> MoveMediaFolder(MoveMediaFolderInput input)
		{
			await _mediaFolderManager.MoveAsync(input.Id, input.NewParentId);

			return await CreateMediaFolderDto(
				 await _mediaFolderRepository.GetAsync(input.Id)
			);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_MediaFolders_ManageOrganizationTree)]
		public async Task DeleteMediaFolder(EntityDto<long> input)
		{
			//await _userMediaFolderRepository.DeleteAsync(x => x.MediaFolderId == input.Id);
			//await _MediaFolderRoleRepository.DeleteAsync(x => x.MediaFolderId == input.Id);
			await _mediaFolderManager.DeleteAsync(input.Id);
		}

		public async Task<List<MediaFolderDto>> GetAll()
		{
			var mediaFolders = await _mediaFolderRepository.GetAllListAsync();
			return ObjectMapper.Map<List<MediaFolderDto>>(mediaFolders);
		}

		private async Task<MediaFolderDto> CreateMediaFolderDto(MediaFolder MediaFolder)
		{
			var dto = ObjectMapper.Map<MediaFolderDto>(MediaFolder);
			//dto.MemberCount =
			//    await _userMediaFolderRepository.CountAsync(uou => uou.MediaFolderId == MediaFolder.Id);
			return dto;
		}

		public async Task SaveMediaAsync(MediaDto input)
		{
			// TODO: should put into a manager
			using (var uow = UnitOfWorkManager.Begin())
			{
				Media media = ObjectMapper.Map<Media>(input);
				media.TenantId = AbpSession.TenantId.Value;
				await _mediaRepository.InsertAsync(media);

				await uow.CompleteAsync();
			}
		}
	}
}