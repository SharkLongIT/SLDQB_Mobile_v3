using System;
using Abp.Application.Services.Dto;

namespace BBK.SaaS.Mdls.Cms.Categories.Dto
{
	public class CmsCatDto : AuditedEntityDto<long>
	{
		public long? ParentId { get; set; }

		public Guid UnqueId { get; set; }

		public string Code { get; set; }

		public string DisplayName { get; set; }

		public string Slug { get; set; }

		public long UsedCount { get; set; }

		public long ViewedCount { get; set; }
	}
}