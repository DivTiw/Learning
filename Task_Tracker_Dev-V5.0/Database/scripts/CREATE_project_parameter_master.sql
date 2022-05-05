
CREATE TABLE [dbo].[project_parameter_master](
	[parameter_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,	
	[group_syscode] [int] NOT NULL,
	[parameter_name] [varchar](100) NOT NULL,
	[parameter_desc] [varchar](2000) NULL,
	[editable_in_release] bit,
	[parameter_datatype] varchar(100),
	[created_by] [int] NULL,
	[created_on] [datetime]  DEFAULT (GETDATE()) NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL
)

ALTER TABLE [dbo].[project_parameter_master]  
ADD CONSTRAINT  FK_pp_group_syscode FOREIGN KEY([group_syscode]) REFERENCES [dbo].[group_master] ([group_syscode])
GO
