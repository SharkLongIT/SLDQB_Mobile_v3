using Abp.Dependency;

namespace BBK.SaaS.Web.Xss
{
    public interface IHtmlSanitizer: ITransientDependency
    {
        string Sanitize(string html);
    }
}