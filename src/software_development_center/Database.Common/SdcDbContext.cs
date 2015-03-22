using System.Data.Entity;
using Database.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;

namespace Database.Common
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class SdcDbContext : IdentityDbContext<ApplicationUser>
	{
		public SdcDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public SdcDbContext(string nameOrConnectionString)
			: base(nameOrConnectionString, throwIfV1Schema: false)
		{
		}

		public DbSet<Comment> Comments { get; set; }
		public DbSet<Issue> Issues { get; set; }
		public DbSet<Project> Projects { get; set; }

		public static SdcDbContext Create()
		{
			var dbSettings = DbSettings.Deserialize();
			return new SdcDbContext(dbSettings.GetConnectionString());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Comment>()
				.HasRequired(x => x.Author)
				.WithMany(x => x.Comments)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Comment>()
				.HasRequired(x => x.Issue)
				.WithMany(x => x.Comments)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Issue>()
				.HasRequired(x => x.Author)
				.WithMany(x => x.Issues)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Issue>()
				.HasOptional(x => x.Performer)
				.WithMany(x => x.ExecutedIssues)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Issue>()
				.HasRequired(x => x.Project)
				.WithMany(x => x.Issues)
				.WillCascadeOnDelete(false);

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
