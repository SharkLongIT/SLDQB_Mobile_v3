using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using BBK.SaaS.Mdls.Cms.Entities;
using NUglify.JavaScript.Syntax;
using Z.Expressions.Compiler.Shared;

namespace BBK.SaaS.Mdls.Cms.Widgets
{
	public interface IWidgetZoneManager
	{
		Task InsertWidgetsToZoneAsync(string zoneName, IEnumerable<WidgetInfo> widgets);
		Task UpdateWidgetsToZoneAsync(string zoneName, List<int> widgetIds);
		Task<WidgetZoneInfo> GetWidgetsInZoneAsync(string zoneName);
		Task ClearWidgetZoneCacheItemAsync(string zoneName);
	}

	public class WidgetZoneManager : IWidgetZoneManager, ISingletonDependency /*: DomainService*/
	{
		public IAbpSession AbpSession { get; set; }

		private readonly ITypedCache<string, WidgetZoneInfo> _widgetZoneCache;
		private readonly IWidgetZoneStore _widgetZoneStore;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		public WidgetZoneManager(ICacheManager cacheManager, IWidgetZoneStore widgetZoneStore, IUnitOfWorkManager unitOfWorkManager)
		{
			_widgetZoneStore = widgetZoneStore;
			_unitOfWorkManager = unitOfWorkManager;

			_widgetZoneCache = cacheManager.GetTenantWidgetZonesCache();
			AbpSession = NullAbpSession.Instance;

		}

		#region Public Methods
		public virtual async Task InsertWidgetsToZoneAsync(string zoneName, IEnumerable<WidgetInfo> widgets)
		{
			//await ClearWidgetZoneCacheItemAsync(zoneName);
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				//await _widgetZoneStore.RemoveAllWidgetsAsync(zoneName);

				foreach (var widgetInfo in widgets)
				{
					await _widgetZoneStore.AddWidgetAsync(zoneName, widgetInfo);
				}
			});
		}

		public virtual async Task UpdateWidgetsToZoneAsync(string zoneName, List<int> widgetIds)
		{
			//var currentWidgets = await GetWidgetZoneCacheItemAsync(zoneName);
			//var widgetZones = widgets.ToArray();
			await ClearWidgetZoneCacheItemAsync(zoneName);
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				await _widgetZoneStore.RemoveAllWidgetsAsync(zoneName);

				for (int i = 0; i < widgetIds.Count(); i++)
				{
					await _widgetZoneStore.AddWidgetInOrderedAsync(zoneName, widgetIds[i], i + 1);
				}
			});
		}

		public virtual async Task<IReadOnlyList<int>> GetWidgetKeysInZoneAsync(string zoneName)
		{
			var cacheItem = await GetWidgetZoneCacheItemAsync(zoneName);

			return cacheItem.Widgets.Select(x => x.WidgetId).ToList();
		}

		public virtual async Task<WidgetZoneInfo> GetWidgetsInZoneAsync(string zoneName)
		{
			return await GetWidgetZoneCacheItemAsync(zoneName);
		}
		
		public async Task ClearWidgetZoneCacheItemAsync(string zoneName)
		{
			var cacheKey = zoneName + "@" + (GetCurrentTenantId() ?? 1);
			await _widgetZoneCache.RemoveAsync(cacheKey);
		}

		public string GetWidgetZoneCacheKey(string zoneName, int tenantId = 1)
		{
			return zoneName + "@" + tenantId;
		}
		#endregion

		#region Private Methods
		private async Task<WidgetZoneInfo> GetWidgetZoneCacheItemAsync(string zoneName)
		{
			var cacheKey = zoneName + "@" + (GetCurrentTenantId() ?? 1);

			return await _widgetZoneCache.GetAsync(cacheKey, async () =>
			{
				WidgetZoneInfo widgetZoneInfo = new(GetCurrentTenantId(), zoneName);
				var widgets = await _widgetZoneStore.GetWidgetsAsync(zoneName);

				widgetZoneInfo.Widgets = widgets.ToList();

				return widgetZoneInfo;
			});
		}

		

		//private Dictionary<string, WidgetInfo> ConvertSettingInfosToDictionary(List<WidgetInfo> settingValues)
		//{
		//	var dictionary = new Dictionary<string, WidgetInfo>();

		//	foreach (var setting in allSettingDefinitions.Join(settingValues,
		//		definition => definition.Name,
		//		value => value.Name,
		//		(definition, value) => new
		//		{
		//			SettingDefinition = definition,
		//			SettingValue = value
		//		}))
		//	{
		//		if (setting.SettingDefinition.IsEncrypted)
		//		{
		//			setting.SettingValue.Value =
		//				SettingEncryptionService.Decrypt(setting.SettingDefinition, setting.SettingValue.Value);
		//		}

		//		dictionary[setting.SettingValue.Name] = setting.SettingValue;
		//	}

		//	return dictionary;
		//}

		private int? GetCurrentTenantId()
		{
			if (_unitOfWorkManager.Current != null)
			{
				return _unitOfWorkManager.Current.GetTenantId();
			}

			return AbpSession.TenantId;
		}

		#endregion
	}


	public static class CacheManagerWidgetZoneExtensions
	{
		public static ITypedCache<string, WidgetZoneInfo> GetTenantWidgetZonesCache(this ICacheManager cacheManager)
		{
			return cacheManager
				.GetCache<string, WidgetZoneInfo>(CmsCacheNames.TenantWidgetZones);
		}
	}

	public class WidgetZoneInfoCacheItemInvalidator :
		IEventHandler<EntityChangedEventData<WidgetMapping>>,
		ITransientDependency
	{
		private readonly ICacheManager _cacheManager;

		public WidgetZoneInfoCacheItemInvalidator(ICacheManager cacheManager)
		{
			_cacheManager = cacheManager;
		}

		public void HandleEvent(EntityChangedEventData<WidgetMapping> eventData)
		{
			var cacheKey = eventData.Entity.ZoneName + "@" + (eventData.Entity.TenantId ?? 1);
			_cacheManager.GetTenantWidgetZonesCache().Remove(cacheKey);
		}
	}
}
