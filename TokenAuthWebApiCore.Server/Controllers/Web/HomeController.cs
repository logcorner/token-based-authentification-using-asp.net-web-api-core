using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokenAuthWebApiCore.Server.Controllers.Web
{
	[Authorize]
	public class HomeController : Controller
	{
		public HomeController()
		{
			
		}
		public IActionResult Index()
		{
			ViewBag.Title = "Welcome";
			return View();
		}
	}
}
