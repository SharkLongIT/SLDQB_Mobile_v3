using System.Collections.Generic;
using BBK.SaaS.DynamicEntityProperties.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.DynamicProperty
{
    public class CreateOrEditDynamicPropertyViewModel
    {
        public DynamicPropertyDto DynamicPropertyDto { get; set; }

        public List<string> AllowedInputTypes { get; set; }
    }
}
