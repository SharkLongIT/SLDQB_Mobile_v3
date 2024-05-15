using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.CatUnit
{
    internal class CatUnitAppService : ProxyAppServiceBase, ICatUnitAppService
    {
        public Task BuildDemoCatAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CatUnitDto> CreateCatUnit(CreateCatUnitInput input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCatUnit(EntityDto<long> input)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CatUnitDto>> GetAll()
        {
            return await ApiClient.GetAsync<List<CatUnitDto>>(GetEndpoint(nameof(GetAll)));
        }

        public Task<ListResultDto<CatUnitDto>> GetCatUnits()
        {
            throw new NotImplementedException();
        }

        public async Task<ListResultDto<CatUnitDto>> GetChildrenCatUnit(long id)
        {
            return await ApiClient.GetAsync<ListResultDto<CatUnitDto>>(GetEndpoint(nameof(GetChildrenCatUnit)), id);
        }

        public async Task<CatFilterList> GetFilterList()
        {
            return await ApiClient.GetAnonymousAsync<CatFilterList>(GetEndpoint(nameof(GetFilterList)));
        }

        public Task<CatUnitDto> MoveCatUnit(MoveCatUnitInput input)
        {
            throw new NotImplementedException();
        }

        public Task<CatUnitDto> UpdateCatUnit(UpdateCatUnitInput input)
        {
            throw new NotImplementedException();
        }
    }
}
