using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Authorization.Dto;
using BBK.SaaS.Mdls.Profile.Entities;

namespace BBK.SaaS.Mdls.Profile.Authorization
{
	public class UserTypeAppService : SaaSAppServiceBase, IUserTypeAppService//, IProfileUserTypeAppService
	{
		private readonly IRepository<Recruiter, long> _recruiterRepository;
		private readonly IRepository<Candidate, long> _candidateRepository;
		private readonly UserManager _userManager;

		public UserTypeAppService(IRepository<Recruiter, long> recruiterRepository
			, IRepository<Candidate, long> candidateRepository,
			UserManager userManager

			)
		{
			_recruiterRepository = recruiterRepository;
			_candidateRepository = candidateRepository;
			_userManager = userManager;
		}

		public async Task Register(int tenantId, UserEditDto input)
		{
			if (input.UserType == SaaS.Authorization.Users.UserTypeEnum.Type1)
			{
				var recruiter = new Recruiter();
				recruiter.TenantId = tenantId;
				recruiter.UserId = input.Id.Value;
				await _recruiterRepository.InsertAsync(recruiter);
				return;
			}

			if (input.UserType == SaaS.Authorization.Users.UserTypeEnum.Type2)
			{
				var candidate = new Candidate();
				candidate.UserId = input.Id.Value;
				candidate.TenantId = tenantId;
				//candidate.ProvinceId = 1;
				//candidate.DistrictId = 1;

				await _candidateRepository.InsertAsync(candidate);
			}

		}

		public virtual async Task Update(int tenantId, UserEditDto input)
		{
			//input.User.Id = AbpSession.UserId;

			Debug.Assert(AbpSession.UserId != null, "input.User.Id should be set.");

			var user = await UserManager.FindByIdAsync(AbpSession.UserId.Value.ToString());

			UserEditDto updateUserDto = new();
			ObjectMapper.Map(user, updateUserDto); //Passwords is not mapped (see mapping configuration)


			//Update user properties
			updateUserDto.Name = input.Name;
			updateUserDto.PhoneNumber = input.PhoneNumber;

			if (!string.IsNullOrEmpty(input.PhoneNumber))
			{
				updateUserDto.UserName = input.PhoneNumber;
			}
			else
			{
				updateUserDto.UserName = user.EmailAddress;
			}

			ObjectMapper.Map(updateUserDto, user); //Passwords is not mapped (see mapping configuration)


			CheckErrors(await UserManager.UpdateAsync(user));
		}
	}
}
