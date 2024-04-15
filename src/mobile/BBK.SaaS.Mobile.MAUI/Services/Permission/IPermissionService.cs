namespace BBK.SaaS.Services.Permission
{
    public interface IPermissionService
    {
        bool HasPermission(string key);
    }
}