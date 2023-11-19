using System;
using System.Security.Permissions;

namespace UserPermission.Domain.Permission.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public int PermissionTypeId { get; set; }
        public PermissionType PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}
