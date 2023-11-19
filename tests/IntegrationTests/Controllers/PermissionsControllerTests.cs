using IntegrationTests.Setup;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UserPermission.API.ViewModels;
using Xunit;

namespace IntegrationTests.Controllers
{
	public class PermissionsControllerTests : ScenarioBase
	{
#warning: TODO Pending
		//[Theory]
		//[InlineData(0)]
		//[InlineData(999999)]
		//[InlineData(-1)]
		//public async Task GetOne_InvalidValues_ReturnNotFound(int permissionId)
		//{
		//	//Arrange
		//	using var server = this.CreateServer();

		//	//Act
		//	var response = await server.CreateClient().GetAsync(Get.Permissions + "/" + permissionId);

		//	//Assert
		//	Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		//}

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
			var req = CreateRequest(typeId, forename, surname);
			using var server = this.CreateServer();

			//Act
			var response = await server.CreateClient().PostAsJsonAsync(Post.Permissions, req);

			//Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

#warning: TODO Pending
		//[Theory]
		//[InlineData(1, "Forename", "Surname")]
		//public async Task Create_CompleteData_ReturnCreated(int typeId, string forename, string surname)
		//{
		//	//Arrange
		//	var req = CreateRequest(typeId, forename, surname);
		//	using var server = this.CreateServer();

		//	//Act
		//	var response = await server.CreateClient().PostAsJsonAsync(Post.Permissions, req);

		//	//Assert
		//	Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		//}

		private static CreatePermissionRequest CreateRequest(int typeId, string forename, string surname)
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
