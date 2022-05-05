USE [task_tracker_build]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[email_type_master](
	[email_type_syscode] [int] NOT NULL,
	[email_type_name] [varchar](100) NOT NULL,
	[description] [varchar](500) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[created_by] [int] NOT NULL DEFAULT(1),
	[created_on] [datetime] NOT NULL DEFAULT(GETDATE()),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NOT NULL DEFAULT(1),
	[is_deleted] [bit] NOT NULL DEFAULT(0),

 CONSTRAINT [PK_email_type_master] PRIMARY KEY CLUSTERED 
(
	[email_type_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


