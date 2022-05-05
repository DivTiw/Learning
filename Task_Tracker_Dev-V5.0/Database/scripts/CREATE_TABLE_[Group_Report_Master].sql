
CREATE TABLE [dbo].[Group_Report_Master]
(
	grp_rpt_syscode INT IDENTITY(1,1) PRIMARY KEY,
	[report_name] varchar(200) NOT NULL ,
	[report_code] varchar(50) NOT NULL,
	[report_desc] varchar(2000),
	[group_syscode] [int] NOT NULL REFERENCES [dbo].[group_master] ([group_syscode]) ,
	[template_syscode] [int] NOT NULL REFERENCES [dbo].[email_template] ([template_syscode]),
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT (getdate()) ,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_deleted] [bit] NULL  DEFAULT(0),
	[is_active] [bit] NULL DEFAULT(1)
)

INSERT INTO [Group_Report_Master] (report_name, report_code, report_desc, group_syscode,template_syscode, created_by)
VALUES ('Task Activity Mgmt Report','TAMR','Win Schedular Report', 1,15,3986)


ALTER TABLE Group_Report_Master
ADD CONSTRAINT UNQ_CODE UNIQUE(report_code, group_syscode)