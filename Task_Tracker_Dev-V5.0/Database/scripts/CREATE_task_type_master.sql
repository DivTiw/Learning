USE [task_tracker_build]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[task_type_master](
	[task_type_syscode] [int] NOT NULL,
	[task_type] [varchar](50) NOT NULL,
	[description] [varchar](500) NOT NULL,
	[code] [varchar](10) NOT NULL,
 CONSTRAINT [PK_task_type_master] PRIMARY KEY CLUSTERED 
(
	[task_type_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


