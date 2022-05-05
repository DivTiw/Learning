CREATE TABLE [dbo].[task_category_master](
	[category_syscode] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[category_name] [varchar](200) NOT NULL,
	[created_by] int,
	[created_on] datetime,
	[modified_by] int,
	[modified_on] datetime,
	[is_active] bit,
	[is_deleted] bit
) 
GO


ALTER TABLE [dbo].[task_category_master] ADD  CONSTRAINT [DF_task_category_master_created_on]  DEFAULT (getdate()) FOR [created_on]
GO