using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.MultiTenancy.Payments.Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    [Table("AppJobApplications", Schema = SaaSProfileConsts.DefaultSchema)]
    public class JobApplication : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public decimal? DesiredSalary { get; set; } // Mức lương mong muốn 
        public long? CurrencyUnit { get; set; } // Đơn vị tiền tệ
        public long? FormOfWorkId { get; set; } // Hình thức làm việc
        [ForeignKey("FormOfWorkId")]
        public CatUnit FormOfWork { get; set; }
        public string Career { get; set; } // Mục tiêu nghề nghiệp
        public long? LiteracyId { get; set; } // Trình độ học vấn || Danh mục bằng cấp
        [ForeignKey("LiteracyId")]
        public CatUnit Literacy { get; set; }
        public long PositionsId { get; set; }  // Vị trí muốn ứng tuyển  
        public CatUnit Positions { get; set; }  // Vị trí muốn ứng tuyển  
        public long? OccupationId { get; set; } // Nghề nghiệp
        [ForeignKey("OccupationId")]
        public CatUnit Occupations { get; set; }
        public long WorkSite { get; set; }   // Nơi muốn làm việc
        [ForeignKey("WorkSite")]
        public GeoUnit Province { get; set; } // địa chỉ nơi muốn làm việc 
        public long ExperiencesId { get; set; }   // Kinh nghiệm làm việc
        public CatUnit Experiences { get; set; }   // Kinh nghiệm làm việc
        public long JobGrade { get; set; }   // Cấp bậc mong muốn
        public string Title { get; set; }   // Tiêu đề hồ sơ ứng Tuyển

        public bool IsPublished { get; set; }  
        public byte Word { get; set; }
        public byte Excel { get; set; }
        public byte PowerPoint { get; set; }
        public string FileCVUrl { get; set; }// File CV
        public long CandidateId { get; set; }
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }
        public ICollection<WorkExperience> WorkExperiences { get; set; }
        public ICollection<LearningProcess> LearningProcess { get; set; }

        public JobApplication()
        {
            WorkExperiences = new HashSet<WorkExperience>();
            LearningProcess = new HashSet<LearningProcess>();
        }
    }

    [Table("AppWorkExperiences", Schema = SaaSProfileConsts.DefaultSchema)]
    public class WorkExperience : AuditedEntity<long>, IMustHaveTenant, IHasCreationTime
    {
        public int TenantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string CompanyName { get; set; }

        public string Positions { get; set; } // vị trí trong công ty 
        [MaxLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; }

        public long JobApplicationId { get; set; }
        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }

    }

    [Table("AppLearningProcesses", Schema = SaaSProfileConsts.DefaultSchema)]
    public class LearningProcess : AuditedEntity<long>, IMustHaveTenant, IHasCreationTime
    {
        public int TenantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AcademicDiscipline { get; set; }  // Ngành học/ môn học
        public string SchoolName { get; set; }
        [MaxLength(SaaSConsts.MaxDescription)]
        public string Description { get; set; }
        public long JobApplicationId { get; set; }
        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }

    }

    [Table("AppApplicationRequests", Schema = SaaSProfileConsts.DefaultSchema)]
    /// <summary>
    /// Danh sách ứng tuyển của Người lao động. Lưu trữ khi NLĐ chọn xin ứng tuyển từ tin tuyển dụng
    /// </summary>
    public class ApplicationRequest : FullAuditedEntity<long>, IMustHaveTenant, IHasCreationTime
    {
        public int TenantId { get; set; }

        public long JobApplicationId { get; set; }
        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }
        public long RecruitmentId { get; set; }
        [ForeignKey("RecruitmentId")]
        public Recruitment Recruitment { get; set; }
        public int Status { get; set; } // Trạng thái của đơn ứng tuyển: Chờ duyệt, Chờ phỏng vấn, Đã có lịch hẹn, Đã phỏng vấn
        [MaxLength(SaaSConsts.MaxDescription)]
        public string Content { get; set; }
    }


}
