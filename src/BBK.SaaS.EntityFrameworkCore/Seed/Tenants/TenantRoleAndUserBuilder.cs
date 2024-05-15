using System.Linq;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BBK.SaaS.Authorization;
using BBK.SaaS.Authorization.Roles;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.EntityFrameworkCore;
using BBK.SaaS.Notifications;
using Abp.Localization;
using System.Collections.Generic;
using System;

namespace BBK.SaaS.Migrations.Seed.Tenants
{
	public class TenantRoleAndUserBuilder
	{
		private readonly SaaSDbContext _context;
		private readonly int _tenantId;

		public TenantRoleAndUserBuilder(SaaSDbContext context, int tenantId)
		{
			_context = context;
			_tenantId = tenantId;
		}

		public void Create()
		{
			CreateRolesAndUsers();
			CreateLanguages();
		}

		private void CreateRolesAndUsers()
		{
			//Admin role

			var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
			if (adminRole == null)
			{
				adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
				_context.SaveChanges();
			}

			//User role

			var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
			if (userRole == null)
			{
				_context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
				_context.SaveChanges();
			}

			//admin user

			var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
			if (adminUser == null)
			{
				adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
				adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
				adminUser.IsEmailConfirmed = true;
				adminUser.ShouldChangePasswordOnNextLogin = false;
				adminUser.IsActive = true;

				_context.Users.Add(adminUser);
				_context.SaveChanges();

				//Assign Admin role to admin user
				_context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
				_context.SaveChanges();

				//User account of admin user
				if (_tenantId == 1)
				{
					_context.UserAccounts.Add(new UserAccount
					{
						TenantId = _tenantId,
						UserId = adminUser.Id,
						UserName = AbpUserBase.AdminUserName,
						EmailAddress = adminUser.EmailAddress
					});
					_context.SaveChanges();
				}

				//Notification subscription
				_context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, adminUser.Id, AppNotificationNames.NewUserRegistered));
				_context.SaveChanges();
			}
		}

		#region Languages VEA data
		public static List<ApplicationLanguageText> InitialLanguageTexts => GetLanguagesDefault();
		private void CreateLanguages()
		{
			foreach (var langTextUnit in InitialLanguageTexts)
			{
				AddLanguageTextIfNotExists(langTextUnit);
			}
		}
		private static List<ApplicationLanguageText> GetLanguagesDefault()
		{
			return new List<ApplicationLanguageText>
			{
                /* EN Language*/
                //new() { Key = "OrgAndPayroll", Value = "Tổ chức bộ máy và biên chế", LanguageName = "en", Source = "netcore", CreationTime = DateTime.Now },

                /* VI Language*/
    //            new ApplicationLanguageText() { Key = "Identity.DuplicateUserName", Value = "Số điện thoại đã được sử dụng!", LanguageName = "vi", Source = "SaaS", CreationTime = DateTime.Now, TenantId = 1 },
				new() { Key = "FirstName", Value = "Tên đầy đủ", LanguageName = "vi", Source = "SaaS", CreationTime = DateTime.Now, TenantId = 1 },
				
				new() { Key = "Identity.DuplicateUserName", Value = "Số điện thoại đã được sử dụng!", LanguageName = "vi", Source = "AbpZero", CreationTime = DateTime.Now, TenantId = 1 },
				new() { Key = "Identity.DuplicateEmail", Value = "Email đã được sử dụng!", LanguageName = "vi", Source = "AbpZero", CreationTime = DateTime.Now, TenantId = 1 },
			};
		}
		private void AddLanguageTextIfNotExists(ApplicationLanguageText languageText)
		{
			if (_context.LanguageTexts.IgnoreQueryFilters().Any(l => l.TenantId == languageText.TenantId && l.LanguageName == languageText.LanguageName && l.Source == languageText.Source && l.Key == languageText.Key))
			{
				return;
			}

			_context.LanguageTexts.Add(languageText);
			_context.SaveChanges();
		}
		#endregion

	}
}

