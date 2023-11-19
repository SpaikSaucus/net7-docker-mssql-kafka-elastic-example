using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.UserCases.FindOne.Queries;
using UserPermission.Domain.Core;
using UserPermission.Domain.Exceptions;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.Application.UserCases.Create.Commands
{
    public class CreatePermissionCommand : IRequest<Permission>
    {
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public int PermissionTypeId { get; set; }
    }


    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, Permission>
    {
        private const string messageExistsPermission = "Permission already exists and was created {0}";
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CreatePermissionCommandHandler> logger;
        private readonly IMediator mediator;
        private readonly IElasticsearchCRUD<Permission> els;

        public CreatePermissionCommandHandler(IUnitOfWork unitOfWork, 
            ILogger<CreatePermissionCommandHandler> logger, 
            IMediator mediator,
            IElasticsearchCRUD<Permission> els)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mediator = mediator;
            this.els = els;
        }

        public async Task<Permission> Handle(CreatePermissionCommand cmd, CancellationToken cancellationToken)
        {
            var query = new PermissionGetQuery()
            {
                EmployeeForename = cmd.EmployeeForename,
                EmployeeSurname = cmd.EmployeeSurname,
                PermissionTypeId = cmd.PermissionTypeId
            };

            var permissionExists = await this.mediator.Send(query, cancellationToken);
            if (permissionExists != null)
            {
                this.logger.LogInformation(messageExistsPermission, permissionExists.PermissionDate);
                throw new DomainException(string.Format(messageExistsPermission, permissionExists.PermissionDate));
            }

            var permission = new Permission()
            {
                EmployeeForename = cmd.EmployeeForename,
                EmployeeSurname = cmd.EmployeeSurname,
                PermissionTypeId = cmd.PermissionTypeId,
                PermissionDate = DateTime.UtcNow
            };

            this.unitOfWork.Repository<Permission>().Add(permission);
            await this.unitOfWork.Complete();

            this.els.Create(permission);

            return permission;
        }
    }
}
