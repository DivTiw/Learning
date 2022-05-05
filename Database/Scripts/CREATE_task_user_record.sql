

CREATE TABLE [dbo].[task_user_record](
	[record_syscode] [int] IDENTITY(1,1) NOT NULL,
	[employee_syscode] [int] NOT NULL,
	[task_syscode] [int] NOT NULL   FOREIGN KEY ([task_syscode]) REFERENCES task_master([task_syscode]),
	[start_time] [datetime] NULL,
	[stop_time] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[record_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



