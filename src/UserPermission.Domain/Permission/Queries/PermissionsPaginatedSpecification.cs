using UserPermission.Domain.Core;

namespace UserPermission.Domain.Permission.Queries
{
    public class PermissionsPaginatedSpecification : BaseSpecification<Models.Permission>
    {
        public PermissionsPaginatedSpecification(int skip, int take)
        {
            base.AddInclude(x => x.PermissionType);
            base.ApplyPaging(skip, take);
        }
    }
}
