using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace BBK.SaaS.Mdls.Cms.Topics.Dto
{
	public class TopicListDto : AuditedEntityDto<long>
	{
		//public long Id { get; set; }
		public string Title { get; set; }
		public bool Published { get; set; }
		public string Slug { get; set; }
		public bool IsPasswordProtected { get; set; }
	}
}
