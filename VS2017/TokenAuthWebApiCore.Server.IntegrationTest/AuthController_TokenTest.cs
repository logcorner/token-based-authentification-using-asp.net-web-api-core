using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenAuthWebApiCore.Server.IntegrationTest.Setup;
using Xunit;
using System;
using TokenAuthWebApiCore.Server.Models;

namespace TokenAuthWebApiCore.Server.IntegrationTest
{
	[TestCaseOrderer("TokenAuthWebApiCore.Server.IntegrationTest.Setup.PriorityOrderer", "TokenAuthWebApiCore.Server.IntegrationTest")]
	public class AuthController_TokenTest : IClassFixture<TestFixture<TestStartup>>
	{
		public HttpClient Client { get; }
		public AuthController_TokenTest(TestFixture<TestStartup> fixture)
		{
			Client = fixture.httpClient;
		}
		
		[Fact(DisplayName = "WhenNoRegisteredUser_SignUpForToken_WithValidModelState_Return_OK"), TestPriority(1)]
		public async Task WhenNoRegisteredUser_SignUpForToken_WithValidModelState_Return_OK()
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

		[Fact(DisplayName = "WhenRegisteredUser_SignIn_WithValidModelState_Return_ValidToken"), TestPriority(2)]
		public async Task WhenRegisteredUser_SignIn_WithValidModelState_Return_ValidToken()
		{
			// Arrange
			var obj = new LoginViewModel
			{
				Email = "simpleuser@yopmail.com",
				Password = "WebApiCore1#"
			};
			string stringData = JsonConvert.SerializeObject(obj);
			var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
			// Act
			var response = await Client.PostAsync($"/api/auth/token", contentData);
			response.EnsureSuccessStatusCode();
			// Assert
			
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			
			var jwToken = JsonConvert.DeserializeObject<JwToken>(
				await response.Content.ReadAsStringAsync());
			Assert.True(jwToken.expiration > DateTime.UtcNow);
			Assert.True(jwToken.token.Split('.').Length == 3);
		}
	}
}

