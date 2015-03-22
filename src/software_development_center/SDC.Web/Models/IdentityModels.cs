using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;

namespace SDC.Web.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}

	[DbConfigurationType(typeof (MySqlEFConfiguration))]
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