USE [task_tracker_build]
GO

/****** Object:  Table [dbo].[workflow_master]    Script Date: 8/10/2020 12:31:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[module_category_master](
	[mod_category_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[mod_category_name] [varchar](200) NULL,
	[created_by] [int] NULL,
	[created_on] [datetime] NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL
)
GO


