
CREATE TABLE [dbo].[module_level_detail](
	[details_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[module_syscode] INT NOT NULL CONSTRAINT FK_MOD FOREIGN KEY REFERENCES [module_master](module_syscode),
	[level_syscode] INT NOT NULL CONSTRAINT FK_LVL FOREIGN KEY REFERENCES [workflow_level_details](level_syscode),
	[weightage] decimal (18,0) NULL,
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT GETDATE(),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL)
GO



