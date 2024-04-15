using System;
using Abp.Application.Services.Dto;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class GeoUnitRoleListDto : EntityDto<long>
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }
        
        public DateTime AddedTime { get; set; }
    }
}