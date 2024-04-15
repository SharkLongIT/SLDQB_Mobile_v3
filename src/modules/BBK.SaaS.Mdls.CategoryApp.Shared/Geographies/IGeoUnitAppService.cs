using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Geographies.Dto;

namespace BBK.SaaS.Mdls.Category.Geographies
{
    public interface IGeoUnitAppService : IApplicationService
    {
        Task<ListResultDto<GeoUnitDto>> GetGeoUnits();

        //Task<PagedResultDto<GeoUnitUserListDto>> GetGeoUnitUsers(GetGeoUnitUsersInput input);
        Task<ListResultDto<GeoUnitDto>> GetChildrenGeoUnit(long id);

		Task<GeoUnitDto> CreateGeoUnit(CreateGeoUnitInput input);

        Task<GeoUnitDto> UpdateGeoUnit(UpdateGeoUnitInput input);

        Task<GeoUnitDto> MoveGeoUnit(MoveGeoUnitInput input);

        Task DeleteGeoUnit(EntityDto<long> input);

        //Task RemoveUserFromGeoUnit(UserToGeoUnitInput input);

        //Task RemoveRoleFromGeoUnit(RoleToGeoUnitInput input);

        //Task AddUsersToGeoUnit(UsersToGeoUnitInput input);

        //Task AddRolesToGeoUnit(RolesToGeoUnitInput input);

        //Task<PagedResultDto<NameValueDto>> FindUsers(FindGeoUnitUsersInput input);

        //Task<PagedResultDto<NameValueDto>> FindRoles(FindGeoUnitRolesInput input);
        
        Task<List<GeoUnitDto>> GetAll();

        Task BuildDemoGeoAsync();

	}
}
