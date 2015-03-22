using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using MySql.Data.Entity;

namespace Database.Common.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<SdcDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			TargetDatabase = new DbConnectionInfo(DbSettings.Deserialize().GetConnectionString(), "MySql.Data.MySqlClient");
			SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
		}

		protected override void Seed(SdcDbContext context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}
	}
}