using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TokenAuthWebApiCore.Server.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TokenAuthWebApiCore.Server
{
	public class Startup
    {
		public IConfigurationRoot Configuration { get; }
		private IHostingEnvironment _env;
		public Startup(IHostingEnvironment env)
        {
			_env = env;

			var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton(Configuration);
			// Add framework services.
			services.AddMvc();
			services.AddDbContext<SecurityContext>(options =>
					options.UseSqlServer(Configuration.GetConnectionString("SecurityConnection"), sqlOptions => sqlOptions.MigrationsAssembly("TokenAuthWebApiCore.Server")));
			
			services.AddIdentity<MyUser, MyRole>(cfg =>
			{
				// if we are accessing the /api and an unauthorized request is made
				// do not redirect to the login page, but simply return "Unauthorized"
				cfg.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
				{
					OnRedirectToLogin = ctx =>
					{
						if (ctx.Request.Path.StartsWithSegments("/api"))
							ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

						return Task.FromResult(0);
					}
				};
			}).AddEntityFrameworkStores<SecurityContext>()
			.AddDefaultTokenProviders();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
			app.UseIdentity();

			app.UseJwtBearerAuthentication(new JwtBearerOptions()
			{
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				TokenValidationParameters = new TokenValidationParameters()
				{
					ValidIssuer = Configuration["JwtSecurityToken:Issuer"],
					ValidAudience = Configuration["JwtSecurityToken:Audience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityToken:Key"])),
					ValidateLifetime = true
				}
			});

			//app.UseMvc();
			app.UseMvc(routes =>
			{
				
			});
			
		}
    }
}
