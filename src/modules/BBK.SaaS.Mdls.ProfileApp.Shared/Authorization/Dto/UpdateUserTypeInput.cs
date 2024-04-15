using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities;
using BBK.SaaS.Authorization.Users;

namespace BBK.SaaS.Mdls.Profile.Authorization.Dto
{
	public class UpdateUserTypeInput
	{
		[Required]
		public UserTypeDto User { get; set; }

		public int UserTypeId { get; set; }

		public UpdateUserTypeInput()
		{

		}
	}

	public class UserTypeDto : IPassivable
	{
		/// <summary>
		/// Set null to create a new user. Set user's Id to update a user
		/// </summary>
		public long UserId { get; set; }

		[StringLength(UserConsts.MaxPhoneNumberLength)]
		public string PhoneNumber { get; set; }

		public string Name { get; set; }

		public bool IsActive { get; set; }

		public virtual UserTypeEnum UserType { get; set; }
	}
}
