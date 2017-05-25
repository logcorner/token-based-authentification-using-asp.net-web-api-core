using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenAuthWebApiCore.Server.Models;
using TokenAuthWebApiCore.Server.IntegrationTest.Setup;
using Xunit;
using System;

namespace TokenAuthWebApiCore.Server.IntegrationTest
{
	public class AuthController_GetTokenTest : IClassFixture<TestFixture<Startup>>
	{
		public HttpClient Client { get; }
		public AuthController_GetTokenTest(TestFixture<Startup> fixture)
		{
			Client = fixture.httpClient;
		}
  
		[Fact]
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

