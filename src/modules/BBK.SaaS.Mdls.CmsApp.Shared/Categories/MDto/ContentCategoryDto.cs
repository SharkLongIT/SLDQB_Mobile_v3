using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.Categories.MDto
{
	public class ContentCategoryDto : AuditedEntityDto<long>
	{
		public long? ParentId { get; set; }

		public Guid UnqueId { get; set; }

		public string Code { get; set; }

		public string DisplayName { get; set; }

		public string Slug { get; set; }

		public long UsedCount { get; set; }

		public long ViewedCount { get; set; }

		public List<ArticleListViewDto> Articles { get; set; } = new List<ArticleListViewDto>();

		public static implicit operator ContentCategoryDto(CmsCat a)
		{
			return a == null ? null : new ContentCategoryDto
			{
				Id = a.Id,
				Slug = a.Slug,
				Code = a.Code,
				UnqueId = a.UnqueId,
				DisplayName = a.DisplayName,
				ParentId = a.ParentId,
			};
		}
	}
}