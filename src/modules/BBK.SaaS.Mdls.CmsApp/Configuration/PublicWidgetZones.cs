using System.Linq;
using System.Reflection;

namespace BBK.SaaS.Mdls.Cms.Configuration;

public static partial class PublicWidgetZones
{
	public static string AllPagesAfterMenu => "allpages_aftermenu";
	public static string AllPagesBeforeFooter => "allpages_beforefooter";
	public static string HomePageAfterMenu => "homepage_aftermenu";
	public static string HomePageAfterQuickFind => "homepage_afterquickfind";
	public static string HomePageAfterNewJobs => "homepage_afternewjobs";
	public static string HomePageAfterNewApps => "homepage_afternewapps";
    public static string HomePageAfterQuickSearch => "homepage_afterquicksearch";
    public static string HomePageBeforeFooter => "homepage_beforefooter";
	public static string CategoryArticlesTop => "category_articles_top";
	public static string CategoryArticlesMiddle => "category_articles_middle";
	public static string CategoryArticlesBottom => "category_articles_bottom";
	public static string CategoryArticlesRightTop => "category_articles_righttop";
	//public static string CategoryArticlesRightBottom => "category_articles_rightbottom";
	public static string ArticleDetailTop => "article_detail_top";
	public static string ArticleDetailMiddle => "article_detail_middle";
	public static string ArticleDetailBottom => "article_detail_bottom";
	public static string ArticleDetailRightTop => "article_detail_righttop";
    //public static string ArticleDetailRightBottom => "article_detail_rightbottom";


    public static string[] GetAllZoneNames()
	{
		return typeof(PublicWidgetZones)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(property => property.PropertyType == typeof(string))
			.Select(property => property.GetValue(null) is string value ? value : null).ToArray();
	}
}
