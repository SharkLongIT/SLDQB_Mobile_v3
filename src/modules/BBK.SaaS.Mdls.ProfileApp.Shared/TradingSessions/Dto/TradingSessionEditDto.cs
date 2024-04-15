using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Profile.TradingSessions.Dto
{
    public class TradingSessionEditDto 
    {
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public string NameTrading { get; set; } // tên phiên giao dịch

        public long ProvinceId { get; set; } // tinh/thanh pho

        public long? DistrictId { get; set; } // quan/huyen

        public long? VillageId { get; set; } // xa/phuong

        [MaxLength(SaaSConsts.MaxUrlLength)] // đia chỉ
        public string Address { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [StringLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; } // mo ta 

        public GeoUnitDto Province { get; set; }
        public GeoUnitDto Village { get; set; }
        public GeoUnitDto District { get; set; }

		public int CountRecruiterMax { get; set; } // so luong toi da nha tuyen dung
		public int CountCandidateMax { get; set; } // so luong toi da nguoi lao dong

		public string Describe { get; set; } // mô tả phiên 

		public string ImgUrl { get; set; } // ảnh 
        public DateTime CreationTime { get; set; }

        // trang thai user
        public int Status { get; set; }
    }

   
}
