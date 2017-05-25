using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TokenAuthWebApiCore.Server.Test.Setup;
using Xunit;
namespace TokenAuthWebApiCore.Server.Test
{
	public class AuthControllerTest : IClassFixture<TestFixture<Startup>>
	{
		public AuthControllerTest(TestFixture<Startup> fixture)
		{
			Client = fixture.httpClient;
		}

		public HttpClient Client { get; }

		[Theory]
		[InlineData("GET")]
		[InlineData("HEAD")]
		[InlineData("POST")]
		public async Task AllMethods_RemovesServerHeader(string method)
		{
			// Arrange
			var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/values");

			// Act
			var response = await Client.SendAsync(request);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var content = await response.Content.ReadAsStringAsync();

			Assert.Equal("Test response", content);
			Assert.False(response.Headers.Contains("Server"), "Should not contain server header");
		}
		[Theory]
		[InlineData("POST")]
		[InlineData("GET")]
		[InlineData(new object[] { "PUT", 1 })]
		[InlineData(new object[] { "DELETE",1 })]
		public async Task WhenNoAuthenticatedUser_MakeRequestRequest_Return_UnAuthorized(string method, int? id =null)
		{
			// Arrange
			var request = new HttpRequestMessage(new HttpMethod(method), $"/api/values/{id}");

			// Act
			var response = await Client.SendAsync(request);

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}
	}
}

