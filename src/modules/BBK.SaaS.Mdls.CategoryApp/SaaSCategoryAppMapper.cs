using AutoMapper;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Category
{
	internal class SaaSCategoryAppMapper
	{
		public static void CreateMappings(IMapperConfigurationExpression configuration)
		{
			//GeoUnit
			configuration.CreateMap<GeoUnit, GeoUnitDto>();

			//Category Unit
			configuration.CreateMap<CatUnit, CatUnitDto>();

		}
	}
}
