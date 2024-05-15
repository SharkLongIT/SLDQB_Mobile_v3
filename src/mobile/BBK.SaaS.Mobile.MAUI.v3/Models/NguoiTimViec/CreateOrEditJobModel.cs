using Abp.AutoMapper;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec
{
    [AutoMapFrom(typeof(JobApplicationEditDto))]

    public class CreateOrEditJobModel: JobApplicationEditDto
    {

    }
}
