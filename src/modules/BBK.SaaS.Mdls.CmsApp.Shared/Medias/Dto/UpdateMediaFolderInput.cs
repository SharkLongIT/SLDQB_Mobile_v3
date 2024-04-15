using System.ComponentModel.DataAnnotations;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.Medias.Dto
{
	public class UpdateMediaFolderInput
	{
		[Range(1, long.MaxValue)]
		public long Id { get; set; }

		[Required]
		[StringLength(MediaFolder.MaxDisplayNameLength)]
		public string DisplayName { get; set; }
	}
}