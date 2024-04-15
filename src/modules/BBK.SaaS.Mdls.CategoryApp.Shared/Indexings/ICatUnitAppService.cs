using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;

namespace BBK.SaaS.Mdls.Category.Indexings
{
    public interface ICatUnitAppService : IApplicationService
    {
        Task<ListResultDto<CatUnitDto>> GetCatUnits();

        //Task<PagedResultDto<CatUnitUserListDto>> GetCatUnitUsers(GetCatUnitUsersInput input);
        Task<ListResultDto<CatUnitDto>> GetChildrenCatUnit(long id);

		Task<CatUnitDto> CreateCatUnit(CreateCatUnitInput input);

        Task<CatUnitDto> UpdateCatUnit(UpdateCatUnitInput input);

        Task<CatUnitDto> MoveCatUnit(MoveCatUnitInput input);

        Task DeleteCatUnit(EntityDto<long> input);

        //Task RemoveUserFromCatUnit(UserToCatUnitInput input);

        //Task RemoveRoleFromCatUnit(RoleToCatUnitInput input);

        //Task AddUsersToCatUnit(UsersToCatUnitInput input);

        //Task AddRolesToCatUnit(RolesToCatUnitInput input);

        //Task<PagedResultDto<NameValueDto>> FindUsers(FindCatUnitUsersInput input);

        //Task<PagedResultDto<NameValueDto>> FindRoles(FindCatUnitRolesInput input);
        
        Task<List<CatUnitDto>> GetAll();

        Task BuildDemoCatAsync();

        #region Mobile/FrontEnd
        Task<CatFilterList> GetFilterList();
        #endregion



    }
}
