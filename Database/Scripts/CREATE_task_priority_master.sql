CREATE TABLE [dbo].[task_priority_master](
	[priority_syscode] [int] NOT NULL,
	[priority_name] [varchar](100) NOT NULL,
	[priority_icon] [varchar](max) NULL,
 CONSTRAINT [PK_task_priority_master] PRIMARY KEY CLUSTERED 
(
	[priority_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
