namespace BBK.SaaS.Web.Areas.Lib.Models.Indexings
{
    public class CreateCategoryUnitModalViewModel
    {
        public long? ParentId { get; set; }
        
        public CreateCategoryUnitModalViewModel(long? parentId)
        {
            ParentId = parentId;
        }
    }
}