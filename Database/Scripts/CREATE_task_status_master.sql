CREATE TABLE [dbo].[task_status_master](
	[status_syscode] [int] NOT NULL,
	[status_name] [varchar](100) NOT NULL,
	[status_icon] [varchar](max) NULL,
 CONSTRAINT [PK_task_status_master] PRIMARY KEY CLUSTERED 
(
	[status_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

