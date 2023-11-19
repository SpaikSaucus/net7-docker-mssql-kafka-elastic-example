using UserPermission.Domain.Permission.Models;

namespace UserPermission.API.ViewModels
{
    /// <summary>
    /// </summary>
    public class CreatePermissionResponse : PermissionResponse
    {
        public CreatePermissionResponse(Permission permission) : base(permission)
        {
        }
    }
}
