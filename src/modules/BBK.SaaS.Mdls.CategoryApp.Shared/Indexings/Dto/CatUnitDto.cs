using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace BBK.SaaS.Mdls.Category.Indexings.Dto
{
    public class CatUnitDto : AuditedEntityDto<long>
    {
        public long? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }
        
        public int RoleCount { get; set; }
    }

    public class CatFilterList
    {
        public List<CatUnitDto> Degree{ get; set;} // bang cap
        public List<CatUnitDto> Career { get; set;} // nganh nghe
        public List<CatUnitDto> Rank { get; set;} // cap bac
        public List<CatUnitDto> FormOfWork { get; set;} // hinh thuc lam viec
        public List<CatUnitDto> Experience { get; set;} // kinh nghiem lam viec
        public List<CatUnitDto> Salary { get; set;} // muc luong
        public List<CatUnitDto> StaffSize { get; set;} // quy mô nhân sự
    }
}