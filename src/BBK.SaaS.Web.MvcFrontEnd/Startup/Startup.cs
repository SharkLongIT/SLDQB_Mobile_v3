using System;
using System.IO;
using Abp.AspNetCore;
using Abp.AspNetCore.SignalR.Hubs;
using BBK.SaaS.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Hangfire;
using Abp.PlugIns;
using Castle.Facilities.Logging;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BBK.SaaS.Authorization;
using BBK.SaaS.Configuration;
using BBK.SaaS.Configure;
using BBK.SaaS.EntityFrameworkCore;
using BBK.SaaS.Identity;
using BBK.SaaS.Schemas;
using BBK.SaaS.Web.Chat.SignalR;
using BBK.SaaS.Web.Common;
using BBK.SaaS.Web.Resources;
using Swashbuckle.AspNetCore.Swagger;
using BBK.SaaS.Web.Swagger;
using Stripe;
using System.Reflection;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.Mvc.Caching;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.HtmlSanitizer;
using HealthChecks.UI;
using HealthChecks.UI.Client;
using HealthChecksUISettings = HealthChecks.UI.Configuration.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using BBK.SaaS.Web.HealthCheck;
using Owl.reCAPTCHA;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BBK.SaaS.Web.Extensions;
using BBK.SaaS.Web.MultiTenancy;
using SecurityStampValidatorCallback = BBK.SaaS.Identity.SecurityStampValidatorCallback;
using BBK.SaaS.Web.Http;

namespace BBK.SaaS.Web.Startup
{
	public class Startup
	{
		private readonly IConfigurationRoot _appConfiguration;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public Startup(IWebHostEnvironment env)
		{
			_appConfiguration = env.GetAppConfiguration();
			_hostingEnvironment = env;

		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

			// MVC
			services.AddControllersWithViews(options =>
				{
					options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
					options.AddAbpHtmlSanitizer();
				})
#if DEBUG
				.AddRazorRuntimeCompilation()
#endif
				.AddNewtonsoftJson();

			if (bool.Parse(_appConfiguration["KestrelServer:IsEnabled"]))
			{
				ConfigureKestrel(services);
			}

			IdentityRegistrar.Register(services);

			services.Configure<SecurityStampValidatorOptions>(opts =>
			{
				opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
			});

			AuthConfigurer.Configure(services, _appConfiguration);

			if (WebConsts.SwaggerUiEnabled)
			{
				ConfigureSwagger(services);
			}

			//Recaptcha
			services.AddreCAPTCHAV3(x =>
			{
				x.SiteKey = _appConfiguration["Recaptcha:SiteKey"];
				x.SiteSecret = _appConfiguration["Recaptcha:SecretKey"];
			});

			//CMSUltility
			//services.AddResponseCaching();
			//services.AddControllers(options =>
			//{
			//    options.CacheProfiles.Add("Default30",
			//        new CacheProfile()
			//        {
			//            Duration = 30
			//        });
			//});

			if (WebConsts.HangfireDashboardEnabled)
			{
				// Hangfire (Enable to use Hangfire instead of default job manager)
				services.AddHangfire(config =>
				{
					config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
				});

				services.AddHangfireServer();
			}

			services.AddScoped<IWebResourceManager, WebResourceManager>();

			services.AddSignalR();

			if (WebConsts.GraphQL.Enabled)
			{
				services.AddAndConfigureGraphQL();
			}

			services.Configure<SecurityStampValidatorOptions>(options =>
			{
				options.ValidationInterval = TimeSpan.Zero;
			});

			if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
			{
				ConfigureHealthChecks(services);
			}

			services.Configure<RazorViewEngineOptions>(options =>
			{
				options.ViewLocationExpanders.Add(new RazorViewLocationExpander());
			});

			//services.AddScoped<SlugRouteTransformer>();

			//Configure Abp and Dependency Injection
			return services.AddAbp<SaaSWebMvcModule>(options =>
			{
				//Configure Log4Net logging
				options.IocManager.IocContainer.AddFacility<LoggingFacility>(
					f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
						? "log4net.config"
						: "log4net.Production.config")
				);

				options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"),
					SearchOption.AllDirectories);
			});

            

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseGetScriptsResponsePerUserCache();

			//Initializes ABP framework.
			app.UseAbp(options =>
			{
				options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSaaSForwardedHeaders();
			}
			else
			{
				app.UseStatusCodePagesWithRedirects("~/Error?statusCode={0}");
				app.UseExceptionHandler("/Error");
				app.UseSaaSForwardedHeaders();
			}

			//CMSUltility
			//app.UseResponseCaching();

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			if (SaaSConsts.PreventNotExistingTenantSubdomains)
			{
				app.UseMiddleware<DomainTenantCheckMiddleware>();
			}

			// CMSUltility Routing
			app
				//.UseSecurityMiddleware()
				.UseMiddleware<RoutingMiddleware>();

			app.UseRouting();

			app.UseAuthentication();

			if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
			{
				app.UseJwtTokenMiddleware();
			}

			app.UseAuthorization();

			using (var scope = app.ApplicationServices.CreateScope())
			{
				if (scope.ServiceProvider.GetService<DatabaseCheckHelper>()
					.Exist(_appConfiguration["ConnectionStrings:Default"]))
				{
					app.UseAbpRequestLocalization();
				}
			}

			if (WebConsts.HangfireDashboardEnabled)
			{
				//Hangfire dashboard & server (Enable to use Hangfire instead of default job manager)
				app.UseHangfireDashboard("/hangfire", new DashboardOptions
				{
					Authorization = new[]
						{new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard)}
				});
			}

			if (bool.Parse(_appConfiguration["Payment:Stripe:IsActive"]))
			{
				StripeConfiguration.ApiKey = _appConfiguration["Payment:Stripe:SecretKey"];
			}

			if (WebConsts.GraphQL.Enabled)
			{
				app.UseGraphQL<MainSchema>();
				if (WebConsts.GraphQL.PlaygroundEnabled)
				{
					app.UseGraphQLPlayground(
						new GraphQLPlaygroundOptions()); //to explorer API navigate https://*DOMAIN*/ui/playground
				}
			}

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<AbpCommonHub>("/signalr");
				endpoints.MapHub<ChatHub>("/signalr-chat");

				endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

				app.ApplicationServices.GetRequiredService<IAbpAspNetCoreConfiguration>().EndpointConfiguration
					.ConfigureAllEndpoints(endpoints);
			});

			if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
			{
				app.UseHealthChecks("/health", new HealthCheckOptions()
				{
					Predicate = _ => true,
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				});

				if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksUI:HealthChecksUIEnabled"]))
				{
					app.UseHealthChecksUI();
				}
			}

			if (WebConsts.SwaggerUiEnabled)
			{
				// Enable middleware to serve generated Swagger as a JSON endpoint
				app.UseSwagger();
				//Enable middleware to serve swagger - ui assets(HTML, JS, CSS etc.)
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint(_appConfiguration["App:SwaggerEndPoint"], "SaaS API V1");
					options.IndexStream = () => Assembly.GetExecutingAssembly()
						.GetManifestResourceStream("BBK.SaaS.Web.wwwroot.swagger.ui.index.html");
					options.InjectBaseUrl(_appConfiguration["App:WebSiteRootAddress"]);
				}); //URL: /swagger
			}
		}

		private void ConfigureKestrel(IServiceCollection services)
		{
			services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
			{
				options.Listen(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 443),
					listenOptions =>
					{
						var certPassword = _appConfiguration.GetValue<string>("Kestrel:Certificates:Default:Password");
						var certPath = _appConfiguration.GetValue<string>("Kestrel:Certificates:Default:Path");
						var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath,
							certPassword);
						listenOptions.UseHttps(new HttpsConnectionAdapterOptions()
						{
							ServerCertificate = cert
						});
					});
			});
		}

		private void ConfigureSwagger(IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo() { Title = "SaaS API", Version = "v1" });
				options.DocInclusionPredicate((docName, description) => true);
				options.ParameterFilter<SwaggerEnumParameterFilter>();
				options.SchemaFilter<SwaggerEnumSchemaFilter>();
				options.OperationFilter<SwaggerOperationIdFilter>();
				options.OperationFilter<SwaggerOperationFilter>();
				options.CustomDefaultSchemaIdSelector();

				// Add summaries to swagger
				var canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
				if (!canShowSummaries)
				{
					return;
				}

				var mvcXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var mvcXmlPath = Path.Combine(AppContext.BaseDirectory, mvcXmlFile);
				options.IncludeXmlComments(mvcXmlPath);

				var applicationXml = $"BBK.SaaS.Application.xml";
				var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
				options.IncludeXmlComments(applicationXmlPath);

				var webCoreXmlFile = $"BBK.SaaS.Web.Core.xml";
				var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
				options.IncludeXmlComments(webCoreXmlPath);
			}).AddSwaggerGenNewtonsoftSupport();
		}

		private void ConfigureHealthChecks(IServiceCollection services)
		{
			services.AddAbpZeroHealthCheck();

			var healthCheckUISection = _appConfiguration.GetSection("HealthChecks")?.GetSection("HealthChecksUI");

			if (bool.Parse(healthCheckUISection["HealthChecksUIEnabled"]))
			{
				services.Configure<HealthChecksUISettings>(settings =>
				{
					healthCheckUISection.Bind(settings, c => c.BindNonPublicProperties = true);
				});

				services.AddHealthChecksUI()
					.AddInMemoryStorage();
			}
		}
	}
}
