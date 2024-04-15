using BBK.SaaS.Dto;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class FindGeoUnitUsersInput : PagedAndFilteredInputDto
    {
        public long GeoUnitId { get; set; }
    }
}
