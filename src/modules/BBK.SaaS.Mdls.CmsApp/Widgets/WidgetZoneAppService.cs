using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Widgets.Dto;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Widgets
{
	public interface IWidgetZoneAppService : IApplicationService
	{
		//Task<bool> GetSlug(string slug);
		//Task<GetConfigWidgetForEditOutput> GetConfigWidgetForEdit(NullableIdDto<int> input);
		//Task<IReadOnlyList<WidgetZoneInfo>> GetWidgetZonesAsync(string[] zoneFilter);
		Task<WidgetZoneInfo> GetWidgetZoneInfo(string zoneName);
	}

	public class WidgetZoneAppService : SaaSAppServiceBase, IWidgetZoneAppService
	{
		private readonly IWidgetZoneManager WidgetZoneManager;
		private readonly IRepository<WidgetMapping, long> _widgetZoneMappingRepository;
		private readonly IRepository<Widget, int> _widgetRepository;
		private readonly IWidgetZoneStore _widgetZoneStore;


		public WidgetZoneAppService(IRepository<WidgetMapping, long> widgetZoneMappingRepository,
			IRepository<Widget, int> widgetRepository, IWidgetZoneManager widgetZoneManager, IWidgetZoneStore widgetZoneStore)
		{
			_widgetZoneMappingRepository = widgetZoneMappingRepository;
			_widgetRepository = widgetRepository;
			WidgetZoneManager = widgetZoneManager;
			_widgetZoneStore = widgetZoneStore;
		}

		public async Task<IReadOnlyList<WidgetZoneInfo>> GetWidgetZonesAsync(string[] zoneFilter)
		{
			string[] AllZoneNames = typeof(PublicWidgetZones)
					.GetProperties(BindingFlags.Public | BindingFlags.Static)
					.Where(property => property.PropertyType == typeof(string))
					.Select(property => property.GetValue(null) is string value ? value : null)
					.Where(item => item != null)
					.ToArray();

			IEnumerable<string> zoneNames = null;

			if (zoneFilter.Length > 0)
			{
				zoneNames = AllZoneNames.Where(x => zoneFilter.Contains(x));
			}
			else
			{
				zoneNames = AllZoneNames.ToList();
			}

			List<WidgetZoneInfo> widgetZones = new List<WidgetZoneInfo>();
			foreach (string zoneName in zoneNames)
			{
				WidgetZoneInfo widgetZoneInfo = new WidgetZoneInfo(AbpSession.TenantId, zoneName);
				var existedZoneInfor = (await WidgetZoneManager.GetWidgetsInZoneAsync(zoneName));

				if (existedZoneInfor != null && existedZoneInfor.Widgets.Count > 0)
				{
					widgetZoneInfo.Widgets = (await WidgetZoneManager.GetWidgetsInZoneAsync(zoneName)).Widgets;
				}
				widgetZones.Add(widgetZoneInfo);
			}

			return widgetZones;
		}

		public async Task<WidgetZoneInfo> GetWidgetZoneInfo(string zoneName)
		{
			var AllZoneNames = PublicWidgetZones.GetAllZoneNames();
			string zoneNameVar = AllZoneNames.Where(x => x == zoneName).FirstOrDefault();

			//WidgetZoneInfo widgetZoneInfo = new WidgetZoneInfo(AbpSession.TenantId, zoneName);
			//var widgetZoneInfo = await WidgetZoneManager.GetWidgetsInZoneAsync(zoneNameVar);

			var widgetZoneInfo = new WidgetZoneInfo(AbpSession.TenantId, zoneNameVar);

			var widgets = await _widgetZoneStore.GetWidgetsAsync(zoneNameVar);
			widgetZoneInfo.Widgets = widgets.ToList();

			//if (existedZoneInfor != null && existedZoneInfor.Widgets.Count > 0)
			//{
			//	widgetZoneInfo.Widgets = existedZoneInfor.Widgets;
			//}

			//// for testing purpose
			//widgetZoneInfo.Widgets.Add(new WidgetInfo()
			//{
			//	WidgetId = 1,
			//	Title = $"Maybe me is an advertisement",
			//	OrderIndex = 0,
			//	HTMLContent = "OK OK OK"
			//});

			//widgetZoneInfo.Widgets.Add(new WidgetInfo()
			//{
			//	WidgetId = 1,
			//	Title = $"Maybe me is an event links",
			//	OrderIndex = 0,
			//	HTMLContent = "yeah yeah"
			//});

			return widgetZoneInfo;
		}

		public async Task<PagedResultDto<NameValueDto>> FindWidgets(FindWidgetsInput input)
		{
			//var widgetIdsInOrganizationUnit = _widgetZoneMappingRepository.GetAll()
			//    .Where(uou => uou.ZoneName == input.zoneName)
			//    .Select(uou => uou.WidgetId);

			var query = _widgetRepository.GetAll()
				.WhereIf(!input.Filter.IsNullOrWhiteSpace(), u => u.Title.Contains(input.Filter));

			var widgetCount = await query.CountAsync();
			var users = await query
				.OrderBy(u => u.CreationTime)
				.PageBy(input)
				.ToListAsync();

			return new PagedResultDto<NameValueDto>(
				widgetCount,
				users.Select(u =>
					new NameValueDto(
						u.Title,
						u.Id.ToString()
					)
				).ToList()
			);
		}

		public async Task AddWidgetsToZone(WidgetsToZoneInput input)
		{
			//await WidgetZoneManager.UpdateWidgetsToZoneAsync(input.ZoneName, input.WidgetIds.Select(x => new WidgetInfo(x, null, null)));
			await WidgetZoneManager.InsertWidgetsToZoneAsync(input.ZoneName, input.WidgetIds.Select(x => new WidgetInfo(x, null, null)));
		}

		public async Task SaveWidgetZoneInfo(WidgetsToZoneInput input)
		{
			//saveWidgetZoneInfo

			await WidgetZoneManager.UpdateWidgetsToZoneAsync(input.ZoneName, input.WidgetIds.ToList());

		}
	}
}
