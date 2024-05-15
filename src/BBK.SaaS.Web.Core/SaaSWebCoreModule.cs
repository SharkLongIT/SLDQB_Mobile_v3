using System;
using System.IO;
using System.Linq;
using System.Text;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.MultiTenancy;
using Abp.AspNetCore.SignalR;
//using BBK.SaaS.Licensing;
using BBK.SaaS.Web;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.IO;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Text;
using Abp.Timing;
using Abp.Web.MultiTenancy;
using Abp.Zero.Configuration;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BBK.SaaS.Authentication.TwoFactor;
using BBK.SaaS.Chat;
using BBK.SaaS.Configuration;
using BBK.SaaS.EntityFrameworkCore;
using BBK.SaaS.Web.Authentication.JwtBearer;
using BBK.SaaS.Web.Authentication.TwoFactor;
using BBK.SaaS.Web.Chat.SignalR;
using BBK.SaaS.Web.Common;
using BBK.SaaS.Web.Configuration;
using BBK.SaaS.Web.DashboardCustomization;
using Abp.Extensions;
using Abp.HtmlSanitizer;
using Abp.HtmlSanitizer.Configuration;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Mdls.Profile;
using BBK.SaaS.Mdls.Category;
using BBK.SaaS.Mdls.Cms;
using BBK.SaaS.Storage;

namespace BBK.SaaS.Web
{
	[DependsOn(
		typeof(SaaSProfileAppModule),
		typeof(SaaSCmsAppModule),
		typeof(SaaSCategoryAppModule),
		typeof(SaaSApplicationModule),
		typeof(SaaSEntityFrameworkCoreModule),
		typeof(AbpAspNetCoreModule),
		//typeof(AbpAspNetZeroCoreWebModule),
		typeof(AbpAspNetCoreSignalRModule),
		//typeof(SaaSGraphQLModule),
		typeof(AbpRedisCacheModule), //AbpRedisCacheModule dependency (and Abp.RedisCache nuget package) can be removed if not using Redis cache
		typeof(AbpAspNetCorePerRequestRedisCacheModule),
		typeof(AbpHangfireAspNetCoreModule), //AbpHangfireModule dependency (and Abp.Hangfire.AspNetCore nuget package) can be removed if not using Hangfire
		typeof(AbpHtmlSanitizerModule)
	)]
	public class SaaSWebCoreModule : AbpModule
	{
		private readonly IWebHostEnvironment _env;
		private readonly IConfigurationRoot _appConfiguration;
		//private readonly MediaTypeManager _mediaTypeManager;

		public SaaSWebCoreModule(IWebHostEnvironment env/*, MediaTypeManager mediaTypeManager*/)
		{
			_env = env;
			_appConfiguration = env.GetAppConfiguration();
			//_mediaTypeManager = mediaTypeManager;
		}

		public override void PreInitialize()
		{
			//Set default connection string
			Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
				SaaSConsts.ConnectionStringName
			);

			//Use database for language management
			Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

			Configuration.Modules.AbpAspNetCore()
				.CreateControllersForAppServices(
					typeof(SaaSApplicationModule).GetAssembly()
				);

			if (_appConfiguration["App:IsAdminWeb"] != null &&
				bool.Parse(_appConfiguration["App:IsAdminWeb"]))
			{
				Configuration.Modules.AbpAspNetCore()
					.CreateControllersForAppServices(
						typeof(SaaSCategoryAppModule).GetAssembly()
					);

				Configuration.Modules.AbpAspNetCore()
					.CreateControllersForAppServices(
						typeof(SaaSCmsAppModule).GetAssembly()/*, "cms"*/
					);

				Configuration.Modules.AbpAspNetCore()
					.CreateControllersForAppServices(
						typeof(SaaSProfileAppModule).GetAssembly()
					);
			}

			Configuration.Caching.Configure(TwoFactorCodeCacheItem.CacheName,
				cache => { cache.DefaultSlidingExpireTime = TwoFactorCodeCacheItem.DefaultSlidingExpireTime; });

			if (_appConfiguration["Authentication:JwtBearer:IsEnabled"] != null &&
				bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
			{
				ConfigureTokenAuth();
			}

			Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();

			Configuration.ReplaceService<IAppConfigurationWriter, AppConfigurationWriter>();

			if (WebConsts.HangfireDashboardEnabled)
			{
				Configuration.BackgroundJobs.UseHangfire();
			}

			//Uncomment this line to use Redis cache instead of in-memory cache.
			//See app.config for Redis configuration and connection string
			//Configuration.Caching.UseRedis(options =>
			//{
			//    options.ConnectionString = _appConfiguration["Abp:RedisCache:ConnectionString"];
			//    options.DatabaseId = _appConfiguration.GetValue<int>("Abp:RedisCache:DatabaseId");
			//});

			// HTML Sanitizer configuration
			Configuration.Modules.AbpHtmlSanitizer()
				.KeepChildNodes()
				.AddSelector<IAccountAppService>(x => nameof(x.IsTenantAvailable))
				.AddSelector<IAccountAppService>(x => nameof(x.Register));
		}

		private void ConfigureTokenAuth()
		{
			IocManager.Register<TokenAuthConfiguration>();
			var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

			tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"])
			);

			tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
			tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
			tokenAuthConfig.SigningCredentials =
				new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
			tokenAuthConfig.AccessTokenExpiration = AppConsts.AccessTokenExpiration;
			tokenAuthConfig.RefreshTokenExpiration = AppConsts.RefreshTokenExpiration;
		}

		public override void Initialize()
		{
			IocManager.RegisterAssemblyByConvention(typeof(SaaSWebCoreModule).GetAssembly());

			//IocManager.RegisterIfNot(MediaTypeManager);
		}

		public override void PostInitialize()
		{
			SetAppFolders();

			IocManager.Resolve<ApplicationPartManager>()
				.AddApplicationPartsIfNotAddedBefore(typeof(SaaSWebCoreModule).Assembly);

			var mediaTypeManager = IocManager.Resolve<MediaTypeManager>();

			// Setup media types
			//_mediaTypeManager.Documents.Add(".pdf", "application/pdf");
			//_mediaTypeManager.Documents.Add(".doc", "application/msword");
			//_mediaTypeManager.Documents.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
			//_mediaTypeManager.Images.Add(".jpg", "image/jpeg");
			//_mediaTypeManager.Images.Add(".jpeg", "image/jpeg");
			//_mediaTypeManager.Images.Add(".jfif", "image/jpeg");
			//_mediaTypeManager.Images.Add(".gif", "image/gif");
			//_mediaTypeManager.Images.Add(".png", "image/png");
			//_mediaTypeManager.Images.Add(".webp", "image/webp");
			//_mediaTypeManager.Videos.Add(".mp4", "video/mp4");
			//_mediaTypeManager.Audio.Add(".mp3", "audio/mpeg");
			//_mediaTypeManager.Audio.Add(".wav", "audio/wav");

			mediaTypeManager.Documents.Add(".pdf", "application/pdf");
			mediaTypeManager.Documents.Add(".doc", "application/msword");
			mediaTypeManager.Documents.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
			mediaTypeManager.Images.Add(".jpg", "image/jpeg");
			mediaTypeManager.Images.Add(".jpeg", "image/jpeg");
			mediaTypeManager.Images.Add(".jfif", "image/jpeg");
			mediaTypeManager.Images.Add(".gif", "image/gif");
			mediaTypeManager.Images.Add(".png", "image/png");
			mediaTypeManager.Images.Add(".webp", "image/webp");
			mediaTypeManager.Videos.Add(".mp4", "video/mp4");
			mediaTypeManager.Audio.Add(".mp3", "audio/mpeg");
			mediaTypeManager.Audio.Add(".wav", "audio/wav");
		}

		private void SetAppFolders()
		{
			var appFolders = IocManager.Resolve<AppFolders>();

			appFolders.SampleProfileImagesFolder = Path.Combine(_env.WebRootPath,
				$"Common{Path.DirectorySeparatorChar}Images{Path.DirectorySeparatorChar}SampleProfilePics");
			appFolders.WebLogsFolder = Path.Combine(_env.ContentRootPath, $"App_Data{Path.DirectorySeparatorChar}Logs");
		}
	}
}