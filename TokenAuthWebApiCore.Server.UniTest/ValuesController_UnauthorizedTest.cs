using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TokenAuthWebApiCore.Server.Controllers;
using Xunit;
using TokenAuthWebApiCore.Server.IntegrationTest.Extensions;

namespace TokenAuthWebApiCore.Server.UnitTest
{
	public class ValuesController_UnauthorizedTest
	{
		private ValuesController _controller;

		public ValuesController_UnauthorizedTest()
		{
			_controller = new ValuesController();
		}

		[Fact]
		public async Task WhenNoAuthenticatedUser_MakeRequestRequest_Return_UnAuthorized()
		{
			// Arrange
			_controller.MockCurrentUser("simpleuser@yopmail.com", "WebApiCore1#");
			// Act
			var result = await _controller.Get();

			// Assert
			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}
	}
}

