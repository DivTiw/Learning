CREATE TABLE [dbo].[release_instructions](
	[release_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,	
	[release_ref] varchar(100) NOT NULL UNIQUE,
	[env_syscode] [int] NOT NULL,
	[project_syscode] [int] NOT NULL,
	[task_syscode] [int] NOT NULL,	
	[remarks] VARCHAR(2000),
	[is_released] [bit] DEFAULT(0),
	[created_by] [int] NULL,
	[created_on] [datetime]  DEFAULT (GETDATE()) NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL
)

ALTER TABLE [dbo].[release_instructions] ADD CONSTRAINT FK_REL_env_syscode FOREIGN KEY([env_syscode]) REFERENCES [dbo].[environment_master] ([env_syscode])
GO

ALTER TABLE [dbo].[release_instructions]  ADD CONSTRAINT FK_REL_proj_syscode  FOREIGN KEY([project_syscode]) REFERENCES [dbo].[project_master] ([project_syscode])
GO

ALTER TABLE [dbo].[release_instructions]  ADD CONSTRAINT FK_REL_task_syscode  FOREIGN KEY([task_syscode]) REFERENCES [dbo].[task_master] ([task_syscode])
GO



