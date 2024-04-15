using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Categories.Dto
{
	public class GetCmsCatInput
	{
		public string Filter { get; set; }
		public string Code { get; set; }
		public long? ParentId { get; set; }
		//public string MaxDepth { get; set; }
	}
}
