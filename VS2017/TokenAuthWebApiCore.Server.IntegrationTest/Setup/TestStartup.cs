using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TokenAuthWebApiCore.Server.IntegrationTest.Setup
{
	public class TestStartup : Startup
	{
		public TestStartup(IHostingEnvironment env) : base(env)
		{
		}
		public override void SetUpDataBase(IServiceCollection services)
		{
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
			var connectionString = connectionStringBuilder.ToString();
			var connection = new SqliteConnection(connectionString);

			services
			  .AddEntityFrameworkSqlite()
			  .AddDbContext<SecurityContext>(
				options => options.UseSqlite(connection)
			  );
		}

		public override void EnsureDatabaseCreated(SecurityContext dbContext)
		{
			dbContext.Database.OpenConnection(); 
			dbContext.Database.EnsureCreated();
		}
		
	}
}
