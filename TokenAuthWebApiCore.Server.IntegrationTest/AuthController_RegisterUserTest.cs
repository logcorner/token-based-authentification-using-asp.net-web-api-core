using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenAuthWebApiCore.Server.Models;
using TokenAuthWebApiCore.Server.IntegrationTest.Setup;
using Xunit;
namespace TokenAuthWebApiCore.Server.IntegrationTest
{
	public class AuthController_RegisterUserTest : IClassFixture<TestFixture<Startup>>
	{
		public AuthController_RegisterUserTest(TestFixture<Startup> fixture)
		{
			Client = fixture.httpClient;
		}

		public HttpClient Client { get; }

		//[Theory]
		//[InlineData("GET")]
		//[InlineData("HEAD")]
		//[InlineData("POST")]
		//public async Task AllMethods_RemovesServerHeader(string method)
		//{
		//	// Arrange
		//	var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/values");

		//	// Act
		//	var response = await Client.SendAsync(request);

		//	// Assert
		//	Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//	var content = await response.Content.ReadAsStringAsync();

		//	Assert.Equal("Test response", content);
		//	Assert.False(response.Headers.Contains("Server"), "Should not contain server header");
		//}
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
	}
}

