

CREATE TABLE [dbo].[task_menu_master](
	[menu_syscode] [int] NOT NULL,
	[menu_name] [varchar](50) NOT NULL,
	[menu_description] [varchar](255) NOT NULL,
	[parent_menu_syscode] [int] NULL,
	[page_url] [varchar](1000) NULL,
	[is_enabled] [char](1) NULL,
	[display_order] [int] NULL,
 CONSTRAINT [PK_task_menu_master] PRIMARY KEY CLUSTERED 
(
	[menu_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


