using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace BBK.SaaS.Mdls.Cms.UrlRecords.Dto
{
	public class UrlRecordListViewDto : Entity<long>
	{
		public int EntityId { get; set; }

		public string EntityName { get; set; }

		public string Slug { get; set; }

		public bool IsActive { get; set; }

		public int LanguageId { get; set; }

		public long ViewedCount { get; set; }
	}
}
