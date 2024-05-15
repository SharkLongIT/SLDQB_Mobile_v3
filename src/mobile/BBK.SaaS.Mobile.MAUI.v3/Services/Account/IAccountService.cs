using BBK.SaaS.ApiClient.Models;

namespace BBK.SaaS.Services.Account
{
    public interface IAccountService
    {
        AbpAuthenticateModel AbpAuthenticateModel { get; set; }
        
        AbpAuthenticateResultModel AuthenticateResultModel { get; set; }

        AbpSignUpModel AbpSignUpModel { get; set; }
        
        AbpSignUpResultModel AbpSignUpResultModel { get; set; }
        
        Task LoginUserAsync();

        Task LogoutAsync();
    }
}
