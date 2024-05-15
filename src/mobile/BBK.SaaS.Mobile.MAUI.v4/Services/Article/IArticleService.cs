using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Services.Article
{
    public interface IArticleService
    {
        Task<string> GetPicture(string encryptedUrl);
    }
}
