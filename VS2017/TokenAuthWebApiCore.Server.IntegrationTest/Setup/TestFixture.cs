using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TokenAuthWebApiCore.Server.IntegrationTest.Setup;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TokenAuthWebApiCore.Server.IntegrationTest.Setup
{
	public class TestFixture<TStartup> : IDisposable where TStartup : class
	{
		private readonly TestServer _testServer;

		public TestFixture()
		{
			var webHostBuilder = new WebHostBuilder().UseStartup<TStartup>();
			_testServer = new TestServer(webHostBuilder);

			httpClient = _testServer.CreateClient();
			httpClient.BaseAddress = new Uri("http://localhost:58834");
		}

		public HttpClient httpClient { get; }

		public void Dispose()
		{
			httpClient.Dispose();
			_testServer.Dispose();
		}
	}
}
