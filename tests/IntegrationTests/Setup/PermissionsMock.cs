using System;
using System.Collections.Generic;
using UserPermission.Domain.Permission.Models;

namespace IntegrationTests.Setup
{
	public static class PermissionsMock
	{
        public static readonly List<Permission> Get = new() {
			new Permission(1, "EmployeeForename1", "EmployeeSurname1", 1, DateTime.Now),
			new Permission(2, "EmployeeForename2", "EmployeeSurname2", 1, DateTime.Now),
			new Permission(3, "EmployeeForename3", "EmployeeSurname3", 1, DateTime.Now)
		};
	}
}
