CREATE TABLE [dbo].[task_category_contact](
	[category_contact_syscode] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[category_syscode] [int] NOT NULL,
	[email_id] [varchar](200) NOT NULL,
	[created_by] int,
	[created_on] datetime,
	[modified_by] int,
	[modified_on] datetime,
	[is_active] bit,
	[is_deleted] bit
) 
GO

ALTER TABLE [dbo].[task_category_contact] ADD  CONSTRAINT [DF_task_category_contact_created_on]  DEFAULT (getdate()) FOR [created_on]
GO

ALTER TABLE [dbo].[task_category_contact]  WITH CHECK ADD  CONSTRAINT [FK_task_category_master] FOREIGN KEY([category_syscode])
REFERENCES [dbo].[task_category_master] ([category_syscode])
GO