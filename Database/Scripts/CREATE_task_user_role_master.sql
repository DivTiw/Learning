CREATE TABLE [dbo].[task_user_role_master](
	[role_syscode] [int] NOT NULL,
	[role_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_task_user_role_master] PRIMARY KEY CLUSTERED 
(
	[role_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
