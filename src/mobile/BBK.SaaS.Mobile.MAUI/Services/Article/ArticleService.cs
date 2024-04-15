using Abp.Dependency;
using BBK.SaaS.Authorization.Users.Profile;
using BBK.SaaS.Mdls.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Services.Article
{
    public class ArticleService : IArticleService, ITransientDependency
    {
        private readonly IArticleFrontEndService _articleFrontEndService;
        public ArticleService(IArticleFrontEndService articleAppService)
        {
            _articleFrontEndService = articleAppService;
        }
        public async Task<string> GetPicture(string encryptedUrl)
        {
            var result = await _articleFrontEndService.GetPicture(encryptedUrl);
            if (string.IsNullOrWhiteSpace(result))
            {
                // todo: return no-image image

                return null;
            }

            return "data:image/png;base64, " + result;
        }
    }
}
