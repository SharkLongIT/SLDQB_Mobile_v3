/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

//using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Http;

/// <summary>
/// Base class for middleware.
/// </summary>
public abstract class MiddlewareBase: IMiddleware
{
    /// <summary>
    /// The next middleware in the pipeline.
    /// </summary>
    protected readonly RequestDelegate _next;

    /// <summary>
    /// The optional logger.
    /// </summary>
    protected ILogger _logger;

    /// <summary>
    /// Reference to the logger to write logs.
    /// </summary>
    //public ILogger Logger { protected get; set; }

    /// <summary>
    /// Creates a new middleware instance.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    public MiddlewareBase(RequestDelegate next)
    {
        //Logger = NullLogger.Instance;
        _next = next;
    }

    /// <summary>
    /// Creates a new middleware instance.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="factory">The logger factory</param>
    public MiddlewareBase(/*RequestDelegate next, */ILoggerFactory factory)
    {
        if (factory != null)
        {
            _logger = factory.CreateLogger(this.GetType().FullName);
        }
        //_logger = NullLogger.Instance;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <param name="api">The current api</param>
    /// <param name="service">The application service</param>
    /// <returns>An async task</returns>
    public abstract Task InvokeAsync(HttpContext context, RequestDelegate next);

    /// <summary>
    /// Checks if the request has already been handled by another
    /// Piranha middleware.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <returns>If the request has already been handled</returns>
    protected bool IsHandled(HttpContext context)
    {
        var values = context.Request.Query["piranha_handled"];
        if (values.Count > 0)
        {
            return values[0] == "true";
        }
        return false;
    }

    /// <summary>
    /// Checks if the request wants a draft.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <returns>If the request is for a draft</returns>
    protected bool IsDraft(HttpContext context)
    {
        var values = context.Request.Query["draft"];
        if (values.Count > 0)
        {
            return values[0] == "true";
        }
        return false;
    }

    /// <summary>
    /// Checks if this is a manager request.
    /// </summary>
    /// <param name="url">The url</param>
    /// <returns>If the given url is for the manager application</returns>
    protected bool IsMgmtRequest(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        if ((url.StartsWith("/api/") || url == "/api")
            || (url.StartsWith("/app/") || url == "/app")
            || (url.StartsWith("/cms/") || url == "/cms")
            || (url.StartsWith("/file/") || url == "/file")
            || (url.StartsWith("/tenantcustomization/") || url == "/tenantcustomization")
            || (url.StartsWith("/profile/") || url == "/profile")
            || (url.StartsWith("/view-resources/") || url == "/view-resources")
            || (url.StartsWith("/assets/") || url == "/assets")
            || (url.StartsWith("/abp"))
            )
        {
            return true;
        }
        return false;
    }
}

