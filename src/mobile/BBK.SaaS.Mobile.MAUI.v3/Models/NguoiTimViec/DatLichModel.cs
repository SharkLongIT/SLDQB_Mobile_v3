using Abp.AutoMapper;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec
{
    [AutoMapFrom(typeof(MakeAnAppointmentDto))]
    public class DatLichModel : MakeAnAppointmentDto
    {
    //public int Title { get; set; }
        public bool IsView {  get; set; }
        public bool IsAgree { get; set; } = false;
        public bool IsReject { get; set; } = false; 
    }
}
