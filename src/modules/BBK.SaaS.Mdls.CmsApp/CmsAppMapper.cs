using AutoMapper;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mdls.Cms.Categories.MDto;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Medias.Dto;
using BBK.SaaS.Mdls.Cms.Topics.Dto;
using BBK.SaaS.Mdls.Cms.UrlRecords.Dto;
using BBK.SaaS.Mdls.Cms.Widgets.Dto;

namespace BBK.SaaS.Mdls.Cms
{
	public class CmsAppMapper
	{
		public static void CreateMappings(IMapperConfigurationExpression configuration)
		{
			// Topics
			configuration.CreateMap<Topic, TopicEditDto>().ReverseMap();
			configuration.CreateMap<Topic, TopicListDto>();

			// Articles
			configuration.CreateMap<CreateArticleInput, Article>();
			configuration.CreateMap<Article, ArticleEditDto>().ReverseMap();
			configuration.CreateMap<Article, ArticleViewDto>().ForMember(x => x.Modified,
				opt => opt.MapFrom(src => src.LastModificationTime.HasValue ? src.LastModificationTime : src.CreationTime)
			).ReverseMap();
			configuration.CreateMap<Article, ArticleListDto>();
			configuration.CreateMap<ArticleCacheItem, ArticleViewDto>();


			// Categories
			configuration.CreateMap<CmsCat, CmsCatDto>();
			configuration.CreateMap<CmsCatEditDto, CmsCat>();

			// Categories for Front-end
            configuration.CreateMap<CmsCat, ContentCategoryDto>();

			// Categories for Front-end
            configuration.CreateMap<CmsCat, ContentCategoryDto>();

			// UrlRecords
			configuration.CreateMap<UrlRecord, UrlRecordListViewDto>();

			// Widgets
			configuration.CreateMap<CreateWidgetInput, Widget>().ReverseMap();
			configuration.CreateMap<Widget, WidgetEditDto>().ReverseMap();
			configuration.CreateMap<Widget, WidgetListDto>();
			configuration.CreateMap<Widget, WidgetSelectDto>();

			// Medias Management
			configuration.CreateMap<MediaFolder, MediaFolderDto>();
			configuration.CreateMap<Media, MediaDto>().ReverseMap();

		}

	}
}
