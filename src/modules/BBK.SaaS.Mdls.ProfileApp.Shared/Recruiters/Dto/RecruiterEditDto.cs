using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Storage;
using System;
using System.IO;

namespace BBK.SaaS.Mdls.Profile.Recruiters.Dto
{
    public class RecruiterEditDto
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }
        public long UserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Address { get; set; }

        //public long GeoUnitId { get; set; }
        //public GeoUnit Location { get; set; }

        //[MaxLength(SaaSConsts.MaxCodeLineLength)]
        public string TaxCode { get; set; }

        //[MaxLength(SaaSConsts.MaxSingleLineLength)]
        public string CompanyName { get; set; }

        public string CompanyPhone0 { get; set; }

        public int HumanResSizeCatId { get; set; } // Quy mô nhân sự
        public CatUnitDto HumanResSizeCat { get; set; } 

        public string BusinessLicenseUrl { get; set; }

        //public FileMgr BusinessLicenseFile { get; set; }

        public string GetBusinessLicenseFileName()
        {
            if (BusinessLicenseUrl != null)
            {
                return Path.GetFileName(BusinessLicenseUrl);
            }
            return string.Empty;
        }

        public string ImageCoverUrl { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }
        public string WebSite { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
        public long VillageId { get; set; }
        public long? SphereOfActivityId { get; set; }
        public CatUnitDto SphereOfActivity { get; set; }
        public DateTime? DateOfEstablishment { get; set; }

        public string Description { get; set; }

        public GeoUnitDto Province { get; set; }
        public GeoUnitDto Village { get; set; }
        public GeoUnitDto District { get; set; }
    }

    public class GetRecruiterForEditOutput
    {
        public Guid? ProfilePictureId { get; set; }
        public UserEditDto User { get; set; }
        public RecruiterEditDto Recruiter { get; set; }

        public GetRecruiterForEditOutput()
        {
        }
    }
}
