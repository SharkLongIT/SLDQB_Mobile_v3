using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;

namespace BBK.SaaS.Web.Areas.Profile.Models.ApplicationRequestModel
{
  [AutoMapFrom(typeof(ApplicationRequestEditDto))]
  public class ApplicationRequestModel : ApplicationRequestEditDto
  {
        public string RecruitmentsTitle { get; set; }    
  }

  public class ApplicationRequestSearchModel : ApplicationRequestSearch
    {

    }



}
