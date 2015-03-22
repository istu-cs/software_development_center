using System.Data.Entity;
using Database.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;

namespace Database.Common
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Fix asp.net identity 2.0 tables under MySQL

			// Explanations: primary keys can easily get too long for MySQL's
			// (InnoDB's) stupid 767 bytes limit.
			// With the two following lines we rewrite the generation to keep
			// those columns "short" enough
			modelBuilder.Entity<IdentityRole>()
				.Property(c => c.Name)
				.HasMaxLength(128)
				.IsRequired();

			// We have to declare the table name here, otherwise IdentityUser
			// will be created
			modelBuilder.Entity<ApplicationUser>()
				.ToTable("AspNetUsers")
				.Property(c => c.UserName)
				.HasMaxLength(128)
				.IsRequired();

			#endregion
		}
	}
}
