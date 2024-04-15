using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Abp;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	[Table("AppCmsCats", Schema = SaaSCmsConsts.DefaultSchema)]

	public class CmsCat : CreationAuditedEntity<long>, IMustHaveTenant, IExtendableObject
	{
		/// <summary>
		/// Maximum length of the <see cref="DisplayName"/> property.
		/// </summary>
		public const int MaxDisplayNameLength = 128;

		/// <summary>
		/// Maximum depth of an UO hierarchy.
		/// </summary>
		public const int MaxDepth = 16;

		/// <summary>
		/// Length of a code unit between dots.
		/// </summary>
		public const int CodeUnitLength = 5;

		/// <summary>
		/// Maximum length of the <see cref="Code"/> property.
		/// </summary>
		public const int MaxCodeLength = MaxDepth * (CodeUnitLength + 1) - 1;

		/// <summary>
		/// TenantId of this entity.
		/// </summary>
		public virtual int TenantId { get; set; }

		public virtual Guid UnqueId { get; set; } = SequentialGuidGenerator.Instance.Create(SequentialGuidGenerator.SequentialGuidDatabaseType.PostgreSql);

		/// <summary>
		/// Parent <see cref="CatUnit"/>.
		/// Null, if this OU is root.
		/// </summary>
		[ForeignKey("ParentId")]
		public virtual CmsCat Parent { get; set; }

		/// <summary>
		/// Parent <see cref="CatUnit"/> Id.
		/// Null, if this OU is root.
		/// </summary>
		public virtual long? ParentId { get; set; }

		///// <summary>
		///// Hierarchical Code of this organization unit.
		///// Example: "00001.00042.00005".
		///// This is a unique code for a Tenant.
		///// It's changeable if OU hierarch is changed.
		///// </summary>
		//[Required]
		//[StringLength(MaxCodeLength)]
		//public virtual string KeyName { get; set; }

		/// <summary>
		/// Hierarchical Code of this organization unit.
		/// Example: "00001.00042.00005".
		/// This is a unique code for a Tenant.
		/// It's changeable if OU hierarch is changed.
		/// </summary>
		[Required]
		[StringLength(MaxCodeLength)]
		public virtual string Code { get; set; }

		/// <summary>
		/// Display name of this role.
		/// </summary>
		[Required]
		[StringLength(MaxDisplayNameLength)]
		public virtual string DisplayName { get; set; }

		[Required]
		[StringLength(SaaSConsts.MaxSingleLineLength)]
		public virtual string Slug { get; set; }

		public virtual long UsedCount { get; set; }

		public virtual long ViewedCount { get; set; }

		[MaxLength(SaaSConsts.MaxShortLineLength)]
		/// <summary>
		/// Gets or sets the meta keywords
		/// </summary>
		public string MetaKeywords { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets or sets the meta description
		/// </summary>
		public string MetaDescription { get; set; }

		[MaxLength(SaaSConsts.MaxShortLineLength)]
		/// <summary>
		/// Gets or sets the meta title
		/// </summary>
		public string MetaTitle { get; set; }

		/// <summary>
		/// Children of this OU.
		/// </summary>
		public virtual ICollection<CmsCat> Children { get; set; }

		//public virtual ICollection<Article> Articles { get; set; }

		public virtual string ExtensionData { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CatUnit"/> class.
		/// </summary>
		public CmsCat(int? tenantId)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CatUnit"/> class.
		/// </summary>
		/// <param name="tenantId">Tenant's Id or null for host.</param>
		/// <param name="displayName">Display name.</param>
		/// <param name="parentId">Parent's Id or null if OU is a root.</param>
		public CmsCat(int tenantId, string displayName, string slug, long? parentId = null)
		{
			TenantId = tenantId;
			DisplayName = displayName;
			Slug = slug;
			ParentId = parentId;
		}

		/// <summary>
		/// Creates code for given numbers.
		/// Example: if numbers are 4,2 then returns "00004.00002";
		/// </summary>
		/// <param name="numbers">Numbers</param>
		public static string CreateCode(params int[] numbers)
		{
			if (numbers.IsNullOrEmpty())
			{
				return null;
			}

			return numbers.Select(number => number.ToString(new string('0', CodeUnitLength))).JoinAsString(".");
		}

		/// <summary>
		/// Appends a child code to a parent code. 
		/// Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
		/// </summary>
		/// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
		/// <param name="childCode">Child code.</param>
		public static string AppendCode(string parentCode, string childCode)
		{
			if (childCode.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
			}

			if (parentCode.IsNullOrEmpty())
			{
				return childCode;
			}

			return parentCode + "." + childCode;
		}

		/// <summary>
		/// Gets relative code to the parent.
		/// Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="parentCode">The parent code.</param>
		public static string GetRelativeCode(string code, string parentCode)
		{
			if (code.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
			}

			if (parentCode.IsNullOrEmpty())
			{
				return code;
			}

			if (code.Length == parentCode.Length)
			{
				return null;
			}

			return code.Substring(parentCode.Length + 1);
		}

		/// <summary>
		/// Calculates next code for given code.
		/// Example: if code = "00019.00055.00001" returns "00019.00055.00002".
		/// </summary>
		/// <param name="code">The code.</param>
		public static string CalculateNextCode(string code)
		{
			if (code.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
			}

			var parentCode = GetParentCode(code);
			var lastUnitCode = GetLastUnitCode(code);

			return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
		}

		/// <summary>
		/// Gets the last unit code.
		/// Example: if code = "00019.00055.00001" returns "00001".
		/// </summary>
		/// <param name="code">The code.</param>
		public static string GetLastUnitCode(string code)
		{
			if (code.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
			}

			var splittedCode = code.Split('.');
			return splittedCode[splittedCode.Length - 1];
		}

		/// <summary>
		/// Gets parent code.
		/// Example: if code = "00019.00055.00001" returns "00019.00055".
		/// </summary>
		/// <param name="code">The code.</param>
		public static string GetParentCode(string code)
		{
			if (code.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
			}

			var splittedCode = code.Split('.');
			if (splittedCode.Length == 1)
			{
				return null;
			}

			return splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
		}
	}
}
