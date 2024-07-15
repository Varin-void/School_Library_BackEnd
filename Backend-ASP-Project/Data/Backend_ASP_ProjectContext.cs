using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend_ASP_Project.Models;
using System.Reflection.Metadata;

namespace Backend_ASP_Project.Data
{
	public class Backend_ASP_ProjectContext : DbContext
	{
		public Backend_ASP_ProjectContext(DbContextOptions<Backend_ASP_ProjectContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<User> Users { get; set; } = default!;
		public DbSet<Role> Roles { get; set; } = default!;
		public DbSet<Group> Groups { get; set; } = default!;
		public DbSet<Book> Books { get; set; } = default!;
		public DbSet<JwtToken> Tokens { get; set; } = default!;
		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	modelBuilder.Entity<Role>()
		//		.HasMany(e => e.Users)
		//		.WithOne(e => e.Role)
		//		.HasForeignKey(e => e.RoleId)
		//		.IsRequired(true);

		//}
	}
}
