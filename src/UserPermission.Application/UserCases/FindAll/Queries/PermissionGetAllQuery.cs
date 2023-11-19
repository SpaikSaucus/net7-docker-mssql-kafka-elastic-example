using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.Shared.DTOs;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;
using UserPermission.Domain.Permission.Queries;

namespace UserPermission.Application.UserCases.FindAll.Queries
{
    public class PermissionGetAllQuery : IRequest<PageDto<Permission>>
    {
        public ushort Offset { get; set; }
        public ushort Limit { get; set; }
        public string Sort { get; set; }
    }

    public class PermissionGetAllQueryHandler : IRequestHandler<PermissionGetAllQuery, PageDto<Permission>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<PermissionGetAllQueryHandler> logger;
        private readonly IElasticsearchCRUD<Permission> els;

        public PermissionGetAllQueryHandler(
            IUnitOfWork unitOfWork, 
            ILogger<PermissionGetAllQueryHandler> logger,
            IElasticsearchCRUD<Permission> els)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.els = els;
        }

        public Task<PageDto<Permission>> Handle(PermissionGetAllQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogDebug("call handle PermissionGetAllQueryHandler.");

            var result = new PageDto<Permission>();
            result.Limit = request.Limit;
            result.Offset = request.Offset;

            var response = this.els.Read(request.Offset, request.Limit);
            if (response != null)
            {
                result.Items = response.Item1;
                result.Total = response.Item2;
            }
            else
            {
                var spec = new PermissionsPaginatedSpecification(request.Offset, request.Limit);
                result.Items = this.unitOfWork.Repository<Permission>().Find(spec);
                result.Total = this.unitOfWork.Repository<Permission>().Count(spec.Criteria);
            }

            return Task.FromResult(result);
        }
    }
}
