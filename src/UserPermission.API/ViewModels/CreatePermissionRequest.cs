using System.ComponentModel.DataAnnotations;

namespace UserPermission.API.ViewModels
{
    /// <summary>
    /// </summary>
    public class CreatePermissionRequest
    {
        /// <summary>
        /// EmployeeForename of the permission.
        /// </summary>
        /// <example>Marty</example>
        [Required]
        public string EmployeeForename { get; set; }

        /// <summary>
        /// EmployeeSurname of the permission.
        /// </summary>
        /// <example>Mcfly</example>
        [Required]
        public string EmployeeSurname { get; set; }

        /// <summary>
        /// Id Type of the permission.
        /// </summary>
        /// <example>1</example>
        [Required]
        public int PermissionTypeId { get; set; }
    }
}
