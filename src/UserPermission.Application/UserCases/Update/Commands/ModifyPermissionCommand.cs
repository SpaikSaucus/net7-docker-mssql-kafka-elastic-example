using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.UserCases.FindOne.Queries;
using UserPermission.Domain.Core;
using UserPermission.Domain.Exceptions;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.Application.UserCases.Update.Commands
{
    public class ModifyPermissionCommand : IRequest<Permission>
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public int PermissionTypeId { get; set; }
    }

    public class ModifyPermissionCommandHandler : IRequestHandler<ModifyPermissionCommand, Permission>
    {
        private const string messageNotExistsPermission = "Permission {0} not found";
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ModifyPermissionCommandHandler> logger;
        private readonly IMediator mediator;
        private readonly IElasticsearchCRUD<Permission> els;

        public ModifyPermissionCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<ModifyPermissionCommandHandler> logger, 
            IMediator mediator,
            IElasticsearchCRUD<Permission> els)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mediator = mediator;
            this.els = els;
        }

        public async Task<Permission> Handle(ModifyPermissionCommand cmd, CancellationToken cancellationToken)
        {
            var permission = await this.mediator.Send(new PermissionGetQuery() { Id = cmd.Id }, cancellationToken);
            if (permission == null)
            {
                this.logger.LogInformation(messageNotExistsPermission, cmd.Id);
                throw new DomainException(string.Format(messageNotExistsPermission, cmd.Id));
            }

            if (!string.IsNullOrEmpty(cmd.EmployeeForename)) 
                permission.EmployeeForename = cmd.EmployeeForename;
            if (!string.IsNullOrEmpty(cmd.EmployeeSurname))
                permission.EmployeeSurname = cmd.EmployeeSurname;
            if (cmd.PermissionTypeId > 0)
                permission.PermissionTypeId = cmd.PermissionTypeId;

            this.unitOfWork.Repository<Permission>().Update(permission);
            await this.unitOfWork.Complete();

            this.els.Update(permission);

            return permission;
        }
    }
}
