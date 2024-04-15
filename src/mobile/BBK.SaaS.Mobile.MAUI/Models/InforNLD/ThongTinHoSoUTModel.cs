using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.InforNLD
{
    [AutoMapFrom(typeof(ApplicationRequestEditDto))]

    public class ThongTinHoSoUTModel : ApplicationRequestEditDto
    {
        public string Positions { get; set; }
        public string Literacy { get; set; }
        public string FormOfWork { get; set; }

        public CandidateEditDto Candidate { get; set; }
        public string Photo {  get; set; }

        //public long candidateId { get; set; }
        //public string CandidateAddress { get; set; }
        //public DateTime CandidateDateOfBirth { get; set; }
        //public bool CandidateMarital { get; set; }
        //public GenderEnum CandidateGender { get; set; }
        //public string Content { get; set; }

    }
}
