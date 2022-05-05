
CREATE TABLE [dbo].[progress_master](
	[progress_syscode] [int] IDENTITY(1,1) NOT NULL,
	[type_detail] [varchar](200) NOT NULL CHECK (type_detail  IN ('Task', 'Module', 'Project')),
	[type_syscode] [int]  NOT NULL,
	[progress] [decimal] (5,2),
	[created_by] [int] NULL,
	[created_on] [datetime] NULL DEFAULT getdate(),
	[modified_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[is_deleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[progress_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--ALTER TABLE [dbo].[task_progress] ADD  DEFAULT (getdate()) FOR [created_on]
--GO


