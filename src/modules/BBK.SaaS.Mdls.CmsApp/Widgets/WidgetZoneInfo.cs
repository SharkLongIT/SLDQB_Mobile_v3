using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms.Widgets
{
	[Serializable]
	public class WidgetZoneInfo
	{
		/// <summary>
		/// TenantId for this widget.
		/// TenantId is null if this widget is not Tenant level.
		/// </summary>
		public int? TenantId { get; set; }

		/// <summary>
		/// Unique name of the widget.
		/// </summary>
		public Guid? UniqueId { get; set; }

		/// <summary>
		/// Name of the widget zone.
		/// </summary>
		public string ZoneName { get; set; }

		public int OrderIndex { get; set; } = 0;

		//public int WidgetId { get; set; }

		public List<WidgetInfo> Widgets { get; set; }

		/// <summary>
		/// Creates a new <see cref="WidgetZoneInfo"/> object.
		/// </summary>
		public WidgetZoneInfo()
		{
			Widgets = new List<WidgetInfo>();
		}

		/// <summary>
		/// Creates a new <see cref="WidgetZoneInfo"/> object.
		/// </summary>
		/// <param name="tenantId">TenantId for this widget. TenantId is null if this widget is not Tenant level.</param>
		/// <param name="uniqueId">Unique name of the widget</param>
		/// <param name="zoneName">Title of the widget</param>
		public WidgetZoneInfo(int? tenantId, string zoneName) : this()
		{
			TenantId = tenantId;
			ZoneName = zoneName;
			//WidgetId = widgetId;
		}
	}

	[Serializable]
	public class WidgetInfo
	{
		///// <summary>
		///// TenantId for this widget.
		///// TenantId is null if this widget is not Tenant level.
		///// </summary>
		//public int? TenantId { get; set; }

		///// <summary>
		///// Id for this widget.
		///// </summary>
		public int WidgetId { get; set; }

		/// <summary>
		/// Unique name of the widget.
		/// </summary>
		public Guid? UniqueId { get; set; }

		//public int OrderIndex { get; set; }

		/// <summary>
		/// Title of the widget.
		/// </summary>
		public string Title { get; set; }

		public string HTMLContent { get; set; }

		public int OrderIndex { get; set;} = 0;

		/// <summary>
		/// Creates a new <see cref="WidgetInfo"/> object.
		/// </summary>
		public WidgetInfo()
		{

		}

		public WidgetInfo(int widgetId, string title, string htmlContent)
		{
			WidgetId = widgetId;
			Title = title;
			HTMLContent = htmlContent;
		}

		/// <summary>
		/// Creates a new <see cref="WidgetInfo"/> object.
		/// </summary>
		/// <param name="tenantId">TenantId for this widget. TenantId is null if this widget is not Tenant level.</param>
		/// <param name="uniqueId">Unique name of the widget</param>
		/// <param name="title">Title of the widget</param>
		public WidgetInfo(Guid? uniqueId, string title, string htmlContent)
		{
			UniqueId = uniqueId;
			Title = title;
			HTMLContent = htmlContent;
		}
	}
}
