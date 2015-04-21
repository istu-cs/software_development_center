using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Database.Entities
{
	// You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class User : IdentityUser
	{
		public User()
		{
			Teams = new Collection<Team>();
		}

		public virtual ICollection<Comment> Comments { get; set; }
		public virtual ICollection<Issue> Issues { get; set; }
		public virtual ICollection<Project> Projects { get; set; }
		public virtual ICollection<Team> Teams { get; set; }
		
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}
}