


CREATE TABLE [dbo].[group_master](
	[group_syscode] [int] IDENTITY(1,1) NOT NULL,
	[group_name] [varchar](1000) NOT NULL,
	[group_description] [varchar](5000) NULL,
	[group_email_id] [varchar](200) NULL,
	[created_by] [int] NULL,
	[created_on] [datetime] NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL,	

PRIMARY KEY CLUSTERED 
(
	[group_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQC_group] UNIQUE NONCLUSTERED 
(
	[group_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


