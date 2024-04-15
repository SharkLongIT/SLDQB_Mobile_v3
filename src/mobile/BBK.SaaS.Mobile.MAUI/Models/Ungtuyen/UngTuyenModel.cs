using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.Ungtuyen
{
    [AutoMapFrom(typeof(ApplicationRequestEditDto))]
    public class UngTuyenModel : ApplicationRequestEditDto
    {
    }
}
