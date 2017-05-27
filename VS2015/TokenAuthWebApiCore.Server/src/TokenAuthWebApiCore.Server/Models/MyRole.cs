using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TokenAuthWebApiCore.Server.Models
{
	public class MyRole : IdentityRole
	{
		public string Description { get; set; }
	}
}