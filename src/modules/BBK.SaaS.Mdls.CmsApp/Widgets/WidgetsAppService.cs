using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using BBK.SaaS.Authorization;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Widgets.Dto;
using BBK.SaaS.Storage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BBK.SaaS.Mdls.Cms.Widgets
{
	public interface IWidgetsAppService : IApplicationService
	{
		//Task<bool> GetSlug(string slug);
		Task<GetConfigWidgetForEditOutput> GetConfigWidgetForEdit(NullableIdDto<int> input);
		Task<List<WidgetTemplate>> GetWidgetTemplates();
		Task<WidgetTemplate> GetWidgetTemplate(string widgetTemplateName);
	}

	[AbpAuthorize(AppPermissions.Pages_Administration_CommFuncs)]
	public class WidgetsAppService : SaaSAppServiceBase, IWidgetsAppService
	{
		private readonly IRepository<Widget, int> _widgetRepo;
		private readonly IRepository<WidgetMapping, long> _widgetMappingRepo;
		private readonly FileServiceFactory _fileServiceFactory;

		public WidgetsAppService(IRepository<Widget, int> widgetRepo, IRepository<WidgetMapping, long> widgetMappingRepo, FileServiceFactory fileServiceFactory)
		{
			_widgetRepo = widgetRepo;
			_widgetMappingRepo = widgetMappingRepo;
			_fileServiceFactory = fileServiceFactory;

		}

		#region Admins API
		public async Task<PagedResultDto<WidgetListDto>> GetWidgetsAsync(GetWidgetsInput input)
		{

			var query = _widgetRepo.GetAll().AsNoTracking()
				.WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Title.Contains(input.Filter))
				.WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
				.WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);

			var count = await query.CountAsync();
			var widgets = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			return new PagedResultDto<WidgetListDto>(
				count,
				ObjectMapper.Map<List<WidgetListDto>>(widgets)
				);
		}

		public async Task<WidgetEditDto> CreateWidgetAsync(CreateWidgetInput input)
		{
			Widget widget = ObjectMapper.Map<Widget>(input);
			widget.TenantId = AbpSession.TenantId.Value;

			// TODO: Put into a manager
			await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				// TODO: will check with template name
				if (!string.IsNullOrEmpty(input.WidgetTemplateName))
				{
					List<WidgetTemplate> widgettemps = null;
					using (var fileService = _fileServiceFactory.Get())
					{
						//var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
						var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
						widgettemps = JsonConvert.DeserializeObject<List<WidgetTemplate>>(Encoding.UTF8.GetString(fileMgr.Content));

						if (widgettemps != null)
						{
							var widgettemp = widgettemps.FirstOrDefault(x => x.Name == input.WidgetTemplateName);

							widget.HTMLContent = widgettemp.Content;
							foreach (var i in input.Fields)
							{
								widget.HTMLContent = widget.HTMLContent.Replace($"#{i.Name}#", i.Value);
							}
						}
					}
				}

				await _widgetRepo.InsertAsync(widget);
				await UnitOfWorkManager.Current.SaveChangesAsync();
			});

			return ObjectMapper.Map<WidgetEditDto>(widget);
		}

		public async Task<WidgetEditDto> UpdateWidgetAsync(WidgetEditDto input)
		{
			// TODO: Put into a manager
			await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				var updatingItem = await _widgetRepo.GetAsync(input.Id);

				updatingItem.Title = input.Title;
				updatingItem.Published = input.Published;
				//updatingItem.OrderIndex = input.OrderIndex;
				updatingItem.HTMLContent = input.HTMLContent;

				await _widgetRepo.UpdateAsync(updatingItem);
				//await _slugRepository.InsertAsync(new UrlRecord() { EntityId = topic.Id, EntityName = nameof(Widgets), Slug = topic.Slug });
				await UnitOfWorkManager.Current.SaveChangesAsync();
			});

			return input;
		}

		public async Task<List<WidgetTemplate>> GetWidgetTemplates()
		{
			List<WidgetTemplate> widgettemps = null;
			using (var fileService = _fileServiceFactory.Get())
			{
				//var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
				var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
				widgettemps = JsonConvert.DeserializeObject<List<WidgetTemplate>>(Encoding.UTF8.GetString(fileMgr.Content));
			}

			return widgettemps;
		}

		public async Task<WidgetTemplate> GetWidgetTemplate(string widgetTemplateName)
		{
			WidgetTemplate widgettemp = null;
			using (var fileService = _fileServiceFactory.Get())
			{
				//var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
				var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
				var widgettemps = JsonConvert.DeserializeObject<List<WidgetTemplate>>(Encoding.UTF8.GetString(fileMgr.Content));

				widgettemp = widgettemps.FirstOrDefault(x => x.Name == widgetTemplateName);
			}

			return widgettemp;
		}

		[AbpAuthorize(AppPermissions.Pages_Administration_CommFuncs_Delete)]
		public async Task<bool> DeleteWidgetAsync(EntityDto<int> input)
		{
			var entity = await _widgetRepo.GetAsync(input.Id);
			await _widgetRepo.DeleteAsync(entity);

			return true;
		}

		public async Task<WidgetEditDto> GetWidgetForEditAsync(EntityDto<int> input)
		{
			var entity = await _widgetRepo.GetAsync(input.Id);
			var dto = ObjectMapper.Map<WidgetEditDto>(entity);

			return dto;
		}

		public async Task<GetConfigWidgetForEditOutput> GetConfigWidgetForEdit(NullableIdDto<int> input)
		{
			WidgetSelectDto widgetDto;
			List<string> displayedInZones = [];
			if (input.Id.HasValue)
			{
				var widget = await _widgetRepo.GetAsync(input.Id.Value);
				displayedInZones = await _widgetMappingRepo.GetAll().Where(item => item.Id == input.Id.Value).Distinct().Select(item => item.ZoneName).ToListAsync();
				widgetDto = ObjectMapper.Map<WidgetSelectDto>(widget);

			}
			else
			{
				widgetDto = new WidgetSelectDto();
			}

			string[] zoneNames = typeof(PublicWidgetZones)
					.GetProperties(BindingFlags.Public | BindingFlags.Static)
					.Where(property => property.PropertyType == typeof(string))
					.Select(property => property.GetValue(null) is string value ? value : null)
					.Where(item => item != null)
					.ToArray();



			//GetConfigWidgetForEditOutput output = new() { 
			//	WidgetZoneName = typeof(PublicWidgetZones)
			//		.GetProperties(BindingFlags.Public | BindingFlags.Static)
			//		.Where(property => property.PropertyType == typeof(string))
			//		.Select(property => property.GetValue(null) is string value ? value : null)
			//		.Where(item => item != null)
			//		.ToArray(),
			//	DisplayedInZones = await _widgetMappingRepo.GetAll().Where(item => item.Id == nullableIdDto.Id.Value).Distinct().Select(item => item.Name).ToListAsync()
			//};

			return new GetConfigWidgetForEditOutput()
			{
				Widget = widgetDto,
				WidgetZoneNames = zoneNames,
				DisplayedInZones = displayedInZones
			};
		}

		#endregion
		//public async Task<ListResultDto<CmsCatDto>> GetCmsCats()
		//{
		//	var categories = await _categoryRepository.GetAllListAsync();

		//	return new ListResultDto<CmsCatDto>(
		//		categories.Select(ou =>
		//		{
		//			var categoryDto = ObjectMapper.Map<CmsCatDto>(ou);
		//			return categoryDto;
		//		}).ToList());
		//}

		#region Frontend API

		#endregion
	}

	//public class FEWidgetsAppService: SaaSAppServiceBase, IWidgetsAppService
	//{

	//}
}
