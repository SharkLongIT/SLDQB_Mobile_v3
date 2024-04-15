using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using BBK.SaaS.Authorization;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Topics.Dto;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Topics
{
	//[AbpAuthorize(AppPermissions.Topics_Administration_CommFuncs)]
	public class TopicAppService : SaaSAppServiceBase, ITopicAppService
	{
		private readonly IRepository<Topic, long> _topicRepository;
		private readonly IRepository<UrlRecord, long> _slugRepository;

		public TopicAppService(IRepository<Topic, long> topicRepository, IRepository<UrlRecord, long> slugRepository)
		{
			_topicRepository = topicRepository;
			_slugRepository = slugRepository;
		}

		#region Admins API
		public async Task<PagedResultDto<TopicListDto>> GetTopicsAsync(GetTopicInput input)
		{

			var query = _topicRepository.GetAll().AsNoTracking()
				.WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Title.Contains(input.Filter) || t.Slug.Contains(input.Filter))
				.WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
				.WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);

			var count = await query.CountAsync();
			var topics = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			return new PagedResultDto<TopicListDto>(
				count,
				ObjectMapper.Map<List<TopicListDto>>(topics)
				);
		}

		public async Task<TopicEditDto> CreateAsync(TopicEditDto input)
		{
			// Ensure slug
			if (string.IsNullOrWhiteSpace(input.Slug))
			{
				input.Slug = Utils.GenerateSlug(input.Title, false);
			}
			else
			{
				input.Slug = Utils.GenerateSlug(input.Slug, false);
			}

			Topic topic = ObjectMapper.Map<Topic>(input);
			topic.TenantId = AbpSession.TenantId.Value;

			// TODO: Put into a manager
			await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(article => article.Slug == input.Slug);
				if (foundSlug != null) { throw new UserFriendlyException("Search engine friendly page name is duplicated!!!"); }

				await _topicRepository.InsertAsync(topic);
				await UnitOfWorkManager.Current.SaveChangesAsync();
				await _slugRepository.InsertAsync(new UrlRecord() { TenantId = AbpSession.TenantId.Value, EntityId = topic.Id, EntityName = nameof(Topic), Slug = topic.Slug });
				await UnitOfWorkManager.Current.SaveChangesAsync();
			});

			return input;
		}

		public async Task<TopicEditDto> UpdateTopicAsync(TopicEditDto input)
		{
			// Ensure slug
			if (string.IsNullOrWhiteSpace(input.Slug))
			{
				input.Slug = Utils.GenerateSlug(input.Title, false);
			}
			else
			{
				input.Slug = Utils.GenerateSlug(input.Slug, false);
			}

			//Topic topic = ObjectMapper.Map<Topic>(input);
			//topic.TenantId = AbpSession.TenantId.Value;
			//await _topicRepository.InsertAsync(topic);

			// TODO: Put into a manager
			await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				var updatingItem = await _topicRepository.GetAsync(input.Id.Value);

				updatingItem.Title = input.Title;
				updatingItem.Body = input.Body;
				updatingItem.Published = input.Published;
				updatingItem.MetaTitle = input.MetaTitle;
				updatingItem.MetaDescription = input.MetaDescription;
				updatingItem.MetaKeywords = input.MetaKeywords;
				updatingItem.OgTitle = input.OgTitle;
				updatingItem.OgDescription = input.OgDescription;
				//updatingItem.OgImageUrl = input.OgImageUrl;


				var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(s => s.Slug == updatingItem.Slug && s.EntityId == input.Id && s.EntityName == nameof(Topic));
				if (foundSlug != null)
				{

					foundSlug.Slug = input.Slug;
					//throw new UserFriendlyException("Search engine friendly page name is duplicated!!!"); 				
				}
				else
				{
					_slugRepository.Insert(new UrlRecord() { TenantId = updatingItem.TenantId, EntityId = updatingItem.Id, EntityName = nameof(Topic), Slug = input.Slug });
				}
				updatingItem.Slug = input.Slug;

				await _topicRepository.UpdateAsync(updatingItem);
				//await _slugRepository.InsertAsync(new UrlRecord() { EntityId = topic.Id, EntityName = nameof(Topic), Slug = topic.Slug });
				await UnitOfWorkManager.Current.SaveChangesAsync();
			});

			return input;
		}

		[AbpAuthorize(AppPermissions.Pages_Administration_CommFuncs_Delete)]
		public async Task<bool> DeleteTopicAsync(EntityDto<long> input)
		{
			var entity = await _topicRepository.GetAsync(input.Id);
			await _topicRepository.DeleteAsync(entity);

			return true;
		}

		public async Task<TopicEditDto> GetTopicForEditAsync(long id)
		{
			var entity = await _topicRepository.GetAsync(id);

			var dto = ObjectMapper.Map<TopicEditDto>(entity);

			return dto;
		}
		#endregion

		#region FrontEnd API
		public async Task<TopicEditDto> GetTopicDetailAsync(long id)
		{
			var topic = await _topicRepository.GetAsync(id);

			var dto = ObjectMapper.Map<TopicEditDto>(topic);

			return dto;
		}


		#endregion

	}
}
