using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Entities
{
	[Table("AppRecruiterFiles", Schema = SaaSProfileConsts.DefaultSchema)]
	public class RecruiterFile: FullAuditedEntity<long>
	{
		public long RecruitmentId { get; set; }

		[MaxLength(SaaSConsts.MaxSingleLineLength)]
		public string FileName { get; set; }
	}
}
