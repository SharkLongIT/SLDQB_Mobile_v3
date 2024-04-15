using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace BBK.SaaS.Mdls.Cms.Medias.Dto
{
	public class MediaFolderDto : AuditedEntityDto<long>
	{
		public long? ParentId { get; set; }

		public string Code { get; set; }

		public string DisplayName { get; set; }

		public int ItemCount { get; set; }
	}

	public class MediaFolderInfo
	{
		public List<NameValueDto<long>> Path { get; set; } = new List<NameValueDto<long>>();
		public List<NameValueDto<long>> ChildFolders { get; set; } = new List<NameValueDto<long>>();
	}
}