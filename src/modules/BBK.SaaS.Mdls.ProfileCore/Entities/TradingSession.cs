using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Mdls.Category.Geographies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    [Table("AppTradingSessions", Schema = SaaSProfileConsts.DefaultSchema)]
    public class TradingSession : FullAuditedEntity<long>
    {
        public int TenantId { get; set; }
        public string NameTrading {  get; set; } // tên phiên giao dịch

        public long ProvinceId { get; set; } // tinh/thanh pho
        [ForeignKey("ProvinceId")]
        public GeoUnit Province { get; set; }

        public long? DistrictId { get; set; } // quan/huyen
        [ForeignKey("DistrictId")]
        public GeoUnit District { get; set; }

        public long? VillageId { get; set; } // xa/phuong
        [ForeignKey("VillageId")]
        public GeoUnit Village { get; set; }

        [MaxLength(SaaSConsts.MaxUrlLength)] // đia chỉ
        public string Address { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CountRecruiterMax {  get; set; } // so luong toi da nha tuyen dung
        public int CountCandidateMax {  get; set; } // so luong toi da nguoi lao dong

        public string Describe {  get; set; } // mô tả phiên 

        public string ImgUrl { get; set; } // ảnh 


		[StringLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; } // mo ta 
    }

    [Table("AppTradingSessionAccounts", Schema = SaaSProfileConsts.DefaultSchema)]
    public class TradingSessionAccount : FullAuditedEntity<long>
    {
        public int TenantId { get; set; }
        public long UsertId { get; set; }

        public long? RecruiterId { get; set; }
        [ForeignKey("RecruiterId")]
        public Recruiter Recruiter { get; set; }

        public long? CandidateId { get; set; }
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }

        public long? JobApplicationId { get; set; }
        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }

        public long TradingSessionId { get; set; }

        [ForeignKey("TradingSessionId")]
        public TradingSession TradingSession { get; set; }

        public int Status { get; set; } = 1; // 0: Được mời, 1: Tham gia, 2: Từ chối
    }

    

}
