using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.UserCases.FindAll.Queries;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;
using Xunit;

namespace UserPermission.UnitTests.UserCases
{
	public class FindAllTests
    {
        const ushort LIMIT = 200;
        const ushort OFFSET = 0;

        [Fact]
        public async Task GetAllQuery_FilterEmpty_ReturnEmpty()
        {
            //Arrange
            var query = CreatePermissionGetAllQuery();
            var stubUnitOfWork = A.Fake<IUnitOfWork>();
            var stubRepository = A.Fake<IRepository<Permission>>();
            var stubLogger = A.Fake<ILogger<PermissionGetAllQueryHandler>>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);
            A.CallTo(() => stubRepository.Count(A<Expression<Func<Permission, bool>>>._)).Returns(0);
            A.CallTo(() => stubRepository.Find(A<ISpecification<Permission>>._)).Returns(new List<Permission>());

            var handler = new PermissionGetAllQueryHandler(stubUnitOfWork, stubLogger, stubEls);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(0, result.Total);
        }

        [Fact]
        public async Task GetAllQuery_FilterEmpty_ReturnOneResult()
        {
            //Arrange
            var query = CreatePermissionGetAllQuery();
            var permissions = new List<Permission> { new Permission() };

            var stubUnitOfWork = A.Fake<IUnitOfWork>();
            var stubRepository = A.Fake<IRepository<Permission>>();
            var stubLogger = A.Fake<ILogger<PermissionGetAllQueryHandler>>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);
            A.CallTo(() => stubRepository.Count(A<Expression<Func<Permission, bool>>>._)).Returns(permissions.Count);
            A.CallTo(() => stubRepository.Find(A<ISpecification<Permission>>._)).Returns(permissions);
			A.CallTo(() => stubEls.Read(A<int>._, A<int>._)).Returns(null);

			var handler = new PermissionGetAllQueryHandler(stubUnitOfWork, stubLogger, stubEls);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(1, result.Total);
        }

        private static PermissionGetAllQuery CreatePermissionGetAllQuery()
        {
            var query = new PermissionGetAllQuery()
            {
                Limit = LIMIT,
                Offset = OFFSET,
                Sort = string.Empty
            };
            return query;
        }
    }
}
