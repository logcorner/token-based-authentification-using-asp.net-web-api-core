using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TokenAuthWebApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using TokenAuthWebApiCore.Server.Filters;

namespace TokenAuthWebApiCore.Server.Controllers.Web
{
	[Route("api/auth")]
	public class AuthController : Controller
    {
		private readonly UserManager<MyUser> _userManager;
		private readonly SignInManager<MyUser> _signInManager;
		private readonly RoleManager<MyRole> _roleManager;
		private IPasswordHasher<MyUser> _passwordHasher;
		private IConfigurationRoot _configurationRoot;
		private ILogger<AuthController> _logger;


		public AuthController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, RoleManager<MyRole> roleManager
			, IPasswordHasher<MyUser> passwordHasher, IConfigurationRoot configurationRoot, ILogger<AuthController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_logger = logger;
			_passwordHasher = passwordHasher;
			_configurationRoot = configurationRoot;
		}
		[AllowAnonymous]
		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var user = new MyUser()
			{
				UserName = model.Email,
				Email = model.Email
			};
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				return Ok(result);
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("error", error.Description);
			}
			return BadRequest(result.Errors);
		}

		[ValidateForm]
		[HttpPost("CreateToken")]
		[Route("token")]
		public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
		{
			try
			{
				var user = await _userManager.FindByNameAsync(model.Email);
				if(user == null)
				{
					return Unauthorized();
				}
					if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
					{
						var userClaims = await _userManager.GetClaimsAsync(user);

						var claims = new[]
						{
						  new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
						  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						  new Claim(JwtRegisteredClaimNames.Email, user.Email)
						}.Union(userClaims);

						var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationRoot["JwtSecurityToken:Key"]));
						var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

						var jwtSecurityToken = new JwtSecurityToken(
						  issuer: _configurationRoot["JwtSecurityToken:Issuer"],
						  audience: _configurationRoot["JwtSecurityToken:Audience"],
						  claims: claims,
						  expires: DateTime.UtcNow.AddMinutes(60),
						  signingCredentials: signingCredentials
						  );
						return Ok(new 
						{
							token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
							expiration = jwtSecurityToken.ValidTo
						});
					}
				return Unauthorized();
			}
			catch (Exception ex)
			{
				_logger.LogError($"error while creating token: {ex}");
				return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating token");
			}
		}
	}
}
