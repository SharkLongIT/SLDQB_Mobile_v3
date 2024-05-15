using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi
{
    public class ViecTimNguoiModel
    {
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public string AddressForWork { get; set; }
        public string Title { get; set; } //Chức danh

        public long FormOfWork { get; set; } // Hình thức làm việc
        public string FormOfWorkName { get; set; } // Hình thức làm việc

        public long Degree { get; set; } // Bằng cấp

        public string Experience { get; set; } // Kinh nghiệm
        
        public long Rank { get; set; } // Cấp bậc

        public int MinAge { get; set; }

        public string SphereOfActivity { get; set; }
        public int MaxAge { get; set; }

        public int GenderRequired { get; set; } // Yêu cầu giới tính

        public int NumberOfRecruits { get; set; } // Số lượng tuyển

        public string ProbationPeriod { get; set; } // Thời hạn thử việc

        public DateTime DeadlineSubmission { get; set; } // Hạn nộp hồ sơ
        public bool Status { get; set; } //  ẩn / hiển 
        public long JobCatUnitId { get; set; } // nghề nghiệp
        public string JobCatUnitName { get; set; }
        public string AvatarUrl { get; set; } //logo cty
        public string ImageCoverUrl { get; set; } // ảnh bìa

        public string CompanyName { get; set; }
        public string AddressName { get; set; }
        public string AddressCompany {  get; set; }
        public string Website {  get; set; }
        public string HumanResSizeCat { get; set; } // Quy mô nhân sự

        public string WorkAddress { get; set; }

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
        public RecruitmentAddressDTO RecruitmentAddress { get; set; }

        public RecruiterEditDto Recruiter { get; set; }
        public long RecruiterId { get; set; }
    }



    public class RecruitmentAddressDTO
    {
        public long ProvinceId { get; set; }
        public string DistrictCode { get; set; }

        public string WorkAddress { get; set; }
    }

    public class RecruitmentContactDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
