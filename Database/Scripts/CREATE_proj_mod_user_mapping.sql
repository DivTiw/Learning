USE [task_tracker_build]
GO

/****** Object:  Table [dbo].[task_user_mapping]    Script Date: 8/10/2020 12:48:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[proj_mod_user_mapping](
	[mapping_syscode] [int] IDENTITY(1,1) NOT NULL,
	[employee_syscode] [int] NULL,
	[role_syscode] [int] NULL FOREIGN KEY([role_syscode]) REFERENCES [dbo].[task_user_role_master] ([role_syscode]),
	[project_syscode] [int] NULL FOREIGN KEY([project_syscode]) REFERENCES [dbo].[project_master] ([project_syscode]),
	[module_syscode] [int] NULL FOREIGN KEY ([module_syscode]) REFERENCES [dbo].[module_master] ([module_syscode]),
	[access_read] bit default 1,
	[access_write] bit,
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT (getdate()) ,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_deleted] [bit] NULL,
	[is_active] [bit] NULL
	)
GO



