

CREATE TABLE [dbo].[group_members](
	[group_member_syscode] [int] IDENTITY(1,1) NOT NULL,
	[group_syscode] [int] FOREIGN KEY REFERENCES [group_master](group_syscode),
	[employee_syscode] [int] NULL,
	[role_syscode] [int]  FOREIGN KEY REFERENCES [task_user_role_master] ([role_syscode]),
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT (getdate()),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_deleted] [bit] NULL,
	[is_active] [bit] NULL
) ON [PRIMARY]
GO



