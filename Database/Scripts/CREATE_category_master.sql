

CREATE TABLE [dbo].[category_master](
	[category_syscode] [int] IDENTITY(1,1) NOT NULL,
	[category_name] [varchar](200) NOT NULL,
	[group_syscode] [int] NULL FOREIGN KEY REFERENCES [group_master](group_syscode),
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT getdate(),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[category_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




