using System;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.API.ViewModels
{
    /// <summary>
    /// </summary>
    public class PermissionResponse
    {
        public PermissionResponse(Permission entity)
        {
            this.Id = entity.Id;
            this.EmployeeForename = entity.EmployeeForename;
            this.EmployeeSurname = entity.EmployeeSurname;
            this.PermissionTypeId = entity.PermissionTypeId;
            this.PermissionDate = entity.PermissionDate;
        }

        /// <summary>
        /// Id of the permission.
        /// </summary>
        /// <example>1111</example>
        public int Id { get; set; }

        /// <summary>
        /// EmployeeForename of the permission.
        /// </summary>
        /// <example>Marty</example>
        public string EmployeeForename { get; set; }

        /// <summary>
        /// EmployeeSurname of the permission.
        /// </summary>
        /// <example>Mcfly</example>
        public string EmployeeSurname { get; set; }

        /// <summary>
        /// Id Type of the permission.
        /// </summary>
        /// <example>Mcfly</example>
        public int PermissionTypeId { get; set; }

        /// <summary>
        /// Created At.
        /// </summary>
        /// <example>2023-07-11T10:15:00-03:00</example>
        public DateTime PermissionDate { get; set; }
    }
}
