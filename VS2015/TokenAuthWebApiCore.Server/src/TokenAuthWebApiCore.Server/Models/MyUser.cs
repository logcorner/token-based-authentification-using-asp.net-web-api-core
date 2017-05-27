using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace TokenAuthWebApiCore.Server.Models
{
	public class MyUser : IdentityUser
	{
		public DateTime JoinDate { get; set; }
		public DateTime JobTitle { get; set; }
		public string Contract { get; set; }
	}
}