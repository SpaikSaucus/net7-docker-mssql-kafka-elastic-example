using System;

namespace UserPermission.Domain.Permission.Models
{
    public class Permission
    {
        public Permission() { }

        public Permission(int id, string forename, string surname, int typeId, DateTime date) { 
            this.Id = id;
            this.EmployeeForename = forename;
            this.EmployeeSurname = surname;
            this.PermissionTypeId = typeId;
            this.PermissionDate = date;
        }

        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public int PermissionTypeId { get; set; }
        public PermissionType PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}
