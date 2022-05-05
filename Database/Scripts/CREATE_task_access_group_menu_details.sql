

CREATE TABLE [dbo].[task_access_group_menu_details](
	[access_group_menu_details_syscode] [int] IDENTITY(1,1) NOT NULL,
	[access_group_syscode] [int] NOT NULL,
	[menu_syscode] [int] NOT NULL,
 CONSTRAINT [PK_access_group_menu_details] PRIMARY KEY CLUSTERED 
(
	[access_group_menu_details_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_access_group_menu_details] UNIQUE NONCLUSTERED 
(
	[access_group_syscode] ASC,
	[menu_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

