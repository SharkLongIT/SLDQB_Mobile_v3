using System.Threading.Tasks;
using BBK.SaaS.Security.Recaptcha;

namespace BBK.SaaS.Test.Base.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
