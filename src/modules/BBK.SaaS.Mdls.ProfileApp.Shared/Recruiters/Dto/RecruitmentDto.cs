using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using System;
using System.Collections.Generic;

namespace BBK.SaaS.Mdls.Profile.Recruiters.Dto
{
    public class RecruitmentDto
    {
        public long? Id { get; set; }
        public int TenantId { get; set; }

        public string Title { get; set; } //Chức danh

        public long FormOfWork { get; set; } // Hình thức làm việc
        public CatUnitDto FormOfWorks { get; set; }
        public long Degree { get; set; } // Bằng cấp
        public CatUnitDto Degrees { get; set; }

        public long Experience { get; set; } // Kinh nghiệm
        public CatUnitDto Experiences { get; set; }

        public long Rank { get; set; } // Cấp bậc
        public CatUnitDto Ranks { get; set; }

        public int MinAge { get; set; }

        public int MaxAge { get; set; }

        public int GenderRequired { get; set; } // Yêu cầu giới tính

        public int NumberOfRecruits { get; set; } // Số lượng tuyển

        public string ProbationPeriod { get; set; } // Thời hạn thử việc

        public DateTime DeadlineSubmission { get; set; } // Hạn nộp hồ sơ
        public bool Status { get; set; } //  ẩn / hiển 
        public long JobCatUnitId { get; set; } // nghề nghiệp
        public string JobCatUnitName { get; set; }
        #region Mức lương & Kỹ năng
        public decimal MinSalary { get; set; }

        public decimal MaxSalary { get; set; }

        public string NecessarySkills { get; set; }
        #endregion

        #region Mô tả công việc
        public string JobDesc { get; set; } // Yêu cầu công việc * Kỹ năng chuyên môn hoặc kỹ năng mềm cần thiết với công việc mà ứng viên cần quan tâm
        public string JobRequirementDesc { get; set; }
        public string BenefitDesc { get; set; } // Quyền lợi * Những quyền lợi, lợi ích với công việc cho ứng viên với vị trí đăng tuyển
        #endregion

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        //public long? RecruitmentContactId { get; set; }
       // public List<RecruitmentAddressDTO> RecruitmentAddress { get; set; }

        public RecruiterEditDto Recruiter { get; set; }
        public long RecruiterId { get; set; }

        public string DistrictCode { get; set; }

        public string WorkAddress { get; set; }
        public long ProvinceId { get; set; }
        public string AddressName { get; set; }
        public bool IsCandidate { get; set; }
        public bool IsRecuiters { get; set; }
    }
   

    //public class RecruitmentAddressDTO
    //{
    //    public long ProvinceId { get; set; }
    //    public string DistrictCode { get; set; }

    //    public string WorkAddress { get; set; }

    //    public string ProvinceName { get; set; }
    //}

    //public class RecruitmentContactDto
    //{
    //    public string FullName { get; set; }
    //    public string Email { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Address { get; set; }
    //}


    public class RecruimentInput : PagedSortedAndFilteredInputDto,IShouldNormalize
    {
        public string Filtered { get; set; }
        public int? NumberOfRecruits { get; set; }
        public long? FormOfWork { get; set; } // Hình thức làm việc
        public long? Job { get; set; } // Hình thức làm việc
        public long? Experience { get; set; } // Kinh nghiệm
        public long? Rank { get; set; } // Kinh nghiệm
        public long? Degree { get; set; } // Kinh nghiệm
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Sorting { get; set; }
        public List<long> WorkSite { get; set; }

        public decimal? Salary { get; set; }
        public decimal? SalaryMax { get; set; }
        public long? RecruiterId { get; set; }
        public int Take { get; set; }
        public int Paging { get; set; }

        // Mobile filter Worksite
        public long? WorkSiteId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}
