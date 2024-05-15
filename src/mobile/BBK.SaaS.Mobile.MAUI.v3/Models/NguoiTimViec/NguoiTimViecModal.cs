using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;

namespace BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec
{
    public class NguoiTimViecModal
    {
        public int? Id { get; set; }
        public Guid? ProfilePictureId { get; set; }
        public string Photo {  get; set; }  
        public UserEditDto User { get; set; }

        public CandidateEditDto Candidate { get; set; }

        public JobApplicationEditDto JobApplication { get; set; }

        public string Positions { get; set; }
        public string Literacy { get; set; }
        public string FormOfWork { get; set; }
        public string Experience { get; set; }
        public string Occupation {  get; set; }
        public string Province {  get; set; }
        public string DistrictName {  get; set; }
        public string ProvinceName {  get; set; }

    }
}
