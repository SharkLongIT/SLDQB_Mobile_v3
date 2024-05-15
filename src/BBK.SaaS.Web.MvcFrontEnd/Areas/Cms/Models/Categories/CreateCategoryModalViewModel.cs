namespace BBK.SaaS.Web.Areas.Cms.Models.Categories
{
    public class CreateCategoryModalViewModel
    {
        public long? ParentId { get; set; }
        
        public CreateCategoryModalViewModel(long? parentId)
        {
            ParentId = parentId;
        }
    }
}