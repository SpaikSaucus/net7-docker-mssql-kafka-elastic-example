using UserPermission.Domain.Permission.Models;

namespace UserPermission.API.ViewModels
{
    /// <summary>
    /// </summary>
    public class ModifyPermissionResponse : PermissionResponse
    {
        public ModifyPermissionResponse(Permission permission) : base(permission)
        {
        }
    }
}
