
CREATE TABLE [dbo].[task_master](
task_syscode		int	 PRIMARY KEY IDENTITY(1,1) NOT NULL,
task_reference		varchar(200),	
task_subject		varchar(200),
task_details		varchar(max)	,
task_status_syscode	int,
task_priority_syscode	int,
parent_task_syscode		int,		
task_on_behalf		int,	
task_owner int,
target_date		datetime,	
start_time		datetime	,
end_time		datetime,
created_by		int	,
created_on		datetime	,
modified_by		int,	
modified_on		datetime	,
is_active		bit	,
is_deleted		bit	
)
GO
ALTER TABLE [dbo].[task_master]  WITH CHECK ADD  CONSTRAINT FK_task_status_syscode FOREIGN KEY (task_status_syscode) REFERENCES dbo.[task_status_master]([status_syscode])
GO
ALTER TABLE [dbo].[task_master] WITH CHECK ADD  CONSTRAINT FK_task_priority_syscode  FOREIGN KEY (task_priority_syscode) REFERENCES task_priority_master (priority_syscode)
GO
ALTER TABLE [dbo].[task_master] ADD  CONSTRAINT [DF_created_on]  DEFAULT (getdate()) FOR [created_on]
GO
