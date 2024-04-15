using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    [Table("AppRecruiters", Schema = SaaSProfileConsts.DefaultSchema)]
	public class Recruiter : FullAuditedEntity<long>, IMustHaveTenant
	{
        public int TenantId { get; set; }

        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User Account { get; set; }

        [MaxLength(SaaSConsts.MaxUrlLength)]
        public string AvatarUrl { get; set; } // Logo of company

        [MaxLength(SaaSConsts.MaxUrlLength)]
        public string Address { get; set; }

        //public long GeoUnitId { get; set; }
        //public GeoUnit Location { get; set; }

        [MaxLength(SaaSConsts.MaxCodeLineLength)]
        public string TaxCode { get; set; }

        [MaxLength(SaaSConsts.MaxSingleLineLength)]
        public string CompanyName { get; set; }

        //[MaxLength(SaaSConsts.MaxCodeLineLength)]
        //public string CompanyPhone0 { get; set; }

        // 00019.00001
        public long? HumanResSizeCatId { get; set; } // Quy mô nhân sự // {c0f9a793-4f1c-4e95-a061-95321f8dfff9} / 19

        [ForeignKey("HumanResSizeCatId")]
        public CatUnit HumanResSizeCat { get; set; }

        [MaxLength(SaaSConsts.MaxUrlLength)]
        public string BusinessLicenseUrl { get; set; }

        public ServiceStatusEnum ServiceStatus { get; set; } = ServiceStatusEnum.None;


        public long? SphereOfActivityId { get; set; } // Lĩnh vực hoạt động

        [ForeignKey("SphereOfActivityId")]
        public CatUnit SphereOfActivity { get; set; }


        public long? ProvinceId { get; set; } // tinh/thanh pho
        [ForeignKey("ProvinceId")]
        public GeoUnit Province { get; set; }

        //[ForeignKey("ProvinceId")]
        //public GeoUnit Province { get; set; }

        // Exp: 00001.00003
        //public int GeoUnitId { get; set; } // quan/huyen
        public long? DistrictId { get; set; } // quan/huyen
        [ForeignKey("DistrictId")]
        public GeoUnit District { get; set; }

        public long? VillageId { get; set; } // xa/phuong
        [ForeignKey("VillageId")]
        public GeoUnit Village { get; set; }


        [MaxLength(SaaSConsts.MaxUrlLength)]
        public string ImageCoverUrl { get; set; } // url anh bia

        [MaxLength(SaaSConsts.MaxSingleLineLength)]
        public string ContactName { get; set; } // tên người liên hệ
        [MaxLength(SaaSConsts.MaxSingleLineLength)]
        public string ContactPhone { get; set; } // Sdt người liên hệ
        [MaxLength(SaaSConsts.MaxSingleLineLength)]
        public string ContactEmail { get; set; } // Email nguoi liên hệ

        [StringLength(SaaSConsts.MaxShortLineLength)]
        public string WebSite { get; set; } // website

        public DateTime? DateOfEstablishment { get; set; }// ngay thanh lap

        [StringLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; } // mo ta cong ty
    }

	//[Table("AppCompanies", Schema = SaaSProfileConsts.DefaultSchema)]
	//public class Company : FullAuditedEntity<long>, IMustHaveTenant
	//{
	//	public int TenantId { get; set; }

	//	public long RecruiterId { get; set; }
	//	[ForeignKey("RecruiterId")]
	//	public Recruiter Recruiter { get; set; }
	//}
}
