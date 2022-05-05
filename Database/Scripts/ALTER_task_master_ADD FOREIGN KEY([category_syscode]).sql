

ALTER TABLE [dbo].[task_master] ADD FOREIGN KEY([category_syscode])
REFERENCES [dbo].[category_master] ([category_syscode])
GO


