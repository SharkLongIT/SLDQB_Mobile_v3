using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	[Table("AppWidgets", Schema = SaaSCmsConsts.DefaultSchema)]
	public class Widget : CreationAuditedEntity<int>, IMayHaveTenant
	{
		/// <summary>
		/// Gets or sets a value the tenant id
		/// </summary>
		public int? TenantId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the entity is published
		/// </summary>
		public bool Published { get; set; }

		//public int OrderIndex { get; set; }

		public virtual int WidgetType { get; set; }

		public virtual Guid UnqueId { get; set; } = SequentialGuidGenerator.Instance.Create(SequentialGuidGenerator.SequentialGuidDatabaseType.PostgreSql);

		[MaxLength(SaaSConsts.MaxSingleLineLength)]
		public string Title { get; set; }

		[MaxLength(SaaSConsts.MaxContent)]
		public string HTMLContent { get; set; }

	}

	//[Table("AppWidgetZones", Schema = SaaSCmsConsts.DefaultSchema)]
	//public class WidgetZone : CreationAuditedEntity<int>, IMayHaveTenant
	//{
	//	/// <summary>
	//	/// Gets or sets a value the tenant id
	//	/// </summary>
	//	public int? TenantId { get; set; }

	//	[MaxLength(SaaSConsts.MaxCodeLineLength)]
	//	public string Name { get; set; }
	//}

	[Table("AppWidgetMappings", Schema = SaaSCmsConsts.DefaultSchema)]
	public class WidgetMapping : Entity<long>, IMayHaveTenant
	{
		//
		// Summary:
		//     TenantId of this entity.
		public virtual int? TenantId { get; set; }

		[MaxLength(SaaSConsts.MaxCodeLineLength)]
		public string ZoneName { get; set; }


		public virtual int OrderIndex { get; set; } = 0;

		//
		// Summary:
		//     Id of the Abp.Organizations.OrganizationUnit.
		public virtual int WidgetId { get; set; }

		[ForeignKey(nameof(WidgetId))]
		public virtual Widget Widget { get; set; }

	}
}
