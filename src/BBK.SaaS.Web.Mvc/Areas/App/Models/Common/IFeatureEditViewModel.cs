using System.Collections.Generic;
using Abp.Application.Services.Dto;
using BBK.SaaS.Editions.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Common
{
    public interface IFeatureEditViewModel
    {
        List<NameValueDto> FeatureValues { get; set; }

        List<FlatFeatureDto> Features { get; set; }
    }
}