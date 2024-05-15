/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Topics.Dto;
using BBK.SaaS.Mdls.Cms.UrlRecords;
using BBK.SaaS.Web.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace BBK.SaaS.Web.Http;

/// <summary>
/// The main application middleware.
/// </summary>
public class RoutingMiddleware : MiddlewareBase, ITransientDependency
{
	//private readonly RoutingOptions _options;
	private readonly ISlugAppService _urlRecordService;
	private readonly ISlugCache _slugCache;
	private readonly SlugManager SlugManager;

	/// <summary>
	/// Creates a new middleware instance.
	/// </summary>
	/// <param name="next">The next middleware in the pipeline</param>
	/// <param name="options">The current routing options</param>
	/// <param name="factory">The logger factory</param>
	public RoutingMiddleware(ISlugAppService urlRecordService, ISlugCache slugCache, SlugManager slugManager, ILoggerFactory factory = null) : base(factory)
	{
		_urlRecordService = urlRecordService;
		//_options = options.Value;
		_slugCache = slugCache;
		SlugManager = slugManager;
	}

	//public RoutingMiddleware(RequestDelegate next) : base(next)
	//{
	//    //_options = options.Value;
	//}

	/// <summary>
	/// Invokes the middleware.
	/// </summary>
	/// <param name="context">The current http context</param>
	/// <param name="api">The current api</param>
	/// <param name="service">The application service</param>
	/// <returns>An async task</returns>
	public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		//var appConfig = new Config(api);

		if (!IsHandled(context) && !IsMgmtRequest(context.Request.Path.Value.ToLower()))
		{
			var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
			var segments = !string.IsNullOrEmpty(url) ? url[1..].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
			int pos = 0;

			//_logger?.LogDebug($"Url: [{url}]");
			//_logger?.LogInformation($"Url: [{url}]");
			//Logger?.Debug($"Url: [{url}]");

			//var hostname = context.Request.Host.Host;
			//Logger?.Debug($"hostname: [{hostname}]");

			SlugCacheItem slugItem = null;

			if (segments.Length > pos)
			{
				// Scan for the most unique slug
				for (var n = segments.Length; n > pos; n--)
				{
					var slug = string.Join("/", segments.Subset(pos, n - pos));
					//var foundSlug = await _urlRecordService.GetSlug(null, slug);
					//slugItem = await _slugCache.GetOrNullAsync(slug, true);

					slugItem = await SlugManager.GetOrNullAsync(slug, true);

					if (slugItem != null)
					{
						pos = n;
						break;
					}
				}
			}
			//else
			//{
			//    page = await api.Pages.GetStartpageAsync<PageBase>(site.Id)
			//        .ConfigureAwait(false);
			//}


			//if (page != null)
			//{
			//    if (!page.IsPublished)
			//    {
			//        // If the page isn't published, and this isn't a request for a draft, skip the request
			//        if (!context.Request.Query.ContainsKey("draft") || context.Request.Query["draft"] != "true")
			//        {
			//            await _next.Invoke(context);
			//            return;
			//        }
			//    }

			//    pageType = App.PageTypes.GetById(page.TypeId);
			//    service.PageId = page.Id;

			//    // Only cache published pages
			//    if (page.IsPublished)
			//    {
			//        service.CurrentPage = page;
			//    }
			//}

			////
			//// 5: Get the current post
			////
			//PostBase post = null;

			//if (_options.UsePostRouting)
			//{
			//    if (page != null && pageType.IsArchive && segments.Length > pos)
			//    {
			//        post = await api.Posts.GetBySlugAsync<PostBase>(page.Id, segments[pos])
			//            .ConfigureAwait(false);

			//        if (post != null)
			//        {
			//            pos++;
			//        }
			//    }

			//    if (post != null)
			//    {
			//        App.PostTypes.GetById(post.TypeId);

			//        // Only cache published posts
			//        if (post.IsPublished)
			//        {
			//            service.CurrentPost = post;
			//        }
			//    }
			//}

			//_logger?.LogDebug($"Found Site: [{ site.Id }]");
			//if (page != null)
			//{
			//    _logger?.LogDebug($"Found Page: [{ page.Id }]");
			//}

			//if (post != null)
			//{
			//    _logger?.LogDebug($"Found Post: [{ post.Id }]");
			//}

			//
			// 6: Route request
			//
			var route = new StringBuilder();
			var query = new StringBuilder();

			//if (post != null)
			//{
			//    if (string.IsNullOrWhiteSpace(post.RedirectUrl))
			//    {
			//        //// Handle HTTP caching
			//        //if (HandleCache(context, site, post, appConfig.CacheExpiresPosts))
			//        //{
			//        //    // Client has latest version
			//        //    return;
			//        //}

			//        route.Append(post.Route ?? "/post");
			//        for (var n = pos; n < segments.Length; n++)
			//        {
			//            route.Append("/");
			//            route.Append(segments[n]);
			//        }

			//        query.Append("id=");
			//        query.Append(post.Id);
			//    }
			//    else
			//    {
			//        _logger?.LogDebug($"Setting redirect: [{ post.RedirectUrl }]");

			//        context.Response.Redirect(post.RedirectUrl, post.RedirectType == RedirectType.Permanent);
			//        return;
			//    }
			//} else 
			//if (page != null && _options.UsePageRouting)
			//{
			//    if (string.IsNullOrWhiteSpace(page.RedirectUrl))
			//    {
			//        route.Append(page.Route ?? (pageType.IsArchive ? "/archive" : "/page"));

			//        // Set the basic query
			//        query.Append("id=");
			//        query.Append(page.Id);

			//        if (!page.ParentId.HasValue && page.SortOrder == 0)
			//        {
			//            query.Append("&startpage=true");
			//        }

			//        if (!pageType.IsArchive || !_options.UseArchiveRouting)
			//        {
			//            //if (HandleCache(context, site, page, appConfig.CacheExpiresPages))
			//            //{
			//            //    // Client has latest version.
			//            //    return;
			//            //}

			//            // This is a regular page, append trailing segments
			//            for (var n = pos; n < segments.Length; n++)
			//            {
			//                route.Append("/");
			//                route.Append(segments[n]);
			//            }
			//        }
			//    }
			//}

			if (slugItem != null)
			{
				if (slugItem.EntityName == nameof(Topic))
				{
					route.Append("/Topic");
				}
				if (slugItem.EntityName == nameof(CmsCat))
				{
					route.Append("/CategoryArticle");
				}
				if (slugItem.EntityName == nameof(Article))
				{
					route.Append("/Article");
				}

				// Set the basic query
				query.Append("id=");
				query.Append($"{slugItem.EntityId}");
			}

			if (route.Length > 0)
			{
				var strRoute = route.ToString();
				var strQuery = query.ToString();

				//_logger?.Debug($"Setting Route: [{ strRoute }?{ strQuery }]");

				context.Request.Path = new PathString(strRoute);
				if (context.Request.QueryString.HasValue)
				{
					context.Request.QueryString =
						new QueryString(context.Request.QueryString.Value + "&" + strQuery);
				}
				else
				{
					context.Request.QueryString =
						new QueryString("?" + strQuery);
				}
			}
		}
		await next.Invoke(context);
	}

	///// <summary>
	///// Handles HTTP Caching Headers and checks if the client has the
	///// latest version in cache.
	///// </summary>
	///// <param name="context">The HTTP Cache</param>
	///// <param name="site">The current site</param>
	///// <param name="content">The current content</param>
	///// <param name="expires">How many minutes the cache should be valid</param>
	///// <returns>If the client has the latest version</returns>
	//public bool HandleCache(HttpContext context, Site site, RoutedContentBase content, int expires)
	//{
	//    var headers = context.Response.GetTypedHeaders();

	//    if (expires > 0 && content.Published.HasValue)
	//    {
	//        _logger?.LogDebug($"Setting HTTP Cache for [{ content.Slug }]");

	//        var lastModified = !site.ContentLastModified.HasValue || content.LastModified > site.ContentLastModified
	//            ? content.LastModified : site.ContentLastModified.Value;
	//        var etag = Utils.GenerateETag(content.Id.ToString(), lastModified);

	//        headers.CacheControl = new CacheControlHeaderValue
	//        {
	//            Public = true,
	//            MaxAge = TimeSpan.FromMinutes(expires),
	//        };
	//        headers.ETag = new EntityTagHeaderValue(etag);
	//        headers.LastModified = lastModified;

	//        if (HttpCaching.IsCached(context, etag, lastModified))
	//        {
	//            _logger?.LogInformation("Client has current version. Setting NotModified");
	//            context.Response.StatusCode = 304;

	//            return true;
	//        }
	//    }
	//    else
	//    {
	//        _logger?.LogDebug($"Setting HTTP NoCache for [{ content.Slug }]");

	//        headers.CacheControl = new CacheControlHeaderValue
	//        {
	//            NoCache = true
	//        };
	//    }
	//    return false;
	//}

	///// <summary>
	///// Gets the matching hostname.
	///// </summary>
	///// <param name="site">The site</param>
	///// <param name="hostname">The requested host</param>
	///// <returns>The hostname split into host and prefix</returns>
	//private string[] GetMatchingHost(Site site, string hostname)
	//{
	//    var result = new string[2];

	//    if (!string.IsNullOrEmpty(site.Hostnames))
	//    {
	//        foreach (var host in site.Hostnames.Split(","))
	//        {
	//            if (host.Trim().ToLower() == hostname)
	//            {
	//                var segments = host.Split("/", StringSplitOptions.RemoveEmptyEntries);

	//                result[0] = segments[0];
	//                result[1] = segments.Length > 1 ? segments[1] : null;

	//                break;
	//            }
	//        }
	//    }
	//    return result;
	//}
}
