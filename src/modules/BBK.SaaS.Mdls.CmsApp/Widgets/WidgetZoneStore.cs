using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using BBK.SaaS.Mdls.Cms.Entities;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Widgets
{
	/// <summary>
	/// Used to perform widget database operations for a zone.
	/// </summary>
	public interface IWidgetZoneStore
	{
		/// <summary>
		/// Adds a widget zone setting to a zone.
		/// </summary>
		/// <param name="zone">Role</param>
		/// <param name="widgetZone">Widget zone setting info</param>
		Task AddWidgetAsync(string zoneName, WidgetInfo widget);

		/// <summary>
		/// Adds a widget zone setting to a zone.
		/// </summary>
		/// <param name="zone">Role</param>
		/// <param name="widgetZone">Widget zone setting info</param>
		Task AddWidgetInOrderedAsync(string zoneName, int widgetId, int orderIndex);

		/// <summary>
		/// Removes a widget zone setting from a zone.
		/// </summary>
		/// <param name="zone">Role</param>
		/// <param name="widgetZone">Widget zone setting info</param>
		Task RemoveWidgetAsync(string zoneName, WidgetInfo widget);

		/// <summary>
		/// Gets widget zone setting informations for a zone.
		/// </summary>
		/// <param name="zoneId">Role id</param>
		/// <returns>List of widget setting informations</returns>
		Task<IList<WidgetInfo>> GetWidgetsAsync(string zoneName);

		/// <summary>
		/// Deleted all widget settings for a zone.
		/// </summary>
		/// <param name="zone">Role</param>
		Task RemoveAllWidgetsAsync(string zoneName);

		/// <summary>
		/// Checks whether a role has a permission grant setting info.
		/// </summary>
		/// <param name="roleId">Role id</param>
		/// <param name="permissionGrant">Permission grant setting info</param>
		/// <returns></returns>
		Task<bool> HasWidgetAsync(string zoneName, WidgetInfo widgetZoneInfo);
	}

	public class WidgetZoneStore : IWidgetZoneStore, ITransientDependency
	{
		public ILogger Logger { get; set; }

		private readonly IRepository<WidgetMapping, long> _widgetZoneMappingRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		public WidgetZoneStore(IRepository<WidgetMapping, long> widgetZoneMappingRepository,
			IUnitOfWorkManager unitOfWorkManager)
		{
			_unitOfWorkManager = unitOfWorkManager;
			_widgetZoneMappingRepository = widgetZoneMappingRepository;
			Logger = NullLogger.Instance;

		}

		public virtual async Task AddWidgetAsync(string zoneName, WidgetInfo widgetInfo)
		{
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				if (await HasWidgetAsync(zoneName, widgetInfo))
				{
					return;
				}

				await _widgetZoneMappingRepository.InsertAsync(
					new WidgetMapping
					{
						TenantId = MultiTenancyConsts.DefaultTenantId,
						ZoneName = zoneName,
						OrderIndex = widgetInfo.OrderIndex,
						WidgetId = widgetInfo.WidgetId,
					});

				await _unitOfWorkManager.Current.SaveChangesAsync();
			});
		}

		public virtual async Task AddWidgetInOrderedAsync(string zoneName, int widgetId, int orderIndex)
		{
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				await _widgetZoneMappingRepository.InsertAsync(
					new WidgetMapping
					{
						TenantId = MultiTenancyConsts.DefaultTenantId,
						ZoneName = zoneName,
						OrderIndex = orderIndex,
						WidgetId = widgetId,
					});
			});
		}

		public async Task<IList<WidgetInfo>> GetWidgetsAsync(string zoneName)
		{
			return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				return (await _widgetZoneMappingRepository.GetAll().AsNoTracking().Where(p => p.ZoneName == zoneName)
					.OrderBy(p => p.OrderIndex)
					.Include(x => x.Widget).ToListAsync())
					.Select(p => new WidgetInfo(p.WidgetId, p.Widget.Title, p.Widget.HTMLContent))
					.ToList();
			});
		}

		public async Task RemoveAllWidgetsAsync(string zoneName)
		{
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				await _widgetZoneMappingRepository.DeleteAsync(
					widgetZone => widgetZone.ZoneName == zoneName
				);
			});
		}

		public async Task RemoveWidgetAsync(string zoneName, WidgetInfo widget)
		{
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				await _widgetZoneMappingRepository.DeleteAsync(
					widgetZone => widgetZone.ZoneName == zoneName &&
								widget.WidgetId == widget.WidgetId
				);
			});
		}

		public virtual async Task<bool> HasWidgetAsync(string zoneName, WidgetInfo widgetZoneInfo)
		{
			return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				return await _widgetZoneMappingRepository.FirstOrDefaultAsync(
					p => p.ZoneName == zoneName &&
						 p.WidgetId == widgetZoneInfo.WidgetId
				) != null;
			});
		}
	}
}
