using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using AutoMapper;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;

namespace BBK.SaaS.Mdls.Profile
{
	public class SaaSProfileAppMapper
	{
		public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
			////Inputs
			//configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
			//configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
			//configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
			//configuration.CreateMap<IInputType, FeatureInputTypeDto>()
			//    .Include<CheckboxInputType, FeatureInputTypeDto>()
			//    .Include<SingleLineStringInputType, FeatureInputTypeDto>()
			//    .Include<ComboboxInputType, FeatureInputTypeDto>();
			//configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
			//configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
			//    .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
			//configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
			//configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
			//    .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

			/* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
			configuration.CreateMap<Recruiter, RecruiterEditDto>().ReverseMap();
			configuration.CreateMap<Contact, ContactDto>().ReverseMap();
			configuration.CreateMap<ApplicationRequest, ApplicationRequestEditDto>().ReverseMap();
			configuration.CreateMap<Recruitment, RecruitmentDto>().ReverseMap();
			configuration.CreateMap<Candidate, CandidateEditDto>().ReverseMap();
			configuration.CreateMap<JobApplication, JobApplicationEditDto>().ReverseMap();
			configuration.CreateMap<WorkExperience, WorkExperienceEditDto>().ReverseMap();
			configuration.CreateMap<LearningProcess, LearningProcessEditDto>().ReverseMap();
			configuration.CreateMap<MakeAnAppointment, MakeAnAppointmentDto>().ReverseMap();
		}
	}
}
