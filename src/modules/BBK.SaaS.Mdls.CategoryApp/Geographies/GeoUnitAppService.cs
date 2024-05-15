using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Linq.Extensions;

namespace BBK.SaaS.Mdls.Category.Geographies
{
	//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits)]
	public class GeoUnitAppService : SaaSAppServiceBase, IGeoUnitAppService
	{
		private readonly GeoUnitManager _geoUnitManager;
		private readonly IRepository<GeoUnit, long> _geoUnitRepository;
		//private readonly IRepository<UserGeoUnit, long> _userGeoUnitRepository;
		//private readonly IRepository<GeoUnitRole, long> _GeoUnitRoleRepository;
		//private readonly RoleManager _roleManager;

		public GeoUnitAppService(
			GeoUnitManager GeoUnitManager,
			IRepository<GeoUnit, long> GeoUnitRepository
			//IRepository<UserGeoUnit, long> userGeoUnitRepository,
			//RoleManager roleManager,
			//IRepository<GeoUnitRole, long> GeoUnitRoleRepository
			)
		{
			_geoUnitManager = GeoUnitManager;
			_geoUnitRepository = GeoUnitRepository;
			//_userGeoUnitRepository = userGeoUnitRepository;
			//_roleManager = roleManager;
			//_GeoUnitRoleRepository = GeoUnitRoleRepository;
		}

		public async Task<ListResultDto<GeoUnitDto>> GetGeoUnits()
		{
			var unit = UnitOfWorkManager.Current;
			if (unit.GetTenantId() == null)
			{
				unit.SetTenantId(1);
			}
			var GeoUnits = await _geoUnitRepository.GetAllListAsync();

			//var GeoUnitMemberCounts = await _userGeoUnitRepository.GetAll()
			//    .GroupBy(x => x.GeoUnitId)
			//    .Select(groupedUsers => new
			//    {
			//        GeoUnitId = groupedUsers.Key,
			//        count = groupedUsers.Count()
			//    }).ToDictionaryAsync(x => x.GeoUnitId, y => y.count);

			//var GeoUnitRoleCounts = await _GeoUnitRoleRepository.GetAll()
			//    .GroupBy(x => x.GeoUnitId)
			//    .Select(groupedRoles => new
			//    {
			//        GeoUnitId = groupedRoles.Key,
			//        count = groupedRoles.Count()
			//    }).ToDictionaryAsync(x => x.GeoUnitId, y => y.count);

			return new ListResultDto<GeoUnitDto>(
				GeoUnits.Select(ou =>
				{
					var GeoUnitDto = ObjectMapper.Map<GeoUnitDto>(ou);
					//GeoUnitDto.MemberCount = GeoUnitMemberCounts.ContainsKey(ou.Id)
					//    ? GeoUnitMemberCounts[ou.Id]
					//    : 0;
					//GeoUnitDto.RoleCount = GeoUnitRoleCounts.ContainsKey(ou.Id)
					//    ? GeoUnitRoleCounts[ou.Id]
					//    : 0;
					return GeoUnitDto;
				}).ToList());
		}

		public async Task<ListResultDto<GeoUnitDto>> GetGeoUnitsByFilter(GetGeoUnitsInput input)
		{
			var unit = UnitOfWorkManager.Current;
			if (unit.GetTenantId() == null)
			{
				unit.SetTenantId(1);
			}

			var query = _geoUnitRepository.GetAll()//.AsNoTracking()
				.WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.DisplayName.Contains(input.Filter))
				.Where(x => x.ParentId == input.ParentId);
			//.ToListAsync();

			var GeoUnits = await query.OrderBy(input.Sorting).ToListAsync();

			//var GeoUnitMemberCounts = await _userGeoUnitRepository.GetAll()
			//    .GroupBy(x => x.GeoUnitId)
			//    .Select(groupedUsers => new
			//    {
			//        GeoUnitId = groupedUsers.Key,
			//        count = groupedUsers.Count()
			//    }).ToDictionaryAsync(x => x.GeoUnitId, y => y.count);

			//var GeoUnitRoleCounts = await _GeoUnitRoleRepository.GetAll()
			//    .GroupBy(x => x.GeoUnitId)
			//    .Select(groupedRoles => new
			//    {
			//        GeoUnitId = groupedRoles.Key,
			//        count = groupedRoles.Count()
			//    }).ToDictionaryAsync(x => x.GeoUnitId, y => y.count);

			return new ListResultDto<GeoUnitDto>(
				GeoUnits.Select(ou =>
				{
					var GeoUnitDto = ObjectMapper.Map<GeoUnitDto>(ou);
					//GeoUnitDto.MemberCount = GeoUnitMemberCounts.ContainsKey(ou.Id)
					//    ? GeoUnitMemberCounts[ou.Id]
					//    : 0;
					//GeoUnitDto.RoleCount = GeoUnitRoleCounts.ContainsKey(ou.Id)
					//    ? GeoUnitRoleCounts[ou.Id]
					//    : 0;
					return GeoUnitDto;
				}).ToList());
		}

		public async Task<ListResultDto<GeoUnitDto>> GetChildrenGeoUnit(long id)
		{
			var GeoUnits = await _geoUnitRepository.GetAll()
				.Where(x => x.ParentId == id).ToListAsync();

			//var GeoUnitMemberCounts = await _userGeoUnitRepository.GetAll()
			//    .GroupBy(x => x.GeoUnitId)
			//    .Select(groupedUsers => new
			//    {
			//        GeoUnitId = groupedUsers.Key,
			//        count = groupedUsers.Count()
			//    }).ToDictionaryAsync(x => x.GeoUnitId, y => y.count);

			//var GeoUnitRoleCounts = await _GeoUnitRoleRepository.GetAll()
			//    .GroupBy(x => x.GeoUnitId)
			//    .Select(groupedRoles => new
			//    {
			//        GeoUnitId = groupedRoles.Key,
			//        count = groupedRoles.Count()
			//    }).ToDictionaryAsync(x => x.GeoUnitId, y => y.count);

			return new ListResultDto<GeoUnitDto>(
				GeoUnits.Select(ou =>
				{
					var GeoUnitDto = ObjectMapper.Map<GeoUnitDto>(ou);
					//GeoUnitDto.MemberCount = GeoUnitMemberCounts.ContainsKey(ou.Id)
					//    ? GeoUnitMemberCounts[ou.Id]
					//    : 0;
					//GeoUnitDto.RoleCount = GeoUnitRoleCounts.ContainsKey(ou.Id)
					//    ? GeoUnitRoleCounts[ou.Id]
					//    : 0;
					return GeoUnitDto;
				}).ToList());
		}

		//public async Task<PagedResultDto<GeoUnitUserListDto>> GetGeoUnitUsers(
		//    GetGeoUnitUsersInput input)
		//{
		//    var query = from ouUser in _userGeoUnitRepository.GetAll()
		//        join ou in _geoUnitRepository.GetAll() on ouUser.GeoUnitId equals ou.Id
		//        join user in UserManager.Users on ouUser.UserId equals user.Id
		//        where ouUser.GeoUnitId == input.Id
		//        select new
		//        {
		//            ouUser,
		//            user
		//        };

		//    var totalCount = await query.CountAsync();
		//    var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

		//    return new PagedResultDto<GeoUnitUserListDto>(
		//        totalCount,
		//        items.Select(item =>
		//        {
		//            var GeoUnitUserDto = ObjectMapper.Map<GeoUnitUserListDto>(item.user);
		//            GeoUnitUserDto.AddedTime = item.ouUser.CreationTime;
		//            return GeoUnitUserDto;
		//        }).ToList());
		//}

		//public async Task<PagedResultDto<GeoUnitRoleListDto>> GetGeoUnitRoles(
		//    GetGeoUnitRolesInput input)
		//{
		//    var query = from ouRole in _GeoUnitRoleRepository.GetAll()
		//        join ou in _geoUnitRepository.GetAll() on ouRole.GeoUnitId equals ou.Id
		//        join role in _roleManager.Roles on ouRole.RoleId equals role.Id
		//        where ouRole.GeoUnitId == input.Id
		//        select new
		//        {
		//            ouRole,
		//            role
		//        };

		//    var totalCount = await query.CountAsync();
		//    var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

		//    return new PagedResultDto<GeoUnitRoleListDto>(
		//        totalCount,
		//        items.Select(item =>
		//        {
		//            var GeoUnitRoleDto = ObjectMapper.Map<GeoUnitRoleListDto>(item.role);
		//            GeoUnitRoleDto.AddedTime = item.ouRole.CreationTime;
		//            return GeoUnitRoleDto;
		//        }).ToList());
		//}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageOrganizationTree)]
		public async Task<GeoUnitDto> CreateGeoUnit(CreateGeoUnitInput input)
		{
			var GeoUnit = new GeoUnit(AbpSession.TenantId, input.DisplayName, input.ParentId);

			await _geoUnitManager.CreateAsync(GeoUnit);
			await CurrentUnitOfWork.SaveChangesAsync();

			return ObjectMapper.Map<GeoUnitDto>(GeoUnit);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageOrganizationTree)]
		public async Task<GeoUnitDto> UpdateGeoUnit(UpdateGeoUnitInput input)
		{
			var GeoUnit = await _geoUnitRepository.GetAsync(input.Id);

			GeoUnit.DisplayName = input.DisplayName;

			await _geoUnitManager.UpdateAsync(GeoUnit);

			return await CreateGeoUnitDto(GeoUnit);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageOrganizationTree)]
		public async Task<GeoUnitDto> MoveGeoUnit(MoveGeoUnitInput input)
		{
			await _geoUnitManager.MoveAsync(input.Id, input.NewParentId);

			return await CreateGeoUnitDto(
				await _geoUnitRepository.GetAsync(input.Id)
			);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageOrganizationTree)]
		public async Task DeleteGeoUnit(EntityDto<long> input)
		{
			//await _userGeoUnitRepository.DeleteAsync(x => x.GeoUnitId == input.Id);
			//await _GeoUnitRoleRepository.DeleteAsync(x => x.GeoUnitId == input.Id);
			await _geoUnitManager.DeleteAsync(input.Id);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageMembers)]
		//public async Task RemoveUserFromGeoUnit(UserToGeoUnitInput input)
		//{
		//    await UserManager.RemoveFromGeoUnitAsync(input.UserId, input.GeoUnitId);
		//}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageRoles)]
		//public async Task RemoveRoleFromGeoUnit(RoleToGeoUnitInput input)
		//{
		//    await _roleManager.RemoveFromGeoUnitAsync(input.RoleId, input.GeoUnitId);
		//}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageMembers)]
		//public async Task AddUsersToGeoUnit(UsersToGeoUnitInput input)
		//{
		//    foreach (var userId in input.UserIds)
		//    {
		//        await UserManager.AddToGeoUnitAsync(userId, input.GeoUnitId);
		//    }
		//}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageRoles)]
		//public async Task AddRolesToGeoUnit(RolesToGeoUnitInput input)
		//{
		//    foreach (var roleId in input.RoleIds)
		//    {
		//        await _roleManager.AddToGeoUnitAsync(roleId, input.GeoUnitId, AbpSession.TenantId);
		//    }
		//}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageMembers)]
		//public async Task<PagedResultDto<NameValueDto>> FindUsers(FindGeoUnitUsersInput input)
		//{
		//    var userIdsInGeoUnit = _userGeoUnitRepository.GetAll()
		//        .Where(uou => uou.GeoUnitId == input.GeoUnitId)
		//        .Select(uou => uou.UserId);

		//    var query = UserManager.Users
		//        .Where(u => !userIdsInGeoUnit.Contains(u.Id))
		//        .WhereIf(
		//            !input.Filter.IsNullOrWhiteSpace(),
		//            u =>
		//                u.Name.Contains(input.Filter) ||
		//                u.Surname.Contains(input.Filter) ||
		//                u.UserName.Contains(input.Filter) ||
		//                u.EmailAddress.Contains(input.Filter)
		//        );

		//    var userCount = await query.CountAsync();
		//    var users = await query
		//        .OrderBy(u => u.Name)
		//        .ThenBy(u => u.Surname)
		//        .PageBy(input)
		//        .ToListAsync();

		//    return new PagedResultDto<NameValueDto>(
		//        userCount,
		//        users.Select(u =>
		//            new NameValueDto(
		//                u.FullName + " (" + u.EmailAddress + ")",
		//                u.Id.ToString()
		//            )
		//        ).ToList()
		//    );
		//}

		//[AbpAuthorize(AppPermissions.Pages_Administration_GeoUnits_ManageRoles)]
		//public async Task<PagedResultDto<NameValueDto>> FindRoles(FindGeoUnitRolesInput input)
		//{
		//    var roleIdsInGeoUnit = _GeoUnitRoleRepository.GetAll()
		//        .Where(uou => uou.GeoUnitId == input.GeoUnitId)
		//        .Select(uou => uou.RoleId);

		//    var query = _roleManager.Roles
		//        .Where(u => !roleIdsInGeoUnit.Contains(u.Id))
		//        .WhereIf(
		//            !input.Filter.IsNullOrWhiteSpace(),
		//            u =>
		//                u.DisplayName.Contains(input.Filter) ||
		//                u.Name.Contains(input.Filter)
		//        );

		//    var roleCount = await query.CountAsync();
		//    var users = await query
		//        .OrderBy(u => u.DisplayName)
		//        .PageBy(input)
		//        .ToListAsync();

		//    return new PagedResultDto<NameValueDto>(
		//        roleCount,
		//        users.Select(u =>
		//            new NameValueDto(
		//                u.DisplayName,
		//                u.Id.ToString()
		//            )
		//        ).ToList()
		//    );
		//}

		public async Task<List<GeoUnitDto>> GetAll()
		{
			var GeoUnits = await _geoUnitRepository.GetAllListAsync();
			return ObjectMapper.Map<List<GeoUnitDto>>(GeoUnits);
		}

		public async Task BuildDemoGeoAsync()
		{
			if (!AbpSession.TenantId.HasValue)
			{
				throw new UserFriendlyException("Cannot use for host!");
			}

			var tenant = await TenantManager.GetByIdAsync(AbpSession.TenantId.Value);

			//Create Organization Units

			var geoUnits = new List<GeoUnit>();

			#region QUẢNG BÌNH
			var i0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Bình");
			var QB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Quảng Bình").FirstOrDefaultAsync();
			if (QB != null)
			{
				var Dong_Hoi = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành Phố Đồng Hới", QB);
				var Minh_Hoa = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Minh Hóa", QB);
				var Tuyen_Hoa = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tuyên Hóa", QB);
				var Quang_Trach = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quảng Trạch", QB);
				var Bo_Trach = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bố Trạch", QB);
				var Quang_Ninh = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quảng Ninh", QB);
				var Le_Thuy = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lệ Thủy", QB);
				var Ba_Don = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Ba Đồn", QB);

				#region Đồng Hới
				var dataDH = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Thành Phố Đồng Hới").FirstOrDefaultAsync();
				if (dataDH != null)
				{
					var dataDH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hải Thành", dataDH);
					var dataDH2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Đồng Phú", dataDH);
					var dataDH3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Bắc Lý", dataDH);
					var dataDH4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Nam Lý", dataDH);
					var dataDH5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Đồng Hải", dataDH);
					var dataDH6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Đồng Sơn", dataDH);
					var dataDH7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Phú Hải", dataDH);
					var dataDH8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Bắc Nghĩa", dataDH);
					var dataDH9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Đức Ninh Đông", dataDH);
					var dataDH10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quang Phú", dataDH);
					var dataDH11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lộc Ninh", dataDH);
					var dataDH12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Bảo Ninh", dataDH);
					var dataDH13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Nghĩa Ninh", dataDH);
					var dataDH14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thuận Đức", dataDH);
					var dataDH15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Đức Ninh", dataDH);
				}
				#endregion

				#region Minh Hoá
				var dataMHoa = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Huyện Minh Hóa").FirstOrDefaultAsync();
				if (dataMHoa != null)
				{
					var dataMH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn Quy Đạt", dataMHoa);
					var dataMH2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Dân Hóa", dataMHoa);
					var dataMH3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trọng Hóa", dataMHoa);
					var dataMH4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hóa Phúc", dataMHoa);
					var dataMH5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hồng Hóa", dataMHoa);
					var dataMH6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hóa Thanh", dataMHoa);
					var dataMH7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hóa Tiến", dataMHoa);
					var dataMH8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hóa Hợp", dataMHoa);
					var dataMH9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Xuân Hóa", dataMHoa);
					var dataMH10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Yên Hóa", dataMHoa);
					var dataMH11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Minh Hóa", dataMHoa);
					var dataMH12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Tân Hóa", dataMHoa);
					var dataMH13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hóa Sơn", dataMHoa);
					var dataMH14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trung Hóa", dataMHoa);
					var dataMH15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thượng Hóa", dataMHoa);
				}
				#endregion

				#region Tuyên Hoá
				var dataTHoa = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Huyện Tuyên Hóa").FirstOrDefaultAsync();
				if (dataTHoa != null)
				{
					var dataTH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn Đồng Lê", dataTHoa);
					var dataTH2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hương Hóa", dataTHoa);
					var dataTH3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Kim Hóa", dataTHoa);
					var dataTH4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thanh Hóa", dataTHoa);
					var dataTH5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thanh Thạch", dataTHoa);
					var dataTH6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thuận Hóa", dataTHoa);
					var dataTH7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lâm Hóa", dataTHoa);
					var dataTH8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lê Hóa", dataTHoa);
					var dataTH9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Sơn Hóa", dataTHoa);
					var dataTH10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Đồng Hóa", dataTHoa);
					var dataTH11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Ngư Hóa", dataTHoa);
					var dataTH12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thạch Hóa", dataTHoa);
					var dataTH13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Đức Hóa", dataTHoa);
					var dataTH14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Phong Hóa", dataTHoa);
					var dataTH15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mai Hóa", dataTHoa);
					var dataTH16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Tiến Hóa", dataTHoa);
					var dataTH17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Châu Hóa", dataTHoa);
					var dataTH18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Cao Quảng", dataTHoa);
					var dataTH19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Văn Hóa", dataTHoa);
				}
				#endregion

				#region Quảng Trạch
				var dataQT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Huyện Quảng Trạch").FirstOrDefaultAsync();
				if (dataQT != null)
				{
					var dataQT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Hợp", dataQT);
					var dataQT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Kim", dataQT);
					var dataQT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Đông", dataQT);
					var dataQT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Phú", dataQT);
					var dataQT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Châu", dataQT);
					var dataQT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Thạch", dataQT);
					var dataQT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Lưu", dataQT);
					var dataQT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Tùng", dataQT);
					var dataQT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Cảnh Dương", dataQT);
					var dataQT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Tiến", dataQT);
					var dataQT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Hưng", dataQT);
					var dataQT12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Xuân", dataQT);
					var dataQT13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Cảnh Hóa", dataQT);
					var dataQT14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Liên Trường", dataQT);
					var dataQT15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Phương", dataQT);
					var dataQT16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Phù Hóa", dataQT);
					var dataQT17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Thanh", dataQT);
				}
				#endregion

				#region Bố Trạch
				var dataBT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Huyện Bố Trạch").FirstOrDefaultAsync();
				if (dataBT != null)
				{
					var dataBT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn Hoàn Lão", dataBT);
					var dataBT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn NT Việt Trung", dataBT);
					var dataBT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Xuân Trạch", dataBT);
					var dataBT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mỹ Trạch", dataBT);
					var dataBT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hạ Trạch", dataBT);
					var dataBT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Bắc Trạch", dataBT);
					var dataBT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lâm Trạch", dataBT);
					var dataBT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thanh Trạch", dataBT);
					var dataBT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Liên Trạch", dataBT);
					var dataBT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Phúc Trạch", dataBT);
					var dataBT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Cự Nẫm", dataBT);
					var dataBT12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hải Phú", dataBT);
					var dataBT13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thượng Trạch", dataBT);
					var dataBT14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Sơn Lộc", dataBT);
					var dataBT15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hưng Trạch", dataBT);
					var dataBT16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Đồng Trạch", dataBT);
					var dataBT17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Đức Trạch", dataBT);
					var dataBT18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn Phong Nha", dataBT);
					var dataBT20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Phú Định", dataBT);
					var dataBT21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trung Trạch", dataBT);
					var dataBT22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Tây Trạch", dataBT);
					var dataBT23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hòa Trạch", dataBT);
					var dataBT24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Đại Trạch", dataBT);
					var dataBT25 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Nhân Trạch", dataBT);
					var dataBT26 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Tân Trạch", dataBT);
					var dataBT27 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Nam Trạch", dataBT);
					var dataBT28 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lý Trạch", dataBT);
				}
				#endregion

				#region Quảng Ninh
				var dataQN = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Huyện Quảng Ninh").FirstOrDefaultAsync();
				if (dataQN != null)
				{
					var dataQN1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn Quán Hàu", dataQN);
					var dataQN2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trường Sơn", dataQN);
					var dataQN3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lương Ninh", dataQN);
					var dataQN4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Vĩnh Ninh", dataQN);
					var dataQN5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Võ Ninh", dataQN);
					var dataQN6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hải Ninh", dataQN);
					var dataQN7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hàm Ninh", dataQN);
					var dataQN8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Duy Ninh", dataQN);
					var dataQN9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Gia Ninh", dataQN);
					var dataQN10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trường Xuân", dataQN);
					var dataQN11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hiền Ninh", dataQN);
					var dataQN12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Xuân Ninh", dataQN);
					var dataQN13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã An Ninh", dataQN);
					var dataQN14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Vạn Ninh", dataQN);
				}
				#endregion

				#region Lệ Thuỷ
				var dataLT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Huyện Lệ Thủy").FirstOrDefaultAsync();
				if (dataLT != null)
				{
					var dataLT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị trấn Kiến Giang", dataLT);
					var dataLT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hồng Thủy", dataLT);
					var dataLT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Ngư Thủy Bắc", dataLT);
					var dataLT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hoa Thủy", dataLT);
					var dataLT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thanh Thủy", dataLT);
					var dataLT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã An Thủy", dataLT);
					var dataLT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Phong Thủy", dataLT);
					var dataLT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Cam Thủy", dataLT);
					var dataLT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Ngân Thủy", dataLT);
					var dataLT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Sơn Thủy", dataLT);
					var dataLT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lộc Thủy", dataLT);
					var dataLT12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Liên Thủy", dataLT);
					var dataLT13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Hưng Thủy", dataLT);
					var dataLT14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Dương Thủy", dataLT);
					var dataLT15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Tân Thủy", dataLT);
					var dataLT16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Phú Thủy", dataLT);
					var dataLT17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Xuân Thủy", dataLT);
					var dataLT18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mỹ Thủy", dataLT);
					var dataLT19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Ngư Thủy", dataLT);
					var dataLT20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mai Thủy", dataLT);
					var dataLT21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Sen Thủy", dataLT);
					var dataLT22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thái Thủy", dataLT);
					var dataLT23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Kim Thủy", dataLT);
					var dataLT24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trường Thủy", dataLT);
					var dataLT25 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lâm Thủy", dataLT);
				}

				#endregion

				#region Ba Đồn
				var dataBD = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Thị xã Ba Đồn").FirstOrDefaultAsync();
				if (dataBD != null)
				{
					var dataBD1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Ba Đồn", dataBD);
					var dataBD2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Quảng Long", dataBD);
					var dataBD3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Quảng Thọ", dataBD);
					var dataBD4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Tiên", dataBD);
					var dataBD5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Trung", dataBD);
					var dataBD6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Quảng Phong", dataBD);
					var dataBD7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Quảng Thuận", dataBD);
					var dataBD8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Tân", dataBD);
					var dataBD9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Hải", dataBD);
					var dataBD10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Sơn", dataBD);
					var dataBD11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Lộc", dataBD);
					var dataBD12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Thủy", dataBD);
					var dataBD13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Văn", dataBD);
					var dataBD14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Quảng Phúc", dataBD);
					var dataBD15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Hòa", dataBD);
					var dataBD16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Quảng Minh", dataBD);
					var dataBD17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Xuân Thủy", dataBD);
					var dataBD18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mỹ Thủy", dataBD);
					var dataBD19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Ngư Thủy", dataBD);
					var dataBD20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mai Thủy", dataBD);
					var dataBD21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Sen Thủy", dataBD);
					var dataBD22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Thái Thủy", dataBD);
					var dataBD23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Kim Thủy", dataBD);
					var dataBD24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Trường Thủy", dataBD);
					var dataBD25 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Lâm Thủy", dataBD);
				}
				#endregion


			}
			#endregion

			#region An Giang
			var i1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Giang");
			var ag = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "An Giang").FirstOrDefaultAsync();
			if (ag != null)
			{
				var LX = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Long Xuyên", ag);
				var CD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Châu Đốc", ag);
				var AP = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện An Phú", ag);
				var TC = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Châu", ag);
				var PT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Tân", ag);
				var CP = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Phú", ag);
				var TB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tịnh Biên", ag);
				var TT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tri Tôn", ag);
				var CT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", ag);
				var CM = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chợ Mới", ag);
				var TS = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thoại Sơn", ag);


                #region Long Xuyen
                var dataLongXuyen = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Long Xuyên").FirstOrDefaultAsync();
                if (dataLongXuyen != null)
                {
                    var dataLX1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Bình", dataLongXuyen);
                    var dataLX2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Long", dataLongXuyen);
                    var dataLX3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Đông Xuyên", dataLongXuyen);
                    var dataLX4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Xuyên", dataLongXuyen);
                    var dataLX5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Bình Đức", dataLongXuyen);
					var dataLX6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Bình Khánh", dataLongXuyen);
                    var dataLX7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Phước", dataLongXuyen);
                    var dataLX8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Quý", dataLongXuyen);
                    var dataLX9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Thới", dataLongXuyen);
                    var dataLX10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Thạnh", dataLongXuyen);
                    var dataLX11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Mỹ Hòa", dataLongXuyen);
                    var dataLX12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mỹ Khánh", dataLongXuyen);
                    var dataLX13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xã Mỹ Hòa Hưng", dataLongXuyen);
                }
                #endregion

            }
            #endregion
            #region brvt
            var i2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bà Rịa - Vũng Tàu");
			var BRVT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bà Rịa - Vũng Tàu").FirstOrDefaultAsync();
			if (BRVT != null)
			{
				var VT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Vũng Tàu", BRVT);
				var BR = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Bà Rịa", BRVT);
				var CDBV = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Đức", BRVT);
				var XMB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Xuyên Mộc", BRVT);
				var LDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Long Điền", BRVT);
				var ĐDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đất Đỏ", BRVT);
				var TTB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Thành", BRVT);
				var CDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Côn Đảo", BRVT);
				var txpm = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Phú Mỹ", BRVT);
			}
			#endregion

			#region Binh Duong
			var i3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Dương");
			var BiD = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Dương").FirstOrDefaultAsync();
			if (BiD != null)
			{
				var TD1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Thủ Dầu Một", BiD);
				var DT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Dầu Tiếng", BiD);
				var BC = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Bến Cát", BiD);
				var PG = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Giáo", BiD);
				var TU = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Tân Uyên", BiD);
				var DA = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Dĩ An", BiD);
				var TA = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Thuận An", BiD);
				var HBTU = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Tân Uyên", BiD);
			}
			#endregion

			#region Binh Phuoc
			var i4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Phước");
			var BP = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Phước").FirstOrDefaultAsync();
			if (BP != null)
			{
				var BP1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Đồng Xoài", BP);
				var BP2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Phước Long", BP);
				var BP3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã  Bình Long", BP);
				var BP4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bù Gia Mập", BP);
				var BP5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lộc Ninh", BP);
				var BP6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bù Đốp", BP);
				var BP7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đồng Phú", BP);
				var BP8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bù Đăng", BP);
				var BP9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chơn Thành", BP);
				var BP10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hớn Quản", BP);
				var BP11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Riềng", BP);
			}
			#endregion


			#region BT
			var i5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Thuận");
			var BT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Thuận").FirstOrDefaultAsync();
			if (BT != null)
			{
				var BT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Phan Thiết", BT);
				var BT2= await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã La Gi", BT);
				var BT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tuy Phong", BT);
				var BT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Bình", BT);
				var BT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hàm Thuận Bắc", BT);
				var BT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hàm Thuận Nam", BT);
				var BT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tánh Linh", BT);
				var BT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đức Linh", BT);
				var BT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hàm Tân", BT);
				var BT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Quí", BT);
			}
			#endregion

			#region Binh Dinh
			var i6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Định");
			var BDI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Định").FirstOrDefaultAsync();
			if (BDI != null)
			{
				var BDI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Quy Nhơn", BDI);
				var BDI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện An Lão", BDI);
				var BDI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã  Hoài Nhơn", BDI);
				var BDI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoài Ân", BDI);
				var BDI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phù Mỹ", BDI);
				var BDI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Thạnh", BDI);
				var BDI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tây Sơn", BDI);
				var BDI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phù Cát", BDI);
				var BDI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã An Nhơn", BDI);
				var BDI10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tuy Phước", BDI);
				var BDI11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vân Canh", BDI);
			}
			#endregion

			#region Bạc Liêu
			var i7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bạc Liêu");
			var BL = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bạc Liêu").FirstOrDefaultAsync();
			if (BL != null)
			{
				var BL1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Bạc Liêu", BL);
				var BL2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hồng Dân", BL);
				var BL3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phước Long", BL);
				var BL4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Lợi", BL);
				var BL5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Giá Rai", BL);
				var BL6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đông Hải", BL);
				var BL7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hòa Bình", BL);
			}
			#endregion

			#region Bắc Giang
			var i8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Giang");
			var BG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bắc Giang").FirstOrDefaultAsync();
			if (BG != null)
			{
				var BG0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Bắc Giang", BG);
				var BG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Thế", BG);
				var BG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Yên", BG);
				var BG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lạng Giang", BG);
				var BG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lục Nam", BG);
				var BG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lục Ngạn", BG);
				var BG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Dũng", BG);
				var BG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Việt Yên", BG);
				var BG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hiệp Hoà", BG);
				var BG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sơn Động", BG);
			}
			#endregion

			#region Bác Kạn
			var i9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Kạn");

			var BK = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bắc Kạn").FirstOrDefaultAsync();
			if (BK != null)
			{
				var BK0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành Phố Bắc Kạn", BK);
				var BK1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Pắc Nạm", BK);
				var BK2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ba Bể", BK);
				var BK3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ngân Sơn", BK);
				var BK4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bạch Thông", BK);
				var BK5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chợ Đồn", BK);
				var BK6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chợ Mới", BK);
				var BK7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Na Rì", BK);
			}
			#endregion

			#region Bắc Ninh
			var i10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Ninh");
			var BN = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bắc Ninh").FirstOrDefaultAsync();
			if (BN != null)
			{
				var BN0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Bắc Ninh", BN);
				var BN1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Phong", BN);
				var BN2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quế Võ", BN);
				var BN3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiên Du", BN);
				var BN4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thuận Thành", BN);
				var BN5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gia Bình", BN);
				var BN6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lương Tài", BN);
				var BN7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Từ Sơn", BN);
			}
			#endregion

			#region Bến Tre
			var i11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bến Tre");
			var BTR = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bến Tre").FirstOrDefaultAsync();
			if (BTR != null)
			{
				var BTR0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Bến Tre", BTR);
				var BTR1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", BTR);
				var BTR2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chợ Lách", BTR);
				var BTR3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mỏ Cày Bắc", BTR);
				var BTR4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Giồng Trôm", BTR);
				var BTR5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Đại", BTR);
				var BTR6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ba Tri", BTR);
				var BTR7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạch Phú", BTR);
                var BTR8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mỏ Cày Nam", BTR);
            }
            #endregion

            #region Cao Bằng
            var i12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cao Bằng");
			var CB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Cao Bằng").FirstOrDefaultAsync();
			if (CB != null)
			{
				var CB0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Cao Bằng", CB);
				var CB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bảo Lâm", CB);
				var CB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bảo Lạc", CB);
				var CB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hà Quảng", CB);
				var CB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trùng Khánh", CB);
				var CB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hạ Lang", CB);
				var CB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, " Huyện Quảng Hòa", CB);
				var CB10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nguyên Bình", CB);
				var CB11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạch An", CB);
				var CB12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoà An", CB);
			}
			#endregion

			#region Cà Mau

			var i13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cà Mau");
			var CMI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Cà Mau").FirstOrDefaultAsync();
			if (CMI != null)
			{
				var CM1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Cà Mau", CMI);
				var CM2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện U Minh", CMI);
				var CM3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thới Bình", CMI);
				var CM4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trần Văn Thời", CMI);
				var CM5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cái Nước", CMI);
				var CM6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đầm Dơi", CMI);
				var CM7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Năm Căn", CMI);
				var CM8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Tân", CMI);
				var CM9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ngọc Hiển", CMI);
			}
			#endregion

			#region Cần Thơ
			var i14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cần Thơ");
			var CTI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Cần Thơ").FirstOrDefaultAsync();
			if (CTI != null)
			{
				var CTI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Ninh Kiều", CTI);
				var CTI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Ô Môn", CTI);
				var CTI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Bình Thuỷ", CTI);
				var CTI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Cái Răng", CTI);
				var CTI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Thốt Nốt", CTI);
				var CTI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Thạch", CTI);
				var CTI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cờ Đỏ", CTI);
				var CTI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phong Điền", CTI);
				var CTI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thới Lai", CTI);
			}
			#endregion

			#region Gia Lai
			var i15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Gia Lai");
			var GL = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Gia Lai").FirstOrDefaultAsync();
			if (GL != null)
			{
				var GL1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Pleiku", GL);
				var GL2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã An Khê", GL);
				var GL3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Ayun Pa", GL);
				var GL4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kbang", GL);
				var GL5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đak Đoa", CTI);
				var GL6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chư Păh", GL);
				var GL7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ia Grai", GL);
				var GL8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mang Yang", GL);
				var GL9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kông Chro", GL);
				var GL10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đức Cơ", GL);
				var GL11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chư Prông", GL);
				var GL12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chư Sê", GL);
				var GL13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đak Pơ", GL);
				var GL14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ia Pa", GL);
				var GL15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông Pa", GL);
				var GL16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Thiện", GL);
				var GL17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chư Pưh", GL);
			}
			#endregion

			#region Hà Giang
			var i16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Giang");
			var HG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Giang").FirstOrDefaultAsync();
			if (HG != null)
			{
				var HG0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hà Giang", HG);
				var HG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đồng Văn", HG);
				var HG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mèo Vạc", HG);
				var HG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Minh", HG);
				var HG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quản Bạ", HG);
				var HG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vị Xuyên", HG);
				var HG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Mê", HG);
				var HG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoàng Su Phì", HG);
				var HG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Xín Mần", HG);
				var HG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Quang", HG);
				var HG10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quang Bình", HG);
			}
			#endregion
			#region Hà Nam

			var i17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Nam");
			var HNA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Nam").FirstOrDefaultAsync();
			if (HNA != null)
			{
				var HNA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Phủ Lý", HNA);
				var HNA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Duy Tiên", HNA);
				var HNA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kim Bảng", HNA);
				var HNA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Liêm", HNA);
				var HNA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Lục", HNA);
				var HNA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lý Nhân", HNA);
			}
			#endregion

			#region Hà Nội
			var i18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Nội");
			var hn = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Nội").FirstOrDefaultAsync();
			if (hn != null)
			{
				var BD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Ba Đình", hn);
				var HK = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hoàn Kiếm", hn);
				var TH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Tây Hồ", hn);
				var LB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Long Biên", hn);
				var CG = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Cầu Giấy", hn);
				var DD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Đống Đa", hn);
				var HTB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hai Bà Trưng", hn);
				var HM = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hoàng Mai", hn);
				var TX = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Thanh Xuân", hn);
				var SS = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sóc Sơn", hn);
				var DA = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đông Anh", hn);
				var GLh = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gia Lâm", hn);
				var NTL = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Nam Từ Liêm", hn);
				var TT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Trì", hn);
				var BTL = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Bắc Từ Liêm", hn);
				var ML = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mê Linh", hn);
				var HD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hà Đông", hn);
				var ST1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Sơn Tây", hn);
				var BV = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ba Vì", hn);
				var PT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phúc Thọ", hn);
				var DP = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đan Phượng", hn);
				var HHD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoài Đức", hn);
				var QO = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quốc Oai", hn);
				var HTT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạch Thất", hn);
				var CM = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chương Mỹ", hn);
				var TO = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Oai", hn);
				var HTTin = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thường Tín", hn);
				var HPX = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Xuyên", hn);
				var HUH = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ứng Hòa", hn);
				var HMD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mỹ Đức", hn);

				#region Hoàn Kiếm
				var hoankiem = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Quận Hoàn Kiếm").FirstOrDefaultAsync();
				if (hoankiem != null)
				{
					var hoankiem1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Phúc Tân", hoankiem);
					var hoankiem2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Đồng Xuân", hoankiem);
					var hoankiem3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Id", hoankiem);
					var hoankiem4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Buồm", hoankiem);
					var hoankiem5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Đào", hoankiem);
					var hoankiem6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Bồ", hoankiem);
					var hoankiem7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Cửa Đông", hoankiem);
					var hoankiem8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Lý Thái Tổ", hoankiem);
					var hoankiem9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Bạc", hoankiem);
					var hoankiem10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Gai", hoankiem);
					var hoankiem11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Chương Dương", hoankiem);
					var hoankiem12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Trống", hoankiem);
					var hoankiem13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Cửa Nam", hoankiem);
					var hoankiem14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Bông", hoankiem);
					var hoankiem15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Tràng Tiền", hoankiem);
					var hoankiem16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Trần Hưng Đạo", hoankiem);
					var hoankiem17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Phan Chu Trinh", hoankiem);
					var hoankiem18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phường Hàng Bài", hoankiem);
				}
				#endregion

			}
			#endregion

			#region Hà Tĩnh
			var i19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Tĩnh");
			var HT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Tĩnh").FirstOrDefaultAsync();
			if (HT != null)
			{
				var HT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hà Tĩnh", HT);
				var HT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Hồng Lĩnh", HT);
				var HT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hương Sơn", HT);
				var HT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đức Thọ", HT);
				var HT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vũ Quang", HT);
				var HT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nghi Xuân", HT);
				var HT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Can Lộc", HT);
				var HT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hương Khê", HT);
				var HT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạch Hà", HT);
				var HT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cẩm Xuyên", HT);
				var HT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kỳ Anh", HT);
				var HT12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Kỳ Anh", HT);
				var HT13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lộc Hà", HT);
			}
			#endregion


			#region Hoà Bỉnh
			var i20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hòa Bình");
			var HB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hòa Bình").FirstOrDefaultAsync();
			if (HB != null)
			{
				var HB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hoà Bình", HB);
				var HB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đà Bắc", HB);
				var HB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kỳ Sơn", HB);
				var HB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lương Sơn", HB);
				var HB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kim Bôi", HB);
				var HB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cao Phong", HB);
				var HB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Lạc", HB);
				var HB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mai Châu", HB);
				var HB9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lạc Sơn", HB);
				var HB10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Thuỷ", HB);
				var HB11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lạc Thuỷ", HB);
			}

			#endregion

			#region Hưng Yên
			var i21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hưng Yên");
			var HY = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hưng Yên").FirstOrDefaultAsync();
			if (HY != null)
			{
				var HY1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hưng Yên", HY);
				var HY2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Lâm", HY);
				var HY3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Giang", HY);
				var HY4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Mỹ", HY);
				var HY5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Mỹ Hào", HY);
				var HY6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ân Thi", HY);
				var HY7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Khoái Châu", HY);
				var HY8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kim Động", HY);
				var HY9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiên Lữ", HY);
				var HY10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phù Cư", HY);
			}
			#endregion

			#region Hải Dương
			var i22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải Dương");
			var HDI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hải Dương").FirstOrDefaultAsync();
			if (HDI != null)
			{
				var HD1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hải Dương", HDI);
				var HD2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Chí Linh", HDI);
				var HD3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nam Sách", HDI);
				var HD4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Kinh Môn", HDI);
				var HD5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kim Thành", HDI);
				var HD6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Hà", HDI);
				var HD7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cẩm Giàng", HDI);
				var HD8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Giang", HDI);
				var HD9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gia Lộc", HDI);
				var HD10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tứ Kỳ", HDI);
				var HD11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ninh Giang", HDI);
				var HD12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Miện", HDI);
			}
			#endregion

			#region Hải Phòng
			var i23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải Phòng");
			var HP = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hải Phòng").FirstOrDefaultAsync();
			if (HP != null)
			{
				var Hp1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hồng Bàng", HP);
				var Hp2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Ngô Quyền", HP);
				var Hp3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Lê Chân", HP);
				var Hp4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hải An", HP);
				var Hp5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Kiến An", HP);
				var Hp6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Đồ Sơn", HP);
				var Hp7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Dương Kinh", HP);
				var Hp8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thuỷ Nguyên", HP);
				var Hp9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện An Dương", HP);
				var Hp10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện An Lão", HP);
				var Hp11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kiến Thuỵ", HP);
				var Hp12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiên Lãng", HP);
				var Hp13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Bảo", HP);
				var Hp14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cát Hải", HP);
				var Hp15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bạch Long Vĩ", HP);
			}
			#endregion

			#region Hậu Giang
			var i24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hậu Giang");
			var HAUG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hậu Giang").FirstOrDefaultAsync();
			if (HAUG != null)
			{
				var HAUG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Vị Thanh", HAUG);
				var HAUG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Ngã Bảy", HAUG);
				var HAUG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành A", HAUG);
				var HAUG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", HAUG);
				var HAUG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phụng Hiệp", HAUG);
				var HAUG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vị Thuỷ", HAUG);
				var HAUG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Long Mỹ", HAUG);
				var HAUG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Long Mỹ", HAUG);
				var HAUG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thuỷ Nguyên", HAUG);
			}
            #endregion

            #region Khánh Hoà
            var i25 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Khánh Hòa");
            var KH = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Khánh Hòa").FirstOrDefaultAsync();
            if (KH != null)
            {
                var Kh1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Nha Trang", KH);
                var Kh2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Cam Ranh", KH);
                var Kh3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cam Lâm", KH);
                var Kh4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vạn Ninh", KH);
                var Kh5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Ninh Hoà", KH);
                var Kh6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Khánh Vĩnh", KH);
                var Kh7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Diên Khánh", KH);
                var Kh8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Khánh Sơn", KH);
                var Kh9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trường Sa", KH);
            }
            #endregion

            #region Kiên Giang
            var i26 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kiên Giang");
            var KG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Kiên Giang").FirstOrDefaultAsync();
            if (KG != null)
            {
                var KG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Rạch Giá", KG);
                var KG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hà Tiên", KG);
                var KG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kiên Lương", KG);
                var KG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hòn Đất", KG);
                var KG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Hiệp", KG);
                var KG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", KG);
                var KG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Giồng Riềng", KG);
                var KG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gò Quao", KG);
                var KG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện An Biên", KG);
                var KG10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện An Minh", KG);
                var KG11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Thuận", KG);
                var KG12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Phú Quốc", KG);
                var KG13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kiên Hải", KG);
                var KG14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện U Minh Thượng", KG);
                var KG15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Giang Thành", KG);
            }
            #endregion

            #region Kon Tum
            var i27 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kon Tum");
            var KT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Kon Tum").FirstOrDefaultAsync();
            if (KT != null)
            {
                var KT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Kon Tum", KT);
                var KT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đak Glêi", KT);
                var KT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ngọc Hồi", KT);
                var KT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đak Tô", KT);
                var KT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kon Plông", KT);
                var KT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kon Rẫy", KT);
                var KT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đak Hà", KT);
                var KT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sa Thầy", KT);
                var KT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tu Mơ Rông", KT);
                var KT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ia H' Drai", KT);
            }
            #endregion


            #region Lai Châu
            var i28 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lai Châu");
            var LCH = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Lai Châu").FirstOrDefaultAsync();
            if (LCH != null)
            {
                var LCH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Lai Châu", LCH);
                var LCH2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tam Đường", LCH);
                var LCH3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mường Tè", LCH);
                var LCH4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sìn Hồ", LCH);
				var LCH5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phong Thổ", LCH);
                var LCH6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Than Uyên", LCH);
                var LCH7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Uyên", LCH);
                var LCH8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nậm Nhùn", LCH);
            }
            #endregion

            #region Long An
            var i29 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Long An");
            var LA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Long An").FirstOrDefaultAsync();
            if (LA != null)
            {
                var LA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Tân An", LA);
                var LA15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Kiến Tường", LA);
                var LA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Hưng", LA);
                var LA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Hưng", LA);
                var LA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mộc Hóa", LA);
                var LA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Thạnh", LA);
                var LA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạnh Hóa", LA);
                var LA7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đức Huệ", LA);
                var LA8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đức Hòa", LA);
                var LA9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bến Lức", LA);
                var LA10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thủ Thừa", LA);
                var LA11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Trụ", LA);
                var LA12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cần Đước", LA);
                var LA13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cần Giuộc", LA);
                var LA14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", LA);
            }
            #endregion

            #region Lào Cai
            var i30 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lào Cai");
            var LCA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Lào Cai").FirstOrDefaultAsync();
            if (LCA != null)
            {
                var LCA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Lào Cai", LCA);
                var LCA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bát Xát", LCA);
                var LCA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mường Khương", LCA);
                var LCA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Si Ma Cai", LCA);
                var LCA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Hà", LCA);
                var LCA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bảo Thắng", LCA);
                var LCA7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bảo Yên", LCA);
                var LCA8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Sa Pa", LCA);
                var LCA9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Bàn", LCA);
            }
            #endregion

            #region Lâm Đồng
            var i31 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lâm Đồng");

            var LD = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Lâm Đồng").FirstOrDefaultAsync();
            if (LD != null)
            {
                var LD1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Đà Lạt", LD);
                var LD2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Bảo Lộc", LD);
                var LD3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đam Rông", LD);
                var LD4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lạc Dương", LD);
                var LD5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lâm Hà", LD);
                var LD6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đơn Dương", LD);
                var LD7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đức Trọng", LD);
                var LD8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Di Linh", LD);
                var LD9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bảo Lâm", LD);
                var LD10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đạ Huoai", LD);
                var LD11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đạ Tẻh", LD);
                var LD12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cát Tiên", LD);
            }

            #endregion

            #region Lạng Sơn
            var i32 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lạng Sơn");
            var LS = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Lạng Sơn").FirstOrDefaultAsync();
            if (LS != null)
            {
                var LS1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Lạng Sơn", LS);
                var LS2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tràng Định", LS);
                var LS3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Gia", LS);
                var LS4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Lãng", LS);
                var LS5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cao Lộc", LS);
                var LS6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Quan", LS);
                var LS7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Sơn", LS);
                var LS8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hữu Lũng", LS);
                var LS9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chi Lăng", LS);
                var LS10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lộc Bình", LS);
                var LS11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đình Lập", LS);
            }
            #endregion

            #region Nam Định
            var i33 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nam Định");
            var ND = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Nam Định").FirstOrDefaultAsync();
            if (ND != null)
            {
                var ND1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Nam Định", ND);
                var ND2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mỹ Lộc", ND);
                var ND3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vụ Bản", ND);
                var ND4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ý Yên", ND);
                var ND5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nghĩa Hưng", ND);
                var ND6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nam Trực", ND);
                var ND7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trực Ninh", ND);
                var ND8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Xuân Trường", ND);
                var ND9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Giao Thủy", ND);
                var ND10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hải Hậu", ND);
            }
            #endregion

            #region Nghệ An
            var i34 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nghệ An");
            var NA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Nghệ An").FirstOrDefaultAsync();
            if (NA != null)
            {
                var NA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Vinh", NA);
                var NA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Cửa Lò", NA);
                var NA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Thái Hòa", NA);
                var NA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quế Phong", NA);
                var NA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quỳ Châu", NA);
                var NA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kỳ Sơn", NA);
                var NA7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tương Dương", NA);
                var NA8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nghĩa Đàn", NA);
                var NA9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quỳ Hợp", NA);
                var NA10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quỳnh Lưu", NA);
                var NA11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Con Cuông", NA);
                var NA12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Kỳ", NA);
                var NA13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Anh Sơn", NA);
                var NA14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Diễn Châu", NA);
                var NA15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Thành", NA);
                var NA16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đô Lương", NA);
                var NA17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Chương", NA);
                var NA18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nghi Lộc", NA);
                var NA19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nam Đàn", NA);
                var NA20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hưng Nguyên", NA);
                var NA21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Hoàng Mai", NA);

            }
            #endregion
            #region Ninh Bình
            var i35 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ninh Bình");
            var NB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Ninh Bình").FirstOrDefaultAsync();
            if (NB != null)
            {
                var NB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Ninh Bình", NB);
                var NB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Tam Điệp", NB);
                var NB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nho Quan", NB);
                var NB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gia Viễn", NB);
                var NB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoa Lư", NB);
                var NB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Khánh", NB);
                var NB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kim Sơn", NB);
                var NB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Mô", NB);
            }
            #endregion
            #region Ninh Thuận
            var i36 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ninh Thuận");

            var NT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Ninh Thuận").FirstOrDefaultAsync();
            if (NT != null)
            {
                var NT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Phan Rang-Tháp Chàm", NT);
                var NT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bác Ái", NT);
                var NT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ninh Sơn", NT);
                var NT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ninh Hải", NT);
                var NT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ninh Phước", NT);
                var NT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thuận Bắc", NT);
                var NT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thuận Nam", NT);
            }
            #endregion
            #region Phú Thọ
            var i37 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Thọ");
            var PT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Phú Thọ").FirstOrDefaultAsync();
            if (PT != null)
            {
                var PT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Việt Trì", PT);
                var PT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Phú Thọ", PT);
                var PT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đoan Hùng", PT);
                var PT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hạ Hòa", PT);
                var PT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Ba", PT);
                var PT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phù Ninh", PT);
                var PT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Lập", PT);
                var PT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cẩm Khê", PT);
                var PT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tam Nông", PT);
                var PT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lâm Thao", PT);
                var PT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Sơn", PT);
                var PT12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Thủy", PT);
                var PT13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Sơn", PT);
            }
            #endregion
            #region Phú Yên
            var i38 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Yên");
            var PY = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Phú Yên").FirstOrDefaultAsync();
            if (PY != null)
            {
                var PY1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Tuy Hòa", PY);
                var PY2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Sông Cầu", PY);
                var PY3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đồng Xuân", PY);
                var PY4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tuy An", PY);
                var PY5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sơn Hòa", PY);
                var PY6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sông Hinh", PY);
                var PY7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tây Hòa", PY);
                var PY8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Hòa", PY);
                var PY9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Đông Hòa", PY);
            }
            #endregion

            #region Quảng Nam
            var i40 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Nam");
            var QNA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Quảng Nam").FirstOrDefaultAsync();
            if (QNA != null)
            {
                var QNA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Tam Kỳ", QNA);
                var QNA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hội An", QNA);
                var QNA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tây Giang", QNA);
                var QNA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đông Giang", QNA);
                var QNA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đại Lộc", QNA);
                var QNA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Điện Bàn", QNA);
                var QNA7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Duy Xuyên", QNA);
                var QNA8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quế Sơn", QNA);
                var QNA9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nam Giang", QNA);
                var QNA10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phước Sơn", QNA);
                var QNA11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hiệp Đức", QNA);
                var QNA12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thăng Bình", QNA);
                var QNA13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiên Phước", QNA);
                var QNA14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Trà My", QNA);
                var QNA15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nam Trà My", QNA);
                var QNA16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Núi Thành", QNA);
                var QNA17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Ninh", QNA);
                var QNA18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nông Sơn", QNA);
            }
            #endregion

            #region Quảng Ngãi
            var i41 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Ngãi");
            var QNG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Quảng Ngãi").FirstOrDefaultAsync();
            if (QNG != null)
            {
                var QNG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Quảng Ngãi", QNG);
                var QNG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Sơn", QNG);
                var QNG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trà Bồng", QNG);
                var QNG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tây Trà", QNG);
                var QNG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sơn Tịnh", QNG);
                var QNG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tư Nghĩa", QNG);
                var QNG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sơn Hà", QNG);
                var QNG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sơn Tây", QNG);
                var QNG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Minh Long", QNG);
                var QNG10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nghĩa Hành", QNG);
                var QNG11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mộ Đức", QNG);
                var QNG12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Đức Phổ", QNG);
                var QNG13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ba Tơ", QNG);
                var QNG14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lý Sơn", QNG);
            }
            #endregion

            #region Quảng Ninh
            var i42 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Ninh");
            var QNI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Quảng Ninh").FirstOrDefaultAsync();
            if (QNI != null)
            {
                var QNI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hạ Long", QNI);
                var QNI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Móng Cái", QNI);
                var QNI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Cẩm Phả", QNI);
                var QNI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Uông Bí", QNI);
                var QNI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Liêu", QNI);
                var QNI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiên Yên", QNI);
                var QNI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đầm Hà", QNI);
                var QNI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hải Hà", QNI);
                var QNI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ba Chẽ", QNI);
                var QNI10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vân Đồn", QNI);
                var QNI11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoành Bồ", QNI);
                var QNI12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Đông Triều", QNI);
                var QNI14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cô Tô", QNI);
                var QNI15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Quảng Yên", QNI);
            }
            #endregion

            #region Quảng Trị
            var i43 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Trị");
            var QT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Quảng Trị").FirstOrDefaultAsync();
            if (QT != null)
            {
                var QT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Đông Hà", QT);
                var QT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Quảng Trị", QT);
                var QT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Linh", QT);
                var QT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hướng Hóa", QT);
                var QT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gio Linh", QT);
                var QT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đakrông", QT);
                var QT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cam Lộ", QT);
                var QT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Triệu Phong", QT);
                var QT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hải Lăng", QT);
                var QT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cồn Cỏ", QT);
            }
            #endregion
            #region Sóc Trăng
            var i44 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Sóc Trăng");
            var ST = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Sóc Trăng").FirstOrDefaultAsync();
            if (ST != null)
            {
                var ST1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Sóc Trăng", ST);
                var ST2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", ST);
                var ST3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kế Sách", ST);
                var ST4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mỹ Tú", ST);
                var ST5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cù Lao Dung", ST);
                var ST6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Long Phú", ST);
                var ST7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mỹ Xuyên", ST);
                var ST8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Ngã Năm", ST);
                var ST9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạnh Trị", ST);
                var ST10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Vĩnh Châu", ST);
                var ST11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trần Đề", ST);
            }
            #endregion

            #region Sơn La
            var i45 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Sơn La");
            var SL = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Sơn La").FirstOrDefaultAsync();
            if (SL != null)
            {
                var SL1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Sơn La", SL);
                var SL2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quỳnh Nhai", SL);
                var SL3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thuận Châu", SL);
                var SL4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mường La", SL);
                var SL5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bắc Yên", SL);
                var SL6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phù Yên", SL);
                var SL7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mộc Châu", SL);
                var SL8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Châu", SL);
                var SL9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mai Sơn", SL);
                var SL10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sông Mã", SL);
                var SL11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sốp Cộp", SL);
            }
            #endregion

          

            #region Thanh Hoá
            var i47 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thanh Hóa");
            var TH = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Thanh Hóa").FirstOrDefaultAsync();
            if (TH != null)
            {
                var TH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Thanh Hóa", TH);
                var TH2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Bỉm Sơn", TH);
                var TH3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Sầm Sơn", TH);
                var TH4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mường Lát", TH);
                var TH5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quan Hóa", TH);
                var TH6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bá Thước", TH);
                var TH7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quan Sơn", TH);
                var TH8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lang Chánh", TH);
                var TH9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ngọc Lặc", TH);
                var TH10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cẩm Thủy", TH);
                var TH11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thạch Thành", TH);
                var TH12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hà Trung", TH);
                var TH13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Lộc", TH);
                var TH14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Định", TH);
                var TH15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thọ Xuân", TH);
                var TH16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thường Xuân", TH);
                var TH17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Triệu Sơn", TH);
                var TH18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thiệu Hóa", TH);
                var TH19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoằng Hóa", TH);
                var TH20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hậu Lộc", TH);
                var TH21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nga Sơn", TH);
                var TH22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Như Xuân", TH);
                var TH23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Như Thanh", TH);
                var TH24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nông Cống", TH);
                var TH25 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đông Sơn", TH);
                var TH26 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quảng Xương", TH);
                var TH27 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tĩnh Gia", TH);
                var TH28 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Nghi Sơn", TH);
            }
            #endregion
            #region Thái Bình
            var i48 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thái Bình");
            var TB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Thái Bình").FirstOrDefaultAsync();
            if (TB != null)
            {
                var TB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Thái Bình", TB);
                var TB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quỳnh Phụ", TB);
                var TB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hưng Hà", TB);
                var TB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đông Hưng", TB);
                var TB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thái Thụy", TB);
                var TB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiền Hải", TB);
                var TB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Kiến Xương", TB);
                var TB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vũ Thư", TB);
            }
            #endregion
            #region Thái Nguyên
            var i49 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thái Nguyên");
            var TNG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Thái Nguyên").FirstOrDefaultAsync();
            if (TNG != null)
            {
                var TNG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Thái Nguyên", TNG);
                var TNG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Sông Công", TNG);
                var TNG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Định Hóa", TNG);
                var TNG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Lương", TNG);
                var TNG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đồng Hỷ", TNG);
                var TNG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Võ Nhai", TNG);
                var TNG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đại Từ", TNG);
                var TNG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Phổ Yên", TNG);
                var TNG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Bình", TNG);
            }
            #endregion

            #region Thừa thiên Huế
            var i50 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thừa Thiên Huế");
            var TTH = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Thừa Thiên Huế").FirstOrDefaultAsync();
            if (TTH != null)
            {
                var TTH1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Huế", TTH);
                var TTH2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phong Điền", TTH);
                var TTH3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Quảng Điền", TTH);
                var TTH4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Vang", TTH);
                var TTH5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Hương Thủy", TTH);
                var TTH6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Hương Trà", TTH);
                var TTH7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện A Lưới", TTH);
                var TTH8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phú Lộc", TTH);
                var TTH9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nam Đông", TTH);
            }
            #endregion

            #region Tiền Giang
            var i51 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tiền Giang");
            var TG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Tiền Giang").FirstOrDefaultAsync();
            if (TG != null)
            {
                var TG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Mỹ Tho", TG);
                var TG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Gò Công", TG);
                var TG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Phước", TG);
                var TG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cái Bè", TG);
                var TG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Cai Lậy", TG);
                var TG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", TG);
                var TG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chợ Gạo", TG);
                var TG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gò Công Tây", TG);
                var TG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gò Công Đông", TG);
                var TG10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Phú Đông", TG);
            }
            #endregion

            #region Trà Vinh
            var i52 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Trà Vinh");
            var TV = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Trà Vinh").FirstOrDefaultAsync();
            if (TV != null)
            {
                var TV1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Trà Vinh", TV);
                var TV2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Càng Long", TV);
                var TV3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cầu Kè", TV);
                var TV4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tiểu Cần", TV);
                var TV5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", TV);
                var TV6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cầu Ngang", TV);
                var TV7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trà Cú", TV);
                var TV8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Duyên Hải", TV);
                var TV9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Duyên Hải", TV);
            }
            #endregion

            #region Tuyên Quang
            var i53 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tuyên Quang");
            var TQ = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Tuyên Quang").FirstOrDefaultAsync();
            if (TQ != null)
            {
                var TQ1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Tuyên Quang", TQ);
                var TQ2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Na Hang", TQ);
                var TQ3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Chiêm Hóa", TQ);
                var TQ4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hàm Yên", TQ);
                var TQ5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Sơn", TQ);
                var TQ6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sơn Dương", TQ);
                var TQ7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lâm Bình", TQ);
            }
            #endregion
            #region Tây Ninh
            var i54 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tây Ninh");
            var TNI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Tây Ninh").FirstOrDefaultAsync();
            if (TNI != null)
            {
                var TNI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Tây Ninh", TNI);
                var TNI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Biên", TNI);
                var TNI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Châu", TNI);
                var TNI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Dương Minh Châu", TNI);
                var TNI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", TNI);
                var TNI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Hòa Thành", TNI);
                var TNI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Gò Dầu", TNI);
                var TNI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bến Cầu", TNI);
                var TNI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Trảng Bàng", TNI);
            }
            #endregion

            #region Vĩnh Long
            var i55 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Long");

            var VL = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Vĩnh Long").FirstOrDefaultAsync();
            if (VL != null)
            {
                var VL1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Vĩnh Long", VL);
                var VL2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Long Hồ", VL);
                var VL3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mang Thít", VL);
                var VL4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vũng Liêm", VL);
                var VL5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tam Bình", VL);
                var VL6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Bình Minh", VL);
                var VL7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trà Ôn", VL);
                var VL8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Tân", VL);
            }
            #endregion

            #region Vĩnh Phúc
            var i56 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Phúc");
            var VP = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Vĩnh Phúc").FirstOrDefaultAsync();
            if (VP != null)
            {
                var VP1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Vĩnh Yên", VP);
                var VP2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Phúc Yên", VP);
                var VP3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lập Thạch", VP);
                var VP4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tam Dương", VP);
                var VP5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tam Đảo", VP);
                var VP6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Xuyên", VP);
                var VP7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Lạc", VP);
                var VP8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Tường", VP);
                var VP9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Sông Lô", VP);
            }
            #endregion

          

            #region Yên Bái
            var i57 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Bái");
            var YB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Yên Bái").FirstOrDefaultAsync();
            if (YB != null)
            {
                var YB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Yên Bái", YB);
                var YB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Nghĩa Lộ", YB);
                var YB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lục Yên", YB);
                var YB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Yên", YB);
                var YB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mù Cang Chải", YB);
                var YB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trấn Yên", YB);
                var YB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trạm Tấu", YB);
                var YB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Văn Chấn", YB);
                var YB9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Yên Bình", YB);
            }
            #endregion

            #region TP Hồ Chí Minh
            var i46 = await CreateAndSaveGeoUnit(geoUnits, tenant, "TP. Hồ Chí Minh");
            var TPHCM = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "TP. Hồ Chí Minh").FirstOrDefaultAsync();
            if (TPHCM != null)
            {
                var TPHCM1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 1", TPHCM);
                var TPHCM2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 12", TPHCM);
                var TPHCM3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Thủ Đức", TPHCM);
                var TPHCM4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 9", TPHCM);
                var TPHCM5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Gò Vấp", TPHCM);
                var TPHCM6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Bình Thạnh", TPHCM);
                var TPHCM7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Tân Bình", TPHCM);
                var TPHCM8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Tân Phú", TPHCM);
                var TPHCM9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Phú Nhuận", TPHCM);
                var TPHCM10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 2", TPHCM);
                var TPHCM11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 3", TPHCM);
                var TPHCM12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 10", TPHCM);
                var TPHCM13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 11", TPHCM);
                var TPHCM14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 4", TPHCM);
                var TPHCM15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 5", TPHCM);
                var TPHCM16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 6", TPHCM);
                var TPHCM17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 7", TPHCM);
                var TPHCM18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận 8", TPHCM);
                var TPHCM19 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Bình Tân", TPHCM);
                var TPHCM20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Củ Chi", TPHCM);
                var TPHCM21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hóc Môn", TPHCM);
                var TPHCM22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Bình Chánh", TPHCM);
                var TPHCM23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nhà Bè", TPHCM);
                var TPHCM24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cần Giờ", TPHCM);
            }
            #endregion

            #region Điện Biên
            var i58 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Điện Biên");
            var DB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Điện Biên").FirstOrDefaultAsync();
            if (DB != null)
            {
                var DB0 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Điện Biên Phủ", DB);
                var DB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị Xã Mường Lay", DB);
                var DB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Huyện Mường Nhé", DB);
                var DB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mường Chà", DB);
                var DB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tủa Chùa", DB);
                var DB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tuần Giáo", DB);
                var DB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Điện Biên", DB);
                var DB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Điện Biên Đông", DB);
                var DB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Mường Ảng", DB);
                var DB9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nậm Pồ", DB);
            }
            #endregion

            #region Đà Nẵng
            var i59 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đà Nẵng");
            var DNG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Đà Nẵng").FirstOrDefaultAsync();
            if (DNG != null)
            {
                var DNG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Liên Chiểu", DNG);
                var DNG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Thanh Khê", DNG);
                var DNG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hải Châu", DNG);
                var DNG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Sơn Trà", DNG);
                var DNG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Ngũ Hành Sơn", DNG);
                var DNG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Cẩm Lệ", DNG);
                var DNG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hòa Vang", DNG);
                var DNG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Hoàng Sa", DNG);
            }
            #endregion
            #region Đắk Lắk
            var i60 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đắk Lắk");
            var DLA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Đắk Lắk").FirstOrDefaultAsync();
            if (DLA != null)
            {
                var DLA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Buôn Ma Thuột", DLA);
                var DLA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị Xã Buôn Hồ", DLA);
                var DLA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ea H'leo", DLA);
                var DLA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ea Súp", DLA);
                var DLA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Buôn Đôn", DLA);
                var DLA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cư M'gar", DLA);
                var DLA7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông Búk", DLA);
                var DLA8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông Năng", DLA);
                var DLA9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ea Kar", DLA);
                var DLA10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện M'Đrắk", DLA);
                var DLA11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông Bông", DLA);
                var DLA12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông Pắc", DLA);
                var DLA13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông A Na", DLA);
                var DLA14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lắk", DLA);
                var DLA15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cư Kuin", DLA);
            }
            #endregion

            #region Đắk Nông
            var i61 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đắk Nông");
            var DNOG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Đắk Nông").FirstOrDefaultAsync();
            if (DNOG != null)
            {
                var DNOG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Gia Nghĩa", DNOG);
                var DNOG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đăk Glong", DNOG);
                var DNOG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cư Jút", DNOG);
                var DNOG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đắk Mil", DNOG);
                var DNOG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Krông Nô", DNOG);
                var DNOG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đắk Song", DNOG);
                var DNOG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Đắk R'Lấp", DNOG);
                var DNOG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tuy Đức", DNOG);
            }
            #endregion

            #region Đồng Nai
            var i62 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Nai");
            var DNAI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Đồng Nai").FirstOrDefaultAsync();
            if (DNAI != null)
            {
                var DNAI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Biên Hòa", DNAI);
                var DNAI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Long Khánh", DNAI);
                var DNAI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Phú", DNAI);
                var DNAI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Vĩnh Cửu", DNAI);
                var DNAI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Định Quán", DNAI);
                var DNAI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Trảng Bom", DNAI);
                var DNAI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thống Nhất", DNAI);
                var DNAI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Cẩm Mỹ", DNAI);
                var DNAI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Long Thành", DNAI);
                var DNAI10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Xuân Lộc", DNAI);
                var DNAI11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Nhơn Trạch", DNAI);
            }
            #endregion

            #region Đồng Tháp
            var i63 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Tháp");
            var DT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Đồng Tháp").FirstOrDefaultAsync();
            if (DT != null)
            {
                var DT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Cao Lãnh", DT);
                var DT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Sa Đéc", DT);
                var DT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thành phố Hồng Ngự", DT);
                var DT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tân Hồng", DT);
                var DT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tam Nông", DT);
                var DT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Tháp Mười", DT);
                var DT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Thanh Bình", DT);
                var DT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lấp Vò", DT);
                var DT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Lai Vung", DT);
                var DT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Châu Thành", DT);
            }
            #endregion

        }

        private async Task<GeoUnitDto> CreateGeoUnitDto(GeoUnit GeoUnit)
		{
			var dto = ObjectMapper.Map<GeoUnitDto>(GeoUnit);
			//dto.MemberCount =
			//    await _userGeoUnitRepository.CountAsync(uou => uou.GeoUnitId == GeoUnit.Id);
			return dto;
		}

		private async Task<GeoUnit> CreateAndSaveGeoUnit(List<GeoUnit> geoUnits, Tenant tenant, string displayName, GeoUnit parent = null)
		{
			var geoUnit = new GeoUnit(tenant.Id, displayName, parent == null ? (long?)null : parent.Id);

			await _geoUnitManager.CreateAsync(geoUnit);
			await CurrentUnitOfWork.SaveChangesAsync();

			geoUnits.Add(geoUnit);

			return geoUnit;
		}
	}
}