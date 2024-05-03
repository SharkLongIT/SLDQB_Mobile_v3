using BBK.SaaS.ApiClient;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.ProfileNTD;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Introduce;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Cms.Introduces;
using Abp.UI;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.LienHeHoiDap;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mdls.Profile.Contacts;

namespace BBK.SaaS.Mobile.MAUI.Pages.Menu
{
	public partial class Index : SaaSMainLayoutPageComponentBase
	{
		protected INavigationService navigationService { get; set; }
		protected IArticleService articleService { get; set; }
		private string _SearchText = "";
		private bool isError = false;
		private ItemsProviderResult<RecruitmentDto> recruitmentDto;
		private readonly RecruimentInput _filter = new RecruimentInput();
		protected IRecruitmentAppService iRecruitmentAppService { get; set; }
		private Virtualize<RecruitmentDto> RecruitmentContrainer { get; set; }
		protected IApplicationContext ApplicationContext { get; set; }
		protected IUserProfileService UserProfileService { get; set; }



		protected IApplicationRequestAppService applicationRequestAppService { get; set; }
		private ItemsProviderResult<ApplicationRequestEditDto> applicationRequestDto;
		private readonly ApplicationRequestSearch _filterRequest = new ApplicationRequestSearch();
		private Virtualize<ApplicationRequestEditDto> ApplicationRequestContainer { get; set; }



		protected IIntroduceAppService introduceAppService { get; set; }
		private ItemsProviderResult<IntroduceEditDto> introduceDto;
		private readonly IntroduceSearch _filterIntroduce = new IntroduceSearch();
		private Virtualize<IntroduceEditDto> IntroduceContainer { get; set; }


		protected IMakeAnAppointmentAppService makeAnAppointmentAppService;
		private Virtualize<MakeAnAppointmentDto> makeAnAppointmentContainer { get; set; }
		private readonly MakeAnAppointmentInput _filterMakeAnAppoint = new MakeAnAppointmentInput();
		private ItemsProviderResult<MakeAnAppointmentDto> makeAnAppointmentDto;


		protected IContactAppService contactAppService;
		private Virtualize<ContactDto> contactContainer { get; set; }
		private readonly ContactSearch _filterContact = new ContactSearch();
		private ItemsProviderResult<ContactDto> contactDto;

		private bool IsUserLoggedIn;
		private string _userImage;
		private long RecruitmentPostCount;
		private long ApplicationCount;
		private long IntroduceCount;
		private long MakeAnAppoint;
		private long ContactCount;

		public Index()
		{
			navigationService = DependencyResolver.Resolve<INavigationService>();
			iRecruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
			ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
			UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
			articleService = DependencyResolver.Resolve<IArticleService>();
			applicationRequestAppService = DependencyResolver.Resolve<IApplicationRequestAppService>();
			introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();
			makeAnAppointmentAppService = DependencyResolver.Resolve<IMakeAnAppointmentAppService>();
			contactAppService = DependencyResolver.Resolve<IContactAppService>();

		}
		protected override async Task OnInitializedAsync()
		{
			IsUserLoggedIn = navigationService.IsUserLoggedIn();
			await LoadRecruitmentPost(new ItemsProviderRequest());
			await LoadApplicationRequest(new ItemsProviderRequest());
			await LoadIntroduce(new ItemsProviderRequest());
			await LoadListMakeAnAppointment(new ItemsProviderRequest());
			await LoadContact(new ItemsProviderRequest());
		}
		private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitmentPost(ItemsProviderRequest request)
		{

			_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
			_filter.SkipCount = request.StartIndex;
			_filter.Take = Math.Clamp(request.Count, 1, 1000);
			_filter.Filtered = _SearchText;


			await UserDialogsService.Block();

			await WebRequestExecuter.Execute(
				async () => await iRecruitmentAppService.GetAll(_filter),
				async (result) =>
				{
					var recruitmentPost = result.Items.ToList();
					recruitmentDto = new ItemsProviderResult<RecruitmentDto>(recruitmentPost, recruitmentPost.Count);
					RecruitmentPostCount = recruitmentPost.Count;
					await UserDialogsService.UnBlock();
				}
			);
			return recruitmentDto;
		}


		private async ValueTask<ItemsProviderResult<ApplicationRequestEditDto>> LoadApplicationRequest(ItemsProviderRequest request)
		{

			_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
			_filter.SkipCount = request.StartIndex;
			await UserDialogsService.Block();
			await WebRequestExecuter.Execute(
			   async () => await applicationRequestAppService.GetAllByRecruiter(_filterRequest),
			   async (result) =>
			   {
				   var recruitmentPost = result.Items.ToList();
				   applicationRequestDto = new ItemsProviderResult<ApplicationRequestEditDto>(recruitmentPost, recruitmentPost.Count);
				   ApplicationCount = applicationRequestDto.TotalItemCount;
				   await UserDialogsService.UnBlock();
			   }
		   );

			return applicationRequestDto;
		}

		private async ValueTask<ItemsProviderResult<IntroduceEditDto>> LoadIntroduce(ItemsProviderRequest request)
		{
			_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
			_filter.SkipCount = request.StartIndex;
			await UserDialogsService.Block();
			await WebRequestExecuter.Execute(
				async () => await introduceAppService.GetAllByUserType(_filterIntroduce),
				async (result) =>
				{
					var jobFilter = ObjectMapper.Map<List<IntroduceEditDto>>(result.Items);
					
					introduceDto = new ItemsProviderResult<IntroduceEditDto>(jobFilter, jobFilter.Count);
					IntroduceCount = introduceDto.TotalItemCount;
					await UserDialogsService.UnBlock();
				}
			);
			return introduceDto;
		}

		private async ValueTask<ItemsProviderResult<MakeAnAppointmentDto>> LoadListMakeAnAppointment(ItemsProviderRequest request)
		{

			_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
			_filter.SkipCount = request.StartIndex;


			await UserDialogsService.Block();
			try
			{
				await WebRequestExecuter.Execute(
							  async () => await makeAnAppointmentAppService.GetAll(_filterMakeAnAppoint),
							  async (result) =>
							  {
								  var makeAnAppointment = result.Items.ToList();
								
								  makeAnAppointmentDto = new ItemsProviderResult<MakeAnAppointmentDto>(makeAnAppointment, makeAnAppointment.Count);
								  MakeAnAppoint = makeAnAppointment.Count;
								  await UserDialogsService.UnBlock();
							  }
						  );

				return makeAnAppointmentDto;
			}
			catch (Exception ex)
			{

				throw new UserFriendlyException(ex.Message);
			}

		}

		private async ValueTask<ItemsProviderResult<ContactDto>> LoadContact(ItemsProviderRequest request)
		{

			_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
			_filter.SkipCount = request.StartIndex;
			await UserDialogsService.Block();
			try
			{
				await WebRequestExecuter.Execute(
							  async () => await contactAppService.GetAllOfMe(_filterContact),
							  async (result) =>
							  {
								  //var makeAnAppointment = result.Items.ToList();
								  var contacts = ObjectMapper.Map<List<ContactDto>>(result.Items);
								
								  contactDto = new ItemsProviderResult<ContactDto>(contacts, contacts.Count);
								  ContactCount = contacts.Count;
								  await UserDialogsService.UnBlock();
							  }
						  );

				return contactDto;
			}
			catch (Exception ex)
			{

				throw new UserFriendlyException(ex.Message);
			}

		}
	}
}
