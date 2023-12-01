using Microsoft.EntityFrameworkCore;
using UserPermission.Domain.Permission.Models;
using UserPermission.Infrastructure.EF;

namespace IntegrationTests.Setup
{
	public class UserPermissionMockDbContext : UserPermissionDbContext
	{
		public UserPermissionMockDbContext(DbContextOptions options) : base(options)
		{
			this.Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Permission>().ToTable("Permission");
			modelBuilder.Entity<PermissionType>().ToTable("PermissionType");

			modelBuilder.Entity<PermissionType>().HasData(
				new PermissionType { Id = 1, Description = "Description1" },
				new PermissionType { Id = 2, Description = "Description2" },
				new PermissionType { Id = 3, Description = "Description3" }
			);

			foreach (var permission in PermissionsMock.Get)
			{
				modelBuilder.Entity<Permission>().HasData(
					new Permission
					{
						Id = permission.Id,
						EmployeeForename = permission.EmployeeForename,
						EmployeeSurname = permission.EmployeeSurname,
						PermissionDate = permission.PermissionDate,
						PermissionTypeId = permission.PermissionTypeId
					}
				);
			}

			base.OnModelCreating(modelBuilder);
		}
	}
}
