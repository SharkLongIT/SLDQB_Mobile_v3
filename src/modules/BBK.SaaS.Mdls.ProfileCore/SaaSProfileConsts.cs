using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile
{
	public class SaaSProfileConsts
	{
		public const string DefaultSchema = "BBKProfile";

	}

	public enum ServiceStatusEnum
	{
		None = 0, Approved = 1, Failed = 2,
	}

	public enum GenderEnum
	{
		Không = 0, Nam = 1, Nữ = 2,
	}

	public enum RecruitmentStatusEnum { 
		Draft = 0, Pending = 1, Approved = 2, Failed = 3,
	}
}
