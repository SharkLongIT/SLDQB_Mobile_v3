using Abp.Application.Services.Dto;
using Abp.HtmlSanitizer;
using Abp.RealTime;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Redis;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Caching;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Categories.MDto;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Cms.UrlRecords;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Web.Models.Cms.Articles;
using BBK.SaaS.Web.Session;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ArticleController : SaaSControllerBase
    {
        //private readonly ITopicAppService _topicAppService;
        private readonly ICmsCatsAppService _categoryAppService;
        private readonly IFECntCategoryAppService _categoryFrontEndService;
        private readonly IArticlesAppService _articlesAppService;
        private readonly IArticleFrontEndService _articleFrontEndService;
        private readonly IAbpPerRequestRedisCacheManager _userCacheManager;
        private readonly IOnlineClientManager _onlineClientManager;
        private readonly ICmsRedisCacheManager _cmsRedisCacheManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISlugCache _slugCache;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        private readonly IIntroduceAppService _introduceAppService;

        public ArticleController(ICmsCatsAppService categoryAppService,
            IFECntCategoryAppService categoryFrontEndService,
            IArticlesAppService articlesAppService,
            IArticleFrontEndService articleFrontEndService,
            IAbpPerRequestRedisCacheManager userCacheManager,
            IOnlineClientManager onlineClientManager,
            ICmsRedisCacheManager cmsRedisCacheManager,
            ICacheManager cacheManager,
            ISlugCache slugCache,
            IPerRequestSessionCache perRequestSessionCache,
            IIntroduceAppService introduceAppService)
        {
            _categoryAppService = categoryAppService;
            _categoryFrontEndService = categoryFrontEndService;
            _articlesAppService = articlesAppService;
            _articleFrontEndService = articleFrontEndService;
            _userCacheManager = userCacheManager;
            _onlineClientManager = onlineClientManager;
            _cmsRedisCacheManager = cmsRedisCacheManager;
            _cacheManager = cacheManager;
            _slugCache = slugCache;
            _perRequestSessionCache = perRequestSessionCache;
            _introduceAppService = introduceAppService;
        }

        //[Route("CategoryArticle")]
        public async Task<IActionResult> ArticleListByCategory(long id, int? page)
        {
            //var dto = await _topicAppService.GetTopicDetailAsync(long.Parse(id));
            //TopicViewModel model = ObjectMapper.Map<TopicViewModel>(dto);
            ListResultDto<ArticleListViewDto> articlesInCat = new();// await _articleFrontEndService.GetArticlesByCategory(new Mdls.Cms.Articles.MDto.GetArticlesByCatInput() { CategoryId = id, MaxResultCount = 5 });

            if (page.HasValue && page.Value > 1)
            {
                //await _categoryFrontEndService.GetCategory(id);
                articlesInCat = await _articleFrontEndService.GetArticlesByCategory(new Mdls.Cms.Articles.MDto.GetArticlesByCatInput() { CategoryId = id, MaxResultCount = 25 });
            }
            else
            {
                articlesInCat = await _articleFrontEndService.GetArticlesByCategory(new Mdls.Cms.Articles.MDto.GetArticlesByCatInput() { CategoryId = id, SkipCount = (page.Value - 1) * 25, MaxResultCount = 25 });
            }

            ListArticlesViewModel model = new();
            model.Articles = articlesInCat.Items;

            ////Try to get from cache
            //var foundCount = _userCacheManager
            //	.GetCache("IUCache")
            //	.Get($"cat-{id}", (id) => { return 1; }) as int?;

            //if (foundCount == 1)
            //{
            //	++foundCount;
            //	await _userCacheManager
            //		.GetCache("IUCache")
            //		.SetAsync($"cat-{id}", foundCount);
            //}
            //else
            //{
            //	++foundCount;
            //	await _userCacheManager
            //		.GetCache("IUCache")
            //		.SetAsync($"cat-{id}", foundCount);
            //}

            //ViewBag.ViewsCount = (_userCacheManager
            //	.GetCache("IUCache")
            //	.Get($"cat-{id}", (id) => { return 1; }) as int?) ?? 0;

            //ViewBag.OnlineClientNum = _onlineClientManager.GetAllClients().Count;

            return View("Index", model);
        }

        [Route("CategoryArticle")]
        public async Task<IActionResult> CategoryArticle(long id, int? page)
        {
            ContentCategoryDto dto = new();

            int pageSize = 9; // five in the top and nine in paging place

            if (page.HasValue && page.Value > 1)
            {
                dto = await _categoryFrontEndService.GetCategory(
                    new GetCategoryInput()
                    {
                        CategoryId = id,
                        SearchArticlesInput = new SearchArticlesInput() { MaxResultCount = 5 }
                    });

                var dtoPage = await _categoryFrontEndService.GetCategory(
                    new GetCategoryInput()
                    {
                        CategoryId = id,
                        SearchArticlesInput = new SearchArticlesInput() { SkipCount = 5 + (page.Value - 1) * pageSize, MaxResultCount = pageSize }
                    });

                dto.Articles.AddRange(dtoPage.Articles);
            }
            else
            {
                dto = await _categoryFrontEndService.GetCategory(
                    new GetCategoryInput()
                    {
                        CategoryId = id,
                        SearchArticlesInput = new SearchArticlesInput() { MaxResultCount = 5 + pageSize }
                    });
            }

            //#region Encrypt Files's Url
            //foreach (var item in dto.Articles)
            //{
            //	item.PrimaryImageUrl = !string.IsNullOrWhiteSpace(item.PrimaryImageUrl) ? HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(item.PrimaryImageUrl)) : string.Empty;
            //}
            //#endregion

            #region Build Paging
            var TotalPages = (int)Math.Ceiling(decimal.Divide(dto.UsedCount, pageSize));
            ViewBag.TotalPages = TotalPages;
            ViewBag.CurrentPage = page.HasValue ? page : 1;
            #endregion

            return View("CategoryDetail", dto);
        }

        [Route("Article")]
        public async Task<IActionResult> ArticleDetail(long id)
        {
            var article = await _articleFrontEndService.GetArticleDetail(new EntityDto<long> { Id = id });

            ArticleViewModel model = new();
            model.Article = article;

            model.Category = await _categoryFrontEndService.GetCategory(
                    new GetCategoryInput()
                    {
                        CategoryId = article.Categories.First().Id,
                        SearchArticlesInput = new SearchArticlesInput() { MaxResultCount = 25 }
                    });

            return View("Detail", model);
        }

        //introduce
        //[AbpAuthorize]
        public async Task<IActionResult> CreateIntroduce(long Id)
        {
            ViewBag.ArticleId = Id;
            var usercurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            IntroduceEditDto model = new IntroduceEditDto();
            if (usercurrent.User != null)
            {
                if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                {
                    var recrui = await _perRequestSessionCache.GetRecruiter();
                    model.Email = recrui.Account.EmailAddress;
                    model.Name = recrui.Account.Name;
                }
                if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                {
                    var Candidate = await _perRequestSessionCache.GetCandidate();
                    model.Email = Candidate.Account.EmailAddress;
                    model.Name = Candidate.Account.Name;
                }
            }
            return PartialView("CreateIntroduce", model);
        }

        public async Task<int> GetCountByUserId()
        {
            int count = await _introduceAppService.GetCountByUserId();
            return count;
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Create(IntroduceEditDto model)
        {
            try
            {
                await _introduceAppService.Create(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}
