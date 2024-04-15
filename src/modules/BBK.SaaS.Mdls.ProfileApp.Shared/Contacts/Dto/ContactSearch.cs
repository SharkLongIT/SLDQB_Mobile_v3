using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;

namespace BBK.SaaS.Mdls.Profile.Contacts.Dto
{
    public class ContactSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public bool? Status { get; set; }
        public string StartDay { get; set; }
        public string EndDay { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
