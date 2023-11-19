using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserPermission.API.ViewModels;
using UserPermission.Application.UserCases.Create.Commands;
using UserPermission.Application.UserCases.FindAll.Queries;
using UserPermission.Application.UserCases.FindOne.Queries;
using UserPermission.Application.UserCases.Update.Commands;

namespace UserPermission.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PermissionsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Returns one Permission according to ID
        /// </summary>
        /// <param name="id" example="1">Identifier from Permission</param>
        /// <returns>Information result for one Permission</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/permissions/1
        /// </remarks>
        /// <response code="200">Request successful</response>
        /// <response code="401">The request is not validly authenticated</response>
        /// <response code="403">The client is not authorized for using this operation</response>
        /// <response code="404">The resource was not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PermissionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<IActionResult> GetOne(int id)
        {
            var permission = await this.mediator.Send(new PermissionGetQuery() { Id = id });
            if (permission == null) return this.NotFound();

            return this.Ok(new PermissionResponse(permission));
        }

        /// <summary>
        /// Returns paginated Permission results
        /// </summary>
        /// <param name="limit" example="200">Number of records per page.</param>
        /// <param name="offset" example="0">Results page you want to retrieve(0..N)</param>
        /// <param name="sort" example="id,desc">Sorting criteria in the format: property,(asc|desc). Default sort order is ascending. Multiple sort criteria are supported.</param>
        /// <returns>Information result for search permissions</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/permissions?sort=id,desc&amp;offset=0&amp;limit=200
        /// </remarks>
        /// <response code="200">Request successful</response>
        /// <response code="401">The request is not validly authenticated</response>
        /// <response code="403">The client is not authorized for using this operation</response>
        /// <response code="404">The resource was not found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PermissionPageResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<IActionResult> GetAll(string sort, ushort offset, ushort limit)
        {
            var query = new PermissionGetAllQuery()
            {
                Limit = limit,
                Offset = offset,
                Sort = sort
            };
            var pageDto = await this.mediator.Send(query);

            return this.Ok(new PermissionPageResponse(pageDto));
        }

        /// <summary>
        /// Create a Permission
        /// </summary>
        /// <returns>Information resulting from the creation permission</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/permissions
        ///     {
        ///         "PermissionTypeId": 1,
        ///         "EmployeeForename": "Marty",
        ///         "EmployeeSurname": "Mcfly"
        ///     }
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="401">The request is not validly authenticated</response>
        /// <response code="403">The client is not authorized for using this operation</response>
        /// <response code="404">The resource was not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePermissionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePermissionRequest req)
        {
            var response = await this.mediator.Send(new CreatePermissionCommand()
            {
                EmployeeForename = req.EmployeeForename,
                EmployeeSurname = req.EmployeeSurname,
                PermissionTypeId = req.PermissionTypeId
            });

            return this.Created($"/api/permissions/{response.Id}", new CreatePermissionResponse(response));
        }

        /// <summary>
        /// Modify a Permission
        /// </summary>
        /// <returns>Information from the permission</returns>
        /// <remarks>
        /// Sample request:
        ///     PATCH /api/permissions/1
        ///     {
        ///         "EmployeeForename": "Marty",
        ///         "EmployeeSurname": "Mcfly",
        ///         "PermissionTypeId": 1
        ///     }
        /// </remarks>
        /// <response code="200">Request successful</response>
        /// <response code="401">The request is not validly authenticated</response>
        /// <response code="403">The client is not authorized for using this operation</response>
        /// <response code="404">The resource was not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModifyPermissionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Modify(int id, [FromBody] ModifyPermissionRequest req)
        {
            var response = await this.mediator.Send(new ModifyPermissionCommand()
            {
                Id = id,
                EmployeeForename = req.EmployeeForename,
                EmployeeSurname = req.EmployeeSurname,
                PermissionTypeId = req.PermissionTypeId
            });
            return this.Ok(new ModifyPermissionResponse(response));
        }
    }
}
