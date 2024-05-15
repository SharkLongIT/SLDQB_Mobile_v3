using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.NhaTuyenDung
{
    public class NhaTuyenDungModel
    {
        //public Guid? ProfilePictureId { get; set; }
        public UserEditDto User { get; set; }
        public RecruiterEditDto Recruiter { get; set; }
    public string Province { get; set; }
        public string District { get; set; }
        public string Village { get; set; }
        public string AvatarUrl { get; set; }
        public string HumanResSizeCatName { get; set; }
        public string SphereOfActivityName { get; set; }
    }
}
