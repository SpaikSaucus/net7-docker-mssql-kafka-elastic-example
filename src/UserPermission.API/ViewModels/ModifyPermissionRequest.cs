namespace UserPermission.API.ViewModels
{
    /// <summary>
    /// </summary>
    public class ModifyPermissionRequest
    {
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
        /// <example>1</example>
        public int PermissionTypeId { get; set; }
    }
}
