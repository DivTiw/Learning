CREATE TABLE [dbo].[module_level_user_mapping](
	[usrmap_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[details_syscode]  INT NOT NULL CONSTRAINT FK_LEVELDETAIL FOREIGN KEY REFERENCES [module_level_detail](details_syscode),
	[employee_syscode] INT NOT NULL,
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT GETDATE(),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL)
GO
