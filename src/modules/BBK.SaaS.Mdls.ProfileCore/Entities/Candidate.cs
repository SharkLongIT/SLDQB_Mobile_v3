using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.MultiTenancy.Payments.Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BBK.SaaS.Mdls.Profile.Entities
{
	[Table("AppCandidates", Schema = SaaSProfileConsts.DefaultSchema)]

	public class Candidate : FullAuditedEntity<long>, IMustHaveTenant
	{
		public int TenantId { get; set; }

		public long UserId { get; set; }

		[ForeignKey("UserId")]
		public User Account { get; set; }

		public bool Marital { get; set; }// Tình trạng    nhân

		public DateTime DateOfBirth { get; set; }

		public GenderEnum Gender { get; set; }

		[MaxLength(SaaSConsts.MaxUrlLength)]
		public string Address { get; set; } // so nha ten đường
        [MaxLength(SaaSConsts.MaxUrlLength)]
        public string AvatarUrl { get; set; }
        public long? ProvinceId { get; set; } // tinh
        public long? DistrictId { get; set; } // quan huyen

        [ForeignKey("ProvinceId")]
        public GeoUnit Province { get; set; }
        [ForeignKey("DistrictId")]
        public GeoUnit District { get; set; }
    }

	
}
