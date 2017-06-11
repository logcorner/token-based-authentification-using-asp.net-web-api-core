using System;

namespace TokenAuthWebApiCore.Server.IntegrationTest.Setup
{
	public class JwToken
    {
		public string token { get; set; }
		public DateTime expiration { get; set; }
	}
}
