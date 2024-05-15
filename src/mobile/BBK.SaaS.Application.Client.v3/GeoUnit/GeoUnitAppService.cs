using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.GeoUnit
{
    public class GeoUnitAppService : ProxyAppServiceBase, IGeoUnitAppService
    {
        public Task BuildDemoGeoAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GeoUnitDto> CreateGeoUnit(CreateGeoUnitInput input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteGeoUnit(EntityDto<long> input)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GeoUnitDto>> GetAll()
        {
            return await ApiClient.GetAsync<List<GeoUnitDto>>(GetEndpoint(nameof(GetAll)));
        }

        public async Task<ListResultDto<GeoUnitDto>> GetChildrenGeoUnit(long id)
        {
            return await ApiClient.GetAsync<ListResultDto<GeoUnitDto>>(GetEndpoint(nameof(GetChildrenGeoUnit)),id);
        }

        public async Task<ListResultDto<GeoUnitDto>> GetGeoUnits()
        {
            return await ApiClient.GetAnonymousAsync<ListResultDto<GeoUnitDto>>(GetEndpoint(nameof(GetGeoUnits)));
        }

        public Task<GeoUnitDto> MoveGeoUnit(MoveGeoUnitInput input)
        {
            throw new NotImplementedException();
        }

        public Task<GeoUnitDto> UpdateGeoUnit(UpdateGeoUnitInput input)
        {
            throw new NotImplementedException();
        }
    }
}
