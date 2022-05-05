CREATE TABLE [dbo].[task_trail](
trail_syscode		int	 PRIMARY KEY IDENTITY(1,1) NOT NULL,
task_syscode		INT	FOREIGN KEY REFERENCES [task_master](task_syscode),
trail_activity_syscode	INT	FOREIGN KEY REFERENCES [task_activity_master](activity_syscode),
trail_start_datetime		DATETIME	,
trail_created_by		INT	,
created_on		datetime	DEFAULT GETDATE(),
modified_by		INT	,
modified_on		DATETIME,
is_deleted		bit	
)