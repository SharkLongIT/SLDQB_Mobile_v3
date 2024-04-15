using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Candidates.Dto
{
    public class CandidateEditDto
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Address { get; set; }
        public bool Marital { get; set; }// Tình trạng hôn nhân
        public DateTime? DateOfBirth { get; set; }
        public GenderEnum Gender { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
        public GeoUnitDto Province { get; set; }
        public GeoUnitDto District { get; set; }

        public UserEditDto Account { get; set; }
    }

    public enum GenderEnum
    {
        None = 0, Male = 1, Female = 2,
    }


    public class GetCandidateForEditOutput
    {
        public Guid? ProfilePictureId { get; set; }
        public UserEditDto User { get; set; }
        public CandidateEditDto Candidate { get; set; }
        public GetCandidateForEditOutput()
        {
        }
        //public GeoUnitDto Province { get; set; }
        //public GeoUnitDto District { get; set; }
    }

    public class Templatepdf
    {
        public int TemplatepdfId { get; set; }
        public string TemplateName { get; set; }
        public string ContentWorkExpTemplatepdf { get; set; }
        public string ContentLearningProcessTemplatepdf { get; set; }
        public string TitleWorkExp { get; set; }
        public string TitleLearning { get; set; }
    }
}
