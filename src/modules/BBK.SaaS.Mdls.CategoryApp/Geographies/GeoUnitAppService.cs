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
				var LX = await CreateAndSaveGeoUnit(geoUnits, tenant, "Long Xuyên", ag);
				var CD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Châu Đốc", ag);
				var AP = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Phú", ag);
				var TC = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tân Châu", ag);
				var PT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Tân", ag);
				var CP = await CreateAndSaveGeoUnit(geoUnits, tenant, "Châu Phú", ag);
				var TB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tịnh Biên", ag);
				var TT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tri Tôn", ag);
				var CT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Châu Thành", ag);
				var CM = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chợ Mới", ag);
				var TS = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thoại Sơn", ag);
			}
			#endregion
			#region brvt
			var i2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bà Rịa - Vũng Tàu");
			var BRVT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bà Rịa - Vũng Tàu").FirstOrDefaultAsync();
			if (BRVT != null)
			{
				var VT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vũng Tàu", BRVT);
				var BR = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bà Rịa", BRVT);
				var CDBV = await CreateAndSaveGeoUnit(geoUnits, tenant, "Châu Đức", BRVT);
				var XMB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xuyên Mộc", BRVT);
				var LDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Long Điền", BRVT);
				var ĐDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đất Đỏ", BRVT);
				var TTB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tân Thành", BRVT);
				var CDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Côn Đảo", BRVT);
			}
			#endregion

			#region Binh Duong
			var i3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Dương");
			var BiD = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Dương").FirstOrDefaultAsync();
			if (BiD != null)
			{
				var TD1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thủ Dầu Một", BiD);
				var DT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Dầu Tiếng", BiD);
				var BC = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bến Cát", BiD);
				var PG = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Giáo", BiD);
				var TU = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tân Uyên", BiD);
				var DA = await CreateAndSaveGeoUnit(geoUnits, tenant, "Dĩ An", BiD);
				var TA = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thuận An", BiD);
			}
			#endregion

			#region Binh Phuoc
			var i4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Phước");
			var BP = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Phước").FirstOrDefaultAsync();
			if (BP != null)
			{
				var BP1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Xoài", BP);
				var BP2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phước Long", BP);
				var BP3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Long", BP);
				var BP4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bù Gia Mập", BP);
				var BP5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lộc Ninh", BP);
				var BP6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bù Đốp", BP);
				var BP7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Phú", BP);
				var BP8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bù Đăng", BP);
				var BP9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chơn Thành", BP);
			}
			#endregion


			#region BT
			var i5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Thuận");
			var BT = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Thuận").FirstOrDefaultAsync();
			if (BT != null)
			{
				var VT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vũng Tàu", BT);
				var BR = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bà Rịa", BT);
				var CDBV = await CreateAndSaveGeoUnit(geoUnits, tenant, "Châu Đức", BT);
				var XMB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xuyên Mộc", BT);
				var LDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Long Điền", BT);
				var ĐDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đất Đỏ", BT);
				var TTB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tân Thành", BT);
				var CDB = await CreateAndSaveGeoUnit(geoUnits, tenant, "Côn Đảo", BT);
			}
			#endregion

			#region Binh Dinh
			var i6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Định");
			var BDI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bình Định").FirstOrDefaultAsync();
			if (BDI != null)
			{
				var BDI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quy Nhơn", BDI);
				var BDI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Lão", BDI);
				var BDI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hoài Nhơn", BDI);
				var BDI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hoài Ân", BDI);
				var BDI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phù Mỹ", BDI);
				var BDI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Thạnh", BDI);
				var BDI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tây Sơn", BDI);
				var BDI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phù Cát", BDI);
				var BDI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Nhơn", BDI);
				var BDI10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tuy Phước", BDI);
				var BDI11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vân Canh", BDI);
			}
			#endregion

			#region Bạc Liêu
			var i7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bạc Liêu");
			var BL = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bạc Liêu").FirstOrDefaultAsync();
			if (BL != null)
			{
				var BL2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hồng Dân", BL);
				var BL3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phước Long", BL);
				var BL4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Lợi", BL);
				var BL5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Giá Rai", BL);
				var BL6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đông Hải", BL);
				var BL7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hòa Bình", BL);
			}
			#endregion

			#region Bắc Giang
			var i8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Giang");
			var BG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bắc Giang").FirstOrDefaultAsync();
			if (BG != null)
			{
				var BG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Thế", BG);
				var BG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tân Yên", BG);
				var BG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lạng Giang", BG);
				var BG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lục Nam", BG);
				var BG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lục Ngạn", BG);
				var BG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Dũng", BG);
				var BG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Việt Yên", BG);
				var BG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hiệp Hoà", BG);
			}
			#endregion

			#region Bác Kạn
			var i9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Kạn");

			var BK = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bắc Kạn").FirstOrDefaultAsync();
			if (BK != null)
			{
				var BK1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Pắc Nạm", BK);
				var BK2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ba Bể", BK);
				var BK3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ngân Sơn", BK);
				var BK4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bạch Thông", BK);
				var BK5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chợ Đồn", BK);
				var BK6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chợ Mới", BK);
				var BK7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Na Rì", BK);
			}
			#endregion

			#region Bắc Ninh
			var i10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Ninh");
			var BN = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bắc Ninh").FirstOrDefaultAsync();
			if (BN != null)
			{
				var BN1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Phong", BN);
				var BN2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quế Võ", BN);
				var BN3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tiên Du", BN);
				var BN4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thuận Thành", BN);
				var BN5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Gia Bình", BN);
				var BN6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lương Tài", BN);
			}
			#endregion

			#region Bến Tre
			var i11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bến Tre");
			var BTR = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Bến Tre").FirstOrDefaultAsync();
			if (BTR != null)
			{
				var BTR1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Châu Thành", BTR);
				var BTR2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chợ Lách", BTR);
				var BTR3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Mỏ Cày", BTR);
				var BTR4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Giồng Trôm", BTR);
				var BTR5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Đại", BTR);
				var BTR6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ba Tri", BTR);
				var BTR7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thạch Phú", BTR);
			}
			#endregion

			#region Cao Bằng
			var i12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cao Bằng");
			var CB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Cao Bằng").FirstOrDefaultAsync();
			if (CB != null)
			{
				var CB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bảo Lâm", CB);
				var CB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bảo Lạc", CB);
				var CB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thông Nông", CB);
				var CB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Quảng", CB);
				var CB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Trà Lĩnh", CB);
				var CB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Trùng Khánh", CB);
				var CB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hạ Lang", CB);
				var CB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Uyên", CB);
				var CB9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phục Hòa", CB);
				var CB10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nguyên Bình", CB);
				var CB11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thạch An", CB);
			}
			#endregion

			#region Cà Mau

			var i13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cà Mau");
			var CMI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Cà Mau").FirstOrDefaultAsync();
			if (CMI != null)
			{
				var CM1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cà Mau", CMI);
				var CM2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "U Minh", CMI);
				var CM3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thới Bình", CMI);
				var CM4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Trần Văn Thời", CMI);
				var CM5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cái Nước", CMI);
				var CM6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đầm Dơi", CMI);
				var CM7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Năm Căn", CMI);
				var CM8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Tân", CMI);
				var CM9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ngọc Hiển", CMI);
			}
			#endregion

			#region Cần Thơ
			var i14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cần Thơ");
			var CTI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Cần Thơ").FirstOrDefaultAsync();
			if (CTI != null)
			{
				var CTI1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ninh Kiều", CTI);
				var CTI2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ô Môn", CTI);
				var CTI3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Thuỷ", CTI);
				var CTI4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cái Răng", CTI);
				var CTI5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thốt Nốt", CTI);
				var CTI6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Thạch", CTI);
				var CTI7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cờ Đỏ", CTI);
				var CTI8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phong Điền", CTI);
				var CTI9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thới Lai", CTI);
			}
			#endregion

			#region Gia Lai
			var i15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Gia Lai");
			var GL = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Gia Lai").FirstOrDefaultAsync();
			if (GL != null)
			{
				var GL1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Pleiku", GL);
				var GL2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Khê", GL);
				var GL3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ayun Pa", GL);
				var GL4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kbang", GL);
				var GL5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đak Đoa", CTI);
				var GL6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chư Păh", GL);
				var GL7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ia Grai", GL);
				var GL8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Mang Yang", GL);
				var GL9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kông Chro", GL);
				var GL10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đức Cơ", GL);
				var GL11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chư Prông", GL);
				var GL12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chư Sê", GL);
				var GL13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đak Pơ", GL);
			}
			#endregion

			#region Hà Giang
			var i16 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Giang");
			var HG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Giang").FirstOrDefaultAsync();
			if (HG != null)
			{
				var HG1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Văn", HG);
				var HG2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Mèo Vạc", HG);
				var HG3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Minh", HG);
				var HG4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quản Bạ", HG);
				var HG5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vị Xuyên", HG);
				var HG6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Mê", HG);
				var HG7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hoàng Su Phì", HG);
				var HG8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Xín Mần", HG);
				var HG9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bắc Quang", HG);
				var HG10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quang Bình", HG);
			}
			#endregion
			#region Hà Nam

			var i17 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Nam");
			var HNA = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Nam").FirstOrDefaultAsync();
			if (HNA != null)
			{
				var HNA1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phủ Lý", HNA);
				var HNA2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Duy Tiên", HNA);
				var HNA3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kim Bảng", HNA);
				var HNA4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thanh Liêm", HNA);
				var HNA5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Lục", HNA);
				var HNA6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lý Nhân", HNA);
			}
			#endregion

			#region Hà Nội
			var i18 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Nội");
			var hn = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hà Nội").FirstOrDefaultAsync();
			if (hn != null)
			{
				var BD = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Ba Đình", hn);
				var HK = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Hoàn Kiếm", hn);
				var TH = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quận Tây Hồ", hn);
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
				var ST = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thị xã Sơn Tây", hn);
				var BV = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Ba Vì", hn);
				var PT = await CreateAndSaveGeoUnit(geoUnits, tenant, "Huyện Phúc Thọ", hn);
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
				var HT1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hà Tĩnh", HT);
				var HT2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hồng Lĩnh", HT);
				var HT3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hương Sơn", HT);
				var HT4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đức Thọ", HT);
				var HT5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vũ Quang", HT);
				var HT6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nghi Xuân", HT);
				var HT7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Can Lộc", HT);
				var HT8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hương Khê", HT);
				var HT9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thạch Hà", HT);
				var HT10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cẩm Xuyên", HT);
				var HT11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kỳ Anh", HT);
				var HT12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lộc Hà", HT);
			}
			#endregion


			#region Hoà Bỉnh
			var i20 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hòa Bình");
			var HB = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hòa Bình").FirstOrDefaultAsync();
			if (HB != null)
			{
				var HB1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hoà Bình", HB);
				var HB2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đà Bắc", HB);
				var HB3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kỳ Sơn", HB);
				var HB4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lương Sơn", HB);
				var HB5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kim Bôi", HB);
				var HB6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cao Phong", HB);
				var HB7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tân Lạc", HB);
				var HB8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Mai Châu", HB);
				var HB9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lạc Sơn", HB);
				var HB10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Thuỷ", HB);
				var HB11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lạc Thuỷ", HB);
			}

			#endregion

			#region Hưng Yên
			var i21 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hưng Yên");
			var HY = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hưng Yên").FirstOrDefaultAsync();
			if (HY != null)
			{
				var HY1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hưng Yên", HY);
				var HY2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Văn Lâm", HY);
				var HY3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Văn Giang", HY);
				var HY4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Mỹ", HY);
				var HY5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Mỹ Hào", HY);
				var HY6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ân Thi", HY);
				var HY7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Khoái Châu", HY);
				var HY8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kim Động", HY);
				var HY9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tiên Lữ", HY);
				var HY10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phù Cư", HY);
			}
			#endregion

			#region Hải Dương
			var i22 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải Dương");
			var HDI = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hải Dương").FirstOrDefaultAsync();
			if (HDI != null)
			{
				var HD1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải Dương", HDI);
				var HD2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Chí Linh", HDI);
				var HD3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nam Sách", HDI);
				var HD4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kinh Môn", HDI);
				var HD5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kim Thành", HDI);
				var HD6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thanh Hà", HDI);
				var HD7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cẩm Giàng", HDI);
				var HD8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bình Giang", HDI);
				var HD9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Gia Lộc", HDI);
				var HD10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tứ Kỳ", HDI);
				var HD11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ninh Giang", HDI);
				var HD12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thanh Miện", HDI);
			}
			#endregion

			#region Hải Phòng
			var i23 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải Phòng");
			var HP = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hải Phòng").FirstOrDefaultAsync();
			if (HP != null)
			{
				var Hp1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hồng Bàng", HP);
				var Hp2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ngô Quyền", HP);
				var Hp3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lê Chân", HP);
				var Hp4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải An", HP);
				var Hp5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kiến An", HP);
				var Hp6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồ Sơn", HP);
				var Hp7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Dương Kinh", HP);
				var Hp8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thuỷ Nguyên", HP);
				var Hp9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Dương", HP);
				var Hp10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Lão", HP);
				var Hp11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kiến Thuỵ", HP);
				var Hp12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tiên Lãng", HP);
				var Hp13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Bảo", HP);
				var Hp14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cát Hải", HP);
				var Hp15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bạch Long Vĩ", HP);
			}
			#endregion

			#region Hậu Giang
			var i24 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hậu Giang");
			var HAUG = await _geoUnitRepository.GetAll().Where(x => x.DisplayName == "Hậu Giang").FirstOrDefaultAsync();
			if (HAUG != null)
			{
				var Hp1 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hồng Bàng", HAUG);
				var Hp2 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ngô Quyền", HAUG);
				var Hp3 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lê Chân", HAUG);
				var Hp4 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Hải An", HAUG);
				var Hp5 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kiến An", HAUG);
				var Hp6 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồ Sơn", HAUG);
				var Hp7 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Dương Kinh", HAUG);
				var Hp8 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thuỷ Nguyên", HAUG);
				var Hp9 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Dương", HAUG);
				var Hp10 = await CreateAndSaveGeoUnit(geoUnits, tenant, "An Lão", HAUG);
				var Hp11 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kiến Thuỵ", HAUG);
				var Hp12 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tiên Lãng", HAUG);
				var Hp13 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Bảo", HAUG);
				var Hp14 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Cát Hải", HAUG);
				var Hp15 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Bạch Long Vĩ", HAUG);
			}
			#endregion
			var i25 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Khánh Hòa");
			var i26 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kiên Giang");
			var i27 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Kon Tum");
			var i28 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lai Châu");
			var i29 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Long An");
			var i30 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lào Cai");
			var i31 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lâm Đồng");
			var i32 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Lạng Sơn");
			var i33 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nam Định");
			var i34 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Nghệ An");
			var i35 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ninh Bình");
			var i36 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Ninh Thuận");
			var i37 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Thọ");
			var i38 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Phú Yên");



			var i40 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Nam");
			var i41 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Ngãi");
			var i42 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Ninh");
			var i43 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Quảng Trị");
			var i44 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Sóc Trăng");
			var i45 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Sơn La");
			var i46 = await CreateAndSaveGeoUnit(geoUnits, tenant, "TP. Hồ Chí Minh");
			var i47 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thanh Hóa");
			var i48 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thái Bình");
			var i49 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thái Nguyên");
			var i50 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Thừa Thiên Huế");
			var i51 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tiền Giang");
			var i52 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Trà Vinh");
			var i53 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tuyên Quang");
			var i54 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Tây Ninh");
			var i55 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Long");
			var i56 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Vĩnh Phúc");
			var i57 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Yên Bái");
			var i58 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Điện Biên");
			var i59 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đà Nẵng");
			var i60 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đắk Lắk");
			var i61 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đắk Nông");
			var i62 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Nai");
			var i63 = await CreateAndSaveGeoUnit(geoUnits, tenant, "Đồng Tháp");


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