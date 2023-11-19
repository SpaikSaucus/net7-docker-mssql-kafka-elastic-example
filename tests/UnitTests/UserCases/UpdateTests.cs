using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserPermission.Application.UserCases.Update.Commands;
using UserPermission.Domain.Core;
using UserPermission.Domain.Exceptions;
using UserPermission.Domain.Permission.Models;
using Xunit;

namespace UserPermission.UnitTests.UserCases
{
	public class UpdateTests
	{
		private const int PermissionId = 1;
		private const string PermissionForename = "forename";
		private const string PermissionSurname = "surname";
		private const int TypeId = 1;
		private const string TypeDescription = "description";

		[Fact]
		public async Task UpdateCommand_PermissionNotExists_ReturnError()
		{
			//Arrange
			var exceptionMsg = string.Format("Permission {0} not found", PermissionId);
			var cmd = CreateCommand();
			var stubUnitOfWork = A.Fake<IUnitOfWork>();
			var stubLogger = A.Fake<ILogger<ModifyPermissionCommandHandler>>();
			var stubMediator = A.Fake<IMediator>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubMediator.Send(A<IRequest<Permission>>._, default)).Returns(Task.FromResult<Permission>(null));

			var handler = new ModifyPermissionCommandHandler(stubUnitOfWork, stubLogger, stubMediator, stubEls);

			//Act
			var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(cmd, CancellationToken.None));

			//Assert
			Assert.Equal(exceptionMsg, exception.Message);
		}

		[Fact]
		public async Task UpdateCommand_PermissionExists_ReturnPermissionOk()
		{
			//Arrange
			var cmd = CreateCommand();
			var permission = CreatePermission();
			var stubUnitOfWork = A.Fake<IUnitOfWork>();
			var stubRepository = A.Fake<IRepository<Permission>>();
			var stubLogger = A.Fake<ILogger<ModifyPermissionCommandHandler>>();
			var stubMediator = A.Fake<IMediator>();
			var stubEls = A.Fake<IElasticsearchCRUD<Permission>>();

			A.CallTo(() => stubMediator.Send(A<IRequest<Permission>>._, default)).Returns(permission);
			A.CallTo(() => stubUnitOfWork.Repository<Permission>()).Returns(stubRepository);

			var handler = new ModifyPermissionCommandHandler(stubUnitOfWork, stubLogger, stubMediator, stubEls);

			//Act
			var result = await handler.Handle(cmd, CancellationToken.None);

			//Assert
			Assert.Equal(permission.Result, result);
		}

		private static ModifyPermissionCommand CreateCommand()
		{
			return new ModifyPermissionCommand()
			{
				Id = PermissionId,
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
