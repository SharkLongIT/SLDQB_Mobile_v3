using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec
{
    public class NguoiLaoDongModel
    {
        //public Guid? ProfilePictureId { get; set; }

        public UserEditDto User { get; set; }

        public CandidateEditDto Candidate { get; set; }

        public string Province { get; set; }
        public string District { get; set; }
    }
}
