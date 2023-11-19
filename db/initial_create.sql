USE [master]
GO

CREATE DATABASE user_permissions
GO

CREATE LOGIN srv_user_permissions WITH PASSWORD=N'Pass1234', DEFAULT_DATABASE=[user_permissions], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO

-----------------------------------------------------------------------
-----------------------------------------------------------------------


USE [user_permissions]
GO


--CREATE User
---------------
CREATE USER srv_user_permissions FOR LOGIN srv_user_permissions
GO
ALTER USER srv_user_permissions WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER srv_user_permissions
GO


--CREATE Tables
---------------
CREATE TABLE dbo.Permission (
	Id int IDENTITY(1,1)  NOT NULL,
	EmployeeForename nvarchar(255) NOT NULL,
	EmployeeSurname nvarchar(255) NOT NULL,
	PermissionTypeId int NOT NULL,
	PermissionDate date NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE dbo.PermissionType (
	Id int IDENTITY(1,1) NOT NULL,
	[Description] text NOT NULL,
 CONSTRAINT [PK_PermissionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- ADD Relations
----------------
ALTER TABLE dbo.Permission WITH CHECK ADD CONSTRAINT FK_Permission_PermissionType FOREIGN KEY(PermissionTypeId)
REFERENCES dbo.PermissionType (Id)
GO
ALTER TABLE dbo.Permission CHECK CONSTRAINT FK_Permission_PermissionType
GO


-- ADD Descriptions
-------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Permission', @level2type=N'COLUMN',@level2name=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Employee Forename' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Permission', @level2type=N'COLUMN',@level2name=N'EmployeeForename'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Employee Surname' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Permission', @level2type=N'COLUMN',@level2name=N'EmployeeSurname'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Permission Type Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Permission', @level2type=N'COLUMN',@level2name=N'PermissionTypeId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Permission granted on Date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Permission', @level2type=N'COLUMN',@level2name=N'PermissionDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PermissionType', @level2type=N'COLUMN',@level2name=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Permission Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PermissionType', @level2type=N'COLUMN',@level2name=N'Description'
GO
