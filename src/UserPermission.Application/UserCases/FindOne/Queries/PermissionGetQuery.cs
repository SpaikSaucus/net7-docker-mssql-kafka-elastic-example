using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;
using UserPermission.Domain.Permission.Queries;

namespace UserPermission.Application.UserCases.FindOne.Queries
{
    public class PermissionGetQuery : IRequest<Permission>
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public int PermissionTypeId { get; set; }
    }

    public class PermissionGetQueryHandler : IRequestHandler<PermissionGetQuery, Permission>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<PermissionGetQueryHandler> logger;
        private readonly IElasticsearchCRUD<Permission> els;

        public PermissionGetQueryHandler(
            IUnitOfWork unitOfWork, 
            ILogger<PermissionGetQueryHandler> logger,
            IElasticsearchCRUD<Permission> els)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.els = els;
        }

        public Task<Permission> Handle(PermissionGetQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogDebug("call handle PermissionGetQueryHandler.");

            var permission = new Permission() 
            {
                Id = request.Id,
                PermissionTypeId = request.PermissionTypeId,
                EmployeeForename = request.EmployeeForename,
                EmployeeSurname = request.EmployeeSurname
            };

            var response = this.els.Read(permission);
            if (response != null)
                return Task.FromResult(response);

            var spec = new PermissionGetSpecification(permission);

            return Task.FromResult(this.unitOfWork.Repository<Permission>().Find(spec).FirstOrDefault());
        }
    }
}