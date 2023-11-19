using System.Collections.Generic;
using System.Linq;
using UserPermission.Application.Shared.DTOs;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.API.ViewModels
{
    /// <summary>
    /// </summary>
    public class PermissionPageResponse
    {
        public PermissionPageResponse(PageDto<Permission> dto) 
        {
            this.Permissions = dto.Items.Select(x => new PermissionResponse(x));
            this.Offset = dto.Offset;
            this.Total = dto.Total;
            this.Limit = dto.Limit;
        }

        /// <summary>
        /// Total result.
        /// </summary>
        /// <example>1</example>
        public long Total { get; set; }

        /// <summary>
        /// Page result (0..N).
        /// </summary>
        /// <example>0</example>
        public uint Offset { get; set; }

        /// <summary>
        /// Limit per page.
        /// </summary>
        /// <example>200</example>
        public ushort Limit { get; set; }

        /// <summary>
        /// List permissions.
        /// </summary>
        public IEnumerable<PermissionResponse> Permissions { get; set; }
    }
}
