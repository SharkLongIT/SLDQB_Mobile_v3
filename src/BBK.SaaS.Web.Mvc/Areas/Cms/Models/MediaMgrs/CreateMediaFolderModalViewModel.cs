namespace BBK.SaaS.Web.Areas.App.Models.MediaFolders
{
    public class CreateMediaFolderModalViewModel
    {
        public long? ParentId { get; set; }
        
        public CreateMediaFolderModalViewModel(long? parentId)
        {
            ParentId = parentId;
        }
    }
}