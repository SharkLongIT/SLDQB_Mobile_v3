﻿using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.InforNLD
{
	[AutoMapFrom(typeof(CandidateEditDto))]
	public class CandidateModel : CandidateEditDto
	{
		public string NameCandidate { get; set; }	
	}
}
