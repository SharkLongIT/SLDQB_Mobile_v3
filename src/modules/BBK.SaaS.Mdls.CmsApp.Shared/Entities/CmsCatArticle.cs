using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	[Table("AppCmsCatArticle", Schema = SaaSCmsConsts.DefaultSchema)]
	public class CmsCatArticle : CreationAuditedEntity<long>, IMustHaveTenant
	{
		//
		// Summary:
		//     TenantId of this entity.
		public virtual int TenantId { get; set; }

		//
		// Summary:
		//     Id of the Role.
		public virtual long ArticleId { get; set; }

		[ForeignKey(nameof(ArticleId))]
		public virtual Article Article { get; set; }

		//
		// Summary:
		//     Id of the Abp.Organizations.OrganizationUnit.
		public virtual long CmsCatId { get; set; }

		[ForeignKey(nameof(CmsCatId))]
		public virtual CmsCat CmsCat { get; set; }

		//
		// Summary:
		//     Specifies if the organization is soft deleted or not.
		public virtual bool IsDeleted { get; set; }

		//
		// Summary:
		//     Initializes a new instance of the Abp.Organizations.OrganizationUnitRole class.
		public CmsCatArticle()
		{
		}

		//
		// Summary:
		//     Initializes a new instance of the Abp.Organizations.OrganizationUnitRole class.
		//
		//
		// Parameters:
		//   tenantId:
		//     TenantId
		//
		//   roleId:
		//     Id of the User.
		//
		//   organizationUnitId:
		//     Id of the Abp.Organizations.OrganizationUnit.
		public CmsCatArticle(int tenantId, long articleId, long cmsCatId)
		{
			TenantId = tenantId;
			ArticleId = articleId;
			CmsCatId = cmsCatId;
		}
	}
}
