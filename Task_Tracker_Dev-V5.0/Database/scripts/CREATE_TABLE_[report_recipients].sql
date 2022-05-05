



CREATE TABLE [dbo].[report_recipients](
	[recipient_syscode] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,	
	[employee_syscode] [int] NOT NULL,	
	[grp_rpt_syscode] [int] NOT NULL REFERENCES Group_Report_Master (grp_rpt_syscode),
	[is_cc] bit not null DEFAULT(0),
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT (getdate()),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_deleted] [bit] NULL DEFAULT(0),
	[is_active] [bit] NULL DEFAULT(1)
) ON [PRIMARY]
GO

INSERT INTO [report_recipients] (employee_syscode,grp_rpt_syscode,created_by)
VALUES (3986,1,3986)

SELECT * FROM report_recipients
