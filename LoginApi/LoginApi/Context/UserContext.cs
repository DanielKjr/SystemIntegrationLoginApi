using LoginApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Context
{
	public class UserContext(IConfiguration configuration) : DbContext
	{
		public DbSet<User> Users { get; set; }
		private string _connectionString = configuration["ConnectionStrings:sqlite"]!;


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(_connectionString);
			base.OnConfiguring(optionsBuilder);
		}
	}
}
