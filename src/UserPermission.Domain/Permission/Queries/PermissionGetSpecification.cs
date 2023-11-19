using UserPermission.Domain.Core;

namespace UserPermission.Domain.Permission.Queries
{
    public class PermissionGetSpecification : BaseSpecification<Models.Permission>
    {
        public PermissionGetSpecification(Models.Permission permission)
        {
            base.AddInclude(x => x.PermissionType);

            if (permission.Id != 0)
            {
                base.SetCriteria(x => x.Id == permission.Id);
            }
            else 
            {
                base.SetCriteria(x => x.PermissionTypeId == permission.PermissionTypeId
                    && x.EmployeeForename == permission.EmployeeForename
                    && x.EmployeeSurname == permission.EmployeeSurname
                );
            }
        }
    }
}
