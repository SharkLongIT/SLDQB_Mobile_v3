using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using Abp.Runtime.Validation;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Storage;
using NUglify.JavaScript.Visitors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.Candidates.Dto      
{

    public class JobApplicationEditDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public decimal? DesiredSalary { get; set; } // Mức lương mong muốn 
        public long CurrencyUnit { get; set; } // Đơn vị tiền tệ
        public long FormOfWorkId { get; set; } // Hình thức làm việc
        public CatUnitDto FormOfWork { get; set; } // Hình thức làm việc
        public string Career { get; set; } // Mục tiêu nghề nghiệp
        
        public long LiteracyId { get; set; } // Trình độ học vấn 
        public CatUnitDto Literacy { get; set; }
        public long? PositionsId { get; set; }  // Vị trí muốn ứng tuyển  

        public CatUnitDto Positions { get; set; }  // Vị trí muốn ứng tuyển  
        public long OccupationId { get; set; } // Nghề nghiệp
        public CatUnitDto Occupations { get; set; }
        public long WorkSite { get; set; }   // Nơi muốn làm việc
        public long ExperiencesId { get; set; }   // Kinh nghiệm làm việc
        public CatUnitDto Experiences { get; set; }   // Kinh nghiệm làm việc
        public long JobGrade { get; set; }   // Cấp bậc mong muốn
        public string Title { get; set; }   // Tiêu đề hồ sơ ứng Tuyển
        public bool IsPublished { get; set; } 
        public byte Word { get; set; }
        public byte Excel { get; set; }
        public byte PowerPoint { get; set; }
        public long CandidateId { get; set; }
        public string FileCVUrl { get; set; }   

        public FileMgr FileMgr { get; set; }
        public CandidateEditDto Candidate { get; set; }
        public GeoUnitDto Province { get; set; }

        //public string BusinessLicenseUrl { get; set; }

        public List<WorkExperienceEditDto> WorkExperiences { get; set; }
        public List<LearningProcessEditDto> LearningProcess { get; set; } 

        //public FileMgr BusinessLicenseFile { get; set; }

        //public string GetBusinessLicenseFileName()
        //{
        //    if (BusinessLicenseUrl != null)
        //    {
        //        return Path.GetFileName(BusinessLicenseUrl);
        //    }
        //    return string.Empty;
        //}
    }
    public class JobApplicationCreate : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public decimal? DesiredSalary { get; set; } // Mức lương mong muốn 
        public long CurrencyUnit { get; set; } // Đơn vị tiền tệ
        public long FormOfWorkId { get; set; } // Hình thức làm việc
        public string Career { get; set; } // Mục tiêu nghề nghiệp

        public long LiteracyId { get; set; } // Trình độ học vấn 
        public long PositionsId { get; set; }  // Vị trí muốn ứng tuyển  

        public long OccupationId { get; set; } // Nghề nghiệp
        public long WorkSite { get; set; }   // Nơi muốn làm việc
        public long ExperiencesId { get; set; }   // Kinh nghiệm làm việc
        public long JobGrade { get; set; }   // Cấp bậc mong muốn
        public string Title { get; set; }   // Tiêu đề hồ sơ ứng Tuyển
        public bool IsPublished { get; set; }
        public byte Word { get; set; }
        public byte Excel { get; set; }
        public byte PowerPoint { get; set; }
        public long CandidateId { get; set; }
        public string FileCVUrl { get; set; }
    }

    public class WorkExperienceEditDto
    {
        public long? Id { get; set; }

        public int TenantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CompanyName { get; set; }
        public string Positions { get; set; } // vị trí trong công ty 
        [MaxLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; }
        public long JobApplicationId { get; set; }
    }

    public class LearningProcessEditDto
    {
        public long? Id { get; set; }   
        public int TenantId { get; set; }
        public string AcademicDiscipline { get; set; } // Ngành học
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SchoolName { get; set; }
        [MaxLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public long JobApplicationId { get; set; }
    }

    public class JobAppSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }

        public int Take { get; set; }
        public DateTime? DateUpdate { get; set; }
        public List<long?> FormOfWorkId { get; set; }
        public long? OccupationId { get; set; }
        public long? AddressId { get; set; }
        public long? LiteracyId { get; set; }
        public decimal? DesiredSalary { get; set; }
        public long? ExperiencesId { get; set; }

        //public GenderEnum? Gender { get; set; } 
        public int? Gender { get; set; } 

        public List<long> WorkSite { get; set; }


        //Mobile
        public long? WorkSiteId { get; set; }

        public decimal? SalaryMin { get; set; } 
        public decimal? SalaryMax { get; set; } 

        public int Paging { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
    public class JobAppSearchOfProfessionalStaff: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public long? CandidateId {  get; set; }

        public int Take { get; set; }
        public DateTime? DateUpdate { get; set; }
        public List<long?> FormOfWorkId { get; set; }
        public long? OccupationId { get; set; }
        public long? AddressId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? LiteracyId { get; set; }
        public decimal? DesiredSalary { get; set; } // Mức lương
        public long? ExperiencesId { get; set; } // Kinh nghiệm làm việc || Danh mục kinh nghiệm làm việc

        public long? Salary {  get; set; }  // 

        //public GenderEnum? Gender { get; set; } 
        public int? Gender { get; set; }

        public List<long> WorkSite { get; set; }

        public int Paging { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }

    public class GetJobApplicationForEditOutput
    {
        public Guid? ProfilePictureId { get; set; }
        public UserEditDto User { get; set; }
        public JobApplicationEditDto JobApplication { get; set; }
        public CandidateEditDto Candidate { get; set; }

        public GetJobApplicationForEditOutput()
        {
        }
    }
}
