using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.UserCases.Create.Commands;
using UserPermission.Domain.Core;
using UserPermission.Domain.Exceptions;
using UserPermission.Domain.Permission.Models;
using Xunit;

namespace UnitTests.UserCases
{
	public class CreateTests
	{
		private const int PermissionId = 1;
		private const string PermissionForename = "forename";
		private const string PermissionSurname = "surname";
		private const int TypeId = 1;
		private const string TypeDescription = "description";

		[Fact]
		public async Task CreateCommand_PermissionExists_ReturnError()
		{
			//Arrange
			var exceptionMsg = "Permission already exists and was created";
			var cmd = CreateCommand();
			var stubUnitOfWork = A.Fake<IUnitOfWork>();
			var stubLogger = A.Fake<ILogger<CreatePermissionCommandHandler>>();
			var stubMediator = A.Fake<IMediator>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubMediator.Send(A<IRequest<Permission>>._, default)).Returns(CreatePermission());

			var handler = new CreatePermissionCommandHandler(stubUnitOfWork, stubLogger, stubMediator, stubEls);

			//Act
			var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(cmd, CancellationToken.None));

			//Assert
			Assert.Equal(exceptionMsg, exception.Message.Substring(0, exceptionMsg.Length));
		}

		[Fact]
		public async Task CreateCommand_PermissionNotExists_ReturnPermissionOk()
		{
			//Arrange
			var cmd = CreateCommand();
			var stubUnitOfWork = A.Fake<IUnitOfWork>();
			var stubRepository = A.Fake<IRepository<Permission>>();
			var stubLogger = A.Fake<ILogger<CreatePermissionCommandHandler>>();
			var stubMediator = A.Fake<IMediator>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubMediator.Send(A<IRequest<Permission>>._, default)).Returns(Task.FromResult<Permission>(null));
			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);

			var handler = new CreatePermissionCommandHandler(stubUnitOfWork, stubLogger, stubMediator, stubEls);

			//Act
			var result = await handler.Handle(cmd, CancellationToken.None);

			//Assert
			Assert.NotNull(result);
		}

		private static CreatePermissionCommand CreateCommand()
		{
			return new CreatePermissionCommand()
			{
				PermissionTypeId = TypeId,
				EmployeeForename = PermissionForename,
				EmployeeSurname = PermissionSurname
			};
		}

		private static Task<Permission>CreatePermission()
		{
			return Task.FromResult(new Permission()
			{
				Id = PermissionId,
				PermissionTypeId = TypeId,
				EmployeeForename = PermissionForename,
				EmployeeSurname = PermissionSurname,
				PermissionDate = DateTime.Now,
				PermissionType = new PermissionType()
				{
					Id = TypeId,
					Description = TypeDescription
				}
			});
		}
	}
}
