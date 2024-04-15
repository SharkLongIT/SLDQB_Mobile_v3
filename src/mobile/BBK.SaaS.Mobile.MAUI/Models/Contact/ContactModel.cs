using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.Contact
{
    [AutoMapFrom(typeof(ContactDto))]
    public partial class ContactModel: ContactDto
    {
    }
}
