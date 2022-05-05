CREATE TABLE [dbo].[task_activity_master](
	[activity_syscode] [int] NOT NULL,
	[activity_name] [varchar](100) NOT NULL,
	[activity_icon] [varchar](max) NULL,
 CONSTRAINT [PK_task_activity_master] PRIMARY KEY CLUSTERED 
(
	[activity_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
