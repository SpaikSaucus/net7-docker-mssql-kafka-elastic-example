using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.UserCases.FindOne.Queries;
using UserPermission.Domain.Core;
using UserPermission.Domain.Permission.Models;
using Xunit;

namespace UnitTests.UserCases
{
	public class FindOneTests
    {
		private const int PermissionId = 1;
		private const string PermissionForename = "forename";
		private const string PermissionSurname = "surname";
		private const int TypeId = 1;

		[Fact]
        public async Task GetOneQuery_IdNotExist_ReturnEmpty()
        {
            //Arrange
            var query = CreatePermissionGetQueryOnlyId();
            var stubUnitOfWork = A.Fake<IUnitOfWork>();
            var stubRepository = A.Fake<IRepository<Permission>>();
            var stubLogger = A.Fake<ILogger<PermissionGetQueryHandler>>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);
            A.CallTo(() => stubRepository.Find(A<ISpecification<Permission>>._)).Returns(new List<Permission>());
			A.CallTo(() => stubEls.Read(A<Permission>._)).Returns(null);

			var handler = new PermissionGetQueryHandler(stubUnitOfWork, stubLogger, stubEls);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOneQuery_IdExists_ReturnResult()
        {
            //Arrange
            var query = CreatePermissionGetQueryOnlyId();
            var permission = new List<Permission> { new Permission() };

            var stubUnitOfWork = A.Fake<IUnitOfWork>();
            var stubRepository = A.Fake<IRepository<Permission>>();
            var stubLogger = A.Fake<ILogger<PermissionGetQueryHandler>>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);
            A.CallTo(() => stubRepository.Find(A<ISpecification<Permission>>._)).Returns(permission);
			A.CallTo(() => stubEls.Read(A<Permission>._)).Returns(null);

			var handler = new PermissionGetQueryHandler(stubUnitOfWork, stubLogger, stubEls);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.NotNull(result);
        }

		[Fact]
		public async Task GetOneQuery_NotExistsPermission_ReturnEmpty()
		{
			//Arrange
			var query = CreatePermissionGetQueryNotById();
			var stubUnitOfWork = A.Fake<IUnitOfWork>();
			var stubRepository = A.Fake<IRepository<Permission>>();
			var stubLogger = A.Fake<ILogger<PermissionGetQueryHandler>>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);
			A.CallTo(() => stubRepository.Find(A<ISpecification<Permission>>._)).Returns(new List<Permission>());
			A.CallTo(() => stubEls.Read(A<Permission>._)).Returns(null);

			var handler = new PermissionGetQueryHandler(stubUnitOfWork, stubLogger, stubEls);

			//Act
			var result = await handler.Handle(query, CancellationToken.None);

			//Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task GetOneQuery_ExistsPermission_ReturnResult()
		{
			//Arrange
			var query = CreatePermissionGetQueryNotById();
			var permission = new List<Permission> { new Permission() };

			var stubUnitOfWork = A.Fake<IUnitOfWork>();
			var stubRepository = A.Fake<IRepository<Permission>>();
			var stubLogger = A.Fake<ILogger<PermissionGetQueryHandler>>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);
			A.CallTo(() => stubRepository.Find(A<ISpecification<Permission>>._)).Returns(permission);
			A.CallTo(() => stubEls.Read(A<Permission>._)).Returns(null);

			var handler = new PermissionGetQueryHandler(stubUnitOfWork, stubLogger, stubEls);

			//Act
			var result = await handler.Handle(query, CancellationToken.None);

			//Assert
			Assert.NotNull(result);
		}

		private static PermissionGetQuery CreatePermissionGetQueryOnlyId()
		{
			return new PermissionGetQuery() { Id = PermissionId };
		}

		private static PermissionGetQuery CreatePermissionGetQueryNotById()
        {
            return new PermissionGetQuery()
            {
                EmployeeForename = PermissionForename,
                EmployeeSurname = PermissionSurname,
                PermissionTypeId = TypeId
            };
        }
    }
}
