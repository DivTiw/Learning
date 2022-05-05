

/****** Object:  Index [UQC_Project]    Script Date: 11/9/2020 12:17:46 PM ******/
ALTER TABLE [dbo].[project_master] DROP CONSTRAINT [UQC_Project]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [UQC_Project]    Script Date: 11/9/2020 12:17:46 PM ******/
ALTER TABLE [dbo].[project_master] ADD  CONSTRAINT [UQC_Project] UNIQUE NONCLUSTERED 
(
	[project_name],[group_syscode]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


