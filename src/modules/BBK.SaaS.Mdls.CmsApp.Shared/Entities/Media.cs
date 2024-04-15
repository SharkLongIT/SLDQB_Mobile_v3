using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Storage;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	[Table("AppMedias", Schema = SaaSCmsConsts.DefaultSchema)]
	public class Media : AuditedEntity<long>, IMayHaveTenant
	{
		/// <summary>
		/// TenantId of this entity.
		/// </summary>
		public virtual int? TenantId { get; set; }

		public virtual Guid UnqueId { get; set; } = SequentialGuidGenerator.Instance.Create(SequentialGuidGenerator.SequentialGuidDatabaseType.PostgreSql);

		///// <summary>
		///// Gets/sets the optional folder id.
		///// </summary>
		//public Guid? FolderId { get; set; }
		public long? FolderId { get; set; }

		/// <summary>
		/// Gets/sets the optional folder.
		/// </summary>
		[ForeignKey("FolderId")]
		public MediaFolder Folder { get; set; }

		/// <summary>
		/// Gets/sets the media type.
		/// </summary>
		public MediaType Type { get; set; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		[Required]
		[StringLength(128)]
		public string Filename { get; set; }

		/// <summary>
		/// Gets/sets the content type.
		/// </summary>
		[Required]
		[StringLength(256)]
		public string ContentType { get; set; }

		/// <summary>
		/// Gets/sets the optional title.
		/// </summary>
		[StringLength(128)]
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the optional alt text.
		/// </summary>
		[StringLength(128)]
		public string AltText { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		[StringLength(512)]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the file size in bytes.
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// Gets/sets the public url.
		/// </summary>
		public string PublicUrl { get; set; }

		/// <summary>
		/// Gets/sets the optional width. This only applies
		/// if the media asset is an image.
		/// </summary>
		public int? Width { get; set; }

		/// <summary>
		/// Gets/sets the optional height. This only applies
		/// if the media asset is an image.
		/// </summary>
		public int? Height { get; set; }
	}
}