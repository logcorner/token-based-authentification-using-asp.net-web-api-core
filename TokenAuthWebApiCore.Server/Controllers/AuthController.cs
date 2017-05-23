using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TokenAuthWebApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Angular2TokenAuthWebApiCore.Models;

namespace Angular2TokenAuthWebApiCore.Controllers.Web
{
	[Route("api/auth")]
	public class AuthController : Controller
    {
		private readonly UserManager<MyUser> userManager;
		private readonly SignInManager<MyUser> signInManager;
		private readonly RoleManager<MyRole> roleManager;

		public AuthController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, RoleManager<MyRole> roleManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.roleManager = roleManager;
		}
		[AllowAnonymous]
		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var user = new MyUser()
			{
				UserName = model.Email,
				Email = model.Email
			};
			var result = await userManager.CreateAsync(user, model.Password);
			
			if (result.Succeeded)
				return Ok(result);

			foreach (var error in result.Errors)
				ModelState.AddModelError("error", error.Description);
			return BadRequest(result.Errors);
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[AllowAnonymous]
		[HttpPost("api/auth/Login")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				var result =
					await
						signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
							lockoutOnFailure: false);
				if (result.Succeeded)
					return RedirectToLocal(returnUrl);
				if (result.RequiresTwoFactor)
				{
					//
				}
				if (result.IsLockedOut)
				{
					return View("Lockout");
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> LogOff()
		{
			await signInManager.SignOutAsync();
			return View("LoggedOut");
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Conference");
		}
	}
}
