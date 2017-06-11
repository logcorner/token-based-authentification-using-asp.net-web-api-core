using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenAuthWebApiCore.Server.IntegrationTest.Setup;
using TokenAuthWebApiCore.Server.Models;
using Xunit;

namespace TokenAuthWebApiCore.Server.IntegrationTest
{
	public class AuthController_RegisterUserTest : IClassFixture<TestFixture<TestStartup>>
	{
		public HttpClient Client { get; }

		public AuthController_RegisterUserTest(TestFixture<TestStartup> fixture)
		{
			Client = fixture.httpClient;
		}

		[Theory]
		[InlineData("", "", "")]
		[InlineData("", "WebApiCore1#", "WebApiCore1#")]
		[InlineData("", "", "WebApiCore1#")]
		[InlineData("", "WebApiCore1#", "")]
		[InlineData("simpleuser@yopmail.com", "WebApiChggore1#", "WebApiCore1#")]
		[InlineData("simpleuser", "WebApiCore1#", "WebApiCore1#")]
		public async Task WhenNoRegisteredUser_SignUp_WithModelError_Return_BadRequest(string email, string passWord, string ConfirmPassword)
		{
			// Arrange
			
			var obj = new RegisterViewModel
			{
				Email= email,
				Password =passWord,
				ConfirmPassword = ConfirmPassword
			};
			string stringData = JsonConvert.SerializeObject(obj);
			var contentData = new StringContent(stringData, Encoding.UTF8,"application/json");
			// Act
			var response = await Client.PostAsync($"/api/auth/register", contentData);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact]
		public async Task WhenNoRegisteredUser_SignUp_WithValidModelState_Return_OK()
		{
			// Arrange
			var obj = new RegisterViewModel
			{
				Email = "simpleuser@yopmail.com",
				Password = "WebApiCore1#",
				ConfirmPassword = "WebApiCore1#"
			};
			string stringData = JsonConvert.SerializeObject(obj);
			var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
			// Act
			var response = await Client.PostAsync($"/api/auth/register", contentData);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}
	}
}

