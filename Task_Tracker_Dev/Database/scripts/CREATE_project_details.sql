CREATE TABLE [dbo].[project_details](
	[details_syscode] int IDENTITY(1,1) NOT NULL PRIMARY KEY,	
	[env_syscode] int NULL,
	[project_syscode] int NULL,
	[parameter_syscode] int NOT NULL,
	[parameter_value] varchar(max),
	[created_by] int NOT NULL,
	[created_on] datetime  DEFAULT (GETDATE()) NOT NULL,
	[modified_by] int NULL,
	[modified_on] datetime NULL,
	[is_active] bit DEFAULT 1 NOT NULL,
	[is_deleted] bit DEFAULT 0 NOT NULL
)

ALTER TABLE [dbo].[project_details] ADD CONSTRAINT FK_PD_env_syscode FOREIGN KEY([env_syscode]) REFERENCES [dbo].[environment_master] ([env_syscode])
GO

ALTER TABLE [dbo].[project_details]  ADD CONSTRAINT FK_PD_proj_syscode  FOREIGN KEY([project_syscode]) REFERENCES [dbo].[project_master] ([project_syscode])
GO

ALTER TABLE [dbo].[project_details]  ADD CONSTRAINT FK_PD_release_syscode  FOREIGN KEY([parameter_syscode]) REFERENCES [dbo].[project_parameter_master] ([parameter_syscode])
GO
