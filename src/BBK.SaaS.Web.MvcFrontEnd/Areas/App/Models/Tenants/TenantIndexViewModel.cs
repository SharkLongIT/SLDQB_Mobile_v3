using System.Collections.Generic;
using BBK.SaaS.Editions.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Tenants
{
    public class TenantIndexViewModel
    {
        public List<SubscribableEditionComboboxItemDto> EditionItems { get; set; }
    }
}