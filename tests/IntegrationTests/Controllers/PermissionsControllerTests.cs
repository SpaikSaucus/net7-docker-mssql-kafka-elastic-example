using IntegrationTests.Setup;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UserPermission.API.ViewModels;
using Xunit;

namespace IntegrationTests.Controllers
{
	public class PermissionsControllerTests : ScenarioBase
	{
		[Theory]
		[InlineData(0)]
		[InlineData(999999)]
		[InlineData(-1)]
		public async Task GetOne_InvalidValues_ReturnNotFound(int permissionId)
		{
			//Arrange
			using var server = this.CreateServer();

			//Act
			var response = await server.CreateClient().GetAsync(Get.Permissions + "/" + permissionId);

			//Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Theory]
		[InlineData(0, "", "")]
		[InlineData(0, "Forename", "")]
		[InlineData(0, "", "Surname")]
		[InlineData(1, "", "")]
		[InlineData(1, "Forename", "")]
		[InlineData(1, "", "Surname")]
		public async Task Create_IncompleteData_ReturnBadRequest(int typeId, string forename, string surname)
		{
			//Arrange
			var req = CreatePermissionRequest(typeId, forename, surname);
			using var server = this.CreateServer();

			//Act
			var response = await server.CreateClient().PostAsJsonAsync(Post.Permissions, req);

			//Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task Create_CompleteData_ReturnCreated()
		{
			//Arrange
			var req = CreatePermissionRequest(1, "Forename", "Surname");
			using var server = this.CreateServer();

			//Act
			var response = await server.CreateClient().PostAsJsonAsync(Post.Permissions, req);

			//Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		}

		[Fact]
		public async Task Create_ExistsPermission_ReturnBadRequest()
		{
			//Arrange
			var req = CreateExistPermissionRequest();
			using var server = this.CreateServer();

			//Act
			var response = await server.CreateClient().PostAsJsonAsync(Post.Permissions, req);

			//Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task Update_ExistsPermission_ReturnBadRequest()
		{
			//Arrange
			var req = CreateExistPermissionRequest();
			using var server = this.CreateServer();

			//Act
			var response = await server.CreateClient().PatchAsJsonAsync(Patch.Permissions + "/" + 2, req);

			//Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		private static CreatePermissionRequest CreateExistPermissionRequest()
		{
			var mock = PermissionsMock.Get.First();
			return CreatePermissionRequest(mock.PermissionTypeId, mock.EmployeeForename, mock.EmployeeSurname);
		}

		private static CreatePermissionRequest CreatePermissionRequest(int typeId, string forename, string surname)
		{
			return new CreatePermissionRequest()
			{
				PermissionTypeId = typeId,
				EmployeeForename = forename,
				EmployeeSurname = surname
			};
		}
	}
}
