
CREATE TABLE [dbo].[task_access_group_master](
	[access_group_syscode] [int] NOT NULL,
	[access_group_name] [varchar](100) NOT NULL,
	[remark] [varchar](100) NULL,
	[group_type] [varchar](10) NULL,
	[group_syscode] [int] NOT NULL,
	[created_by] [int] NULL,
	[created_on] [datetime] NOT NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] not null,
	[is_deleted] [bit] not null
	
 CONSTRAINT [PK_access_group_master] PRIMARY KEY CLUSTERED 
(
	[access_group_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[task_access_group_master] ADD  CONSTRAINT [DF_access_group_master_created_date]  DEFAULT (getdate()) FOR [created_date]
GO


