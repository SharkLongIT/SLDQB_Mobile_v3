using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Mdls.Category.Indexings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    [Table("AppRecruitments", Schema = SaaSProfileConsts.DefaultSchema)]
    public class Recruitment : FullAuditedEntity<long>, IMustHaveTenant
    {
        //public Recruitment()
        //{
        //    this.RecruitmentAddresses = new HashSet<RecruitmentAddress>();
        //}

        public int TenantId { get; set; }

        [MaxLength(SaaSConsts.MaxSingleLineLength)]
        public string Title { get; set; } //Chức danh

        public long FormOfWork { get; set; } // Hình thức làm việc
        [ForeignKey("FormOfWork")]
        public CatUnit FormOfWorks { get; set; }
        public long Degree { get; set; } // Bằng cấp
        [ForeignKey("Degree")]
        public CatUnit Degrees { get; set; }

        public long Experience { get; set; } // Kinh nghiệm
        [ForeignKey("Experience")]
        public CatUnit Experiences { get; set; }

        public long Rank { get; set; } // Cấp bậc
        [ForeignKey("Rank")]
        public CatUnit Ranks { get; set; }


        public int MinAge { get; set; }

        public int MaxAge { get; set; }

        public GenderEnum GenderRequired { get; set; } = GenderEnum.None; // Yêu cầu giới tính

        public int NumberOfRecruits { get; set; } // Số lượng tuyển

        [MaxLength(SaaSConsts.MaxCodeLineLength)]
        public string ProbationPeriod { get; set; } // Thời hạn thử việc

        public DateTime DeadlineSubmission { get; set; } // Hạn nộp hồ sơ

        public bool Status { get; set; } //  ẩn / hiển 
        public long JobCatUnitId { get; set; } // nghề nghiệp

        //      public long JobCatId { get; set; } // nghề nghiệp:
        [ForeignKey("JobCatUnitId")]
        public CatUnit JobCatUnits { get; set; }


        public long RecruiterId { get; set; } // nghề nghiệp

        [ForeignKey("RecruiterId")]
        public Recruiter Recruiter { get; set; }


        #region Mức lương & Kỹ năng
        public decimal MinSalary { get; set; }

        public decimal MaxSalary { get; set; }

        [MaxLength(SaaSConsts.MaxDoubleLineLength)]
        public string NecessarySkills { get; set; }
        #endregion

        #region Mô tả công việc
        [MaxLength(SaaSConsts.MaxDescription)]
        public string JobDesc { get; set; } // Yêu cầu công việc * Kỹ năng chuyên môn hoặc kỹ năng mềm cần thiết với công việc mà ứng viên cần quan tâm
        [MaxLength(SaaSConsts.MaxDescription)]
        public string JobRequirementDesc { get; set; }
        [MaxLength(SaaSConsts.MaxDescription)]
        public string BenefitDesc { get; set; } // Quyền lợi * Những quyền lợi, lợi ích với công việc cho ứng viên với vị trí đăng tuyển
        #endregion

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        //dia chi
        public string DistrictCode { get; set; }
        public string WorkAddress { get; set; }
        public long ProvinceId { get; set; }

        //public virtual long? RecruitmentContactId { get; set; }
        //[ForeignKey("RecruitmentContactId")]
        //public virtual RecruitmentContact RecruitmentContact { get; set; }
        // public virtual ICollection<RecruitmentAddress> RecruitmentAddresses { get; set; }
    }

    //[Table("AppRecruitmentAddresses", Schema = SaaSProfileConsts.DefaultSchema)]
    //public class RecruitmentAddress : FullAuditedEntity<long>, IMustHaveTenant
    //{
    //    public int TenantId { get; set; }

    //    public long RecruitmentId { get; set; }
    //    [ForeignKey("RecruitmentId")]
    //    public Recruitment Recruitment { get; set; }

    //    [MaxLength(SaaSConsts.MaxShortLineLength)]
    //    public string DistrictCode { get; set; }

    //    public string WorkAddress { get; set; }
    //}

    //public class RecruitmentContact : CreationAuditedEntity<long>
    //{
    //    public int TenantId { get; set; }
    //    public string FullName { get; set; }
    //    public string Email { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Address { get; set; }
    //}
}
