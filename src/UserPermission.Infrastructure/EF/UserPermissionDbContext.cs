using Microsoft.EntityFrameworkCore;
using UserPermission.Domain.Permission.Models;

namespace UserPermission.Infrastructure.EF
{
    public class UserPermissionDbContext : DbContext
    {
        public UserPermissionDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().ToTable("Permission");

            base.OnModelCreating(modelBuilder);
        }
    }
}
