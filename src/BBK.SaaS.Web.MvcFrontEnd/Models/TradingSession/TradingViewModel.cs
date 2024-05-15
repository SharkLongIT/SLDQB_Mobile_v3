using Abp.AutoMapper;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Models.TradingSession
{
    [AutoMapFrom(typeof(TradingSessionEditDto))]
    public class TradingViewModel
    {
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public string NameTrading { get; set; } // tên phiên giao dịch

        public long ProvinceId { get; set; } // tinh/thanh pho

        public long? DistrictId { get; set; } // quan/huyen

        public long? VillageId { get; set; } // xa/phuong

        public string Address { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Description { get; set; } // mo ta 

        public GeoUnitDto Province { get; set; }
        public GeoUnitDto Village { get; set; }
        public GeoUnitDto District { get; set; }

        public int CountRecruiterMax { get; set; } // so luong toi da nha tuyen dung
        public int CountCandidateMax { get; set; } // so luong toi da nguoi lao dong

        public string Describe { get; set; } // mô tả phiên 

        public string ImgUrl { get; set; } // ảnh 


        // trang thai user
        public int Status { get; set; }

        public bool IsCheckMax { get; set; } = false; // check so luong nguoi tham gia day phien giao dich chua
    }

    public class GetAllTrading
    {
        public List<TradingViewModel> Trading { get; set; }
      
    }
}
