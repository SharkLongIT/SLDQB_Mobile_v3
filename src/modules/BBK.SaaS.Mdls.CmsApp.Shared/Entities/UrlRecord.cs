using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	[Table("AppUrlRecords", Schema = SaaSCmsConsts.DefaultSchema)]

	public class UrlRecord : CreationAuditedEntity<long>, IMustHaveTenant
	{
		/// <summary>
		/// Gets or sets a value the tenant id
		/// </summary>
		public int TenantId { get; set; }

		/// <summary>
		/// Gets or sets the entity identifier
		/// </summary>
		public long EntityId { get; set; }

		[Required]
		[MaxLength(400)]
		/// <summary>
		/// Gets or sets the entity name
		/// </summary>
		public string EntityName { get; set; }

		[Required]
		[MaxLength(400)]
		/// <summary>
		/// Gets or sets the slug
		/// </summary>
		public string Slug { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether the record is active
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the language identifier
		/// </summary>
		public int LanguageId { get; set; }

		public long ViewedCount { get; set; }
	}
}
