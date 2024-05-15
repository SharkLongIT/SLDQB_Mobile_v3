using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi
{
    [AutoMapFrom(typeof(RecruitmentDto))]
    public class RecruitmentModel : RecruitmentDto
    {
        public string Photo {  get; set; }
    }
}
