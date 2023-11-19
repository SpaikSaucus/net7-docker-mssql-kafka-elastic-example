using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.Infrastructure.Services
{
    public class ElasticsearchCRUD : IElasticsearchCRUD<Permission>
    {
        private readonly IElasticClient elasticClient;
        private readonly ILogger logger;

        public ElasticsearchCRUD(IElasticClient elasticClient, ILogger<ElasticsearchCRUD> logger)
        {
            this.elasticClient = elasticClient;
            this.logger = logger;
        }

        public async void Create(Permission permission) 
        {
            var response = await this.elasticClient.IndexDocumentAsync(permission);
            if (!response.IsValid)
                this.logger.LogError("ELS: Error in create document: {0}", response.OriginalException);
        }

        public Permission Read(int permissionId)
        {
            var permission = new Permission() { Id = permissionId };
            return this.Read(permission);
        }

        public Permission Read(Permission permission)
        {
            GetResponse<Permission> response;

            if (permission.Id > 0)
            {
                response = this.elasticClient.Get<Permission>(permission.Id);
                if (!response.IsValid || !response.Found)
                {
                    if(!response.Found)
                        this.logger.LogWarning("ELS: Warning document not found: {0}", permission.Id);
                    else
                        this.logger.LogError("ELS: Error in read document: {0}", response.OriginalException);

                    return null;
                }

                return response.Source;
            }

            var searchDescriptor = new SearchDescriptor<Permission>();

            if (permission.PermissionTypeId > 0)
            {
                searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(match => match
                                .Field(x => x.PermissionTypeId)
                                .Query(permission.PermissionTypeId.ToString())
                            )
                        )
                    )
                );
            }

            if (!string.IsNullOrEmpty(permission.EmployeeForename))
            {
                searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(match => match
                                .Field(x => x.EmployeeForename)
                                .Query(permission.EmployeeForename)
                            )
                        )
                    )
                );
            }

            if (!string.IsNullOrEmpty(permission.EmployeeSurname))
            {
                searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(match => match
                                .Field(x => x.EmployeeSurname)
                                .Query(permission.EmployeeSurname)
                            )
                        )
                    )
                );
            }

            var searchResponse = this.elasticClient.Search<Permission>(searchDescriptor);
            if (!searchResponse.IsValid)
            {
                this.logger.LogError("ELS: Error in read document: {0}", searchResponse.OriginalException);
                return null;
            }

            return searchResponse.Documents.FirstOrDefault();
        }

        public Tuple<IEnumerable<Permission>, long> Read(int skip, int take)
        {
            var response = this.elasticClient.Search<Permission>(s => s
                .From((skip - 1) * take)
                .Size(take)
                .Query(q => q.MatchAll())
            );

            if (!response.IsValid)
            {
                this.logger.LogError("ELS: Error in read all documents by Search: {0}", response.OriginalException);
                return null;
            }

            var countResponse = this.elasticClient.Count<Permission>(s => s
                .Query(q => q.MatchAll())
            );

            if (!countResponse.IsValid)
            {
                this.logger.LogError("ELS: Error in read all documents by Count: {0}", countResponse.OriginalException);
                return null;
            }

            return new Tuple<IEnumerable<Permission>, long>(response.Documents.ToList(), countResponse.Count);
        }

        public async void Update(Permission permission)
        {
            var response = await this.elasticClient.UpdateAsync<Permission, object>(permission.Id, u => u
                .Doc(permission)
                .DocAsUpsert());

            if (!response.IsValid)
                this.logger.LogError("ELS: Error in update document: {0}", response.OriginalException);
        }

        public async void Delete(int permissionId)
        {
            var response = await this.elasticClient.DeleteAsync<Permission>(permissionId, d => d);
            if (!response.IsValid)
                this.logger.LogError("ELS: Error in delete document: {0}", response.OriginalException);
        }
    }
}
