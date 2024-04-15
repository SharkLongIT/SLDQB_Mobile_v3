using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using BBK.SaaS.Authorization;
using System.Linq.Dynamic.Core;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using BBK.SaaS.Authorization.Roles;
using BBK.SaaS.MultiTenancy;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BBK.SaaS.Mdls.Category.Indexings
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits)]
    public class CatUnitAppService : SaaSAppServiceBase, ICatUnitAppService
     {
          private readonly CatUnitManager _catUnitManager;
          private readonly IRepository<CatUnit, long> _catUnitRepository;

          public CatUnitAppService(
               CatUnitManager CatUnitManager,
               IRepository<CatUnit, long> CatUnitRepository
               )
          {
               _catUnitManager = CatUnitManager;
               _catUnitRepository = CatUnitRepository;
          }

          public async Task<ListResultDto<CatUnitDto>> GetCatUnits()
          {
               var catUnits = await _catUnitRepository.GetAllListAsync();

               return new ListResultDto<CatUnitDto>(
                    catUnits.Select(ou =>
                    {
                         var CatUnitDto = ObjectMapper.Map<CatUnitDto>(ou);
                         return CatUnitDto;
                    }).ToList());
          }

          //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits_ManageOrganizationTree)]
          public async Task<ListResultDto<CatUnitDto>> GetRootCatUnits()
          {
               var catUnits = await _catUnitRepository.GetAll()
                    .Where(x => x.ParentId == null).ToListAsync();

               return new ListResultDto<CatUnitDto>(
                    catUnits.Select(ou =>
                    {
                         var CatUnitDto = ObjectMapper.Map<CatUnitDto>(ou);
                         return CatUnitDto;
                    }).ToList());
          }

          //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits_ManageOrganizationTree)]
          public async Task<ListResultDto<CatUnitDto>> GetChildrenCatUnit(long id)
          {
               var catUnits = await _catUnitRepository.GetAll()
                    .Where(x => x.ParentId == id).ToListAsync();

               return new ListResultDto<CatUnitDto>(
                    catUnits.Select(ou =>
                    {
                         var CatUnitDto = ObjectMapper.Map<CatUnitDto>(ou);
                         //CatUnitDto.MemberCount = CatUnitMemberCounts.ContainsKey(ou.Id)
                         //    ? CatUnitMemberCounts[ou.Id]
                         //    : 0;
                         //CatUnitDto.RoleCount = CatUnitRoleCounts.ContainsKey(ou.Id)
                         //    ? CatUnitRoleCounts[ou.Id]
                         //    : 0;
                         return CatUnitDto;
                    }).ToList());
          }

          //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits_ManageOrganizationTree)]
          public async Task<CatUnitDto> CreateCatUnit(CreateCatUnitInput input)
          {
               var CatUnit = new CatUnit(AbpSession.TenantId, input.DisplayName, input.ParentId);

               await _catUnitManager.CreateAsync(CatUnit);
               await CurrentUnitOfWork.SaveChangesAsync();

               return ObjectMapper.Map<CatUnitDto>(CatUnit);
          }

          //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits_ManageOrganizationTree)]
          public async Task<CatUnitDto> UpdateCatUnit(UpdateCatUnitInput input)
          {
               var CatUnit = await _catUnitRepository.GetAsync(input.Id);

               CatUnit.DisplayName = input.DisplayName;

               await _catUnitManager.UpdateAsync(CatUnit);

               return await CreateCatUnitDto(CatUnit);
          }

          //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits_ManageOrganizationTree)]
          public async Task<CatUnitDto> MoveCatUnit(MoveCatUnitInput input)
          {
               await _catUnitManager.MoveAsync(input.Id, input.NewParentId);

               return await CreateCatUnitDto(
                    await _catUnitRepository.GetAsync(input.Id)
               );
          }

          //[AbpAuthorize(AppPermissions.Pages_Administration_CatUnits_ManageOrganizationTree)]
          public async Task DeleteCatUnit(EntityDto<long> input)
          {
               //await _userCatUnitRepository.DeleteAsync(x => x.CatUnitId == input.Id);
               //await _CatUnitRoleRepository.DeleteAsync(x => x.CatUnitId == input.Id);
               await _catUnitManager.DeleteAsync(input.Id);
          }

          public async Task<List<CatUnitDto>> GetAll()
          {
               var catUnits = await _catUnitRepository.GetAllListAsync();
               return ObjectMapper.Map<List<CatUnitDto>>(catUnits);
          }

          public async Task BuildDemoCatAsync()
          {
               if (!AbpSession.TenantId.HasValue)
               {
                    throw new UserFriendlyException("Cannot use for host!");
               }

               var tenant = await TenantManager.GetByIdAsync(AbpSession.TenantId.Value);

               //Create Category Units

               var catUnits = new List<CatUnit>();
               #region Ngành nghề 
               var producing = await CreateAndSaveCatUnit(catUnits, tenant, "Ngành nghề");
               var Nganhnghe = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Ngành nghề").FirstOrDefaultAsync();
               if (Nganhnghe != null)
               {
                    var Nganhnghe1 = await CreateAndSaveCatUnit(catUnits, tenant, "Hành chính - Thư ký ", Nganhnghe);
                    var Nganhnghe2 = await CreateAndSaveCatUnit(catUnits, tenant, "Nhà hàng - Khách sạn - Du lịch", Nganhnghe);
                    var Nganhnghe3 = await CreateAndSaveCatUnit(catUnits, tenant, "Bán sỉ - Bán lẻ - Quản lý cửa hàng ", Nganhnghe);
                    var Nganhnghe4 = await CreateAndSaveCatUnit(catUnits, tenant, "Marketing", Nganhnghe);
                    var Nganhnghe5 = await CreateAndSaveCatUnit(catUnits, tenant, "Bán hàng - Kinh doanh", Nganhnghe);
                    var Nganhnghe6 = await CreateAndSaveCatUnit(catUnits, tenant, "Kế Toán", Nganhnghe);
                    var Nganhnghe7 = await CreateAndSaveCatUnit(catUnits, tenant, "Tài chính - Đầu tư - Chứng khoán", Nganhnghe);
                    var Nganhnghe8 = await CreateAndSaveCatUnit(catUnits, tenant, "Kiểm Toán", Nganhnghe);
                    var Nganhnghe9 = await CreateAndSaveCatUnit(catUnits, tenant, "Khoa học - Kĩ thuật", Nganhnghe);
                    var Nganhnghe10 = await CreateAndSaveCatUnit(catUnits, tenant, "Nghề Nghiệp khác", Nganhnghe);
                    var Nganhnghe11 = await CreateAndSaveCatUnit(catUnits, tenant, "An ninh - Bảo vệ ", Nganhnghe);
                    var Nganhnghe12 = await CreateAndSaveCatUnit(catUnits, tenant, "Thiết kế- Sáng tạo nghệ thuật", Nganhnghe);
                    var Nganhnghe13 = await CreateAndSaveCatUnit(catUnits, tenant, "Kiến trúc - Thiết kế nội thất ", Nganhnghe);
                    var Nganhnghe14 = await CreateAndSaveCatUnit(catUnits, tenant, "IT Phần mềm", Nganhnghe);
                    var Nganhnghe15 = await CreateAndSaveCatUnit(catUnits, tenant, "IT Phần cứng - Mạng", Nganhnghe);
                    var Nganhnghe16 = await CreateAndSaveCatUnit(catUnits, tenant, "Sản xuất - Lắp ráp - Chế biến ", Nganhnghe);
                    var Nganhnghe17 = await CreateAndSaveCatUnit(catUnits, tenant, "Vận hành - Bảo trì - Bảo dưỡng", Nganhnghe);
                    var Nganhnghe18 = await CreateAndSaveCatUnit(catUnits, tenant, "Nông - Lâm - Ngư Nghiệp", Nganhnghe);
                    var Nganhnghe19 = await CreateAndSaveCatUnit(catUnits, tenant, "Thu mua - Kho vận - Chuỗi cung ứng ", Nganhnghe);
                    var Nganhnghe20 = await CreateAndSaveCatUnit(catUnits, tenant, "Xuất nhập khẩu", Nganhnghe);
                    var Nganhnghe21 = await CreateAndSaveCatUnit(catUnits, tenant, "Vận tải - Lái xe - Giao nhận", Nganhnghe);
                    var Nganhnghe22 = await CreateAndSaveCatUnit(catUnits, tenant, "Ngân hàng", Nganhnghe);
                    var Nganhnghe23 = await CreateAndSaveCatUnit(catUnits, tenant, "Khai thác năng lượng - Khoáng sản - Địa chất", Nganhnghe);
                    var Nganhnghe24 = await CreateAndSaveCatUnit(catUnits, tenant, "Y tế - Chăm sóc sức khỏe", Nganhnghe);
                    var Nganhnghe25 = await CreateAndSaveCatUnit(catUnits, tenant, "Nhân sự ", Nganhnghe);
                    var Nganhnghe26 = await CreateAndSaveCatUnit(catUnits, tenant, "Bảo hiểm", Nganhnghe);
                    var Nganhnghe27 = await CreateAndSaveCatUnit(catUnits, tenant, "Thông tin - Truyền thông - Quảng cáo", Nganhnghe);
                    var Nganhnghe28 = await CreateAndSaveCatUnit(catUnits, tenant, "Luật - Pháp lý - Tuân Thủ", Nganhnghe);
                    var Nganhnghe29 = await CreateAndSaveCatUnit(catUnits, tenant, "Quản lý dự án", Nganhnghe);
                    var Nganhnghe30 = await CreateAndSaveCatUnit(catUnits, tenant, "Quản lý tiêu chuẩn và chất lượng ", Nganhnghe);
                    var Nganhnghe31 = await CreateAndSaveCatUnit(catUnits, tenant, "Bất động sản ", Nganhnghe);
                    var Nganhnghe32 = await CreateAndSaveCatUnit(catUnits, tenant, "Chăm sóc khách hàng ", Nganhnghe);
                    var Nganhnghe33 = await CreateAndSaveCatUnit(catUnits, tenant, "Xây dựng ", Nganhnghe);
                    var Nganhnghe34 = await CreateAndSaveCatUnit(catUnits, tenant, "Giáo dục - Đào tạo", Nganhnghe);
                    var Nganhnghe35 = await CreateAndSaveCatUnit(catUnits, tenant, "Phân tích - Thống kê dữ liệu", Nganhnghe);
                    var Nganhnghe36 = await CreateAndSaveCatUnit(catUnits, tenant, "Biên phiên dịch", Nganhnghe);
                    var Nganhnghe37 = await CreateAndSaveCatUnit(catUnits, tenant, "Bưu chính viễn thông ", Nganhnghe);
                    var Nganhnghe38 = await CreateAndSaveCatUnit(catUnits, tenant, "Dầu khí", Nganhnghe);
                    var Nganhnghe39 = await CreateAndSaveCatUnit(catUnits, tenant, "Dệt may - Da giày - Thời trang", Nganhnghe);
                    var Nganhnghe40 = await CreateAndSaveCatUnit(catUnits, tenant, "Điện - Điện tử - Điện lạnh", Nganhnghe);
                    var Nganhnghe41 = await CreateAndSaveCatUnit(catUnits, tenant, "Hóa học - Hóa Sinh", Nganhnghe);
                    var Nganhnghe42 = await CreateAndSaveCatUnit(catUnits, tenant, "Dược phẩm", Nganhnghe);
                    var Nganhnghe43 = await CreateAndSaveCatUnit(catUnits, tenant, "Môi trường - Xử lý chất thải ", Nganhnghe);
                    var Nganhnghe44 = await CreateAndSaveCatUnit(catUnits, tenant, "Thực phẩm - Đồ uống ", Nganhnghe);
                    var Nganhnghe45 = await CreateAndSaveCatUnit(catUnits, tenant, "Chăn nuôi - Thú y", Nganhnghe);
                    var Nganhnghe46 = await CreateAndSaveCatUnit(catUnits, tenant, "Cơ khí Ô tô - Tự động hóa ", Nganhnghe);
                    var Nganhnghe47 = await CreateAndSaveCatUnit(catUnits, tenant, "Công nghệ thực phẩm - Dinh dưỡng ", Nganhnghe);
                    var Nganhnghe48 = await CreateAndSaveCatUnit(catUnits, tenant, "Lao động phổ thông", Nganhnghe);
                    var Nganhnghe59 = await CreateAndSaveCatUnit(catUnits, tenant, "Phi chính phủ- Phi lợi nhuận", Nganhnghe);
                    var Nganhnghe50 = await CreateAndSaveCatUnit(catUnits, tenant, "Truyền hình - Báo chí - Biên Tập", Nganhnghe);
                    var Nganhnghe51 = await CreateAndSaveCatUnit(catUnits, tenant, "Xuất bản - In ấn", Nganhnghe);
                    var Nganhnghe52 = await CreateAndSaveCatUnit(catUnits, tenant, "Thực tập sinh", Nganhnghe);
               }

               #endregion

               #region Bằng cấp
               var BangCap = await CreateAndSaveCatUnit(catUnits, tenant, "Bằng cấp");
               var bangcap = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Bằng cấp").FirstOrDefaultAsync();
               if (bangcap != null)
               {
                    var bangcap1 = await CreateAndSaveCatUnit(catUnits, tenant, "Tất cả trình độ", bangcap);
                    var bangcap2 = await CreateAndSaveCatUnit(catUnits, tenant, "Trên đại học", bangcap);
                    var bangcap3 = await CreateAndSaveCatUnit(catUnits, tenant, "Đại học ", bangcap);
                    var bangcap4 = await CreateAndSaveCatUnit(catUnits, tenant, "Cao đẳng ", bangcap);
                    var bangcap5 = await CreateAndSaveCatUnit(catUnits, tenant, "Trung cấp", bangcap);
                    var bangcap6 = await CreateAndSaveCatUnit(catUnits, tenant, "Trung học", bangcap);
                    var bangcap7 = await CreateAndSaveCatUnit(catUnits, tenant, "Chứng chỉ ", bangcap);
                    var bangcap8 = await CreateAndSaveCatUnit(catUnits, tenant, "Không yêu cầu", bangcap);
               }
               #endregion
               #region Cấp bậc
               var i2 = await CreateAndSaveCatUnit(catUnits, tenant, "Cấp bậc");
               var capbac = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Cấp bậc").FirstOrDefaultAsync();
               if (capbac != null)
               {
                    var capbac1 = await CreateAndSaveCatUnit(catUnits, tenant, "Quản lý cấp cao", capbac);
                    var capbac2 = await CreateAndSaveCatUnit(catUnits, tenant, "Quản lý cấp trung", capbac);
                    var capbac3 = await CreateAndSaveCatUnit(catUnits, tenant, "Quán lý nhóm - Giám sát", capbac);
                    var capbac4 = await CreateAndSaveCatUnit(catUnits, tenant, "Chuyên viên - Nhân viên", capbac);
                    var capbac5 = await CreateAndSaveCatUnit(catUnits, tenant, "Chuyên gia", capbac);
                    var capbac6 = await CreateAndSaveCatUnit(catUnits, tenant, "Cộng tác viên", capbac);
               }
               #endregion

               #region Hình thức làm việc 
               var i3 = await CreateAndSaveCatUnit(catUnits, tenant, "Hình thức làm việc");
               var hinhthuclamviec = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Hình thức làm việc").FirstOrDefaultAsync();
               if (hinhthuclamviec != null)
               {
                    var hinhthuclamviec1 = await CreateAndSaveCatUnit(catUnits, tenant, "Toàn thời gian cố định", hinhthuclamviec);
                    var hinhthuclamviec2 = await CreateAndSaveCatUnit(catUnits, tenant, "Toàn thời gian tạm thời", hinhthuclamviec);
                    var hinhthuclamviec3 = await CreateAndSaveCatUnit(catUnits, tenant, "Bán thời gian cố định", hinhthuclamviec);
                    var hinhthuclamviec4 = await CreateAndSaveCatUnit(catUnits, tenant, "Bán thời gian tạm thời ", hinhthuclamviec);
                    var hinhthuclamviec5 = await CreateAndSaveCatUnit(catUnits, tenant, "Theo hợp đồng tư vấn ", hinhthuclamviec);
                    var hinhthuclamviec6 = await CreateAndSaveCatUnit(catUnits, tenant, "Thực tập", hinhthuclamviec);
                    var hinhthuclamviec7 = await CreateAndSaveCatUnit(catUnits, tenant, "Khác", hinhthuclamviec);
               }

               #endregion
               #region Kinh nghiệm làm việc
               var i4 = await CreateAndSaveCatUnit(catUnits, tenant, "Kinh nghiệm làm việc");
               var kinhnghiemklamviec = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Kinh nghiệm làm việc").FirstOrDefaultAsync();
               if (kinhnghiemklamviec != null)
               {
                    var kinhnghiemklamviec1 = await CreateAndSaveCatUnit(catUnits, tenant, "Tất cả kinh nghiệm", kinhnghiemklamviec);
                    var kinhnghiemklamviec2 = await CreateAndSaveCatUnit(catUnits, tenant, "Chưa có kinh nghiệm", kinhnghiemklamviec);
                    var kinhnghiemklamviec3 = await CreateAndSaveCatUnit(catUnits, tenant, "Dưới 1 năm", kinhnghiemklamviec);
                    var kinhnghiemklamviec4 = await CreateAndSaveCatUnit(catUnits, tenant, "1 năm", kinhnghiemklamviec);
                    var kinhnghiemklamviec5 = await CreateAndSaveCatUnit(catUnits, tenant, "2 năm", kinhnghiemklamviec);
                    var kinhnghiemklamviec6 = await CreateAndSaveCatUnit(catUnits, tenant, "3 năm ", kinhnghiemklamviec);
                    var kinhnghiemklamviec7 = await CreateAndSaveCatUnit(catUnits, tenant, "4 năm", kinhnghiemklamviec);
                    var kinhnghiemklamviec8 = await CreateAndSaveCatUnit(catUnits, tenant, "5 năm ", kinhnghiemklamviec);
                    var kinhnghiemklamviec9 = await CreateAndSaveCatUnit(catUnits, tenant, "Trên 5 năm", kinhnghiemklamviec);
               }

               #endregion
               #region Mức lương

               var i5 = await CreateAndSaveCatUnit(catUnits, tenant, "Mức lương");
               var mucluong = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Mức lương").FirstOrDefaultAsync();
               if (mucluong != null)
               {
                     var mucluong2 = await CreateAndSaveCatUnit(catUnits, tenant,"1-3 triệu", mucluong);
                     var mucluong3 = await CreateAndSaveCatUnit(catUnits, tenant,"3-5 triệu", mucluong);
                     var mucluong4 = await CreateAndSaveCatUnit(catUnits, tenant,"5-7 triệu", mucluong);
                     var mucluong1 = await CreateAndSaveCatUnit(catUnits, tenant,"7-10 triệu", mucluong);
                     var mucluong5 = await CreateAndSaveCatUnit(catUnits, tenant,"10-15 triệu", mucluong);
                     var mucluong6 = await CreateAndSaveCatUnit(catUnits, tenant,"15-20 triệu", mucluong);
                     var mucluong7 = await CreateAndSaveCatUnit(catUnits, tenant,"20-30 triệu", mucluong);
                     var mucluong8 = await CreateAndSaveCatUnit(catUnits, tenant,"30-40 triệu", mucluong);
                     var mucluong9 = await CreateAndSaveCatUnit(catUnits, tenant,"40-50 triệu", mucluong);
                    var mucluong10 = await CreateAndSaveCatUnit(catUnits, tenant,"trên 50 triệu", mucluong);
                    var mucluong11 = await CreateAndSaveCatUnit(catUnits, tenant, "Thỏa thuận", mucluong);
               }

               #endregion
               #region Quy mô nhân sự
               var i6 = await CreateAndSaveCatUnit(catUnits, tenant, "Quy mô nhân sự");
               var quymonhansu = await _catUnitRepository.GetAll().Where(x => x.DisplayName == "Quy mô nhân sự").FirstOrDefaultAsync();
               if(quymonhansu != null)
               {
                    var quymonhansu1 = await CreateAndSaveCatUnit(catUnits, tenant, "Dưới 50 người ", quymonhansu);
                    var quymonhansu2 = await CreateAndSaveCatUnit(catUnits, tenant, "Từ 50-100 người ", quymonhansu);
                    var quymonhansu3 = await CreateAndSaveCatUnit(catUnits, tenant, "Từ 100-200 người", quymonhansu);
                    var quymonhansu4 = await CreateAndSaveCatUnit(catUnits, tenant, "Từ 200-300 người", quymonhansu);
                    var quymonhansu5 = await CreateAndSaveCatUnit(catUnits, tenant, "Từ 400-500 người", quymonhansu);
                    var quymonhansu7 = await CreateAndSaveCatUnit(catUnits, tenant, "Trên 500 người", quymonhansu);
               }
               #endregion 
          }

          private async Task<CatUnitDto> CreateCatUnitDto(CatUnit CatUnit)
          {
               var dto = ObjectMapper.Map<CatUnitDto>(CatUnit);
               //dto.MemberCount =
               //    await _userCatUnitRepository.CountAsync(uou => uou.CatUnitId == CatUnit.Id);
               return dto;
          }

          private async Task<CatUnit> CreateAndSaveCatUnit(List<CatUnit> geoUnits, Tenant tenant, string displayName, CatUnit parent = null)
          {
               var geoUnit = new CatUnit(tenant.Id, displayName, parent == null ? (long?)null : parent.Id);

               await _catUnitManager.CreateAsync(geoUnit);
               await CurrentUnitOfWork.SaveChangesAsync();

               geoUnits.Add(geoUnit);

               return geoUnit;
          }

        public async Task<CatFilterList> GetFilterList()
        {
            var unit = UnitOfWorkManager.Current;
            if (unit.GetTenantId() == null)
            {
                unit.SetTenantId(1);
            }
            try
            {
                CatFilterList catFilterList = new CatFilterList();

                #region Degree // Bằng cấp
                var degree = _catUnitRepository.GetAll().Where(x=>x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
                if(degree != null)
                {
                   catFilterList.Degree =  GetChildrenCatUnit(degree.Id).Result.Items.ToList();
                }
                #endregion

                #region ngành nghề 
                var career = _catUnitRepository.GetAll().Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
                if(career != null)
                {
                    catFilterList.Career =  GetChildrenCatUnit(career.Id).Result.Items.ToList();
                }
                #endregion

                #region cấp bậc
                var rank = _catUnitRepository.GetAll().Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault();
                if(rank != null)
                {
                    catFilterList.Rank =  GetChildrenCatUnit(rank.Id).Result.Items.ToList();
                }
                #endregion

                #region FormOfWork // hình thức làm việc
                var formOfWork = _catUnitRepository.GetAll().Where(x => x.DisplayName.Equals("Hình thức làm việc")).FirstOrDefault();
                if(formOfWork != null)
                {
                    catFilterList.FormOfWork =  GetChildrenCatUnit(formOfWork.Id).Result.Items.ToList();
                }
                #endregion
                #region experience
                var experience = _catUnitRepository.GetAll().Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
                if(experience != null)
                {
                    catFilterList.Experience =  GetChildrenCatUnit(experience.Id).Result.Items.ToList();
                }
                #endregion
                #region StaffSize // quy mô nhân sự
                var staffSize = _catUnitRepository.GetAll().Where(x => x.DisplayName.Equals("Quy mô nhân sự")).FirstOrDefault();
                if (staffSize != null)
                {
                    catFilterList.StaffSize = GetChildrenCatUnit(staffSize.Id).Result.Items.ToList();
                }
                #endregion


                #region Mức lương 
                var salary = _catUnitRepository.GetAll().Where(x => x.DisplayName.Equals("Mức lương")).FirstOrDefault();
                if (salary != null)
                {
                    catFilterList.Salary =  GetChildrenCatUnit(salary.Id).Result.Items.ToList();
                }
                #endregion


                return catFilterList; 

            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}