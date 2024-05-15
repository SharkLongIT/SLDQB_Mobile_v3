using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BBK.SaaS.Authorization.Delegation;
using BBK.SaaS.Authorization.Roles;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Chat;
using BBK.SaaS.Editions;
using BBK.SaaS.Friendships;
using BBK.SaaS.MultiTenancy;
using BBK.SaaS.MultiTenancy.Accounting;
using BBK.SaaS.MultiTenancy.Payments;
using BBK.SaaS.Storage;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.EntityFrameworkCore
{
	public class SaaSDbContext : AbpZeroDbContext<Tenant, Role, User, SaaSDbContext>
	{
		/* Define an IDbSet for each entity of the application */

		#region Profile
		public virtual DbSet<Recruiter> Recruiters { get; set; }
		public virtual DbSet<Recruitment> Recruitments { get; set; }
		//public virtual DbSet<RecruitmentAddress> RecruitmentAddresses { get; set; }
		public virtual DbSet<Candidate> Candidates { get; set; }
		public virtual DbSet<JobApplication> JobApplications { get; set; }
		public virtual DbSet<WorkExperience> WorkExperiences { get; set; }
		public virtual DbSet<LearningProcess> LearningProcess { get; set; }
		public virtual DbSet<MakeAnAppointment> MakeAnAppointment { get; set; }
		public virtual DbSet<ApplicationRequest> ApplicationRequests { get; set; }
		public virtual DbSet<Contact> Contacts { get; set; }
		public virtual DbSet<TradingSession> TradingSessions { get; set; }
		public virtual DbSet<TradingSessionAccount> CountUserTradingSessions { get; set; }
		#endregion

		#region Cms
		public virtual DbSet<UrlRecord> UrlRecords { get; set; }
		public virtual DbSet<Topic> Pages { get; set; }
		public virtual DbSet<CmsCat> CmsCats { get; set; }
		public virtual DbSet<CmsCatArticle> CmsCatArticles { get; set; }
		public virtual DbSet<Article> Articles { get; set; }
		public virtual DbSet<Widget> Widgets { get; set; }
		//public virtual DbSet<WidgetZone> WidgetZones { get; set; }
		public virtual DbSet<WidgetMapping> WidgetMappings { get; set; }
		public virtual DbSet<MediaFolder> MediaFolders { get; set; }
		public virtual DbSet<Media> Medias { get; set; }

		//lien let gioi thieu
		public virtual DbSet<Introduce> Introduces { get; set; }

		#endregion

		#region System/Core
		public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

		public virtual DbSet<Friendship> Friendships { get; set; }

		public virtual DbSet<ChatMessage> ChatMessages { get; set; }

		public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

		public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

		public virtual DbSet<Invoice> Invoices { get; set; }

		public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

		public virtual DbSet<UserDelegation> UserDelegations { get; set; }

		public virtual DbSet<RecentPassword> RecentPasswords { get; set; }
		#endregion

		#region Extends Libs Service
		public virtual DbSet<GeoUnit> GeoUnits { get; set; }
		public virtual DbSet<CatUnit> CatUnits { get; set; }
		#endregion

		public SaaSDbContext(DbContextOptions<SaaSDbContext> options)
			: base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Profile.Index
			modelBuilder.Entity<Recruiter>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.UserId, e.Id });
				//b.HasIndex(e => new { e.Code, e.TenantId });
			});

			modelBuilder.Entity<Recruitment>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Id });
				//b.HasIndex(e => new { e.Code, e.TenantId });
			});

			//modelBuilder.Entity<RecruitmentAddress>(b =>
			//{
			//	b.HasIndex(e => new { e.TenantId, e.RecruitmentId, e.Id });
			//	//b.HasIndex(e => new { e.Code, e.TenantId });
			//});

			modelBuilder.Entity<TradingSession>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Id });
				//b.HasIndex(e => new { e.Code, e.TenantId });
			});

			modelBuilder.Entity<TradingSessionAccount>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.TradingSessionId, e.RecruiterId });
				b.HasIndex(e => new { e.TenantId, e.TradingSessionId, e.CandidateId });
				//b.HasIndex(e => new { e.Code, e.TenantId });
			});
			#endregion

			#region Cms.Index
			modelBuilder.Entity<UrlRecord>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Slug });
				b.HasIndex(e => new { e.Slug });
				//b.HasIndex(e => new { e.Code, e.TenantId });
			});

			modelBuilder.Entity<Widget>(b =>
			{
				b.HasIndex(e => new { e.TenantId });
			});

			modelBuilder.Entity<WidgetMapping>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.ZoneName, e.WidgetId });
			});

			modelBuilder.Entity<Media>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Id });
				b.HasIndex(e => new { e.TenantId, e.FolderId });
				b.HasIndex(e => new { e.TenantId, e.UnqueId });
			});

			modelBuilder.Entity<MediaFolder>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Code }).IsUnique(false);
				b.HasIndex(e => new { e.Code, e.TenantId });
				b.HasIndex(e => new { e.TenantId, e.UnqueId });
			});
			#endregion

			#region AbpCore.Index
			modelBuilder.Entity<BinaryObject>(b =>
			{
				b.HasIndex(e => new { e.TenantId });
			});

			modelBuilder.Entity<ChatMessage>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
				b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
				b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
				b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
			});

			modelBuilder.Entity<Friendship>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.UserId });
				b.HasIndex(e => new { e.TenantId, e.FriendUserId });
				b.HasIndex(e => new { e.FriendTenantId, e.UserId });
				b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
			});

			modelBuilder.Entity<Tenant>(b =>
			{
				b.HasIndex(e => new { e.SubscriptionEndDateUtc });
				b.HasIndex(e => new { e.CreationTime });
			});

			modelBuilder.Entity<SubscriptionPayment>(b =>
			{
				b.HasIndex(e => new { e.Status, e.CreationTime });
				b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
			});

			//modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
			//{
			//    b.HasQueryFilter(m => !m.IsDeleted)
			//        .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
			//        .IsUnique()
			//        .HasFilter("[IsDeleted] = 0");
			//});

			modelBuilder.Entity<UserDelegation>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.SourceUserId });
				b.HasIndex(e => new { e.TenantId, e.TargetUserId });
			});
			#endregion

			#region GeoUnit.IX_TenantId_Code

			modelBuilder.Entity<GeoUnit>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Code }).IsUnique(false);
				b.HasIndex(e => new { e.Code, e.TenantId });
			});

			modelBuilder.Entity<CatUnit>(b =>
			{
				b.HasIndex(e => new { e.TenantId, e.Code }).IsUnique(false);
				b.HasIndex(e => new { e.Code, e.TenantId });
			});

			#endregion
		}
	}
}
