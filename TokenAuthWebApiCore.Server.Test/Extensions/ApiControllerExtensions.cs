using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace TokenAuthWebApiCore.Server.Test.Extensions
{
	public static class ApiControllerExtensions
	{
		public static void MockCurrentUser(this Controller controller, string userId, string username)
		{
			var identity = new GenericIdentity(username);
			identity.AddClaim(
				new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", username));
			identity.AddClaim(
				new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId));

			var principal = new GenericPrincipal(identity, null);
			Mock<Controller> mockcontroller =new Mock<Controller>();
			mockcontroller.Setup(c => c.User).Returns(principal);
			controller = mockcontroller.Object;
		}
	}
}
