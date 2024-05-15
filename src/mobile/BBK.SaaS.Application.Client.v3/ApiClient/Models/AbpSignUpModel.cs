using Abp.Dependency;

namespace BBK.SaaS.ApiClient.Models
{
	public class AbpSignUpModel : ISingletonDependency
	{
		public string Name { get; set; }

		public string UserNameOrEmailAddress { get; set; }

		public string Password { get; set; }
	}

	public class AbpSignUpResultModel
	{
		public long UserId { get; set; }
	}
}
