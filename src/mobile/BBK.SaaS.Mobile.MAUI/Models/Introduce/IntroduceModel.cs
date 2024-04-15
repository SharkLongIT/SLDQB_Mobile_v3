using Abp.AutoMapper;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.Introduce
{
    [AutoMapFrom(typeof(IntroduceEditDto))]
    public class IntroduceModel : IntroduceEditDto
    {
      
    }
}
