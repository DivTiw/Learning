
CREATE TABLE [dbo].[user_preferences](
	[pref_syscode] [int] IDENTITY(1,1) NOT NULL,
	[employee_syscode] [int] NULL,
	[project_syscode] [int] NULL,
	[module_syscode] [int] NULL,
	[task_syscode] [int] NULL,
	[email_mute] [bit] NULL,
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT (getdate()) ,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL,
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[user_preferences]  WITH CHECK ADD FOREIGN KEY([module_syscode])
REFERENCES [dbo].[module_master] ([module_syscode])
GO

ALTER TABLE [dbo].[user_preferences]  WITH CHECK ADD FOREIGN KEY([project_syscode])
REFERENCES [dbo].[project_master] ([project_syscode])
GO

ALTER TABLE [dbo].[user_preferences]  WITH CHECK ADD FOREIGN KEY([task_syscode])
REFERENCES [dbo].[task_master] ([task_syscode])
GO




