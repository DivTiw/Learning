
CREATE TABLE [dbo].[environment_master](
	[env_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[group_syscode] [int] NULL,
	[env_name] [varchar](100) NOT NULL,
	[env_code] [varchar](10) NOT NULL,
	[env_desc] [varchar](2000) NOT NULL,
	[created_by] [int] NULL,
	[created_on] [datetime]  DEFAULT (GETDATE()) NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL
)

ALTER TABLE [dbo].[environment_master]  
ADD CONSTRAINT FK_group_syscode FOREIGN KEY([group_syscode]) REFERENCES [dbo].[group_master] (group_syscode)
GO


