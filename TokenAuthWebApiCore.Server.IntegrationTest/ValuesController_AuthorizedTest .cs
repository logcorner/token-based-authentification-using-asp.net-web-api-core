using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TokenAuthWebApiCore.Server.IntegrationTest.Setup;
using TokenAuthWebApiCore.Server.Models;
using Xunit;
using System;

namespace TokenAuthWebApiCore.Server.IntegrationTest
{
	public class ValuesController_AuthorizedTest  : IClassFixture<TestFixture<Startup>>
	{
		public ValuesController_AuthorizedTest(TestFixture<Startup> fixture)
		{
			Client = fixture.httpClient;
		}

		public HttpClient Client { get; }

		[Theory]
		[InlineData("POST","myRequestBody")]
		[InlineData("GET", "myRequestBody")]
		[InlineData(new object[] { "PUT", "myRequestBody", 1 })]
		[InlineData(new object[] { "DELETE", "myRequestBody", 1 })]
		public async Task WhenAuthenticatedUser_MakeRequestRequest_Return_Ok(string method,string obj =null, int? id =null)
		{
			// Arrange
			var jwToken = await getJwToken();
			string token =$"bearer {jwToken.token}";
						
			string stringData = JsonConvert.SerializeObject(obj);
			var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");

			var request = new HttpRequestMessage(new HttpMethod(method), $"/api/values/{id}");
			request.Content = contentData;
			request.Headers.Add("Authorization", token);
			// Act
			var response = await Client.SendAsync(request);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		private async Task<JwToken> getJwToken()
		{
			var loginViewModel = new LoginViewModel
			{
				Email = "simpleuser@yopmail.com",
				Password = "WebApiCore1#"
			};
			string loginViewModelData = JsonConvert.SerializeObject(loginViewModel);
			var contentData = new StringContent(loginViewModelData, Encoding.UTF8, "application/json");

			var response = await Client.PostAsync($"/api/auth/token", contentData);
			response.EnsureSuccessStatusCode();
			var jwToken = JsonConvert.DeserializeObject<JwToken>(
				await response.Content.ReadAsStringAsync());
			return jwToken;
		}
	}
}

