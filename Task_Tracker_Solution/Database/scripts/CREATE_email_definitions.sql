USE [task_tracker_build]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
--DROP TABLE [dbo].[email_definitions]
CREATE TABLE [dbo].[email_definitions](
	[definition_syscode] [int] NOT NULL IDENTITY,
	[email_type_syscode] [int] NOT NULL,
	[task_type_syscode] [int] NULL, 
	[status_syscode] [int] NULL,
	[template_syscode] [int] NOT NULL, 
	[to_recipients] [varchar] (2000),
	[cc_recipients] [varchar] (2000),
	[created_by] [int] NOT NULL DEFAULT(1),
	[created_on] [datetime] NOT NULL DEFAULT(GETDATE()),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NOT NULL DEFAULT(1),
	[is_deleted] [bit] NOT NULL DEFAULT(0),
	CONSTRAINT [FK_EmailType_syscode] FOREIGN KEY (email_type_syscode) REFERENCES email_type_master(email_type_syscode),
	CONSTRAINT [FK_TaskType_syscode] FOREIGN KEY (task_type_syscode) REFERENCES task_type_master(task_type_syscode),
	CONSTRAINT [FK_Status_syscode] FOREIGN KEY (status_syscode) REFERENCES task_status_master(status_syscode),
	CONSTRAINT [FK_Template_syscode] FOREIGN KEY (template_syscode) REFERENCES email_template(template_syscode),
 CONSTRAINT [PK_email_definitions] PRIMARY KEY CLUSTERED 
(
	[definition_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

