using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Medias.Dto;

namespace BBK.SaaS.Mdls.Cms.Medias
{
	public interface IMediasMgrAppService : IApplicationService
	{
		Task<ListResultDto<MediaFolderDto>> GetMediaFolders();

		Task<ListResultDto<MediaFolderDto>> GetChildrenMediaFolder(long id);

		Task<MediaFolderDto> CreateMediaFolder(CreateMediaFolderInput input);

		Task<MediaFolderDto> UpdateMediaFolder(UpdateMediaFolderInput input);

		Task<MediaFolderDto> MoveMediaFolder(MoveMediaFolderInput input);

		Task DeleteMediaFolder(EntityDto<long> input);

		//Task RemoveRoleFromMediaFolder(RoleToMediaFolderInput input);

		//Task AddRolesToMediaFolder(RolesToMediaFolderInput input);

		//Task<PagedResultDto<NameValueDto>> FindRoles(FindMediaFolderRolesInput input);

		Task<List<MediaFolderDto>> GetAll();

		Task SaveMediaAsync(MediaDto input);
	}
}
