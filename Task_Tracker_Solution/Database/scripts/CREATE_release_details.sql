CREATE TABLE [dbo].[release_details](
	[release_detail_syscode] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,	
	[release_syscode] INT NOT NULL,
	[parameter_syscode] [int] NOT NULL,
	[parameter_value] varchar(max),
	[created_by] [int] NULL,
	[created_on] [datetime]  DEFAULT (GETDATE()) NULL,
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_active] [bit] NULL,
	[is_deleted] [bit] NULL
)

ALTER TABLE [dbo].[release_details] ADD CONSTRAINT FK_RD_release_syscode FOREIGN KEY([release_syscode]) REFERENCES [dbo].[release_instructions] ([release_syscode])
GO





